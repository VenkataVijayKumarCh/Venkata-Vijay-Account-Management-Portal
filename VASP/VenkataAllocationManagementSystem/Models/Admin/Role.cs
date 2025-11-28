using System.ComponentModel.DataAnnotations;
using VenkataAllocationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace VenkataAllocationManagementSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required, StringLength(100)]
        public required string RoleName { get; set; }

        [Required, StringLength(500)]
        public bool IsActive { get; set; }

        public DateOnly CreatedDate { get; set; } 

        // Navigation property for related users
        // public ICollection<User> Users { get; set; } = new List<User>();

    }
}