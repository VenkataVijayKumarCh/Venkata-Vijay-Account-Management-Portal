using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VenkataAllocationManagementSystem.CustomClass;

namespace VenkataAllocationManagementSystem.Models
{
    public class Allocation
    {
        [Key]
        public int AllocationId { get; set; }

        [Required]
        [ForeignKey("AssociateId")]
        public int AssociateId { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public int ProjectId { get; set; }


        public bool IsActive { get; set; }

        public required DateOnly StartDate { get; set; }

        // [AllocationEndDateValidation("Project.ProjectEndDate", ErrorMessage = "Allocation End Date cannot be after Project End Date.")]
        public required DateOnly EndDate { get; set; }

        public decimal AllocationPercentage { get; set; }
        
        [ForeignKey("BillabilityTypeId")]
        public int BillabilityTypeId { get; set; }
    }
}