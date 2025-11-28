using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager)]
    public class PortfolioController : Controller
    {
    private readonly ILogger<PortfolioController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public PortfolioController(ILogger<PortfolioController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #region Portfolio Dashboard

        public async Task<IActionResult> PortfolioDashboard()
        {
            // Return the list of all Projects for selection
            var projects = await _dbContext.Projects
                .ToListAsync();    

            return View(new PortfolioDashboardViewModel { Projects = projects });
        }

        [HttpPost]
        public async Task<IActionResult> PortfolioDashboard(PortfolioDashboardViewModel portfolioDashboardView)
        {
            if (portfolioDashboardView.ProjectId == 0)
            {
                //System.Diagnostics.EventLog.WriteEntry("Application","No project selected in PortfolioDashboard", System.Diagnostics.EventLogEntryType.Warning);
                ModelState.AddModelError("ProjectId", "Please select a project.");                
                return View(portfolioDashboardView);
            }

            // var vm = new PortfolioDashboardViewModel();
            var projects = await _dbContext.Projects
                .ToListAsync();
            portfolioDashboardView.Projects = projects;
            //portfolioDashboardView.ProjectId = portfolioDashboardView.ProjectId;
//System.Diagnostics.EventLog.WriteEntry("Application","PortfolioDashboard method invoked", System.Diagnostics.EventLogEntryType.Information);
            // 1) Project Allocated Hours (from TimesheetLineItems)
            portfolioDashboardView.ProjectAllocations = await (from tli in _dbContext.TimesheetLineItems
                                        join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                                        join p in _dbContext.Projects on ts.ProjectId equals p.ProjectId 
                                        where p.ProjectId == portfolioDashboardView.ProjectId
                                        group tli by p.ProjectName into g
                                        select new ProjectAllocationDto
                                        {
                                            ProjectName = g.Key,
                                            AllocatedHours = g.Sum(x => x.HoursWorked)
                                        }).ToListAsync();
//System.Diagnostics.EventLog.WriteEntry("Application","Utilization per associate", System.Diagnostics.EventLogEntryType.Information);
            // 2) Utilization per associate (example: last 30 days)
            var fromDate = DateTime.UtcNow.AddDays(-30);
            portfolioDashboardView.Utilization = await (from tli in _dbContext.TimesheetLineItems
                                    join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                                    join a in _dbContext.Associates on ts.AssociateId equals a.AssociateId
                                    where ts.ProjectId == portfolioDashboardView.ProjectId
                                    //where ts.TimesheetStartDate.ToDateTime(TimeOnly.MinValue) >= fromDate // adapt if using DateOnly
                                    group new { tli, ts } by new { a.AssociateId, a.FullName } into g
                                    select new UtilizationDto
                                    {
                                        AssociateName = g.Key.FullName,
                                        BillableHours = g.Sum(x => x.tli.HoursWorked),
                                        // AvailableHours: assumes 8*working days in period; adapt to your business logic
                                        AvailableHours = g.Select(x => x.ts.TimesheetStartDate).Distinct().Count() * 5 * 8
                                    }).ToListAsync();
//System.Diagnostics.EventLog.WriteEntry("Application","Submission status counts", System.Diagnostics.EventLogEntryType.Information);
            // 3) Submission status counts
            portfolioDashboardView.SubmissionStatuses = await _dbContext.Timesheets
                .Where(t => t.ProjectId == portfolioDashboardView.ProjectId)
                .GroupBy(t => t.Status)
                .Select(g => new SubmissionStatusDto { Status = g.Key!, Count = g.Count() })
                .ToListAsync();

            // 4) Timesheet submission and Non-submission rates per project

            // vm.TimesheetSubmissionRates = await (from ts in _dbContext.Timesheets
            //                                     where ts.TimesheetStartDate >= fromDate
            //                                     group ts by ts.AssociateId into g
            //                                     select new TimesheetSubmissionRateDto
            //                                     {
            //                                         AssociateId = g.Key,
            //                                         TotalTimesheets = g.Count(),
            //                                         SubmittedTimesheets = g.Count(t => t.Status == "Submitted")
            //                                     }).ToListAsync();

            portfolioDashboardView.TimesheetCompliance = await TimesheetComplianceReport(portfolioDashboardView.ProjectId);
            portfolioDashboardView.TimesheetCompliance = portfolioDashboardView.TimesheetCompliance.Where(tc => tc.ProjectName != null).ToList().Select(tc =>
            {
                tc.ProjectName = tc.ProjectName;
                tc.WeekStartDate = tc.WeekStartDate;
                tc.Submitted = tc.Submitted;
                tc.Approved = tc.Approved;
                tc.Rejected = tc.Rejected;
                tc.NotSubmitted = tc.NotSubmitted;
                return tc;
            }).GroupBy(tc => new { tc.ProjectName, tc.WeekStartDate })
              .Select(g => new TimesheetComplianceDto
              {
                  ProjectName = g.Key.ProjectName,
                  WeekStartDate = g.Key.WeekStartDate,
                  Submitted = g.Sum(x => x.Submitted),
                  Approved = g.Sum(x => x.Approved),
                  Rejected = g.Sum(x => x.Rejected),
                  NotSubmitted = g.Sum(x => x.NotSubmitted)
              }).ToList();

            return View(portfolioDashboardView);
        }

        // CSV export example for project allocations
        public async Task<FileResult> ExportProjectAllocationsCsv()
        {
            var rows = await (from tli in _dbContext.TimesheetLineItems
                            join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                            join p in _dbContext.Projects on ts.ProjectId equals p.ProjectId
                            group tli by p.ProjectName into g
                            select new { ProjectName = g.Key, AllocatedHours = g.Sum(x => x.HoursWorked) }).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Project,AllocatedHours");
            foreach (var r in rows) sb.AppendLine($"{EscapeCsv(r.ProjectName)},{r.AllocatedHours}");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "project_allocations.csv");

            static string EscapeCsv(string s) => "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        public async Task<List<TimesheetComplianceDto>> TimesheetComplianceReport(int projectId)
        {
            // var reportingPeriod = new DateTime(2025, 11, 1); // Example
            var reportingPeriod = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive == true).ToListAsync();

            var project = await _dbContext.Projects.Where(p => p.ProjectId == projectId).FirstOrDefaultAsync();

            var data = new List<TimesheetComplianceDto>();

            foreach (var period in reportingPeriod)
            {
                var submittedTS = _dbContext.Timesheets
                    .Where(t => t.TimesheetPeriodId == period!.TimesheetPeriodId && t.Status == "Submitted")
                    .Count();
                var approvedTS = _dbContext.Timesheets
                    .Where(t => t.TimesheetPeriodId == period!.TimesheetPeriodId && t.Status == "Approved")
                    .Count();
                var rejectedTS = _dbContext.Timesheets
                    .Where(t => t.TimesheetPeriodId == period!.TimesheetPeriodId && t.Status == "Rejected")
                    .Count();
                var allocatedAssociateCountPeriod = _dbContext.Allocations
                    .Where(a => a.ProjectId == project!.ProjectId && 
                                a.StartDate <= period!.WeekEndDate && 
                                (a.EndDate >= period!.WeekStartDate))
                    .Select(a => a.AssociateId)
                    .Distinct()
                    .Count();

                // var NotSubmittedTS = _dbContext.Allocations.Where(a => a.ProjectId == project!.ProjectId).Count() - (submittedTS + approvedTS + rejectedTS);
                var NotSubmittedTS = allocatedAssociateCountPeriod - (submittedTS + approvedTS + rejectedTS);
//System.Diagnostics.EventLog.WriteEntry("Application",$"Period: {period!.WeekStartDate}, Submitted: {submittedTS}, Approved: {approvedTS}, Rejected: {rejectedTS}, Not Submitted: {NotSubmittedTS}", System.Diagnostics.EventLogEntryType.Information);
                data.Add(new TimesheetComplianceDto()
                    {
                        ProjectName = project!.ProjectName,
                        WeekStartDate = period!.WeekStartDate.ToString("yyyy-MM-dd"),
                        Submitted = submittedTS,
                        Approved = approvedTS,
                        Rejected = rejectedTS,
                        NotSubmitted = NotSubmittedTS
                    });
                    //, NotSubmitted = p.Associates.Count() - p.Timesheets.Count(t => t.Period == reportingPeriod)
            } 

            return data;    
        }
    
        #endregion Portfolio Dashboard

        #region Associate Burndown Report

        public async Task<IActionResult> AssociateBurnDownRpt()
        {
            // Return the list of all Projects for selection
            var projects = await _dbContext.Projects
                .ToListAsync();

            // Return the list of all Associates for selection
            var associates = await _dbContext.Associates
                .ToListAsync();
//System.Diagnostics.EventLog.WriteEntry("Application","AssociateBurnDownRpt method invoked", System.Diagnostics.EventLogEntryType.Information);
//System.Diagnostics.EventLog.WriteEntry("Application",$"Projects count: {projects.Count}, Associates count: {associates.Count}", System.Diagnostics.EventLogEntryType.Information);
            return View(new PortfolioDashboardViewModel { Projects = projects, Associates = associates });
        }

        [HttpPost]
         public async Task<IActionResult> AssociateBurnDownRpt(PortfolioDashboardViewModel portfolioDashboardView)
        {
            // Return the list of all Projects for selection
            var projects = await _dbContext.Projects
                .ToListAsync();

            // Return the list of all Associates for selection
            var associates = await _dbContext.Associates
                .ToListAsync();

            portfolioDashboardView.Projects = projects;
            portfolioDashboardView.Associates = associates;
            portfolioDashboardView.ProjectId = portfolioDashboardView.ProjectId != 0 ? portfolioDashboardView.ProjectId : 0;
            portfolioDashboardView.AssociateId = portfolioDashboardView.AssociateId != 0 ? portfolioDashboardView.AssociateId : 0;
            portfolioDashboardView.TimesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            
            if (portfolioDashboardView.ProjectId != 0)
            {
                var allocatedAssociates = await _dbContext.Allocations
                    .Where(a => a.ProjectId == portfolioDashboardView.ProjectId && 
                                (portfolioDashboardView.AssociateId != 0 ? a.AssociateId == portfolioDashboardView.AssociateId : true)
                          )
                    .Select(a => a.AssociateId)
                    .Distinct()
                    .ToListAsync();

                // var allocatedAssociates = await _dbContext.Allocations
                //     .Where(a => a.ProjectId == portfolioDashboardView.ProjectId || portfolioDashboardView.AssociateId == 0 
                //                 && (portfolioDashboardView.AssociateId != 0 ? a.AssociateId == portfolioDashboardView.AssociateId : true)
                //           )
                //     .Select(a => a.AssociateId)
                //     .Distinct()
                //     .ToListAsync();

                foreach (var associateId in allocatedAssociates)
                {
                    BurndownDataDto burndownDataDto = new BurndownDataDto()
                    {
                        Associate = await _dbContext.Associates
                            .Where(a => a.AssociateId == associateId)
                            .FirstOrDefaultAsync(),
                        Project = await _dbContext.Projects
                            .Where(p => p.ProjectId == portfolioDashboardView.ProjectId)
                            .FirstOrDefaultAsync(),
                        Timesheets = await _dbContext.Timesheets
                            .Where(ts => ts.ProjectId == portfolioDashboardView.ProjectId && ts.AssociateId == associateId)
                            .OrderBy(ts => ts.TimesheetPeriodId)
                            // .Union(
                            //     await _dbContext.Timesheets
                            //         .Where(ts => ts.ProjectId == portfolioDashboardView.ProjectId && ts.AssociateId == associateId)
                            //         .OrderBy(ts => ts.TimesheetPeriodId)
                            //         .ToListAsync()
                            // )
                            .ToListAsync()                      
                    };
                    burndownDataDto.Timesheets.AddRange(
                        await _dbContext.TimesheetPeriods
                            .Where(tsp => tsp.IsActive &&
                                          !_dbContext.Timesheets.Any(ts => ts.AssociateId == associateId && ts.TimesheetPeriodId == tsp.TimesheetPeriodId))
                            .Select(tsp => new Timesheet
                            {
                                //TimesheetId = Guid.Empty,
                                AssociateId = associateId,
                                ProjectId = portfolioDashboardView.ProjectId,
                                TimesheetPeriodId = tsp.TimesheetPeriodId,
                                TimesheetStartDate = tsp.WeekStartDate,
                                TimesheetEndDate = tsp.WeekEndDate,
                                TotalHours = 0m
                            })
                            .ToListAsync()
                    );
//System.Diagnostics.EventLog.WriteEntry("Application", $"Timesheet count: {burndownDataDto.Timesheets!.Count}", System.Diagnostics.EventLogEntryType.Information);
                    portfolioDashboardView.BurndownData!.Add(burndownDataDto);
                }
            }

            return View(portfolioDashboardView);
        }

        #endregion Associate Burndown Report

    }
}