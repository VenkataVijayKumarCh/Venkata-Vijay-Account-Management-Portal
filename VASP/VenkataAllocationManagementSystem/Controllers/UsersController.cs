using Microsoft.AspNetCore.Mvc;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;
// Ensure you have this namespace for forms authentication

namespace VenkataAllocationManagementSystem.Controllers;

[CustomAuthorize(Roles.Admin)]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> UserManagement()
    {
        var userDetails = new UserManagementViewModel()
        {
            Users = await _dbContext.Users.ToListAsync()
        };
        return View(userDetails);
    }

    public async Task<IActionResult> CreateUser()
    {
        var roles = await _dbContext.Roles.ToListAsync();

        var createUser = new UserManagementViewModel
        {
            AvailableRoles = roles //, SelectedRoles = roles
        };
        return View(createUser);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(UserManagementViewModel userInfo)
    {
        if (ModelState.IsValid)
        {
            userInfo.User!.CreatedAt = DateTime.Now;
            userInfo.User.IsActive = true;
            _dbContext.Users.Add(userInfo.User);
            _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
            _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
            _dbContext.CurrentUser = User.Identity!.Name!;
            await _dbContext.SaveChangesAsync();
            // System.Diagnostics.EventLog.WriteEntry("Application", $"User Created with userid : {userInfo.User.UserId}");
            if (userInfo.User.UserId > 0)
            {
                if (userInfo.SelectedRoles == null || userInfo.SelectedRoles.Any())
                {
                    // System.Diagnostics.EventLog.WriteEntry("Application", "No Selected User Roles Mapping");
                    ModelState.AddModelError("SelectedRoles", "Please select at least one role.");
                }


                foreach (var roleId in userInfo.SelectedRoles!)
                {
                    // System.Diagnostics.EventLog.WriteEntry("Application", "User Roles Mapping Started");
                    UserRole userRole = new UserRole()
                    {
                        UserId = userInfo.User.UserId,
                        RoleId = roleId,
                        CreatedOn = DateTime.Now
                    };
                    await _dbContext.UserRoles.AddAsync(userRole);
                }
                _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                _dbContext.CurrentUser = User.Identity!.Name!;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }
        return View(userInfo);
    }

    // GET: Users/EditUser/5
    public async Task<IActionResult> EditUser(int UserId)
    {
        var userInfo = await _dbContext.Users.FindAsync(UserId);
        if (userInfo == null)
        {
            return NotFound();
        }
        else
        {
            var availableRoles = _dbContext.Roles;
            var selectedRoles = (from rl in _dbContext.Roles
                                 join ur in _dbContext.UserRoles on rl.RoleId equals ur.RoleId
                                 where ur.UserId == UserId
                                 select rl);

            var UserDetails = new UserManagementViewModel
            {
                User = userInfo,
                AvailableRoles = availableRoles,
                SelectedRoles = _dbContext.UserRoles.Where(ur => ur.UserId == UserId).Select(ur => ur.RoleId).ToList()
            };
            return View(UserDetails);
        }
    }

    // POST: Users/EditUser/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UserManagementViewModel userInfo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // var existingUserInfo = _dbContext.Users.FindAsync(userInfo.User!.UserId);
                
                userInfo.User!.LastModifiedOn = DateTime.Now;

                _dbContext.Update(userInfo.User);
                // _dbContext.Entry(userInfo.User).Property(u => u.Password).IsModified = false;
                // _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                // _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                // _dbContext.CurrentUser = User.Identity!.Name!;
                
                await _dbContext.SaveChangesAsync();

                // Update UserRoles
                var existingRoleIds = _dbContext.UserRoles
                    .Where(ur => ur.UserId == userInfo.User.UserId)
                    .Select(ur => ur.RoleId)
                    .ToList();

                // Check if there's any change
                bool rolesChanged = !existingRoleIds.OrderBy(x => x).SequenceEqual(userInfo.SelectedRoles!.OrderBy(x => x));

                if (rolesChanged)
                {
                    var existingUserRoles = _dbContext.UserRoles.Where(ur => ur.UserId == userInfo.User.UserId);
                    _dbContext.UserRoles.RemoveRange(existingUserRoles);
                    // _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                    // _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                    // _dbContext.CurrentUser = User.Identity!.Name!;
                    await _dbContext.SaveChangesAsync();

                    foreach (var roleId in userInfo.SelectedRoles!)
                    {
                        // System.Diagnostics.EventLog.WriteEntry("Application", "User Roles Mapping Started");
                        UserRole userRole = new UserRole()
                        {
                            UserId = userInfo.User.UserId,
                            RoleId = roleId,
                            CreatedOn = DateTime.Now
                        };
                        await _dbContext.UserRoles.AddAsync(userRole);
                    }
                    // _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                    // _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                    // _dbContext.CurrentUser = User.Identity!.Name!;
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(UserManagement));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.Users.AnyAsync(a => a.UserId == userInfo.User!.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return View(User);
    }

    public async Task<IActionResult> ViewUser(int UserId)
    {
        var User = await _dbContext.Users.FindAsync(UserId);
        if (User == null)
        {
            return NotFound();
        }

        var userInfo = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
        var availableRoles = _dbContext.Roles;
        var selectedRoles = (from rl in _dbContext.Roles
                             join ur in _dbContext.UserRoles on rl.RoleId equals ur.RoleId
                             where ur.UserId == UserId
                             select rl);

        var UserInfo = new UserManagementViewModel
        {
            User = userInfo,
            AvailableRoles = availableRoles,
            SelectedRoles = _dbContext.UserRoles.Where(ur => ur.UserId == UserId).Select(ur => ur.RoleId).ToList()
        };
        return View(UserInfo);
    }

    // GET: Users/DeleteUser/5
    public async Task<IActionResult> DeleteUser(int UserId)
    {
        var userInfo = await _dbContext.Users.FindAsync(UserId);
        if (userInfo == null)
        {
            return NotFound();
        }
        else
        {
            userInfo.IsActive = false; // Soft delete
            userInfo.LastModifiedOn = DateTime.Now;
            _dbContext.Update(userInfo);
            _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
            _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
            _dbContext.CurrentUser = User.Identity!.Name!;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }
    }
};