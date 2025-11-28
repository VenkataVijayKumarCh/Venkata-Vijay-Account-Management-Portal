using System.Collections.Generic;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class ManagementDashboardViewModel
    {
        public IEnumerable<Account>? Accounts { get; set; }
        public IEnumerable<Project>? Projects { get; set; }
        public IEnumerable<Associate>? Associates { get; set; }
        public IEnumerable<Allocation>? Allocations { get; set; }
        public IEnumerable<Timesheet>? Timesheets { get; set; }
        public decimal RevenueGenerated { get; set; }
    }
}