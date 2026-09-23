using System.ComponentModel.DataAnnotations;

namespace VenkataAllocationManagementSystem.Models
{
    public enum BillabilityTypesEnum
    {
        [Display(Name = "Billable", Description = "Billable")]
        Billable = 1,
        [Display(Name = "Non-Billable", Description = "Non-Billable")]
        NonBillable = 2
    }
}