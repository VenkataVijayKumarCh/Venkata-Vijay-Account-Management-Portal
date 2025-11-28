using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required, StringLength(150)]
        public required string ProjectName { get; set; }

        [Required, StringLength(1000)]
        public required string ProjectDescription { get; set; }

        [StringLength(50)]
        public string? SOWNo { get; set; }

        public decimal SOWValue { get; set; }        

        [ForeignKey("AccountId")]
        public required int AccountId { get; set; }

        public required bool IsActive { get; set; } = true;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? PurchaseOrder { get; set; }
    }
}