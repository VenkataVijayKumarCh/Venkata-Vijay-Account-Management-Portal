using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization.Policy;
// Ensure you have this namespace for forms authentication

namespace VenkataAllocationManagementSystem.Controllers;

[AllowAnonymous]
public class AccessController : Controller
{
    private readonly ILogger<AccessController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AccessController(ILogger<AccessController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Login()
    {
        // System.Diagnostics.EventLog.WriteEntry("Application", "Accessing Login Page");
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("PortfolioManagement", "Management");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AppLogin(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(user.Password))
        {
            ModelState.AddModelError(string.Empty, "UserEmail and Password are required.");
            return View("Login");
        }
        else
        {
            _logger.LogInformation("User Email: {UserEmail} Password: {Password}", user.UserEmail, user.Password);

            var validUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserEmail == user.UserEmail && u.Password == user.Password);
            // System.Diagnostics.EventLog.WriteEntry("Application", "User Id: " + validUser.UserId.ToString());
            if (validUser != null)
            {
                var userRoles = (from userroles in _dbContext.UserRoles
                                 join roles in _dbContext.Roles on userroles.RoleId equals roles.RoleId
                                 where userroles.UserId == validUser.UserId
                                 select roles.RoleName).ToList();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, validUser.UserName),
                    new Claim(ClaimTypes.GivenName, validUser.FirstName),
                    new Claim(ClaimTypes.Surname, validUser.LastName),
                    new Claim(ClaimTypes.Email, validUser.UserEmail),
                    new Claim("UserId", validUser.UserId.ToString())                    
                };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("PortfolioManagement", "Management");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Login");
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Access");
    }
    
    public IActionResult AccessDenied()
    {
        return View();
    }
};