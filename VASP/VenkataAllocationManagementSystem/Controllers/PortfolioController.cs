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
using SQLitePCL;
using System.Data;

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
// System.Diagnostics.EventLog.WriteEntry("Application",$"Period: {period!.WeekStartDate}, Submitted: {submittedTS}, Approved: {approvedTS}, Rejected: {rejectedTS}, Not Submitted: {NotSubmittedTS}, allocated: {allocatedAssociateCountPeriod}", System.Diagnostics.EventLogEntryType.Information);
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
            // System.Diagnostics.EventLog.WriteEntry("Application","AssociateBurnDownRpt POST method invoked", System.Diagnostics.EventLogEntryType.Information);
            // System.Diagnostics.EventLog.WriteEntry("Application",$"ProjectId: {portfolioDashboardView.ProjectId}, AssociateId: {portfolioDashboardView.AssociateId}", System.Diagnostics.EventLogEntryType.Information);
            
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
            // portfolioDashboardView.TimesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            portfolioDashboardView.TimesheetPeriods = await _dbContext.TimesheetPeriods
                            .Where(p => _dbContext.Timesheets
                                    .Select(t => t.TimesheetPeriodId)
                                    .Distinct()
                                    .Contains(p.TimesheetPeriodId)).ToListAsync();
            
            if (portfolioDashboardView.ProjectId != 0)
            {
                var allocatedAssociates = await _dbContext.Allocations
                    .Where(a => a.ProjectId == portfolioDashboardView.ProjectId && 
                                (portfolioDashboardView.AssociateId != 0 ? a.AssociateId == portfolioDashboardView.AssociateId : true)
                          )
                    .Select(a => new { a.AssociateId, a.StartDate })
                    .Distinct()
                    .ToListAsync();

                // var allocatedAssociates = await _dbContext.Allocations
                //     .Where(a => a.ProjectId == portfolioDashboardView.ProjectId || portfolioDashboardView.AssociateId == 0 
                //                 && (portfolioDashboardView.AssociateId != 0 ? a.AssociateId == portfolioDashboardView.AssociateId : true)
                //           )
                //     .Select(a => a.AssociateId)
                //     .Distinct()
                //     .ToListAsync();

                foreach (var associate in allocatedAssociates)
                {
                    BurndownDataDto burndownDataDto = new BurndownDataDto()
                    {
                        Associate = await _dbContext.Associates
                            .Where(a => a.AssociateId == associate.AssociateId)
                            .FirstOrDefaultAsync(),
                        Project = await _dbContext.Projects
                            .Where(p => p.ProjectId == portfolioDashboardView.ProjectId)
                            .FirstOrDefaultAsync(),
                        Timesheets = await _dbContext.Timesheets
                            .Where(ts => ts.ProjectId == portfolioDashboardView.ProjectId && ts.AssociateId == associate.AssociateId)
                            .OrderBy(ts => ts.TimesheetPeriodId)
                            // .Union(
                            //     await _dbContext.Timesheets
                            //         .Where(ts => ts.ProjectId == portfolioDashboardView.ProjectId && ts.AssociateId == associateId)
                            //         .OrderBy(ts => ts.TimesheetPeriodId)
                            //         .ToListAsync()
                            // )
                            .ToListAsync(),
                        StartDate = associate.StartDate
                                              
                    };
                    burndownDataDto.Timesheets.AddRange(
                        await _dbContext.TimesheetPeriods
                            .Where(tsp => tsp.IsActive &&
                                          !_dbContext.Timesheets.Any(ts => ts.AssociateId == associate.AssociateId && ts.TimesheetPeriodId == tsp.TimesheetPeriodId))
                            .Select(tsp => new Timesheet
                            {
                                //TimesheetId = Guid.Empty,
                                AssociateId = associate.AssociateId,
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

        //#endregion Associate Burndown Report

        public async Task<IActionResult> ExportAssociateBurndowtoCSV(int projectId, int associateId)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application","ExportAssociateBurndowtoCSV method invoked", System.Diagnostics.EventLogEntryType.Information);
            // System.Diagnostics.EventLog.WriteEntry("Application",$"ProjectId: {projectId}, AssociateId: {associateId}", System.Diagnostics.EventLogEntryType.Information);
            
            var associateburndownData = (from ts in _dbContext.Timesheets 
                join tsp in _dbContext.TimesheetPeriods on ts.TimesheetPeriodId equals tsp.TimesheetPeriodId
                join p in _dbContext.Projects on ts.ProjectId equals p.ProjectId
                join assoc in _dbContext.Associates on ts.AssociateId equals assoc.AssociateId
                where (ts.ProjectId == (projectId > 0 ? projectId : p.ProjectId )
                       && ts.AssociateId == (associateId > 0 ? associateId : assoc.AssociateId))
                group new {ts, tsp, p, assoc} by new {p.ProjectName, assoc.AssociateEmployeeId, assoc.FullName, tsp.WeekStartDate, ts.TotalHours} into expts
                orderby expts.Key.WeekStartDate
                select new 
                {   
                    expts.Key.ProjectName,
                    expts.Key.AssociateEmployeeId,
                    expts.Key.FullName,
                    expts.Key.WeekStartDate,
                    expts.Key.TotalHours
                }
            ).ToList<dynamic>(); 

            DataTable dtAssociateBurndownData = PivotTimesheetData(associateburndownData);

            string outputPath = @"C:\Exports\TimesheetData.xlsx";

            // Call the function to perform the export
            // Ensure the C:\Exports folder exists or use a path where your user has write permission.
            Common.ExportToExcel exportToExcel = new Common.ExportToExcel();
            exportToExcel.ExportDataTableToExcel(dtAssociateBurndownData, outputPath);

            if (projectId > 0 || associateId > 0)
            {
                 var projects = await _dbContext.Projects
                .ToListAsync();

                // Return the list of all Associates for selection
                var associates = await _dbContext.Associates
                    .ToListAsync();

                var portfolioDashboardView = new PortfolioDashboardViewModel
                {
                    Projects = projects,
                    Associates = associates,
                    ProjectId = projectId,
                    AssociateId = associateId
                };

                return View("AssociateBurnDownRpt", portfolioDashboardView);
            }

            // return View(RedirectToAction("AssociateBurnDownRpt", new PortfolioDashboardViewModel{ProjectId=projectId, AssociateId=associateId}));
            return(RedirectToAction("AssociateBurnDownRpt"));
        }

        public DataTable PivotTimesheetData(List<dynamic> queryResults)
        {
            DataTable pivotedDt = new DataTable("PivotedTimesheets");

            if (queryResults == null || queryResults.Count == 0)
            {
                return pivotedDt;
            }

            // 1. Identify Dynamic Columns (Week Start Dates) as DateOnly
            var weekDates = queryResults
                .Select(r => (DateOnly)r.WeekStartDate)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // The first core columns are fixed
            pivotedDt.Columns.Add("ProjectName", typeof(string));
            pivotedDt.Columns.Add("AssociateEmployeeId", typeof(string));
            pivotedDt.Columns.Add("FullName", typeof(string));
            
            // 2. Add Dynamic Columns for each WeekStartDate
            foreach (DateOnly date in weekDates)
            {
                string colName = $"{date.ToString("MM/dd/yyyy")}";
                pivotedDt.Columns.Add(colName, typeof(decimal)); // Column to hold total hours
            }

            pivotedDt.Columns.Add("Total Hours", typeof(decimal));

            // 3. Group and Flatten the Data
            var groupedData = queryResults
                .GroupBy(r => new { r.ProjectName, r.AssociateEmployeeId, r.FullName })
                .Select(g => new
                {
                    g.Key.ProjectName,
                    g.Key.AssociateEmployeeId,
                    g.Key.FullName,
                    // Create a dictionary mapping WeekStartDate (DateOnly) to the sum of TotalHours for that week
                    WeeklyTotals = g.ToDictionary(
                        r => (DateOnly)r.WeekStartDate,
                        r => Convert.ToDecimal(r.TotalHours))
                });

            // 4. Populate the Pivoted DataTable
            foreach (var projectGroup in groupedData)
            {
                DataRow newRow = pivotedDt.NewRow();
                
                // Populate fixed columns
                newRow["ProjectName"] = projectGroup.ProjectName;
                newRow["AssociateEmployeeId"] = projectGroup.AssociateEmployeeId;
                newRow["FullName"] = projectGroup.FullName;

                // Populate dynamic week columns
                foreach (DateOnly date in weekDates)
                {
                    string colName = $"{date.ToString("MM/dd/yyyy")}";

                    // Check if the current project has hours logged for this specific week
                    if (projectGroup.WeeklyTotals.TryGetValue(date, out var totalHoursObj))
                    {
                        decimal totalHours = Convert.ToDecimal(totalHoursObj);
                        newRow[colName] = totalHours;
                    }
                    else
                    {
                        // If no hours were logged for this week, set the value to 0
                        newRow[colName] = 0.00m;
                    }
                }
                // Calculate and set the Total Hours across all weeks
                // newRow["Total Hours"] = projectGroup.WeeklyTotals.Sum(kvp => Convert.ToDecimal(kvp.Value));
                decimal totalLoggedHours = 0.00m;
                foreach (var hours in projectGroup.WeeklyTotals.Values)
                {
                    totalLoggedHours += Convert.ToDecimal(hours);
                }
                newRow["Total Hours"] = totalLoggedHours;

                pivotedDt.Rows.Add(newRow);
            }

            pivotedDt.Columns["ProjectName"]!.ColumnName = "Project Name";
            pivotedDt.Columns["AssociateEmployeeId"]!.ColumnName = "Associate ID";
            pivotedDt.Columns["FullName"]!.ColumnName = "Associate Name";

            return pivotedDt;
        }

        #endregion Associate Burndown Report

        #region ML Summary Report

        public async Task<IActionResult> MLSummaryReport()
        {
            var viewModel = new MLSummaryReportViewModel();

            // 1) No of accounts and Projects
            viewModel.TotalAccounts = await _dbContext.Accounts.CountAsync();
            viewModel.TotalProjects = await _dbContext.Projects.CountAsync();

            // Account-wise project counts
            viewModel.AccountProjectCounts = await (from p in _dbContext.Projects
                                                     join a in _dbContext.Accounts on p.AccountId equals a.AccountId
                                                     group p by a.AccountName into g
                                                     select new AccountProjectCountDto
                                                     {
                                                         AccountName = g.Key,
                                                         ProjectCount = g.Count()
                                                     }).ToListAsync();

            // 2) Summary on Timesheets status
            var timesheetStatuses = await _dbContext.Timesheets
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            viewModel.TotalTimesheets = timesheetStatuses.Sum(t => t.Count);
            viewModel.TimesheetStatusSummary = timesheetStatuses.Select(t => new TimesheetStatusSummaryDto
            {
                Status = t.Status ?? "Unknown",
                Count = t.Count,
                Percentage = viewModel.TotalTimesheets > 0 ? (decimal)t.Count / viewModel.TotalTimesheets * 100 : 0
            }).ToList();

            viewModel.SubmittedCount = timesheetStatuses.FirstOrDefault(t => t.Status == "Submitted")?.Count ?? 0;
            viewModel.ApprovedCount = timesheetStatuses.FirstOrDefault(t => t.Status == "Approved")?.Count ?? 0;
            viewModel.RejectedCount = timesheetStatuses.FirstOrDefault(t => t.Status == "Rejected")?.Count ?? 0;
            viewModel.PendingCount = timesheetStatuses.FirstOrDefault(t => t.Status == "Pending" || t.Status == "Draft")?.Count ?? 0;

            // 3) Revenue Quick Summary by month
            // Calculate revenue from timesheets based on allocation hourly rate and hours worked
            var monthlyRevenueData = await (from tli in _dbContext.TimesheetLineItems
                                             join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                                             join tsp in _dbContext.TimesheetPeriods on ts.TimesheetPeriodId equals tsp.TimesheetPeriodId
                                             join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                             where (ts.Status == "Approved" || ts.Status == "Submitted")
                                             group new { tli, tsp, alloc } by new { tsp.WeekStartDate.Year, tsp.WeekStartDate.Month } into g
                                             select new MonthlyRevenueDto
                                             {
                                                 Year = g.Key.Year,
                                                 Month = g.Key.Month,
                                                 MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                                                 Revenue = g.Sum(x => x.tli.HoursWorked * (x.alloc.AllocationPercentage > 0 ? x.alloc.AllocationPercentage : 50)) // Use allocation percentage as hourly rate proxy
                                             }).ToListAsync();

            viewModel.MonthlyRevenues = monthlyRevenueData
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .Take(12)
                .ToList();
            viewModel.TotalRevenue = viewModel.MonthlyRevenues.Sum(m => m.Revenue);

            // 4) Revenue Prediction vs Actual with Variance
            // For prediction, we'll use a simple algorithm: average of last 3 months as prediction for current month
            var sortedMonthlyData = viewModel.MonthlyRevenues.OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();
            var predictions = new List<RevenuePredictionDto>();

            for (int i = 0; i < sortedMonthlyData.Count; i++)
            {
                var current = sortedMonthlyData[i];
                decimal predictedRevenue;

                // Use average of previous 3 months as prediction
                if (i >= 3)
                {
                    var previous3 = sortedMonthlyData.Skip(i - 3).Take(3).ToList();
                    predictedRevenue = previous3.Average(p => p.Revenue);
                }
                else if (i > 0)
                {
                    var previous = sortedMonthlyData.Take(i).ToList();
                    predictedRevenue = previous.Any() ? previous.Average(p => p.Revenue) : current.Revenue;
                }
                else
                {
                    predictedRevenue = current.Revenue; // First month - no prediction
                }

                var variance = current.Revenue - predictedRevenue;
                var variancePercentage = predictedRevenue > 0 ? (variance / predictedRevenue) * 100 : 0;

                predictions.Add(new RevenuePredictionDto
                {
                    Year = current.Year,
                    Month = current.Month,
                    MonthName = current.MonthName,
                    PredictedRevenue = predictedRevenue,
                    ActualRevenue = current.Revenue,
                    Variance = variance,
                    VariancePercentage = variancePercentage
                });
            }

            viewModel.RevenuePredictions = predictions.OrderByDescending(p => p.Year).ThenByDescending(p => p.Month).ToList();
            viewModel.TotalPredictedRevenue = viewModel.RevenuePredictions.Sum(p => p.PredictedRevenue);
            viewModel.TotalActualRevenue = viewModel.RevenuePredictions.Sum(p => p.ActualRevenue);
            viewModel.TotalVariance = viewModel.TotalActualRevenue - viewModel.TotalPredictedRevenue;

            // 5) Financial Metrics
            var financialMetrics = new FinancialMetricsDto();

            // Revenue per Project
            var revenuePerProject = await (from tli in _dbContext.TimesheetLineItems
                                            join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                                            join p in _dbContext.Projects on ts.ProjectId equals p.ProjectId
                                            join a in _dbContext.Accounts on p.AccountId equals a.AccountId
                                            join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                            where ts.Status == "Approved" || ts.Status == "Submitted"
                                            group new { tli, p, a, alloc } by new { p.ProjectName, a.AccountName } into g
                                            select new RevenuePerProjectDto
                                            {
                                                ProjectName = g.Key.ProjectName,
                                                AccountName = g.Key.AccountName,
                                                Revenue = g.Sum(x => x.tli.HoursWorked * (x.alloc.AllocationPercentage > 0 ? x.alloc.AllocationPercentage : 50))
                                            }).ToListAsync();

            financialMetrics.RevenuePerProject = revenuePerProject.OrderByDescending(r => r.Revenue).ToList();
            financialMetrics.TotalProjectRevenue = financialMetrics.RevenuePerProject.Sum(r => r.Revenue);
            if (financialMetrics.TotalProjectRevenue > 0)
            {
                foreach (var item in financialMetrics.RevenuePerProject)
                {
                    item.PercentageOfTotal = (item.Revenue / financialMetrics.TotalProjectRevenue) * 100;
                }
            }

            // Revenue per Associate
            var revenuePerAssociate = await (from tli in _dbContext.TimesheetLineItems
                                              join ts in _dbContext.Timesheets on tli.TimesheetId equals ts.TimesheetId
                                              join assoc in _dbContext.Associates on ts.AssociateId equals assoc.AssociateId
                                              join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                              where ts.Status == "Approved" || ts.Status == "Submitted"
                                              group new { tli, assoc, alloc } by new { assoc.FullName, assoc.AssociateEmployeeId } into g
                                              select new RevenuePerAssociateDto
                                              {
                                                  AssociateName = g.Key.FullName,
                                                  EmployeeId = g.Key.AssociateEmployeeId,
                                                  Revenue = g.Sum(x => x.tli.HoursWorked * (x.alloc.AllocationPercentage > 0 ? x.alloc.AllocationPercentage : 50))
                                              }).ToListAsync();

            financialMetrics.RevenuePerAssociate = revenuePerAssociate.OrderByDescending(r => r.Revenue).ToList();
            financialMetrics.TotalAssociateRevenue = financialMetrics.RevenuePerAssociate.Sum(r => r.Revenue);
            if (financialMetrics.TotalAssociateRevenue > 0)
            {
                foreach (var item in financialMetrics.RevenuePerAssociate)
                {
                    item.PercentageOfTotal = (item.Revenue / financialMetrics.TotalAssociateRevenue) * 100;
                }
            }

            // Cost Variance (using SOWValue as budget and actual cost derived from timesheets)
            var costVariance = await (from p in _dbContext.Projects
                                      join a in _dbContext.Accounts on p.AccountId equals a.AccountId
                                      let budgetedCost = p.SOWValue > 0 ? p.SOWValue : 0
                                      let actualCost = (from ts in _dbContext.Timesheets
                                                        join tli in _dbContext.TimesheetLineItems on ts.TimesheetId equals tli.TimesheetId
                                                        join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                                        where ts.ProjectId == p.ProjectId && (ts.Status == "Approved" || ts.Status == "Submitted")
                                                        select tli.HoursWorked * (alloc.AllocationPercentage > 0 ? alloc.AllocationPercentage : 50)).Sum()
                                      select new CostVarianceDto
                                      {
                                          ProjectName = p.ProjectName,
                                          BudgetedCost = budgetedCost,
                                          ActualCost = actualCost,
                                          Variance = budgetedCost - actualCost,
                                          VariancePercentage = budgetedCost > 0 ? ((budgetedCost - actualCost) / budgetedCost) * 100 : 0,
                                          Status = budgetedCost == 0 ? "No Budget" : (actualCost <= budgetedCost ? "Under Budget" : "Over Budget")
                                      }).ToListAsync();

            financialMetrics.CostVariances = costVariance.OrderBy(c => c.ProjectName).ToList();
            financialMetrics.TotalBudgetedCost = financialMetrics.CostVariances.Sum(c => c.BudgetedCost);
            financialMetrics.TotalActualCost = financialMetrics.CostVariances.Sum(c => c.ActualCost);
            financialMetrics.TotalCostVariance = financialMetrics.TotalBudgetedCost - financialMetrics.TotalActualCost;

            // Profit Margin
            var profitMargins = await (from p in _dbContext.Projects
                                       let revenue = (from ts in _dbContext.Timesheets
                                                      join tli in _dbContext.TimesheetLineItems on ts.TimesheetId equals tli.TimesheetId
                                                      join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                                      where ts.ProjectId == p.ProjectId && (ts.Status == "Approved" || ts.Status == "Submitted")
                                                      select tli.HoursWorked * (alloc.AllocationPercentage > 0 ? alloc.AllocationPercentage : 50)).Sum()
                                       let cost = (from ts in _dbContext.Timesheets
                                                   join tli in _dbContext.TimesheetLineItems on ts.TimesheetId equals tli.TimesheetId
                                                   join alloc in _dbContext.Allocations on new { ts.AssociateId, ts.ProjectId } equals new { alloc.AssociateId, alloc.ProjectId }
                                                   where ts.ProjectId == p.ProjectId && (ts.Status == "Approved" || ts.Status == "Submitted")
                                                   select tli.HoursWorked * (alloc.AllocationPercentage > 0 ? alloc.AllocationPercentage : 50) * 0.7m).Sum() // Assuming 70% cost ratio
                                       select new ProfitMarginDto
                                       {
                                           ProjectName = p.ProjectName,
                                           Revenue = revenue,
                                           Cost = cost,
                                           ProfitMargin = revenue - cost,
                                           ProfitMarginPercentage = revenue > 0 ? ((revenue - cost) / revenue) * 100 : 0
                                       }).ToListAsync();

            financialMetrics.ProfitMargins = profitMargins.OrderByDescending(p => p.ProfitMarginPercentage).ToList();
            financialMetrics.TotalRevenue = financialMetrics.ProfitMargins.Sum(p => p.Revenue);
            financialMetrics.TotalCost = financialMetrics.ProfitMargins.Sum(p => p.Cost);
            financialMetrics.OverallProfitMargin = financialMetrics.TotalRevenue - financialMetrics.TotalCost;
            financialMetrics.TotalRevenue = financialMetrics.TotalRevenue > 0 ? financialMetrics.TotalRevenue : viewModel.TotalRevenue;

            viewModel.FinancialMetrics = financialMetrics;

            // 6) Timesheet Compliance Report - aggregate across all projects
            var reportingPeriod = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive == true).ToListAsync();
            var allTimesheetCompliance = new List<TimesheetComplianceDto>();
            var activeStatusValues = new[]
            {
                TimesheetStatusEnum.Submitted.ToString(),
                TimesheetStatusEnum.Approved.ToString(),
                TimesheetStatusEnum.Rejected.ToString()
            };

            foreach (var period in reportingPeriod)
            {
                var activeAllocations = await _dbContext.Allocations
                    .Where(a => a.StartDate <= period.WeekEndDate && a.EndDate >= period.WeekStartDate)
                    .Select(a => new { a.AssociateId, a.ProjectId })
                    .Distinct()
                    .ToListAsync();

                var timesheetEntries = await _dbContext.Timesheets
                    .Where(t => t.TimesheetPeriodId == period.TimesheetPeriodId)
                    .Select(t => new { t.AssociateId, t.ProjectId, t.Status, t.CreatedOn, t.ModifiedOn })
                    .ToListAsync();

                var latestStatusByPair = timesheetEntries
                    .GroupBy(t => new { t.AssociateId, t.ProjectId })
                    .Select(g => g.OrderByDescending(t => t.ModifiedOn ?? t.CreatedOn).First())
                    .ToList();

                var submittedPairs = latestStatusByPair
                    .Where(t => t.Status == TimesheetStatusEnum.Submitted.ToString())
                    .Select(t => new { t.AssociateId, t.ProjectId })
                    .Distinct()
                    .Count();
                var approvedPairs = latestStatusByPair
                    .Where(t => t.Status == TimesheetStatusEnum.Approved.ToString())
                    .Select(t => new { t.AssociateId, t.ProjectId })
                    .Distinct()
                    .Count();
                var rejectedPairs = latestStatusByPair
                    .Where(t => t.Status == TimesheetStatusEnum.Rejected.ToString())
                    .Select(t => new { t.AssociateId, t.ProjectId })
                    .Distinct()
                    .Count();

                var completedPairs = latestStatusByPair
                    .Where(t => activeStatusValues.Contains(t.Status))
                    .Select(t => new { t.AssociateId, t.ProjectId })
                    .Distinct()
                    .ToList();

                var missingPairs = activeAllocations
                    .Where(a => !completedPairs.Any(c => c.AssociateId == a.AssociateId && c.ProjectId == a.ProjectId))
                    .ToList();

                var defaulterAssociateIds = missingPairs.Select(m => m.AssociateId).Distinct().ToList();
                var defaulterProjectIds = missingPairs.Select(m => m.ProjectId).Distinct().ToList();

                var associateNames = await _dbContext.Associates
                    .Where(a => defaulterAssociateIds.Contains(a.AssociateId))
                    .ToDictionaryAsync(a => a.AssociateId, a => a.FullName);

                var projectNames = await _dbContext.Projects
                    .Where(p => defaulterProjectIds.Contains(p.ProjectId))
                    .ToDictionaryAsync(p => p.ProjectId, p => p.ProjectName);

                var defaulterDetails = missingPairs
                    .Select(m => new
                    {
                        AssociateName = associateNames.ContainsKey(m.AssociateId) ? associateNames[m.AssociateId] : "Unknown"
                        //, ProjectName = projectNames.ContainsKey(m.ProjectId) ? projectNames[m.ProjectId] : "Unknown"
                    })
                    .Distinct()
                    // .Select(x => $"{x.AssociateName} ({x.ProjectName})")
                    .Select(x => $"{x.AssociateName}")
                    .OrderBy(x => x)
                    .ToList();

                var defaulterNames = defaulterDetails.Any()
                    ? string.Join(", ", defaulterDetails)
                    : string.Empty;

                allTimesheetCompliance.Add(new TimesheetComplianceDto
                {
                    ProjectName = "All Projects",
                    WeekStartDate = period.WeekStartDate.ToString("yyyy-MM-dd"),
                    Submitted = submittedPairs,
                    Approved = approvedPairs,
                    Rejected = rejectedPairs,
                    NotSubmitted = missingPairs.Count,
                    DefaulterNames = defaulterNames
                });
            }

            viewModel.TimesheetCompliance = allTimesheetCompliance;
            viewModel.TotalAllocatedAssociates = await _dbContext.Allocations.Select(a => a.AssociateId).Distinct().CountAsync();
            viewModel.TotalTimesheetComplianceSubmitted = allTimesheetCompliance.Sum(tc => tc.Submitted);
            viewModel.TotalTimesheetComplianceApproved = allTimesheetCompliance.Sum(tc => tc.Approved);
            viewModel.TotalTimesheetComplianceRejected = allTimesheetCompliance.Sum(tc => tc.Rejected);
            viewModel.TotalTimesheetComplianceNotSubmitted = allTimesheetCompliance.Sum(tc => tc.NotSubmitted);
            
            var totalExpected = viewModel.TotalTimesheetComplianceSubmitted + viewModel.TotalTimesheetComplianceApproved + 
                                viewModel.TotalTimesheetComplianceRejected + viewModel.TotalTimesheetComplianceNotSubmitted;
            viewModel.ComplianceRate = totalExpected > 0 ? 
                (decimal)(viewModel.TotalTimesheetComplianceSubmitted + viewModel.TotalTimesheetComplianceApproved) / totalExpected * 100 : 0;

            return View(viewModel);
        }

        #endregion ML Summary Report

    }
}