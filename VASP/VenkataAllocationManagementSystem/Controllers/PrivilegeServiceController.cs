using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using System.Collections.Frozen;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http.Features;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.Controllers
{
     [CustomAuthorize(Roles.Admin, Roles.Manager, Roles.User)]
    public class PrivilegeServiceController : Controller
    {
        private readonly ILogger<PrivilegeServiceController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public PrivilegeServiceController(ILogger<PrivilegeServiceController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Privilege/Index
        public async Task<IActionResult> PrivilegeMatrix()
        {
            var roles = await _dbContext.Roles.ToListAsync();
            return View(roles); // Dropdown of roles
        }

        // GET: PrivilegeService/GetMatrix?roleId=2
        public async Task<IActionResult> GetMatrix(int roleId)
        {
            var modules = await _dbContext.Modules.ToListAsync();
            var modulePrivileges = await _dbContext.ModulePrivileges
                .Include(mp => mp.Module)
                .ToListAsync();

            var roleModulePrivileges = await _dbContext.RoleModulePrivileges
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            var vm = new PrivilegeMatrixViewModel
            {
                RoleId = roleId,
                Modules = modules,
                ModulePrivileges = modulePrivileges,
                RoleModulePrivileges = roleModulePrivileges
            };

            return PartialView("_PrivilegeMatrix", vm); // Render matrix dynamically
        }

        // POST: PrivilegeService/UpdateMatrix
        [HttpPost]
        public async Task<IActionResult> UpdateMatrix(PrivilegeMatrixUpdateViewModel model)
    {
        var existing = await _dbContext.RoleModulePrivileges
            .Where(rp => rp.RoleId == model.RoleId)
            .ToListAsync();

        // Remove revoked privileges
        var toRemove = existing.Where(e => !model.SelectedPrivilegeIds
            .Contains(e.ModulePrivilegeId)).ToList();
        _dbContext.RoleModulePrivileges.RemoveRange(toRemove);

        // Add new privileges
        var toAdd = model.SelectedPrivilegeIds.Where(np => !existing
            .Any(e => e.ModulePrivilegeId == np)).ToList();

        foreach (var modulePrivilegeId in toAdd)
        {
            _dbContext.RoleModulePrivileges.Add(new RoleModulePrivilege
            {
                RoleId = model.RoleId,
                ModulePrivilegeId = modulePrivilegeId
            });
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToAction("PrivilegeMatrix");
    }

        // public bool HasPrivilege(string roleName, string moduleName, string privilegeName)
        // {
        //     return _dbContext.RoleModulePrivileges
        //         .Any(rp => rp.Role.RoleName == roleName
        //                 && rp.Module.ModuleName == moduleName
        //                 && rp.ModulePrivilege.PrivilegeName == privilegeName);
        // }

        // public async Task UpdateRolePrivileges(int roleId, List<(string module, string privilege)> newPrivileges)
        // {
        //     var existing = await _dbContext.RolePrivileges
        //         .Where(rp => rp.RoleId == roleId)
        //         .ToListAsync();

        //     // Find privileges to remove
        //     var toRemove = existing.Where(e => !newPrivileges
        //         .Any(np => np.module == e.Module.ModuleName && np.privilege == e.Privilege.PrivilegeName)).ToList();

        //     // Find privileges to add
        //     var toAdd = newPrivileges.Where(np => !existing
        //         .Any(e => e.Module.ModuleName == np.module && e.Privilege.PrivilegeName == np.privilege)).ToList();

        //     _dbContext.RolePrivileges.RemoveRange(toRemove);

        //     foreach (var np in toAdd)
        //     {
        //         var moduleId = _dbContext.Modules.First(m => m.ModuleName == np.module).ModuleId;
        //         var privilegeId = _dbContext.ModulePrivileges.First(p => p.PrivilegeName == np.privilege).PrivilegeId;

        //         _dbContext.RoleModulePrivileges.Add(new RoleModulePrivilege
        //         {
        //             RoleId = roleId,
        //             ModuleId = moduleId,
        //             PrivilegeId = privilegeId
        //         });
        //     }

        //     await _dbContext.SaveChangesAsync();
        // }

    }
}