using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class PrivilegeMatrixViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public IEnumerable<Module> Modules { get; set; } = new List<Module>();
        public IEnumerable<ModulePrivilege> ModulePrivileges { get; set; } = new List<ModulePrivilege>();
        public IEnumerable<RoleModulePrivilege> RoleModulePrivileges { get; set; } = new List<RoleModulePrivilege>();
    }


    public class PrivilegeMatrixUpdateViewModel
    {
        public int RoleId { get; set; }
        public List<int> SelectedPrivilegeIds { get; set; } = new List<int>();
    }


    public class PrivilegeSelection
    {
        public int ModuleId { get; set; }
        public int PrivilegeId { get; set; }
    }

}