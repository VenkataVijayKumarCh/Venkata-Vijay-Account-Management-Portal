using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class PortfolioReportsViewModel
    {
        public int PortfolioId { get; set; }
        public string? PortfolioName { get; set; }
        public DateOnly ReportStartDate { get; set; }
        public DateOnly ReportEndDate { get; set; }
        public List<PortfolioDashboardViewModel>? DashboardData { get; set; }
        
    }
    
    public class ProjectAllocationDto
    {
        public string ProjectName { get; set; } = "";
        public decimal AllocatedHours { get; set; }
    }

    public class UtilizationDto
    {
        public string AssociateName { get; set; } = "";
        public decimal BillableHours { get; set; }
        public decimal AvailableHours { get; set; }
        public decimal UtilizationPercent => AvailableHours == 0 ? 0 : (BillableHours / AvailableHours) * 100;
    }

    public class SubmissionStatusDto
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

    public class TimesheetComplianceDto
{
    public string? ProjectName { get; set; }
    public string WeekStartDate { get; set; } = "";
    public int Submitted { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int NotSubmitted { get; set; }
}

    public class BurndownDataDto
    {
        public Associate? Associate { get; set; }
        public List<Timesheet>? Timesheets { get; set; }
        public Project? Project { get; set; }
    }
    
    public class PortfolioDashboardViewModel
    {
        public List<ProjectAllocationDto> ProjectAllocations { get; set; } = new();
        public List<UtilizationDto> Utilization { get; set; } = new();
        public List<SubmissionStatusDto> SubmissionStatuses { get; set; } = new();
        public List<TimesheetComplianceDto> TimesheetCompliance { get; set; } = new();
        public List<Project>? Projects { get; set; }
        public int ProjectId { get; set; }
        public List<Associate>? Associates { get; set; }
        public int AssociateId { get; set; }
        public List<BurndownDataDto>? BurndownData { get; set; } = new();
        public List<TimesheetPeriod>? TimesheetPeriods { get; set; } = new();
    }

}