using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;

        // Navigation property
        public ICollection<ModulePrivilege> RolePrivileges { get; set; } = new List<ModulePrivilege>();
    }
}