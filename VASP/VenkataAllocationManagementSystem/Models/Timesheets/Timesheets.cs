using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class Timesheet
{
    public Guid TimesheetId { get; set; } // PK
    public int TimesheetPeriodId { get; set; } // FK
    public int AssociateId { get; set; } // FK
    //[ForeignKey("ProjectId")]
    public int ProjectId { get; set; } // FK

    public DateOnly TimesheetStartDate { get; set; }
    public DateOnly TimesheetEndDate { get; set; }
    public string? Status { get; set; } // Draft, Submitted, Approved

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public decimal TotalHours { get; set; }

    public TimesheetPeriod? TimesheetPeriod { get; set; }

    public int TimesheetStatusId { get; set; }

    // public ICollection<TimesheetLineItem>? TimesheetLineItems { get; set; }
}
}