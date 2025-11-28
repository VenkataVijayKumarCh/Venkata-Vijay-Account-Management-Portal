using System.ComponentModel.DataAnnotations;
using VenkataAllocationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace VenkataAllocationManagementSystem.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }        
        public DateTime CreatedOn { get; set; }

    }
}