using System.ComponentModel.DataAnnotations;

namespace VenkataAllocationManagementSystem.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required, StringLength(1000)]
        public string? AccountName { get; set; }

        public string? Description { get; set; }

        public required bool IsActive { get; set; } = true;
    }
}