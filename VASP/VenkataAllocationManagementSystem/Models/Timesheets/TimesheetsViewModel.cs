using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class TimesheetsViewModel
    {
        public Guid TimesheetId { get; set; }
        public int TimesheetPeriodId { get; set; }
        public int ProjectId { get; set; }
        public DateOnly TimesheetStartDate { get; set; }
        public DateOnly TimesheetEndDate { get; set; }
        public List<TimesheetLineItem>? TimesheetLineItems { get; set; }
        public string? Description { get; set; }

        public string? TimesheetStatus { get; set; }

        public int CreatedBy { get; set; }
        public int AssociateId { get; set; }

        public string? AssociateName { get; set; }
        public int CurrentAssociateId { get; set; }

        public decimal? TotalHours { get; set; }

        public List<Project>? Projects { get; set; }

        public List<Associate>? Associates { get; set; }

        public List<TimesheetPeriod>? TimesheetPeriods { get; set; }

        public List<AssociateTimesheetRow>? AssociateTimesheetRows { get; set; }

        public bool ShowTimesheetTable { get; set; }

        //public TimesheetGridViewModel TimesheetGridViewModel { get; set; }

        public List<TimesheetsViewModel>? TimesheetsInfo { get; set; }

        // public Timesheet? TimesheetInfo { get; set; }

        public string? ProjectName { get; set; }
        public bool IsEditMode { get; set; } = false;

        public int FilteredTimesheetPeriodId { get; set; } = 0;
    }

    public class AssociateTimesheetRow
    {
        public int AssociateId { get; set; }

        public List<TimesheetLineItem>? TimesheetLineItems { get; set; }
    }
    
    // public class TimesheetGridViewModel
    // {
    //     public int TimesheetPeriodId { get; set; }
    //     // Add properties for associates, week dates, etc.
    //     public List<TimesheetPeriod>? TimesheetPeriods { get; set; }

    //     public List<TimesheetLineItem>? TimesheetLineItems { get; set; }

    //     public bool ShowTimesheetTable { get; set; }
    // }


    // public class ProjectTimesheetRow
    // {
    //     public Guid ProjectId { get; set; }
    //     public string? ProjectName { get; set; }

    //     public decimal MondayHours { get; set; }
    //     public decimal TuesdayHours { get; set; }
    //     public decimal WednesdayHours { get; set; }
    //     public decimal ThursdayHours { get; set; }
    //     public decimal FridayHours { get; set; }
    //     public decimal SaturdayHours { get; set; }
    //     public decimal SundayHours { get; set; }

    //     public string? Description { get; set; }
    // }

}