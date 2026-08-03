using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager)]
    // [Authorize(Roles = "Admin")]
    // [Authorize]    
    public class ManagementController : Controller
    {


        private readonly ILogger<ManagementController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ManagementController(ILogger<ManagementController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // public async Task<IActionResult> Index()
        // {
        //     // var vm = new ManagementDashboardViewModel
        //     // {
        //     //     Accounts = await _context.Accounts.ToListAsync(),
        //     //     Projects = await _context.Projects.Include(p => p.Account).ToListAsync(),
        //     //     Associates = await _context.Associates.ToListAsync(),
        //     //     Allocations = await _context.Allocations
        //     //         .Include(a => a.Associate)
        //     //         .Include(a => a.Project)
        //     //         .ThenInclude(p => p.Account)
        //     //         .ToListAsync()
        //     // };

        //     // return View(vm);
        //     return View();
        // }

        public async Task<IActionResult> PortfolioManagement()
        {
            // retrieve the number of accounts, projects, associates, and allocations
            var vm = new ManagementDashboardViewModel
            {
                Accounts = await _dbContext.Accounts.ToListAsync(),
                Projects = await _dbContext.Projects.ToListAsync(),
                Associates = await _dbContext.Associates.ToListAsync(),
                Allocations = await _dbContext.Allocations.ToListAsync(),
                Timesheets = await _dbContext.Timesheets.ToListAsync(),
                RevenueGenerated = await CalculateRevenueGenerated() ?? 0.00m
            };

            return View(vm);
        }

        public async Task<decimal?> CalculateRevenueGenerated()
        {
            // Implement your logic to calculate revenue generated
            // decimal? revenue = 0.00m;

            // // Example logic (replace with actual calculation)
            // var timesheets = _dbContext.Timesheets.ToList();
            // foreach (var timesheet in timesheets)
            // {
            //     var billrate = await (from al in _dbContext.Allocations
            //         join ar in _dbContext.AllocationRates on al.AllocationId equals ar.AllocationId
            //         where al.AssociateId == timesheet.AssociateId
            //         select ar.AllocationBillRate).FirstOrDefaultAsync();

            //    revenue += timesheet.TotalHours * billrate;
            // }     

            var revenue = await (
                from ts in _dbContext.Timesheets
                join al in _dbContext.Allocations on ts.AssociateId equals al.AssociateId
                join ar in _dbContext.AllocationRates on al.AllocationId equals ar.AllocationId
                select ts.TotalHours * ar.AllocationBillRate // * (ar.AllocationPercentage / 100m)
            ).SumAsync();       

            return revenue;
        }
    }
}