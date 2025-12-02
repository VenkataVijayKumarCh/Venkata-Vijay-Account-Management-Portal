using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using Microsoft.EntityFrameworkCore.Query;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;
using Humanizer;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager, Roles.User)]
    public class TimesheetPeriodsController : Controller
    {
        private readonly ILogger<TimesheetPeriodsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public TimesheetPeriodsController(ILogger<TimesheetPeriodsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // public IActionResult Index()
        // {
        //     var periods = _dbContext.TimesheetPeriods.OrderByDescending(p => p.WeekStartDate).ToList();
        //     return View(periods);
        // }

        // public IActionResult Create()
        // {
        //     return View();
        // }

        // [HttpPost]
        // public IActionResult Create(TimesheetPeriod model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         //model.TimesheetPeriodId = Guid.NewGuid();
        //         _dbContext.TimesheetPeriods.Add(model);
        //         _dbContext.SaveChanges();
        //         return RedirectToAction("Index");
        //     }
        //     return View(model);
        // }

        public async Task<IActionResult> ManageTimesheetPeriods()
        {
            var tsperiods = new TimesheetPeriodsViewModel()
            {
                TimesheetPeriods = await _dbContext.TimesheetPeriods
                    .OrderBy(tp => tp.TimesheetPeriodId)
                    .ToListAsync()
            };

            return View(tsperiods);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTimesheetPeriods(TimesheetPeriodsViewModel tspModel)
        {
            if(ModelState.IsValid)
            {
                var activetsperiodIds = new List<int>();
                foreach (var tsperiod in tspModel.TimesheetPeriods!)
                {
                    if (tsperiod.IsActive)
                    {
                        activetsperiodIds.Add(tsperiod.TimesheetPeriodId);
                    }
                }               

                // First deactivate all
                await _dbContext.TimesheetPeriods.Where(tp => tp.IsActive == true).ExecuteUpdateAsync(setters => setters
                    .SetProperty(tp => tp.IsActive, tp => false)
                );

                // Then activate selected ones
                await _dbContext.TimesheetPeriods
                .Where(tp => activetsperiodIds.Contains(tp.TimesheetPeriodId))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(tp => tp.IsActive, tp => true)
                );
                _dbContext.SaveChanges();

                return RedirectToAction("ManageTimesheetPeriods");
            }

            return RedirectToAction("ManageTimesheetPeriods");
        }
    }
}

