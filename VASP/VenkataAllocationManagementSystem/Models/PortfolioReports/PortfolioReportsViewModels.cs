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
        public string? DefaulterNames { get; set; } = "";
    }

    public class BurndownDataDto
    {
        public Associate? Associate { get; set; }
        public List<Timesheet>? Timesheets { get; set; }
        public Project? Project { get; set; }
        public DateOnly StartDate { get; set; }
    }

    public class WeeklyRevenueProjectionDto
    {
        public string? ProjectName { get; set; }
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public int ExpectedWorkingDays { get; set; }
        public int EffectiveWorkingDays { get; set; }
        public decimal ActualRevenue { get; set; }
        public decimal AdjustedProjection { get; set; }
        public decimal Variance { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayCount { get; set; }
    }

    public class WeeklyComplianceDetailDto
    {
        public string? ProjectName { get; set; }
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public decimal SubmittedHours { get; set; }
        public decimal LeaveOverlapHours { get; set; }
        public decimal HolidayOverlapHours { get; set; }
        public decimal TotalOverlapHours { get; set; }
        public int OverlapEntries { get; set; }
        public string Status { get; set; } = "Healthy";
    }
    
    public class PortfolioDashboardViewModel
    {
        public List<ProjectAllocationDto> ProjectAllocations { get; set; } = new();
        public List<UtilizationDto> Utilization { get; set; } = new();
        public List<SubmissionStatusDto> SubmissionStatuses { get; set; } = new();
        public List<TimesheetComplianceDto> TimesheetCompliance { get; set; } = new();
        public List<WeeklyRevenueProjectionDto> WeeklyRevenueProjection { get; set; } = new();
        public List<WeeklyComplianceDetailDto> WeeklyComplianceDetails { get; set; } = new();
        public List<Project>? Projects { get; set; }
        public int ProjectId { get; set; }
        public List<Associate>? Associates { get; set; }
        public int AssociateId { get; set; }
        public List<BurndownDataDto>? BurndownData { get; set; } = new();
        public List<TimesheetPeriod>? TimesheetPeriods { get; set; } = new();
    }

    #region ML Summary Report ViewModels

    public class MLSummaryReportViewModel
    {
        // 1) No of accounts and Projects
        public int TotalAccounts { get; set; }
        public int TotalProjects { get; set; }
        public List<AccountProjectCountDto> AccountProjectCounts { get; set; } = new();

        // 2) Summary on Timesheets status
        public List<TimesheetStatusSummaryDto> TimesheetStatusSummary { get; set; } = new();
        public int TotalTimesheets { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }

        // 3) Revenue Quick Summary by month
        public List<MonthlyRevenueDto> MonthlyRevenues { get; set; } = new();
        public decimal TotalRevenue { get; set; }

        // 4) Revenue Prediction vs Actual with Variance
        public List<RevenuePredictionDto> RevenuePredictions { get; set; } = new();
        public decimal TotalPredictedRevenue { get; set; }
        public decimal TotalActualRevenue { get; set; }
        public decimal TotalVariance { get; set; }

        // 5) Financial Metrics
        public FinancialMetricsDto FinancialMetrics { get; set; } = new();

        // 6) Timesheet Compliance Report
        public List<TimesheetComplianceDto> TimesheetCompliance { get; set; } = new();
        public int TotalAllocatedAssociates { get; set; }
        public int TotalTimesheetComplianceSubmitted { get; set; }
        public int TotalTimesheetComplianceApproved { get; set; }
        public int TotalTimesheetComplianceRejected { get; set; }
        public int TotalTimesheetComplianceNotSubmitted { get; set; }
        public decimal ComplianceRate { get; set; }
    }

    public class AccountProjectCountDto
    {
        public string? AccountName { get; set; }
        public int ProjectCount { get; set; }
    }

    public class TimesheetStatusSummaryDto
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = "";
        public decimal Revenue { get; set; }
    }

    public class RevenuePredictionDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = "";
        public decimal PredictedRevenue { get; set; }
        public decimal ActualRevenue { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
    }

    // 5) Financial Metrics
    public class FinancialMetricsDto
    {
        // Revenue per Project
        public List<RevenuePerProjectDto> RevenuePerProject { get; set; } = new();
        public decimal TotalProjectRevenue { get; set; }

        // Revenue per Associate
        public List<RevenuePerAssociateDto> RevenuePerAssociate { get; set; } = new();
        public decimal TotalAssociateRevenue { get; set; }

        // Cost Variance
        public List<CostVarianceDto> CostVariances { get; set; } = new();
        public decimal TotalBudgetedCost { get; set; }
        public decimal TotalActualCost { get; set; }
        public decimal TotalCostVariance { get; set; }

        // Profit Margin
        public List<ProfitMarginDto> ProfitMargins { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal OverallProfitMargin { get; set; }
    }

    public class RevenuePerProjectDto
    {
        public string? ProjectName { get; set; }
        public string? AccountName { get; set; }
        public decimal Revenue { get; set; }
        public decimal PercentageOfTotal { get; set; }
    }

    public class RevenuePerAssociateDto
    {
        public string? AssociateName { get; set; }
        public string? EmployeeId { get; set; }
        public decimal Revenue { get; set; }
        public decimal PercentageOfTotal { get; set; }
    }

    public class CostVarianceDto
    {
        public string? ProjectName { get; set; }
        public decimal BudgetedCost { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; } = ""; // Under Budget, Over Budget, On Track
    }

    public class ProfitMarginDto
    {
        public string? ProjectName { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal ProfitMarginPercentage { get; set; }
    }

    #endregion

}