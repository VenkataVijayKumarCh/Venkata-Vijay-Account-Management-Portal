using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Enums;
using VenkataAllocationManagementSystem.CustomClass;
using System.Security.Claims;

namespace VenkataAllocationManagementSystem.Controllers
{
    // [CustomAuthorize(Roles.Viewer, Roles.User, Roles.Admin, Roles.Manager)]
    public class UserProfileController : Controller
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserProfileController(ILogger<UserProfileController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> MyProfile()
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "Came to My Profile action method");
            // var userId = User.Identity.GetUserId();
            // var user = _dbContext.Users.Find(userId);

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            // System.Diagnostics.EventLog.WriteEntry("Application", $"User Email : {userEmail}");
            var userInfo = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserEmail.Equals(userEmail));

            var uRoles = (from ur in _dbContext.UserRoles
                         join r in _dbContext.Roles on ur.RoleId equals r.RoleId 
                         join u in _dbContext.Users on ur.UserId equals u.UserId
                         where u.UserId.Equals(userInfo!.UserId)
                         select r.RoleName).ToList();

            var roles = String.Join(", ", uRoles);


            if (userInfo != null)
            {
                var userProfile = new UserProfileViewModel()
                {
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    UserEmail = userInfo.UserEmail,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    UserRoles = roles
                };

                return View(userProfile);
            }
            else
            {
                return NotFound();
            }           
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult MyProfile(UserProfileViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = db.Users.Find(model.UserId);
        //         user.FullName = model.FullName;
        //         user.Department = model.Department;
        //         db.SaveChanges();

        //         ViewBag.Message = "Profile updated successfully.";
        //     }

        //     return View(model);
        // }
    }
}