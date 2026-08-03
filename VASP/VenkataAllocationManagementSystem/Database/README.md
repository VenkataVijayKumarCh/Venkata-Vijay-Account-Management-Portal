# Leave Management System - Database Implementation Guide

## Overview

This directory contains all the SQL scripts needed to set up the Leave Management System database for the Venkata Allocation Management System.

---

## Files in This Directory

### 1. **01_CreateLeaveManagementSchema.sql**
Creates the complete database schema for Leave Management.

**Contains:**
- `Holidays` table - Stores holiday information
- `LeaveRequests` table - Stores leave request records
- `LeaveTypes` table - Lookup table for leave types (optional normalization)
- `LeaveStatuses` table - Lookup table for leave statuses (optional normalization)
- Views for reporting:
  - `vw_LeaveRequestDetails` - Join of leave requests with associate info
  - `vw_HolidayCalendar` - Holiday calendar with day of week info
- Stored Procedures:
  - `sp_CalculateWorkingDays` - Calculate working days between dates
  - `sp_GetAssociateLeaveSummary` - Get leave summary for an associate
  - `sp_CheckLeaveConflict` - Check for overlapping leave requests

---

### 2. **02_LoadMasterData.sql**
Populates lookup values and initial holiday data.

**Populates:**
- Leave Types: Annual, Sick, Casual, Maternity, Paternity, Other
- Leave Statuses: Pending, Approved, Rejected
- Holidays for 2026-2028:
  - Recurring holidays (New Year, Republic Day, Independence Day, Gandhi Jayanti, Christmas)
  - Festival holidays (Diwali, Holi)
  - Company-specific holidays

**Features:**
- Idempotent (safe to run multiple times)
- Prevents duplicate insertions
- Includes summary statistics

---

### 3. **03_QueryExamples.sql**
Contains useful queries for reporting, analysis, and maintenance.

**Includes:**
- View all leave requests with associate details
- Holiday queries and calendar views
- Leave summary reports
- Resource availability calculations (for revenue forecasting)
- Leave conflict detection
- Data validation queries
- Maintenance and archival queries
- Performance reports

---

## Execution Steps

### Prerequisites
- SQL Server Management Studio (SSMS) or SQL Server query editor
- Database: `VenkataAllocationManagementSystem`
- Connection: Ensure you can connect to your SQL Server instance

### Step 1: Create the Schema

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Select your database: `VenkataAllocationManagementSystem`
4. Open file: `01_CreateLeaveManagementSchema.sql`
5. Execute the script (F5 or click "Execute")

**Expected Output:**
```
Table [dbo].[Holidays] created successfully.
Table [dbo].[LeaveRequests] created successfully.
Table [dbo].[LeaveTypes] created successfully.
Table [dbo].[LeaveStatuses] created successfully.
View [dbo].[vw_LeaveRequestDetails] created successfully.
View [dbo].[vw_HolidayCalendar] created successfully.
Stored Procedure [dbo].[sp_CalculateWorkingDays] created successfully.
Stored Procedure [dbo].[sp_GetAssociateLeaveSummary] created successfully.
Stored Procedure [dbo].[sp_CheckLeaveConflict] created successfully.
```

### Step 2: Load Master Data

1. Open file: `02_LoadMasterData.sql`
2. Execute the script

**Expected Output:**
```
Populating [dbo].[LeaveTypes]...
Leave Types populated successfully.
Populating [dbo].[LeaveStatuses]...
Leave Statuses populated successfully.
Populating Recurring Holidays...
Recurring Holidays populated successfully.
Populating Festival Holidays...
Festival Holidays populated successfully.
Populating Company-Specific Holidays...
Company-Specific Holidays populated successfully.

MASTER DATA SUMMARY
========================================
Leave Types: [Count]
Leave Statuses: [Count]
Holidays: [Total] [Recurring] [OneTime]
```

### Step 3: Verify Installation (Optional)

Run this verification query:

```sql
-- Check tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Holidays', 'LeaveRequests', 'LeaveTypes', 'LeaveStatuses');

-- Check data was loaded
SELECT 'LeaveTypes' AS TableName, COUNT(*) AS Count FROM [dbo].[LeaveTypes]
UNION ALL
SELECT 'LeaveStatuses', COUNT(*) FROM [dbo].[LeaveStatuses]
UNION ALL
SELECT 'Holidays', COUNT(*) FROM [dbo].[Holidays];
```

---

## Database Schema Details

### Holidays Table

```sql
CREATE TABLE [dbo].[Holidays] (
    [HolidayId] INT PRIMARY KEY IDENTITY(1,1),
    [HolidayName] NVARCHAR(150) NOT NULL,
    [HolidayDate] DATE NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsRecurring] BIT NOT NULL DEFAULT 0,
    [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

**Indexes:**
- `IX_Holidays_Date` - On `HolidayDate` for date range queries
- `IX_Holidays_Recurring` - On `IsRecurring` for annual holiday queries

---

### LeaveRequests Table

```sql
CREATE TABLE [dbo].[LeaveRequests] (
    [LeaveRequestId] INT PRIMARY KEY IDENTITY(1,1),
    [AssociateId] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [RequestedDays] DECIMAL(5,2) NOT NULL,
    [LeaveType] NVARCHAR(50) NOT NULL DEFAULT 'Annual',
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [Notes] NVARCHAR(500) NULL,
    [RequestedOn] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [ApprovedOn] DATETIME NULL,
    [ApprovedBy] NVARCHAR(MAX) NULL,
    
    CONSTRAINT [FK_LeaveRequests_Associates] 
        FOREIGN KEY ([AssociateId]) REFERENCES [dbo].[Associates]([AssociateId])
        ON DELETE CASCADE
);
```

**Indexes:**
- `IX_LeaveRequests_AssociateId` - For associate lookups
- `IX_LeaveRequests_Dates` - For date range queries
- `IX_LeaveRequests_Status` - For status filtering
- `IX_LeaveRequests_LeaveType` - For leave type analysis
- `IX_LeaveRequests_AssociateId_Dates` - Composite for common queries

---

### LeaveTypes Table (Lookup)

```sql
CREATE TABLE [dbo].[LeaveTypes] (
    [LeaveTypeId] INT PRIMARY KEY IDENTITY(1,1),
    [LeaveTypeName] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(200) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

**Default Values:**
- Annual - Annual paid leave
- Sick - Sick leave for medical reasons
- Casual - Casual unpaid leave
- Maternity - Maternity leave for expectant mothers
- Paternity - Paternity leave for new fathers
- Other - Other types of leave

---

### LeaveStatuses Table (Lookup)

```sql
CREATE TABLE [dbo].[LeaveStatuses] (
    [LeaveStatusId] INT PRIMARY KEY IDENTITY(1,1),
    [StatusName] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(200) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

**Default Values:**
- Pending - Leave request awaiting approval
- Approved - Leave request has been approved
- Rejected - Leave request has been rejected

---

## Views Provided

### vw_LeaveRequestDetails
Joins leave requests with associate information for easier reporting.

```sql
SELECT * FROM [dbo].[vw_LeaveRequestDetails]
WHERE [Status] = 'Approved'
ORDER BY [AssociateName], [StartDate];
```

---

### vw_HolidayCalendar
Shows holidays with day of week and year/month breakdown.

```sql
SELECT * FROM [dbo].[vw_HolidayCalendar]
WHERE [HolidayDate] >= CAST(GETDATE() AS DATE)
ORDER BY [HolidayDate];
```

---

## Stored Procedures

### sp_CalculateWorkingDays
Calculates the number of working days between two dates, excluding weekends and holidays.

```sql
DECLARE @WorkingDays DECIMAL(5,2);
EXEC [dbo].[sp_CalculateWorkingDays] 
    @StartDate = '2026-07-01',
    @EndDate = '2026-07-31',
    @WorkingDays = @WorkingDays OUTPUT;
SELECT @WorkingDays AS [WorkingDays];
```

---

### sp_GetAssociateLeaveSummary
Gets a summary of leave requests for a specific associate in a given year.

```sql
EXEC [dbo].[sp_GetAssociateLeaveSummary] 
    @AssociateId = 1,
    @Year = 2026;
```

---

### sp_CheckLeaveConflict
Checks if a proposed leave period conflicts with existing approved or pending leaves.

```sql
DECLARE @HasConflict BIT;
EXEC [dbo].[sp_CheckLeaveConflict]
    @AssociateId = 1,
    @StartDate = '2026-08-01',
    @EndDate = '2026-08-15',
    @HasConflict = @HasConflict OUTPUT;
SELECT CASE WHEN @HasConflict = 1 THEN 'Conflict Found' ELSE 'No Conflict' END;
```

---

## Common Queries for Revenue Forecasting

### Get Resource Availability for a Month

```sql
SELECT 
    a.[FullName],
    a.[AssociateEmployeeId],
    -- Working days calculation
    (
        SELECT COUNT(*) FROM [dbo].[Holidays]
        WHERE MONTH([HolidayDate]) = MONTH(GETDATE())
        AND DATEPART(WEEKDAY, [HolidayDate]) NOT IN (1, 7)
    ) AS [Holidays],
    -- Approved leave days
    (
        SELECT COALESCE(SUM(CAST([RequestedDays] AS INT)), 0)
        FROM [dbo].[LeaveRequests]
        WHERE [AssociateId] = a.[AssociateId]
        AND [Status] = 'Approved'
        AND MONTH([StartDate]) = MONTH(GETDATE())
    ) AS [ApprovedLeaveDays],
    -- Available working days
    22 - (
        SELECT COUNT(*) FROM [dbo].[Holidays]
        WHERE MONTH([HolidayDate]) = MONTH(GETDATE())
        AND DATEPART(WEEKDAY, [HolidayDate]) NOT IN (1, 7)
    ) - (
        SELECT COALESCE(SUM(CAST([RequestedDays] AS INT)), 0)
        FROM [dbo].[LeaveRequests]
        WHERE [AssociateId] = a.[AssociateId]
        AND [Status] = 'Approved'
        AND MONTH([StartDate]) = MONTH(GETDATE())
    ) AS [AvailableWorkingDays]
FROM [dbo].[Associates] a
WHERE a.[AssociateStatusId] = 1  -- Active
ORDER BY a.[FullName];
```

---

## Adding Custom Holidays

To add a company-specific holiday:

```sql
INSERT INTO [dbo].[Holidays] 
([HolidayName], [HolidayDate], [Description], [IsRecurring])
VALUES 
('Holiday Name', '2026-MM-DD', 'Description', 1);
-- Set IsRecurring = 1 for annual holidays, 0 for one-time
```

---

## Data Maintenance

### View All Pending Approvals Older Than 7 Days

```sql
SELECT 
    [LeaveRequestId], [AssociateId], [StartDate], [EndDate],
    [RequestedOn], DATEDIFF(DAY, [RequestedOn], GETDATE()) AS [DaysPending]
FROM [dbo].[LeaveRequests]
WHERE [Status] = 'Pending'
  AND DATEDIFF(DAY, [RequestedOn], GETDATE()) > 7
ORDER BY [RequestedOn];
```

### Archive Old Records

```sql
-- Check old leave records (for archival planning)
SELECT COUNT(*) AS [RecordsOlderThan2Years]
FROM [dbo].[LeaveRequests]
WHERE YEAR([StartDate]) < YEAR(GETDATE()) - 1;
```

---

## Troubleshooting

### Issue: Foreign Key Error When Inserting Leave Request

**Cause:** AssociateId doesn't exist in Associates table

**Solution:**
```sql
-- Verify Associates exist
SELECT COUNT(*) FROM [dbo].[Associates];

-- Verify specific AssociateId
SELECT * FROM [dbo].[Associates] WHERE [AssociateId] = @YourAssociateId;
```

---

### Issue: Holidays Not Showing in Auto-Calculation

**Cause:** Holiday date might be in the past or for a different year

**Solution:**
```sql
-- Check holidays in date range
SELECT * FROM [dbo].[Holidays]
WHERE [HolidayDate] BETWEEN '2026-07-01' AND '2026-12-31'
ORDER BY [HolidayDate];
```

---

### Issue: Stored Procedure Not Found

**Cause:** Script might not have executed successfully

**Solution:**
```sql
-- Check if procedure exists
SELECT * FROM sys.objects WHERE name = 'sp_CalculateWorkingDays';

-- Re-run the schema creation script
```

---

## Integration with Application

The Leave Management module in the application automatically uses these tables:

1. **ManageLeaves View** → Reads/Writes `LeaveRequests` table
2. **ManageHolidays View** → Reads/Writes `Holidays` table
3. **Auto-calculation** → Calls `sp_CalculateWorkingDays` stored procedure
4. **Date validation** → Checks against `Holidays` table for non-working days

---

## Performance Considerations

1. **Indexes**: All major queries are indexed for optimal performance
2. **Composite Indexes**: `IX_LeaveRequests_AssociateId_Dates` optimizes common multi-field queries
3. **Partitioning**: For large datasets (100K+ records), consider partitioning by year
4. **Archive**: Archive records older than 2-3 years to maintain performance

---

## Backup Recommendations

Before modifying holiday data:

```sql
-- Backup current holidays
SELECT * INTO [dbo].[Holidays_Backup_20260707] FROM [dbo].[Holidays];

-- Backup current leave requests
SELECT * INTO [dbo].[LeaveRequests_Backup_20260707] FROM [dbo].[LeaveRequests];
```

---

## Support & Questions

For questions or issues with the Leave Management database:

1. Check the Query Examples file for similar scenarios
2. Review stored procedure logic in the schema creation script
3. Verify data integrity using the validation queries provided
4. Ensure all prerequisites (Associates table) are properly populated

---

**Last Updated:** 2026-07-07  
**Version:** 1.0  
**Database Version:** SQL Server 2016+
