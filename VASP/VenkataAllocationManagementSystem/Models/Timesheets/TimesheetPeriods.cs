using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class TimesheetPeriod
    {
        public int TimesheetPeriodId { get; set; } // PK
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public bool IsActive { get; set; }

        // public ICollection<Timesheet>? Timesheets { get; set; }
    }
}