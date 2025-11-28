using VenkataAllocationManagementSystem.Models;
using VenkataAllocationManagementSystem.CustomClass;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class AllocationManagementViewModel
    {
        public List<Allocation> AllocationsInfo { get; set; } = new List<Allocation>();
        public int AllocationId { get; set; }
        public string? AccountName { get; set; }
        public string? ProjectName { get; set; }
        public string? AssociateName { get; set; }

        public DateOnly ProjectStartDate { get; set; }
        public DateOnly ProjectEndDate { get; set; } // End date of the project

        [AllocationStartDateValidation("ProjectStartDate", ErrorMessage = "Allocation Start Date cannot be before Project Start Date.")]
        [AllocationEndDateValidation("ProjectEndDate", ErrorMessage = "Allocation Start Date cannot be after Project End Date.")]
        public DateOnly StartDate { get; set; }

        [AllocationEndDateValidation("ProjectEndDate", ErrorMessage = "Allocation End Date cannot be after Project End Date.")]
        [AllocationStartDateValidation("ProjectStartDate", ErrorMessage = "Allocation End Date cannot be before Project Start Date.")]
        public DateOnly EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public bool IsActive { get; set; } // True = Active, False = Inactive
        public List<AllocationManagementViewModel> AllocationsMgmtInfo { get; set; } = new List<AllocationManagementViewModel>();

        public int AssociateId { get; set; }

        public int ProjectId { get; set; }

        public int AccountId { get; set; }

        public int BillabilityTypeId { get; set; }

        public IEnumerable<BillabilityTypes>? BillabilityTypes { get; set; }

        public IEnumerable<Account>? Accounts { get; set; }

        public IEnumerable<Project>? Projects { get; set; } 

        public IEnumerable<Associate>? Associates { get; set; }

        public IList<AllocationRate> AllocationRates { get; set; } = new List<AllocationRate>();

        public AllocationRate? AllocationRate { get; set; }
    }
}