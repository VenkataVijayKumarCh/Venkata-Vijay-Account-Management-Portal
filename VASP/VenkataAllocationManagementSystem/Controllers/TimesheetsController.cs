using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using Microsoft.EntityFrameworkCore.Query;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;
using System.Security.Claims;
using System.Collections;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager, Roles.User)]
    public class TimesheetsController : Controller
    {
        private readonly ILogger<TimesheetsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public TimesheetsController(ILogger<TimesheetsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ViewMyTimesheets()
        {
            var userId = GetCurrentUserId();
            var myAssociateId = GetAssociateIdFromUserId(userId);

            var projects = await _dbContext.Projects.ToListAsync();
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            var timesheetStatuses = await _dbContext.TimesheetStatus.ToListAsync();
            // var timesheets = _dbContext.Timesheets
            //     .Include(t => t.TimesheetPeriod)
            //     .Where(t => t.AssociateId == userId)
            //     .ToList();

            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId
                    where t.AssociateId == userId
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName,
                        CurrentAssociateId = myAssociateId  // Add this line
                    }).ToListAsync();

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo,
                CurrentAssociateId = myAssociateId,
                Projects = projects,
                TimesheetPeriods = timesheetPeriods,
                TimesheetStatuses = timesheetStatuses
                //FilteredTimesheetPeriodId = tsperiodId,
            };
            return View(timesheetsDetails);
        }

        [HttpPost]
        public async Task<IActionResult> ViewMyTimesheets(TimesheetsViewModel tsModel)
        {
            // int tsprojectId = 0,int tsperiodId = 0, int tsstatusId = 0
            // System.Diagnostics.EventLog.WriteEntry("Application", "ViewMyTimesheets POST called with tsprojectId: " + tsModel.ProjectId.ToString() + ", tsperiodId: " + tsModel.TimesheetPeriodId.ToString() + ", tsstatusId: " + tsModel.TimesheetStatusId.ToString(), System.Diagnostics.EventLogEntryType.Information);
            var userId = GetCurrentUserId();
            var myAssociateId = GetAssociateIdFromUserId(userId);

            var projects = await _dbContext.Projects.ToListAsync();
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            var timesheetStatuses = await _dbContext.TimesheetStatus.ToListAsync();
            // var timesheets = _dbContext.Timesheets
            //     .Include(t => t.TimesheetPeriod)
            //     .Where(t => t.AssociateId == userId)
            //     .ToList();

            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId
                    where t.AssociateId == userId &&
                        (t.ProjectId == tsModel.ProjectId || tsModel.ProjectId == 0)
                        && (t.TimesheetPeriodId == tsModel.TimesheetPeriodId || tsModel.TimesheetPeriodId == 0)
                        && (t.TimesheetStatusId == tsModel.TimesheetStatusId || tsModel.TimesheetStatusId == 0)
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName,
                        CurrentAssociateId = myAssociateId,  // Add this line
                        BillabilityTypeId = t.BillabilityTypeId
                    }).ToListAsync();

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo,
                CurrentAssociateId = myAssociateId,
                Projects = projects,
                TimesheetPeriods = timesheetPeriods,
                TimesheetStatuses = timesheetStatuses
            };
            return View(timesheetsDetails);
        }

        public async Task<IActionResult> ViewTeamMemberTimesheets(int tsperiodId = 0, int tsassociateId = 0)
        {
            var userId = GetCurrentUserId();
            var myAssociateId = GetAssociateIdFromUserId(userId);
            // var timesheets = _dbContext.Timesheets
            //     .Include(t => t.TimesheetPeriod)
            //     .Where(t => t.AssociateId == userId)
            //     .ToList();

            var projects = await _dbContext.Projects.ToListAsync();
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            var associates = await _dbContext.Associates.ToListAsync();            
            var timesheetStatuses = await _dbContext.TimesheetStatus.ToListAsync();

            // && (tsperiodId == 0 || tp.TimesheetPeriodId == tsperiodId)

            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId 
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId 
                    where (tp.TimesheetPeriodId == (tsperiodId != 0 ? tsperiodId : tp.TimesheetPeriodId)
                        && (t.AssociateId == (tsassociateId != 0 ? tsassociateId : t.AssociateId)))
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName,
                        CurrentAssociateId = myAssociateId  // Add this line                        
                    }).ToListAsync();

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo.ToList(),
                CurrentAssociateId = myAssociateId,
                Projects = projects,
                TimesheetPeriods = timesheetPeriods,
                Associates = associates,
                //FilteredTimesheetPeriodId = tsperiodId,
                TimesheetPeriodId = tsperiodId != 0 ? tsperiodId : 0,
                AssociateId = tsassociateId != 0 ? tsassociateId : 0,
                TimesheetStatuses = timesheetStatuses
            };            
            return View(timesheetsDetails);
        }

        [HttpPost]
        public async Task<IActionResult> ViewTeamMemberTimesheets(TimesheetsViewModel tsModel)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "ViewTeamMemberTimesheets POST called with tsModel.TimesheetStatusId: " + tsModel.TimesheetStatusId.ToString(), System.Diagnostics.EventLogEntryType.Information);
            var userId = GetCurrentUserId();
            var myAssociateId = GetAssociateIdFromUserId(userId);
            // var timesheets = _dbContext.Timesheets
            //     .Include(t => t.TimesheetPeriod)
            //     .Where(t => t.AssociateId == userId)
            //     .ToList();

            var projects = await _dbContext.Projects.ToListAsync();
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            var associates = await _dbContext.Associates.ToListAsync();
            var timesheetStatuses = await _dbContext.TimesheetStatus.ToListAsync();

            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId
                    where (t.ProjectId == tsModel.ProjectId || tsModel.ProjectId == 0) 
                        && (t.AssociateId == tsModel.AssociateId || tsModel.AssociateId == 0)
                        && (t.TimesheetPeriodId == tsModel.TimesheetPeriodId || tsModel.TimesheetPeriodId == 0)
                        && (t.TimesheetStatusId == tsModel.TimesheetStatusId || tsModel.TimesheetStatusId == 0)
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName,
                        CurrentAssociateId = myAssociateId  // Add this line                        
                    }).ToListAsync();

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo,
                CurrentAssociateId = myAssociateId,
                Projects = projects,
                TimesheetPeriods = timesheetPeriods,
                Associates = associates,
                TimesheetStatuses = timesheetStatuses,
            };
            return View(timesheetsDetails);
        }


        public async Task<IActionResult> ViewTimesheet(Guid timesheetId)
        {
            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId
                    where t.TimesheetId == timesheetId
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,                        
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName, // CurrentAssociateId = myAssociateId  // Add this line
                        BillabilityTypeId = t.BillabilityTypeId
                    }).ToListAsync();

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo,
                TimesheetLineItems = _dbContext.TimesheetLineItems
                                        .Where(tli => tli.TimesheetId == timesheetId)
                                        .OrderBy(tli => tli.WorkDate)
                                        .ToList()
            };
            return View(timesheetsDetails);
        }

        public async Task<IActionResult> EditTimesheet(Guid timesheetId)
        {
            var timesheetsInfo = await (from t in _dbContext.Timesheets
                    join p in _dbContext.Projects on t.ProjectId equals p.ProjectId
                    join tp in _dbContext.TimesheetPeriods on t.TimesheetPeriodId equals tp.TimesheetPeriodId
                    join a in _dbContext.Associates on t.AssociateId equals a.AssociateId
                    where t.TimesheetId == timesheetId
                    select new TimesheetsViewModel
                    {
                        TimesheetId = t.TimesheetId,
                        TimesheetPeriodId = t.TimesheetPeriodId,                        
                        ProjectName = p.ProjectName,
                        TimesheetStartDate = t.TimesheetStartDate,
                        TimesheetEndDate = t.TimesheetEndDate,
                        TimesheetStatus = t.Status,
                        TimesheetStatusId = t.TimesheetStatusId,
                        TotalHours = t.TotalHours,
                        CreatedBy = t.CreatedBy,
                        AssociateId = t.AssociateId,
                        AssociateName = a.FullName, //, CurrentAssociateId = myAssociateId  // Add this line
                        BillabilityTypeId = t.BillabilityTypeId,
                        AssociateTimesheetRows = new List<AssociateTimesheetRow>
                        {
                            new AssociateTimesheetRow
                            {
                                AssociateId = t.AssociateId,
                                BillabilityTypeId = t.BillabilityTypeId
                            }
                        }
                    }).ToListAsync();

            var lineItems = await _dbContext.TimesheetLineItems
                .Where(tli => tli.TimesheetId == timesheetId)
                .OrderBy(tli => tli.WorkDate)
                .ToListAsync();
            SetEntryValues(lineItems);

            var timesheetsDetails = new TimesheetsViewModel
            {
                TimesheetsInfo = timesheetsInfo,
                TimesheetLineItems = lineItems,
                IsEditMode = true,
                BillabilityTypes = _dbContext.BillabilityTypes.ToList()
            };

            System.Diagnostics.EventLog.WriteEntry("Application", "EditTimesheet GET called with BillabilityTypeId: " + timesheetsDetails.TimesheetsInfo[0].BillabilityTypeId.ToString());

            return View(timesheetsDetails);
        }

        [HttpPost]
        public async Task<IActionResult> EditTimesheet(TimesheetsViewModel modelUpdatets)
        {
            System.Diagnostics.EventLog.WriteEntry("Application","EditTimesheet POST called with BillabilityTypeId: " + modelUpdatets.TimesheetsInfo[0].BillabilityTypeId.ToString());
            NormalizeLineItems(modelUpdatets.TimesheetLineItems, ModelState);
            if (ModelState.IsValid)
            {
                var timesheet = await _dbContext.Timesheets
                    .FirstOrDefaultAsync(t => t.TimesheetId == modelUpdatets.TimesheetId);

                if (timesheet != null)
                {
                    timesheet.TotalHours = modelUpdatets.TotalHours;
                    timesheet.Status = TimesheetStatusEnum.Submitted.ToString(); // "Submitted";
                    timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.Submitted;
                    timesheet.ModifiedBy = GetCurrentUserId();
                    timesheet.ModifiedOn = DateTime.Now;
                    timesheet.BillabilityTypeId = modelUpdatets.TimesheetsInfo[0].BillabilityTypeId;

                    _dbContext.Timesheets.Update(timesheet);

                    foreach (var item in modelUpdatets.TimesheetLineItems!)
                    {
                        var existingItem = await _dbContext.TimesheetLineItems
                            .FirstOrDefaultAsync(tli => tli.TimesheetLineItemId == item.TimesheetLineItemId);

                        if (existingItem != null)
                        {
                            existingItem.HoursWorked = item.HoursWorked;
                            existingItem.DayCode = item.DayCode;
                            existingItem.Description = modelUpdatets.TimesheetLineItems[0].Description;
                            //existingItem.BillabilityTypeId = modelUpdatets.AssociateTimesheetRows[0].BillabilityTypeId;
                            _dbContext.TimesheetLineItems.Update(existingItem);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("ViewMyTimesheets");
                }
            }
            return View(modelUpdatets);
        }


        #region  Create Timesheets for Single Associate


        // public async Task<IActionResult> CreateTimesheets()
        // {
        //     var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();


        //     var projects = await _dbContext.Projects.ToListAsync();
        //     // ViewBag.Projects = projects.Select(p => new { Value = p.ProjectId, Text = p.ProjectName }).ToList();

        //     var model = new TimesheetsViewModel
        //     {
        //         TimesheetPeriods = timesheetPeriods,
        //         Projects = projects,
        //         ShowTimesheetTable = false
        //         //,ProjectId = 13 //await _dbContext.Allocations.FirstOrDefaultAsync(al => al.AssociateId.Equals(GetCurrentUserId));
        //     };
        //     return View(model);
        // }
        
        // [HttpPost]
        // public IActionResult CreateTimesheets(TimesheetGridViewModel model)
        // {
        //     var periods = _dbContext.TimesheetPeriods.ToList();
        //     model.TimesheetPeriods = periods;

        //     if (model.TimesheetPeriodId > 0)
        //     {
        //         // Prepare the grid view model for the selected period
        //         var periodId = periods.First(p => p.TimesheetPeriodId == model.TimesheetPeriodId).TimesheetPeriodId;
        //         var gridModel = new TimesheetGridViewModel
        //         {
        //             TimesheetLineItems = GenerateEmptyTimesheetLineItems(periodId),
        //             // Populate associates, week dates, etc.
        //         };
        //         model.TimesheetLineItems = gridModel.TimesheetLineItems;
        //         model.ShowTimesheetTable = true;
        //     }
        //     return View(model);
        // }

        public async Task<IActionResult> CreateTimesheets(int periodId)
        {
            if (periodId == 0)
            {
                periodId = GetCurrentPeriodId();
            }

            var projects = await _dbContext.Projects.ToListAsync();
            var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();
            // ViewBag.Projects = projects.Select(p => new { Value = p.ProjectId, Text = p.ProjectName }).ToList();

            var model = new TimesheetsViewModel
            {
                TimesheetPeriodId = periodId,
                TimesheetStartDate = GetStartDate(periodId),
                TimesheetEndDate = GetEndDate(periodId),
                TimesheetLineItems = GenerateEmptyTimesheetLineItems(periodId),
                Projects = projects,
                BillabilityTypes = billabilityTypes,
                BillabilityTypeId = (int)BillabilityTypesEnum.Billable
                //,ProjectId = 13 //await _dbContext.Allocations.FirstOrDefaultAsync(al => al.AssociateId.Equals(GetCurrentUserId));
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateTimesheets(TimesheetsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var timesheet = new Timesheet
                {
                    TimesheetId = Guid.NewGuid(),
                    TimesheetPeriodId = model.TimesheetPeriodId,
                    AssociateId = GetCurrentUserId(),
                    ProjectId = model.ProjectId,
                    TimesheetStartDate = model.TimesheetStartDate,
                    TimesheetEndDate = model.TimesheetEndDate,
                    TotalHours = model.TotalHours,
                    Status = TimesheetStatusEnum.Submitted.ToString(), //"Submitted",
                    TimesheetStatusId = (int)TimesheetStatusEnum.Submitted,
                    CreatedBy = GetCurrentUserId(),
                    CreatedOn = DateTime.Now, 
                    BillabilityTypeId = model.BillabilityTypeId
                };

                _dbContext.Timesheets.Add(timesheet);

                foreach (var item in model.TimesheetLineItems!)
                {
                    item.TimesheetLineItemId = Guid.NewGuid();
                    item.TimesheetId = timesheet.TimesheetId;
                    item.Description = model.Description;
                    _dbContext.TimesheetLineItems.Add(item);
                }

                _dbContext.SaveChanges();
                return RedirectToAction("ViewTimesheets");
            }
            return View(model);
        }

        #endregion

        #region Create Multiple Timesheets

        public async Task<IActionResult> CreateMultipleTimesheets()
        {
            var projects = await _dbContext.Projects.ToListAsync();
            var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();
            
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();            

            var model = new TimesheetsViewModel
            {
                TimesheetPeriodId = 0,
                ProjectId = 0,
                TimesheetPeriods = timesheetPeriods,
                Projects = projects,
                BillabilityTypes = billabilityTypes,
                IncludeResignationAssociates = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultipleTimesheets(TimesheetsViewModel model, string action)
        {
            //System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets POST called with action: " + action, System.Diagnostics.EventLogEntryType.Information);
            // action will be "load" when user clicked Load button,
            // "submitAll" when user clicked Submit All Timesheets,
            // or null/empty if the form was submitted via Enter (no button value).
            if (string.Equals(action, "load", StringComparison.OrdinalIgnoreCase))
            {
                // Populate model.TimesheetLineItems (load rows for selected Period/Project)
                // Return the view so the partial renders the table for editing.
                await PopulateTimesheetLines(model);
                // System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets POST after PopulateTimesheetLines with AssociateTimesheetRows count: " + (model.AssociateTimesheetRows != null ? model.AssociateTimesheetRows.Count.ToString() : "null"), System.Diagnostics.EventLogEntryType.Information);
                ModelState.Clear();  // Clear ModelState so new values show
                return View(model);
            }

            //System.Diagnostics.EventLog.WriteEntry("Application", "model.AssociateTimesheetRows: " + model.AssociateTimesheetRows.Count.ToString(), System.Diagnostics.EventLogEntryType.Information);

            if (string.Equals(action, "submitAll", StringComparison.OrdinalIgnoreCase))
            {
                    if (!NormalizeLineItems(
                        model.AssociateTimesheetRows?.SelectMany(row => row.TimesheetLineItems ?? new List<TimesheetLineItem>()),
                        ModelState))
                    {
                        return View(model);
                    }

                // Validate & save all timesheets
                if (!ModelState.IsValid)
                {
                    await PopulateTimesheetLines(model); // ensure partial can re-render with errors
                    ModelState.Clear();  // Clear ModelState so new values show
                    return View(model);
                }

                await SaveAllTimesheets(model);
                return RedirectToAction("ViewTeamMemberTimesheets"); // or wherever
            }

            // Fallback when no button value is present (user pressed Enter).
            // Choose a sensible default or treat it as invalid.
            await PopulateTimesheetLines(model);
            ModelState.Clear();  // Clear ModelState so new values show
            return View(model);
        }

        private async Task PopulateTimesheetLines(TimesheetsViewModel model)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets called with periodId:  " + model.TimesheetPeriodId.ToString(), System.Diagnostics.EventLogEntryType.Information);

            // System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets called with projectId:  " + model.ProjectId.ToString(), System.Diagnostics.EventLogEntryType.Information);
            var projects = await _dbContext.Projects.ToListAsync();
            var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();

            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();
            //System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets called with timesheetperiods:  " + timesheetPeriods.Count.ToString(), System.Diagnostics.EventLogEntryType.Information);
            //var associates = await _dbContext.Associates.ToListAsync();

            var associatesAllocated = new List<Associate>();
            foreach (var project in projects.Where(p => p.ProjectId == model.ProjectId))
            {
                var allocatedAssociates = GetAllocatedAssociates(project.ProjectId, model.IncludeResignationAssociates);
                foreach (var assoc in allocatedAssociates)
                {
                    // Verify if timesheet already exists for this associate, period, and project and add to model only if not exists
                    var existingTimesheet = _dbContext.Timesheets
                        .FirstOrDefault(t => t.AssociateId == assoc.AssociateId
                                        && t.TimesheetPeriodId == model.TimesheetPeriodId
                                        && t.ProjectId == model.ProjectId);
                    if (existingTimesheet != null)
                    {
                        continue; // Skip adding this associate as they already have a timesheet for this period/project
                    }
                    if (!associatesAllocated.Any(a => a.AssociateId == assoc.AssociateId))
                    {
                        associatesAllocated.Add(assoc);
                    }
                }
            }
            //System.Diagnostics.EventLog.WriteEntry("Application", "CreateMultipleTimesheets called with associates:  " + associates.Count.ToString(), System.Diagnostics.EventLogEntryType.Information);


            model.TimesheetPeriodId = model.TimesheetPeriodId;
            model.TimesheetStartDate = GetStartDate(model.TimesheetPeriodId);
            model.TimesheetEndDate = GetEndDate(model.TimesheetPeriodId);
            // model.TimesheetLineItems = GenerateEmptyTimesheetLineItems(model.TimesheetPeriodId);
            model.Projects = projects;
            model.Associates = associatesAllocated; // associates,
            //Associates = GetAllocatedAssociates(13), // Initially empty, will be populated via AJAX based on selected project
            model.TimesheetPeriods = timesheetPeriods;
            model.BillabilityTypes = billabilityTypes;
            // model.AssociateTimesheetRows = new List<AssociateTimesheetRow>()
            // {
            //     new AssociateTimesheetRow()
            //     {
            //         AssociateId = 0,
            //         TimesheetLineItems = GenerateEmptyTimesheetLineItems(model.TimesheetPeriodId)
            //     }
            // };

            model.AssociateTimesheetRows = null;

            foreach (var associate in associatesAllocated)
            {
                var associateRow = new AssociateTimesheetRow
                {
                    AssociateId = associate.AssociateId,
                    TimesheetLineItems = await GetTimesheetLineItemsWithAbsencesAsync(
                        model.TimesheetPeriodId, associate.AssociateId),
                    BillabilityTypeId = (int)BillabilityTypesEnum.Billable
                };
                if (model.AssociateTimesheetRows == null)
                {
                    model.AssociateTimesheetRows = new List<AssociateTimesheetRow>();
                }
                model.AssociateTimesheetRows.Add(associateRow);
            }            
        }

        private async Task SaveAllTimesheets(TimesheetsViewModel model)
        {
            foreach (var associateRow in model.AssociateTimesheetRows!)
            {
                // System.Diagnostics.EventLog.WriteEntry("Application", "Saving timesheet for AssociateId: " + associateRow.AssociateId.ToString(), System.Diagnostics.EventLogEntryType.Information);
                var timesheet = new Timesheet
                {
                    TimesheetId = Guid.NewGuid(),
                    TimesheetPeriodId = model.TimesheetPeriodId,
                    AssociateId = associateRow.AssociateId,
                    ProjectId = model.ProjectId,
                    TimesheetStartDate = model.TimesheetStartDate,
                    TimesheetEndDate = model.TimesheetEndDate,
                    TotalHours = associateRow.TimesheetLineItems!.Sum(tli => tli.HoursWorked),
                    Status = TimesheetStatusEnum.Submitted.ToString(),
                    CreatedBy = GetCurrentUserId(),
                    CreatedOn = DateTime.Now,
                    TimesheetStatusId = (int)TimesheetStatusEnum.Submitted,
                    BillabilityTypeId = associateRow.BillabilityTypeId
                };

                _dbContext.Timesheets.Add(timesheet);

                foreach (var item in associateRow.TimesheetLineItems!)
                {
                    item.TimesheetLineItemId = Guid.NewGuid();
                    item.TimesheetId = timesheet.TimesheetId;
                    item.Description = model.Description;
                    //item.BillabilityTypeId = associateRow.BillabilityTypeId;
                    _dbContext.TimesheetLineItems.Add(item);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region  Supporting Functions

        private int GetCurrentPeriodId()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var period = _dbContext.TimesheetPeriods
                .FirstOrDefault(p => today >= p.WeekStartDate && today <= p.WeekEndDate);

            if (period == null)
                throw new Exception("No matching timesheet period found for today.");

            return period.TimesheetPeriodId;
        }        

        private DateOnly GetStartDate(int periodId)
        {
            System.Diagnostics.Debug.WriteLine("GetStartDate called with periodId: " + periodId);
            var period = _dbContext.TimesheetPeriods.FirstOrDefault(p => p.TimesheetPeriodId == periodId);
            if (period == null)
            {
                throw new ArgumentException("Invalid TimesheetPeriodId.");
            }

            return period.WeekStartDate;
        }

        private DateOnly GetEndDate(int periodId)
        {
            var period = _dbContext.TimesheetPeriods.FirstOrDefault(p => p.TimesheetPeriodId == periodId);
            if (period == null)
            {
                throw new ArgumentException("Invalid TimesheetPeriodId.");
            }

            return period.WeekEndDate;
        }

        private int GetCurrentUserId()
        {
            if(User.Identity!.IsAuthenticated)
            {
                if(User.Identity != null & @User.Identity!.IsAuthenticated)
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
            }

            throw new UnauthorizedAccessException("User ID claim not found or invalid.");
        }

        private List<TimesheetLineItem> GenerateEmptyTimesheetLineItems(int periodId)
        {
            var period = _dbContext.TimesheetPeriods.FirstOrDefault(p => p.TimesheetPeriodId == periodId);
            if (period == null)
            {
                throw new ArgumentException("Invalid TimesheetPeriodId.");
            }

            var TimesheetLineItems = new List<TimesheetLineItem>();
            var currentDate = period.WeekStartDate;
            var billabilityTypes = _dbContext.BillabilityTypes.ToList();

            while (currentDate <= period.WeekEndDate)
            {
                TimesheetLineItems.Add(new TimesheetLineItem
                {
                    WorkDate = currentDate,
                    HoursWorked = 0,
                    Description = string.Empty 
                    // ,
                    // BillabilityTypes = billabilityTypes,
                    // BillabilityTypeId = billabilityTypes.FirstOrDefault()?.BillabilityTypeId ?? 1
                });

                currentDate = currentDate.AddDays(1);
            }

            return TimesheetLineItems;
        }

        private async Task<List<TimesheetLineItem>> GetTimesheetLineItemsWithAbsencesAsync(
            int periodId, int associateId)
        {
            var lineItems = GenerateEmptyTimesheetLineItems(periodId);
            var startDate = lineItems.First().WorkDate;
            var endDate = lineItems.Last().WorkDate;

            var leaveRequests = await _dbContext.LeaveRequests
                .Where(l => l.AssociateId == associateId
                    && l.LeaveStatusId == (int)LeaveStatuses.Approved
                    && l.StartDate <= endDate
                    && l.EndDate >= startDate)
                .Select(l => new { l.StartDate, l.EndDate, l.IncludeWeekends })
                .ToListAsync();

            var holidayDates = await _dbContext.Holidays
                .Where(h => h.HolidayDate >= startDate && h.HolidayDate <= endDate)
                .Select(h => h.HolidayDate)
                .ToListAsync();

            foreach (var item in lineItems)
            {
                var isLeaveDay = leaveRequests.Any(l =>
                    item.WorkDate >= l.StartDate && item.WorkDate <= l.EndDate &&
                    (l.IncludeWeekends || (item.WorkDate.DayOfWeek != DayOfWeek.Saturday &&
                        item.WorkDate.DayOfWeek != DayOfWeek.Sunday)));

                var isWeekend = IsWeekend(item.WorkDate);
                item.DayCode = isWeekend
                    ? "W"
                    : isLeaveDay
                        ? "L"
                        : holidayDates.Contains(item.WorkDate) ? "H" : null;
                item.EntryValue = GetEntryValue(item);
            }

            return lineItems;
        }

        private static void SetEntryValues(IEnumerable<TimesheetLineItem> lineItems)
        {
            foreach (var item in lineItems)
            {
                item.EntryValue = GetEntryValue(item);
            }
        }

        private static string GetEntryValue(TimesheetLineItem item)
        {
            if (IsWeekend(item.WorkDate) && item.HoursWorked <= 0)
            {
                return "W";
            }

            if (IsWeekend(item.WorkDate) && item.HoursWorked > 0)
            {
                return item.HoursWorked.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            }

            return item.DayCode is "H" or "L" or "W"
                ? item.DayCode
                : item.HoursWorked.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static bool IsWeekend(DateOnly date)
        {
            return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        }

        private static bool NormalizeLineItems(
            IEnumerable<TimesheetLineItem>? lineItems, ModelStateDictionary modelState)
        {
            if (lineItems == null)
            {
                modelState.AddModelError("TimesheetLineItems", "Timesheet entries are missing.");
                return false;
            }

            var isValid = true;
            foreach (var item in lineItems)
            {
                var value = item.EntryValue?.Trim();
                var isWeekend = IsWeekend(item.WorkDate);
                if (string.IsNullOrEmpty(value))
                {
                    item.DayCode = isWeekend ? "W" : null;
                    item.HoursWorked = 0;
                    continue;
                }

                if (string.Equals(value, "H", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(value, "L", StringComparison.OrdinalIgnoreCase))
                {
                    if (isWeekend)
                    {
                        modelState.AddModelError(
                            "TimesheetLineItems",
                            $"Weekend date {item.WorkDate:dd MMM yyyy} cannot be marked as H or L. Enter hours or W.");
                        isValid = false;
                        continue;
                    }

                    item.DayCode = value!.ToUpperInvariant();
                    item.HoursWorked = 0;
                    continue;
                }

                if ((decimal.TryParse(value, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out var hours) ||
                    decimal.TryParse(value, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out hours)) &&
                    hours >= 0 && hours <= 24)
                {
                    item.DayCode = hours > 0 ? "Hours" : isWeekend ? "W" : null;
                    item.HoursWorked = hours;
                    continue;
                }

                if (string.Equals(value, "W", StringComparison.OrdinalIgnoreCase) && isWeekend)
                {
                    item.DayCode = "W";
                    item.HoursWorked = 0;
                    continue;
                }

                modelState.AddModelError("TimesheetLineItems", $"'{item.EntryValue}' is not a valid hour value, H, or L.");
                isValid = false;
            }

            return isValid;
        }

        private List<Associate> GetAllocatedAssociates(int projectId, bool includeResignationAssociates)
        {
            // Fetch associates allocated to the specified project
            var project = _dbContext.Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (project == null)
            {
                throw new ArgumentException("Invalid ProjectId.");
            }
            // Retrieve associates allocated to the project with AssociateId and FullName


            // var associates = _dbContext.Allocations
            //     .Where(a => a.ProjectId == projectId)
            //     .Select(a => new Associate
            //     {
            //         AssociateId = a.AssociateId,
            //         FullName = a.FullName
            //     }).ToList();

            var associates = from a in _dbContext.Associates
                             join al in _dbContext.Allocations on a.AssociateId equals al.AssociateId                             
                             where al.ProjectId == projectId && (includeResignationAssociates == false ? al.IsActive == true : true)
                             select a;

            return associates.ToList();
        }

        private int GetAssociateIdFromUserId(int userId)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var associate = _dbContext.Associates.FirstOrDefault(a => a.Email == userEmail);
            if (associate == null)
            {
                throw new InvalidOperationException($"No associate found for user ID {userId}");
            }
            return associate.AssociateId;
        }

        #endregion

        public async Task<IActionResult> CreateMyTimesheets()
        {
            var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();

            var projects = await _dbContext.Projects.ToListAsync();
            var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();
            // ViewBag.Projects = projects.Select(p => new { Value = p.ProjectId, Text = p.ProjectName }).ToList();
            // System.Diagnostics.EventLog.WriteEntry("Application", "No of Timesheet Periods: " + timesheetPeriods.Count.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "No of Projects: " + projects.Count.ToString());
            var model = new TimesheetsViewModel
            {
                TimesheetPeriodId = 0,
                ProjectId = 0,
                TimesheetPeriods = timesheetPeriods,
                Projects = projects,
                BillabilityTypes = billabilityTypes,
                BillabilityTypeId = (int)BillabilityTypesEnum.Billable
                //,ProjectId = 13 //await _dbContext.Allocations.FirstOrDefaultAsync(al => al.AssociateId.Equals(GetCurrentUserId));
            };
            return View(model);
        }

        // [HttpPost]
        // public async Task<IActionResult> CreateTimesheets1(int timesheetPeriodId, int projectId)
        // {
        //     var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();

        //     var projects = await _dbContext.Projects.ToListAsync();

        //     var model = new TimesheetsViewModel
        //     {
        //         TimesheetPeriodId = timesheetPeriodId,
        //         ProjectId = projectId,
        //         TimesheetPeriods = timesheetPeriods,
        //         Projects = projects,
        //         TimesheetLineItems = GenerateEmptyTimesheetLineItems(timesheetPeriodId)
        //     };
        //     return View(model);

        // }

        [HttpPost]
        public async Task<IActionResult> CreateMyTimesheets(TimesheetsViewModel model, string action)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "Model State: " + ModelState.IsValid.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Timesheet Reporting Period Id: " + model.TimesheetPeriodId.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Project Id: " + model.ProjectId.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Timesheet Start Date: " + model.TimesheetStartDate.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Timesheet End Date: " + model.TimesheetEndDate.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Total Hours: " + model.TotalHours.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Description: " + model.Description?.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "Associate Id: " + GetCurrentUserId().ToString());

            if (ModelState.IsValid)
            {
                // System.Diagnostics.EventLog.WriteEntry("Application", "No of Timesheet Line Items: " + model.TimesheetLineItems?.Count.ToString());
                // if (model.TimesheetLineItems != null && model.TimesheetLineItems.Count > 0)
                if (string.Equals(action, "submitmyts", StringComparison.OrdinalIgnoreCase))
                {
                    if (!NormalizeLineItems(model.TimesheetLineItems, ModelState))
                    {
                        return View(model);
                    }

                    // System.Diagnostics.EventLog.WriteEntry("Application", "Before Saving Timesheet");
                    var timesheet = new Timesheet
                    {
                        TimesheetId = Guid.NewGuid(),
                        TimesheetPeriodId = model.TimesheetPeriodId,
                        AssociateId = GetCurrentUserId(),
                        ProjectId = model.ProjectId,
                        TimesheetStartDate = model.TimesheetStartDate,
                        TimesheetEndDate = model.TimesheetEndDate,
                        TotalHours = model.TimesheetLineItems!.Sum(tli => tli.HoursWorked), // model.TotalHours,
                        Status = TimesheetStatusEnum.Submitted.ToString(),
                        CreatedBy = GetCurrentUserId(),
                        CreatedOn = DateTime.Now,
                        TimesheetStatusId = (int)TimesheetStatusEnum.Submitted,
                        BillabilityTypeId = model.AssociateTimesheetRows[0].BillabilityTypeId
                    };

                    _dbContext.Timesheets.Add(timesheet);

                    // System.Diagnostics.EventLog.WriteEntry("Application", "Before Saving Timesheet Line Items");

                    foreach (var item in model.TimesheetLineItems!)
                    {
                        item.TimesheetLineItemId = Guid.NewGuid();
                        item.TimesheetId = timesheet.TimesheetId;
                        item.Description = model.Description;
                        _dbContext.TimesheetLineItems.Add(item);
                    }

                    _dbContext.SaveChanges();
                    return RedirectToAction("ViewMyTimesheets");
                }
                else
                {
                    
                    // System.Diagnostics.EventLog.WriteEntry("Application", "Timesheet Reporting Period Id: " + model.TimesheetPeriodId.ToString());
                    var timesheetPeriods = await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive).ToListAsync();

                    var projects = await _dbContext.Projects.ToListAsync();
                    var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();

                    model.TimesheetPeriodId = model.TimesheetPeriodId;
                    model.ProjectId = model.ProjectId;
                    model.TimesheetStartDate = GetStartDate(model.TimesheetPeriodId);
                    model.TimesheetEndDate = GetEndDate(model.TimesheetPeriodId);
                    model.TimesheetPeriods = timesheetPeriods;
                    model.Projects = projects;
                    model.BillabilityTypes = billabilityTypes;
                    model.BillabilityTypeId = 0;
                    model.AssociateTimesheetRows = new List<AssociateTimesheetRow>
                    {
                        new AssociateTimesheetRow
                        {
                            AssociateId = GetCurrentUserId(),
                            BillabilityTypeId = (int)BillabilityTypesEnum.Billable
                        }
                    };
                    var timesheetExists = _dbContext.Timesheets.Any(ts =>
                        ts.TimesheetPeriodId == model.TimesheetPeriodId &&
                        ts.AssociateId == GetCurrentUserId());

                    if (timesheetExists)
                    {
                        // Timesheet already exists for this period and user
                        // it should display timesheet already exists message and redirect to view my timesheets page
                        TempData["ErrorMessage"] = "Timesheet already exists for the selected period.";                        
                    }
                    else
                    {
                        model.TimesheetLineItems = await GetTimesheetLineItemsWithAbsencesAsync(
                            model.TimesheetPeriodId, GetCurrentUserId());
                    }
                    ModelState.Clear();  // Clear ModelState so new values show
                    return View(model);
                }

            }
            return View(model);
        }

        #region  ReOpen, Approve and Reject Timesheets

        public IActionResult ApproveTimesheet(Guid timesheetId, string origin="", int tsperiodId = 0, int tsassociateId = 0)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "ApproveTimesheet called for TS Period: " + tsperiodId.ToString(), System.Diagnostics.EventLogEntryType.Information);
            var timesheet = _dbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet != null)
            {
                timesheet.Status = TimesheetStatusEnum.Approved.ToString();
                timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.Approved;
                timesheet.ModifiedBy = GetCurrentUserId();
                timesheet.ModifiedOn = DateTime.Now;
                _dbContext.SaveChanges();
            }
            if (!string.IsNullOrEmpty(origin) && origin == "ViewTeamMemberTimesheets")
            {
                return RedirectToAction("ViewTeamMemberTimesheets", new {  tsperiodId = tsperiodId, tsassociateId = tsassociateId });
            }
            return RedirectToAction("ViewMyTimesheets");
        }

        public IActionResult RejectTimesheet(Guid timesheetId, string origin="", int tsperiodId = 0, int tsassociateId = 0)
        {
            var timesheet = _dbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet != null)
            {
                timesheet.Status = TimesheetStatusEnum.Rejected.ToString();
                timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.Rejected;
                timesheet.ModifiedBy = GetCurrentUserId();
                timesheet.ModifiedOn = DateTime.Now;
                _dbContext.SaveChanges();
            }
            if (!string.IsNullOrEmpty(origin) && origin == "ViewTeamMemberTimesheets")
            {
                return RedirectToAction("ViewTeamMemberTimesheets", new { tsperiodId = tsperiodId, tsassociateId = tsassociateId });
            }
            return RedirectToAction("ViewMyTimesheets");
        }

        public IActionResult ReOpenTimesheet(Guid timesheetId, string origin="", int tsperiodId = 0, int tsassociateId = 0)
        {
            var timesheet = _dbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet != null)
            {
                timesheet.Status = TimesheetStatusEnum.ReOpen.ToString();
                timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.ReOpen;
                timesheet.ModifiedBy = GetCurrentUserId();
                timesheet.ModifiedOn = DateTime.Now;
                _dbContext.SaveChanges();
            }
            if (!string.IsNullOrEmpty(origin) && origin == "ViewTeamMemberTimesheets")
            {
                return RedirectToAction("ViewTeamMemberTimesheets", new { tsperiodId = tsperiodId, tsassociateId = tsassociateId });
            }
            return RedirectToAction("ViewMyTimesheets");
        }

        public IActionResult BulkApproveTimesheet(List<Guid> selectedTimesheets, string origin="", int tsperiodId = 0, int tsassociateId = 0)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "BulkApproveTimesheet called with selectedTimesheets count: " + (selectedTimesheets != null ? selectedTimesheets.Count.ToString() : "null"), System.Diagnostics.EventLogEntryType.Information);
            foreach (var timesheetId in selectedTimesheets!)
            {
                var timesheet = _dbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
                if (timesheet != null)
                {
                    timesheet.Status = TimesheetStatusEnum.Approved.ToString();
                    timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.Approved;
                    timesheet.ModifiedBy = GetCurrentUserId();
                    timesheet.ModifiedOn = DateTime.Now;
                    _dbContext.SaveChanges();
                }
            }
            if (!string.IsNullOrEmpty(origin) && origin == "ViewTeamMemberTimesheets")
            {
                return RedirectToAction("ViewTeamMemberTimesheets", new {  tsperiodId = tsperiodId, tsassociateId = tsassociateId });
            }
            return RedirectToAction("ViewMyTimesheets");
        }

        public IActionResult BulkRejectTimesheet(List<Guid> selectedTimesheets, string origin="", int tsperiodId = 0, int tsassociateId = 0)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "BulkApproveTimesheet called with selectedTimesheets count: " + (selectedTimesheets != null ? selectedTimesheets.Count.ToString() : "null"), System.Diagnostics.EventLogEntryType.Information);
            foreach (var timesheetId in selectedTimesheets!)
            {
                var timesheet = _dbContext.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
                if (timesheet != null)
                {
                    timesheet.Status = TimesheetStatusEnum.Rejected.ToString();
                    timesheet.TimesheetStatusId = (int)TimesheetStatusEnum.Rejected;
                    timesheet.ModifiedBy = GetCurrentUserId();
                    timesheet.ModifiedOn = DateTime.Now;
                    _dbContext.SaveChanges();
                }
            }
            if (!string.IsNullOrEmpty(origin) && origin == "ViewTeamMemberTimesheets")
            {
                return RedirectToAction("ViewTeamMemberTimesheets", new {  tsperiodId = tsperiodId, tsassociateId = tsassociateId });
            }
            return RedirectToAction("ViewMyTimesheets");
        }
        
        
        #endregion

    }
}

