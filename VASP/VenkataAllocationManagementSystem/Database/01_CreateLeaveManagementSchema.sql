-- =====================================================
-- LEAVE MANAGEMENT SYSTEM - DATABASE SCHEMA
-- Created: 2026-07-07
-- Database: VenkataAllocationManagementSystem
-- =====================================================

-- =====================================================
-- 1. HOLIDAYS TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Holidays]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Holidays] (
        [HolidayId] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        [HolidayName] NVARCHAR(150) NOT NULL,
        [HolidayDate] DATE NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsRecurring] BIT NOT NULL DEFAULT 0,
        [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
    );

    -- Create indexes for Holiday queries
    CREATE INDEX [IX_Holidays_Date] ON [dbo].[Holidays]([HolidayDate]);
    CREATE INDEX [IX_Holidays_Recurring] ON [dbo].[Holidays]([IsRecurring]);
    
    PRINT 'Table [dbo].[Holidays] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Holidays] already exists.';
END

-- =====================================================
-- 2. LEAVE REQUESTS TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveRequests]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaveRequests] (
        [LeaveRequestId] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        [AssociateId] INT NOT NULL,
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NOT NULL,
        [RequestedDays] DECIMAL(5, 2) NOT NULL,
        [LeaveType] NVARCHAR(50) NOT NULL DEFAULT 'Annual',
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Notes] NVARCHAR(500) NULL,
        [RequestedOn] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [ApprovedOn] DATETIME NULL,
        [ApprovedBy] NVARCHAR(MAX) NULL,
        
        -- Foreign Key relationship
        CONSTRAINT [FK_LeaveRequests_Associates] 
            FOREIGN KEY ([AssociateId]) REFERENCES [dbo].[Associates]([AssociateId])
            ON DELETE CASCADE
    );

    -- Create indexes for LeaveRequest queries
    CREATE INDEX [IX_LeaveRequests_AssociateId] ON [dbo].[LeaveRequests]([AssociateId]);
    CREATE INDEX [IX_LeaveRequests_Dates] ON [dbo].[LeaveRequests]([StartDate], [EndDate]);
    CREATE INDEX [IX_LeaveRequests_Status] ON [dbo].[LeaveRequests]([Status]);
    CREATE INDEX [IX_LeaveRequests_LeaveType] ON [dbo].[LeaveRequests]([LeaveType]);
    CREATE INDEX [IX_LeaveRequests_AssociateId_Dates] ON [dbo].[LeaveRequests]([AssociateId], [StartDate], [EndDate]);
    
    PRINT 'Table [dbo].[LeaveRequests] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[LeaveRequests] already exists.';
END

-- =====================================================
-- 3. LEAVE TYPES LOOKUP TABLE (Optional - for normalization)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaveTypes] (
        [LeaveTypeId] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        [LeaveTypeName] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
    );

    PRINT 'Table [dbo].[LeaveTypes] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[LeaveTypes] already exists.';
END

-- =====================================================
-- 4. LEAVE STATUS LOOKUP TABLE (Optional - for normalization)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaveStatuses] (
        [LeaveStatusId] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        [StatusName] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE()
    );

    PRINT 'Table [dbo].[LeaveStatuses] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[LeaveStatuses] already exists.';
END

-- =====================================================
-- 5. VIEW FOR LEAVE REQUESTS WITH ASSOCIATE DETAILS
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[vw_LeaveRequestDetails]') AND type in (N'V'))
BEGIN
    CREATE VIEW [dbo].[vw_LeaveRequestDetails] AS
    SELECT 
        lr.[LeaveRequestId],
        lr.[AssociateId],
        a.[FullName] AS [AssociateName],
        a.[AssociateEmployeeId],
        lr.[StartDate],
        lr.[EndDate],
        lr.[RequestedDays],
        lr.[LeaveType],
        lr.[Status],
        lr.[Notes],
        lr.[RequestedOn],
        lr.[ApprovedOn],
        lr.[ApprovedBy],
        DATEDIFF(DAY, lr.[StartDate], lr.[EndDate]) + 1 AS [CalendarDays],
        CASE 
            WHEN lr.[Status] = 'Approved' THEN 1
            WHEN lr.[Status] = 'Pending' THEN 0
            WHEN lr.[Status] = 'Rejected' THEN -1
        END AS [StatusOrder]
    FROM [dbo].[LeaveRequests] lr
    INNER JOIN [dbo].[Associates] a ON lr.[AssociateId] = a.[AssociateId];

    PRINT 'View [dbo].[vw_LeaveRequestDetails] created successfully.';
END
ELSE
BEGIN
    PRINT 'View [dbo].[vw_LeaveRequestDetails] already exists.';
END

-- =====================================================
-- 6. VIEW FOR HOLIDAY CALENDAR
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[vw_HolidayCalendar]') AND type in (N'V'))
BEGIN
    CREATE VIEW [dbo].[vw_HolidayCalendar] AS
    SELECT 
        [HolidayId],
        [HolidayName],
        [HolidayDate],
        DATENAME(WEEKDAY, [HolidayDate]) AS [DayOfWeek],
        [Description],
        [IsRecurring],
        [CreatedOn],
        YEAR([HolidayDate]) AS [Year],
        MONTH([HolidayDate]) AS [Month],
        DAY([HolidayDate]) AS [Day]
    FROM [dbo].[Holidays]
    ORDER BY [HolidayDate];

    PRINT 'View [dbo].[vw_HolidayCalendar] created successfully.';
END
ELSE
BEGIN
    PRINT 'View [dbo].[vw_HolidayCalendar] already exists.';
END

-- =====================================================
-- 7. STORED PROCEDURE - Get Working Days Between Dates
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CalculateWorkingDays]') AND type in (N'P'))
BEGIN
    CREATE PROCEDURE [dbo].[sp_CalculateWorkingDays]
        @StartDate DATE,
        @EndDate DATE,
        @WorkingDays DECIMAL(5,2) OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @CurrentDate DATE = @StartDate;
        DECLARE @Count DECIMAL(5,2) = 0;

        -- Loop through each day in the range
        WHILE @CurrentDate <= @EndDate
        BEGIN
            -- Check if it's not a weekend (1 = Sunday, 7 = Saturday)
            IF DATEPART(WEEKDAY, @CurrentDate) NOT IN (1, 7)
            BEGIN
                -- Check if it's not a holiday
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayDate] = @CurrentDate)
                BEGIN
                    SET @Count = @Count + 1;
                END
            END

            -- Move to next day
            SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
        END

        SET @WorkingDays = @Count;
    END

    PRINT 'Stored Procedure [dbo].[sp_CalculateWorkingDays] created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored Procedure [dbo].[sp_CalculateWorkingDays] already exists.';
END

-- =====================================================
-- 8. STORED PROCEDURE - Get Leave Summary for Associate
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAssociateLeaveSummary]') AND type in (N'P'))
BEGIN
    CREATE PROCEDURE [dbo].[sp_GetAssociateLeaveSummary]
        @AssociateId INT,
        @Year INT
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            @AssociateId AS [AssociateId],
            (SELECT [FullName] FROM [dbo].[Associates] WHERE [AssociateId] = @AssociateId) AS [AssociateName],
            [LeaveType],
            [Status],
            COUNT(*) AS [NumberOfRequests],
            SUM([RequestedDays]) AS [TotalDays],
            MIN([StartDate]) AS [EarliestLeaveDate],
            MAX([EndDate]) AS [LatestLeaveDate]
        FROM [dbo].[LeaveRequests]
        WHERE [AssociateId] = @AssociateId
            AND YEAR([StartDate]) = @Year
        GROUP BY [LeaveType], [Status]
        ORDER BY [LeaveType], [Status];
    END

    PRINT 'Stored Procedure [dbo].[sp_GetAssociateLeaveSummary] created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored Procedure [dbo].[sp_GetAssociateLeaveSummary] already exists.';
END

-- =====================================================
-- 9. STORED PROCEDURE - Check for Leave Conflicts
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CheckLeaveConflict]') AND type in (N'P'))
BEGIN
    CREATE PROCEDURE [dbo].[sp_CheckLeaveConflict]
        @AssociateId INT,
        @StartDate DATE,
        @EndDate DATE,
        @HasConflict BIT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;

        -- Check if there are any overlapping approved or pending leaves
        IF EXISTS (
            SELECT 1 
            FROM [dbo].[LeaveRequests]
            WHERE [AssociateId] = @AssociateId
                AND [Status] IN ('Approved', 'Pending')
                AND [StartDate] <= @EndDate
                AND [EndDate] >= @StartDate
        )
        BEGIN
            SET @HasConflict = 1;
        END
        ELSE
        BEGIN
            SET @HasConflict = 0;
        END
    END

    PRINT 'Stored Procedure [dbo].[sp_CheckLeaveConflict] created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored Procedure [dbo].[sp_CheckLeaveConflict] already exists.';
END

PRINT '========================================';
PRINT 'Leave Management Schema Created Successfully!';
PRINT '========================================';
