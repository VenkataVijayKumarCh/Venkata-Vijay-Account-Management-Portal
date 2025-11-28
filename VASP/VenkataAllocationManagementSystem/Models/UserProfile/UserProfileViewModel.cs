using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class UserProfileViewModel
    {

        public User? User { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        // public IEnumerable<Role>? UserRoles { get; set; }

        public string? UserRoles { get; set; }
    }
}

