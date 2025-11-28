using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class UserManagementViewModel
    {

        public User? User { get; set; }

        public IEnumerable<User>? Users { get; set; }

        // public int UserId { get; set; }

        // public string? UserName { get; set; }

        // public string? UserEmail { get; set; }

        // public string? Password { get; set; }

        // public DateTime CreatedAt { get; set; }

        // public required string FirstName { get; set; }

        // public required string LastName { get; set; }

        // For multiple role selection
        public IEnumerable<int>? SelectedRoles { get; set; } = new List<int>();

        public IEnumerable<Role>? AvailableRoles { get; set; } = new List<Role>();


        // public string? UserRoles { get; set; }
    }
}

