using System.ComponentModel.DataAnnotations;

namespace VenkataAllocationManagementSystem.Models
{
    public class Associate
    {
        public int AssociateId { get; set; }

        public required string AssociateEmployeeId { get; set; }

        [Required, StringLength(1000)]
        public required string FullName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        [Required, StringLength(50)]
        public required string ContactNumber { get; set; }

        public required int AssociateTypeId { get; set; }

        public required int AssociateStatusId { get; set; }
        
        public DateOnly? JoiningDate { get; set; } 

        public DateOnly? TerminationDate { get; set; }
    }
}