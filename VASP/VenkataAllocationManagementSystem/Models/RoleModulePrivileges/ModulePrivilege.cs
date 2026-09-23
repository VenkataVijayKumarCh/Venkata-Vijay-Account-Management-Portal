using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class ModulePrivilege
    {
        public int ModulePrivilegeId { get; set; }

        public int ModuleId { get; set; }
        public string PrivilegeName { get; set; } = string.Empty; 
        // Examples: "View", "Create", "Update", "Delete", "Approve"

        // Navigation
        public Module Module { get; set; } = null!;
        public ICollection<RoleModulePrivilege> RoleModulePrivileges { get; set; } = new List<RoleModulePrivilege>();
    }
}