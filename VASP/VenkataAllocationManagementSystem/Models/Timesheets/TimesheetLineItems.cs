using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class TimesheetLineItem
    {
        public Guid TimesheetLineItemId { get; set; } // PK
        public Guid TimesheetId { get; set; } // FK

        public DateOnly WorkDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string? Description { get; set; }

        public Timesheet? Timesheet { get; set; }

        // public int AssociateId { get; set; }

        // public Guid ProjectId { get; set; }
        // public Project? Project { get; set; }
        // public decimal MondayHours { get; set; }
        // public decimal TuesdayHours { get; set; }
        // public decimal WednesdayHours { get; set; }
        // public decimal ThursdayHours { get; set; }
        // public decimal FridayHours { get; set; }
        // public decimal SaturdayHours { get; set; }
        // public decimal SundayHours { get; set; }

    }
}