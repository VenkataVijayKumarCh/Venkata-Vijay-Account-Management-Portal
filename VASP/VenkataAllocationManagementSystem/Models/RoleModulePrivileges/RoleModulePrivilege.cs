using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class RoleModulePrivilege
    {
        public int RoleModulePrivilegeId { get; set; }
        public int RoleId { get; set; }
        public int ModulePrivilegeId { get; set; }

        // Navigation
        public Role Role { get; set; } = null!;
        public ModulePrivilege ModulePrivilege { get; set; } = null!;
    }
}