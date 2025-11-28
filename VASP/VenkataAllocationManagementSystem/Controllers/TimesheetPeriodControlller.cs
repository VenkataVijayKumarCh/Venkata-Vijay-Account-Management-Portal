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

namespace VenkataAllocationManagementSystem.Controllers
{
    public class TimesheetPeriodsController : Controller
    {
        private readonly ILogger<TimesheetPeriodsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public TimesheetPeriodsController(ILogger<TimesheetPeriodsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var periods = _dbContext.TimesheetPeriods.OrderByDescending(p => p.WeekStartDate).ToList();
            return View(periods);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TimesheetPeriod model)
        {
            if (ModelState.IsValid)
            {
                //model.TimesheetPeriodId = Guid.NewGuid();
                _dbContext.TimesheetPeriods.Add(model);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}

