-- =====================================================
-- LEAVE MANAGEMENT SYSTEM - QUERY EXAMPLES & UTILITIES
-- Created: 2026-07-07
-- Database: VenkataAllocationManagementSystem
-- =====================================================
-- This script contains useful queries for reporting and analysis
-- =====================================================

-- =====================================================
-- 1. VIEW ALL LEAVE REQUESTS WITH ASSOCIATE DETAILS
-- =====================================================

-- Get all leave requests
SELECT * FROM [dbo].[vw_LeaveRequestDetails]
ORDER BY [AssociateName], [StartDate];

-- Get all approved leaves for current year
SELECT * FROM [dbo].[vw_LeaveRequestDetails]
WHERE [Status] = 'Approved'
  AND YEAR([StartDate]) = YEAR(GETDATE())
ORDER BY [AssociateName], [StartDate];

-- Get pending leave approvals
SELECT * FROM [dbo].[vw_LeaveRequestDetails]
WHERE [Status] = 'Pending'
ORDER BY [RequestedOn] DESC;

-- =====================================================
-- 2. VIEW ALL HOLIDAYS
-- =====================================================

-- Get all holidays
SELECT * FROM [dbo].[vw_HolidayCalendar]
ORDER BY [HolidayDate];

-- Get upcoming holidays (next 30 days)
SELECT * FROM [dbo].[vw_HolidayCalendar]
WHERE [HolidayDate] BETWEEN CAST(GETDATE() AS DATE) 
                       AND DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
ORDER BY [HolidayDate];

-- Get holidays by month
SELECT [Month], [Year], COUNT(*) AS [Count]
FROM [dbo].[vw_HolidayCalendar]
GROUP BY [Year], [Month]
ORDER BY [Year], [Month];

-- =====================================================
-- 3. LEAVE SUMMARY QUERIES
-- =====================================================

-- Get leave summary by associate for current year
SELECT 
    a.[AssociateId],
    a.[FullName],
    COUNT(CASE WHEN lr.[Status] = 'Approved' THEN 1 END) AS [ApprovedCount],
    SUM(CASE WHEN lr.[Status] = 'Approved' THEN lr.[RequestedDays] ELSE 0 END) AS [ApprovedDays],
    COUNT(CASE WHEN lr.[Status] = 'Pending' THEN 1 END) AS [PendingCount],
    SUM(CASE WHEN lr.[Status] = 'Pending' THEN lr.[RequestedDays] ELSE 0 END) AS [PendingDays],
    COUNT(CASE WHEN lr.[Status] = 'Rejected' THEN 1 END) AS [RejectedCount]
FROM [dbo].[Associates] a
LEFT JOIN [dbo].[LeaveRequests] lr ON a.[AssociateId] = lr.[AssociateId]
    AND YEAR(lr.[StartDate]) = YEAR(GETDATE())
WHERE a.[AssociateStatusId] = (SELECT [AssociateStatusId] FROM [dbo].[AssociateStatus] WHERE [AssociateStatusName] = 'Active')
GROUP BY a.[AssociateId], a.[FullName]
ORDER BY a.[FullName];

-- Get leave summary by leave type
SELECT 
    [LeaveType],
    COUNT(*) AS [NumberOfRequests],
    SUM([RequestedDays]) AS [TotalDays],
    COUNT(CASE WHEN [Status] = 'Approved' THEN 1 END) AS [ApprovedCount],
    COUNT(CASE WHEN [Status] = 'Pending' THEN 1 END) AS [PendingCount],
    COUNT(CASE WHEN [Status] = 'Rejected' THEN 1 END) AS [RejectedCount]
FROM [dbo].[LeaveRequests]
WHERE YEAR([StartDate]) = YEAR(GETDATE())
GROUP BY [LeaveType]
ORDER BY [NumberOfRequests] DESC;

-- =====================================================
-- 4. RESOURCE AVAILABILITY QUERIES (FOR REVENUE FORECASTING)
-- =====================================================

-- Calculate available working days for each associate (current month)
DECLARE @CurrentMonth INT = MONTH(GETDATE());
DECLARE @CurrentYear INT = YEAR(GETDATE());

SELECT 
    a.[AssociateId],
    a.[FullName],
    a.[AssociateEmployeeId],
    -- Calculate calendar days in month
    DAY(EOMONTH(DATEFROMPARTS(@CurrentYear, @CurrentMonth, 1))) AS [CalendarDays],
    -- Count working days (excluding weekends)
    (
        SELECT COUNT(*)
        FROM (
            SELECT TOP (DAY(EOMONTH(DATEFROMPARTS(@CurrentYear, @CurrentMonth, 1))))
                   ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS DayNumber
            FROM sys.objects
        ) Days
        WHERE DATEPART(WEEKDAY, DATEFROMPARTS(@CurrentYear, @CurrentMonth, DayNumber)) NOT IN (1, 7)
    ) AS [WorkingDays],
    -- Count holidays in month
    (
        SELECT COUNT(*)
        FROM [dbo].[Holidays]
        WHERE YEAR([HolidayDate]) = @CurrentYear
          AND MONTH([HolidayDate]) = @CurrentMonth
          AND DATEPART(WEEKDAY, [HolidayDate]) NOT IN (1, 7)
    ) AS [HolidaysInMonth],
    -- Count approved leave days
    (
        SELECT COALESCE(SUM(CAST([RequestedDays] AS INT)), 0)
        FROM [dbo].[LeaveRequests]
        WHERE [AssociateId] = a.[AssociateId]
          AND [Status] = 'Approved'
          AND YEAR([StartDate]) = @CurrentYear
          AND MONTH([StartDate]) = @CurrentMonth
    ) AS [ApprovedLeaveDays],
    -- Available working days
    (
        SELECT COUNT(*)
        FROM (
            SELECT TOP (DAY(EOMONTH(DATEFROMPARTS(@CurrentYear, @CurrentMonth, 1))))
                   ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS DayNumber
            FROM sys.objects
        ) Days
        WHERE DATEPART(WEEKDAY, DATEFROMPARTS(@CurrentYear, @CurrentMonth, DayNumber)) NOT IN (1, 7)
    ) 
    - (
        SELECT COUNT(*)
        FROM [dbo].[Holidays]
        WHERE YEAR([HolidayDate]) = @CurrentYear
          AND MONTH([HolidayDate]) = @CurrentMonth
          AND DATEPART(WEEKDAY, [HolidayDate]) NOT IN (1, 7)
    )
    - (
        SELECT COALESCE(SUM(CAST([RequestedDays] AS INT)), 0)
        FROM [dbo].[LeaveRequests]
        WHERE [AssociateId] = a.[AssociateId]
          AND [Status] = 'Approved'
          AND YEAR([StartDate]) = @CurrentYear
          AND MONTH([StartDate]) = @CurrentMonth
    ) AS [AvailableWorkingDays]
FROM [dbo].[Associates] a
WHERE a.[AssociateStatusId] = (SELECT [AssociateStatusId] FROM [dbo].[AssociateStatus] WHERE [AssociateStatusName] = 'Active')
ORDER BY a.[FullName];

-- =====================================================
-- 5. LEAVE CONFLICT CHECK
-- =====================================================

-- Find overlapping leave requests for same associate
SELECT 
    lr1.[LeaveRequestId] AS [FirstLeaveId],
    lr2.[LeaveRequestId] AS [OverlappingLeaveId],
    lr1.[AssociateId],
    a.[FullName],
    lr1.[StartDate] AS [FirstLeaveStart],
    lr1.[EndDate] AS [FirstLeaveEnd],
    lr2.[StartDate] AS [OverlapStart],
    lr2.[EndDate] AS [OverlapEnd],
    lr1.[Status] AS [FirstStatus],
    lr2.[Status] AS [OverlapStatus]
FROM [dbo].[LeaveRequests] lr1
INNER JOIN [dbo].[LeaveRequests] lr2 ON lr1.[AssociateId] = lr2.[AssociateId]
    AND lr1.[LeaveRequestId] < lr2.[LeaveRequestId]
    AND lr1.[Status] IN ('Approved', 'Pending')
    AND lr2.[Status] IN ('Approved', 'Pending')
    AND lr1.[StartDate] <= lr2.[EndDate]
    AND lr1.[EndDate] >= lr2.[StartDate]
INNER JOIN [dbo].[Associates] a ON lr1.[AssociateId] = a.[AssociateId]
ORDER BY a.[FullName], lr1.[StartDate];

-- =====================================================
-- 6. STORED PROCEDURE USAGE EXAMPLES
-- =====================================================

-- Example 1: Calculate working days between two dates
DECLARE @WorkingDays DECIMAL(5,2);
EXEC [dbo].[sp_CalculateWorkingDays] 
    @StartDate = '2026-07-01',
    @EndDate = '2026-07-31',
    @WorkingDays = @WorkingDays OUTPUT;

SELECT @WorkingDays AS [WorkingDaysInJuly2026];

-- Example 2: Get leave summary for a specific associate
EXEC [dbo].[sp_GetAssociateLeaveSummary] 
    @AssociateId = 1,
    @Year = 2026;

-- Example 3: Check for leave conflicts
DECLARE @HasConflict BIT;
EXEC [dbo].[sp_CheckLeaveConflict]
    @AssociateId = 1,
    @StartDate = '2026-08-01',
    @EndDate = '2026-08-15',
    @HasConflict = @HasConflict OUTPUT;

SELECT CASE WHEN @HasConflict = 1 THEN 'Conflict Found' ELSE 'No Conflict' END AS [ConflictStatus];

-- =====================================================
-- 7. DATA VALIDATION QUERIES
-- =====================================================

-- Check for invalid date ranges (end date before start date)
SELECT [LeaveRequestId], [AssociateId], [StartDate], [EndDate]
FROM [dbo].[LeaveRequests]
WHERE [EndDate] < [StartDate];

-- Check for leave requests with missing associates
SELECT lr.[LeaveRequestId], lr.[AssociateId]
FROM [dbo].[LeaveRequests] lr
LEFT JOIN [dbo].[Associates] a ON lr.[AssociateId] = a.[AssociateId]
WHERE a.[AssociateId] IS NULL;

-- Check for approved leaves without approval details
SELECT [LeaveRequestId], [Status], [ApprovedOn], [ApprovedBy]
FROM [dbo].[LeaveRequests]
WHERE [Status] = 'Approved'
  AND ([ApprovedOn] IS NULL OR [ApprovedBy] IS NULL);

-- =====================================================
-- 8. MAINTENANCE QUERIES
-- =====================================================

-- Archive old leave requests (read-only check)
SELECT COUNT(*) AS [OldLeaveRecords]
FROM [dbo].[LeaveRequests]
WHERE YEAR([StartDate]) < YEAR(GETDATE()) - 1;

-- Identify leave requests awaiting action (older than 7 days)
SELECT 
    [LeaveRequestId],
    [AssociateId],
    [StartDate],
    [EndDate],
    [Status],
    [RequestedOn],
    DATEDIFF(DAY, [RequestedOn], GETDATE()) AS [DaysPending]
FROM [dbo].[LeaveRequests]
WHERE [Status] = 'Pending'
  AND DATEDIFF(DAY, [RequestedOn], GETDATE()) > 7
ORDER BY [RequestedOn];

-- Get holiday statistics
SELECT 
    'Total Holidays' AS [Metric],
    COUNT(*) AS [Value]
FROM [dbo].[Holidays]
UNION ALL
SELECT 'Recurring Holidays', COUNT(*) FROM [dbo].[Holidays] WHERE [IsRecurring] = 1
UNION ALL
SELECT 'One-Time Holidays', COUNT(*) FROM [dbo].[Holidays] WHERE [IsRecurring] = 0
UNION ALL
SELECT 'Holidays in 2026', COUNT(*) FROM [dbo].[Holidays] WHERE YEAR([HolidayDate]) = 2026
UNION ALL
SELECT 'Holidays in 2027', COUNT(*) FROM [dbo].[Holidays] WHERE YEAR([HolidayDate]) = 2027;

-- =====================================================
-- 9. REPORTING QUERIES
-- =====================================================

-- Monthly leave trends
SELECT 
    YEAR([StartDate]) AS [Year],
    MONTH([StartDate]) AS [Month],
    [Status],
    COUNT(*) AS [Count],
    SUM([RequestedDays]) AS [TotalDays],
    CAST(AVG([RequestedDays]) AS DECIMAL(5,2)) AS [AvgDaysPerRequest]
FROM [dbo].[LeaveRequests]
GROUP BY YEAR([StartDate]), MONTH([StartDate]), [Status]
ORDER BY [Year], [Month], [Status];

-- Leave utilization by type and status
SELECT 
    [LeaveType],
    [Status],
    COUNT(*) AS [NumberOfRequests],
    SUM([RequestedDays]) AS [TotalDays],
    CAST(AVG([RequestedDays]) AS DECIMAL(5,2)) AS [AvgDaysPerRequest],
    MIN([StartDate]) AS [EarliestDate],
    MAX([EndDate]) AS [LatestDate]
FROM [dbo].[LeaveRequests]
WHERE YEAR([StartDate]) = YEAR(GETDATE())
GROUP BY [LeaveType], [Status]
ORDER BY [LeaveType], [Status];

-- Associates with highest leave usage
SELECT TOP 10
    a.[FullName],
    a.[AssociateEmployeeId],
    COUNT(lr.[LeaveRequestId]) AS [LeaveRequests],
    SUM(CASE WHEN lr.[Status] = 'Approved' THEN lr.[RequestedDays] ELSE 0 END) AS [ApprovedDays],
    SUM(CASE WHEN lr.[Status] = 'Pending' THEN lr.[RequestedDays] ELSE 0 END) AS [PendingDays],
    SUM(lr.[RequestedDays]) AS [TotalRequestedDays]
FROM [dbo].[Associates] a
LEFT JOIN [dbo].[LeaveRequests] lr ON a.[AssociateId] = lr.[AssociateId]
    AND YEAR(lr.[StartDate]) = YEAR(GETDATE())
WHERE a.[AssociateStatusId] = (SELECT [AssociateStatusId] FROM [dbo].[AssociateStatus] WHERE [AssociateStatusName] = 'Active')
GROUP BY a.[AssociateId], a.[FullName], a.[AssociateEmployeeId]
ORDER BY [ApprovedDays] DESC;
