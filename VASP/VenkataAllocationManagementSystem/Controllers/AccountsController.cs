using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin)]
    public class AccountsController : Controller
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AccountsController(ILogger<AccountsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> AccountManagement()
        {
            var vm = new ManagementDashboardViewModel
            {
                Accounts = await _dbContext.Accounts.ToListAsync()
            };
            return View(vm);
        }

        // GET: Accounts/Create
        public IActionResult CreateAccount()
        {
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccount(Account account)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Accounts.Add(account);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(AccountManagement));
            }
            return View(account);
        }

        // GET: Accounts/EditAccount/5
        public async Task<IActionResult> EditAccount(int AccountId)
        {
            var account = await _dbContext.Accounts.FindAsync(AccountId);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/EditAccount/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount(int AccountId, Account account)
        {
            if (AccountId != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(account);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(AccountManagement));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.Accounts.AnyAsync(a => a.AccountId == AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(account);
        }

        public async Task<IActionResult> ViewAccount(int AccountId)
        {
            var account = await _dbContext.Accounts.FindAsync(AccountId);
            if (account == null)
            {
                return NotFound();
            }
            var projects = await _dbContext.Projects
                .Where(p => p.AccountId == AccountId)
                .ToListAsync();
            var accountProjectInfo = new AccountManagementViewModel
            {
                Account = account,
                Projects = new List<Project>(projects)
            };
            return View(accountProjectInfo);
        }

        // GET: Accounts/DeleteAccount/5
        public async Task<IActionResult> DeleteAccount(int AccountId)
        {
            var account = await _dbContext.Accounts.FindAsync(AccountId);
            if (account == null)
            {
                return NotFound();
            }
            else
            {
                account.IsActive = false; // Soft delete
                _dbContext.Update(account);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(AccountManagement));
            }
            //return View(account);
        }

    }
}