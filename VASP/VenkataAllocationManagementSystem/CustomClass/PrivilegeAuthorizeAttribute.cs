using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using VenkataAllocationManagementSystem.Data;
using X.PagedList.Extensions;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace VenkataAllocationManagementSystem.CustomClass
{
    public class PrivilegeAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly string _privilegeName;

        public PrivilegeAuthorizeAttribute(string moduleName, string privilegeName)
        {
            _moduleName = moduleName;
            _privilegeName = privilegeName;
            // System.Diagnostics.EventLog.WriteEntry("Application", $"PrivilegeAuthorizeAttribute initialized with Module: {_moduleName}, Privilege: {_privilegeName}");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // var db = (VenkataAllocationManagementSystem.Data.ApplicationDbContext)context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext));
            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var userRole = context.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // System.Diagnostics.EventLog.WriteEntry("Application", $"Hi came to here userRole: {userRole}");
            // if (string.IsNullOrEmpty(userRole))
            // {
            //     context.Result = new ForbidResult();
            //     return;
            // }

            var hasPrivilege = db.RoleModulePrivileges
                .Any(rmp => userRole.Contains(rmp.Role.RoleName) &&
                            rmp.ModulePrivilege.Module.ModuleName == _moduleName &&
                            rmp.ModulePrivilege.PrivilegeName == _privilegeName);

            if (!hasPrivilege)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
