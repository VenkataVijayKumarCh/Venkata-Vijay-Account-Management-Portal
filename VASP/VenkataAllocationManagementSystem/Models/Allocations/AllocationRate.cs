using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VenkataAllocationManagementSystem.CustomClass;

namespace VenkataAllocationManagementSystem.Models
{
    public class AllocationRate
    {
        [Key]
        public int AllocationRateId { get; set; }

        public int AllocationId { get; set; }
        
        public DateOnly AllocationRateStartDate { get; set; }

        public DateOnly AllocationRateEndDate { get; set; }

        public decimal AllocationPercentage { get; set; }

        public decimal AllocationBillRate { get; set; }
        
        [ForeignKey("BillabilityTypeId")]
        public int BillabilityTypeId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } 

        public int? ModifiedBy { get; set; } 
    }
}