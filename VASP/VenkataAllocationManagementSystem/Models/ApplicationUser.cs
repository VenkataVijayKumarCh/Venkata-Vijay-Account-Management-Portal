using System.ComponentModel.DataAnnotations;
using VenkataAllocationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace VenkataAllocationManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();        

    }
}