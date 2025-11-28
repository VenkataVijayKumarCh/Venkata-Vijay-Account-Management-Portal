using System.ComponentModel.DataAnnotations;
using VenkataAllocationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VenkataAllocationManagementSystem.Models
{
    public class User
    {
        // Parameterless constructor (automatically present unless you define others)
        // public User() { }

        // private readonly ApplicationDbContext? dbContext;

        // public User(ApplicationDbContext dbContext)
        // {
        //     this.dbContext = dbContext;
        // }


        public required int UserId { get; set; }

        [Required, StringLength(100)]
        public required string UserName { get; set; }

        [Required, StringLength(500)]
        public required string UserEmail { get; set; }

        [Required, StringLength(100)]
        [BindingBehavior(BindingBehavior.Optional)] // Prevent binding from HTTP requests
        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public bool IsActive { get; set; }
        
        public DateTime? LastModifiedOn { get; set; }

    }
}