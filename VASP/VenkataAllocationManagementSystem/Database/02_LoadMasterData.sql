-- =====================================================
-- LEAVE MANAGEMENT SYSTEM - MASTER DATA SCRIPT
-- Created: 2026-07-07
-- Database: VenkataAllocationManagementSystem
-- This script populates lookup values and initial data
-- =====================================================

-- =====================================================
-- 1. POPULATE LEAVE TYPES LOOKUP TABLE
-- =====================================================
PRINT 'Populating [dbo].[LeaveTypes]...';

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Annual')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Annual', 'Annual paid leave', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Sick')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Sick', 'Sick leave for medical reasons', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Casual')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Casual', 'Casual unpaid leave', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Maternity')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Maternity', 'Maternity leave for expectant mothers', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Paternity')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Paternity', 'Paternity leave for new fathers', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveTypes] WHERE [LeaveTypeName] = 'Other')
BEGIN
    INSERT INTO [dbo].[LeaveTypes] ([LeaveTypeName], [Description], [IsActive])
    VALUES ('Other', 'Other types of leave', 1);
END

PRINT 'Leave Types populated successfully.';

-- =====================================================
-- 2. POPULATE LEAVE STATUSES LOOKUP TABLE
-- =====================================================
PRINT 'Populating [dbo].[LeaveStatuses]...';

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveStatuses] WHERE [StatusName] = 'Pending')
BEGIN
    INSERT INTO [dbo].[LeaveStatuses] ([StatusName], [Description], [IsActive])
    VALUES ('Pending', 'Leave request awaiting approval', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveStatuses] WHERE [StatusName] = 'Approved')
BEGIN
    INSERT INTO [dbo].[LeaveStatuses] ([StatusName], [Description], [IsActive])
    VALUES ('Approved', 'Leave request has been approved', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[LeaveStatuses] WHERE [StatusName] = 'Rejected')
BEGIN
    INSERT INTO [dbo].[LeaveStatuses] ([StatusName], [Description], [IsActive])
    VALUES ('Rejected', 'Leave request has been rejected', 1);
END

PRINT 'Leave Statuses populated successfully.';

-- =====================================================
-- 3. POPULATE HOLIDAYS TABLE - RECURRING HOLIDAYS
-- =====================================================
PRINT 'Populating Recurring Holidays...';

-- Populate for the current year and next 2 years (2026, 2027, 2028)
DECLARE @Year INT = YEAR(GETDATE());
DECLARE @EndYear INT = @Year + 2;

WHILE @Year <= @EndYear
BEGIN
    -- New Year Day
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'New Year Day' AND YEAR([HolidayDate]) = @Year)
    BEGIN
        INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
        VALUES ('New Year Day', CONVERT(DATE, CONVERT(VARCHAR(4), @Year) + '-01-01'), 'New Year Celebration', 1);
    END

    -- Republic Day (Jan 26 - India)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Republic Day' AND YEAR([HolidayDate]) = @Year)
    BEGIN
        INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
        VALUES ('Republic Day', CONVERT(DATE, CONVERT(VARCHAR(4), @Year) + '-01-26'), 'National Republic Day', 1);
    END

    -- Independence Day (Aug 15 - India)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Independence Day' AND YEAR([HolidayDate]) = @Year)
    BEGIN
        INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
        VALUES ('Independence Day', CONVERT(DATE, CONVERT(VARCHAR(4), @Year) + '-08-15'), 'National Independence Day', 1);
    END

    -- Gandhi Jayanti (Oct 2)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Gandhi Jayanti' AND YEAR([HolidayDate]) = @Year)
    BEGIN
        INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
        VALUES ('Gandhi Jayanti', CONVERT(DATE, CONVERT(VARCHAR(4), @Year) + '-10-02'), 'Birth Anniversary of Mahatma Gandhi', 1);
    END

    -- Christmas
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Christmas' AND YEAR([HolidayDate]) = @Year)
    BEGIN
        INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
        VALUES ('Christmas', CONVERT(DATE, CONVERT(VARCHAR(4), @Year) + '-12-25'), 'Christmas Celebration', 1);
    END

    SET @Year = @Year + 1;
END

PRINT 'Recurring Holidays populated successfully.';

-- =====================================================
-- 4. POPULATE HOLIDAYS TABLE - REGION SPECIFIC
-- (Comment out sections for regions not applicable)
-- =====================================================

-- DIWALI - Festival of Lights (Usually October/November - varies by lunar calendar)
-- For 2026: November 8, 2026
-- For 2027: October 29, 2027
-- For 2028: November 16, 2028

PRINT 'Populating Festival Holidays...';

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Diwali' AND YEAR([HolidayDate]) = 2026)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Diwali', '2026-11-08', 'Festival of Lights', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Diwali' AND YEAR([HolidayDate]) = 2027)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Diwali', '2027-10-29', 'Festival of Lights', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Diwali' AND YEAR([HolidayDate]) = 2028)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Diwali', '2028-11-16', 'Festival of Lights', 1);
END

-- Holi - Festival of Colors (Usually March - varies by lunar calendar)
-- For 2026: March 15, 2026
-- For 2027: March 4, 2027
-- For 2028: March 22, 2028

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Holi' AND YEAR([HolidayDate]) = 2026)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Holi', '2026-03-15', 'Festival of Colors', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Holi' AND YEAR([HolidayDate]) = 2027)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Holi', '2027-03-04', 'Festival of Colors', 1);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Holi' AND YEAR([HolidayDate]) = 2028)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Holi', '2028-03-22', 'Festival of Colors', 1);
END

PRINT 'Festival Holidays populated successfully.';

-- =====================================================
-- 5. POPULATE OPTIONAL COMPANY-SPECIFIC HOLIDAYS
-- =====================================================
PRINT 'Populating Company-Specific Holidays...';

-- Company Foundation Day
IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Company Foundation Day' AND YEAR([HolidayDate]) = 2026)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Company Foundation Day', '2026-04-15', 'Company Founding Anniversary', 1);
END

-- Summer Break (Optional - adjust dates as needed)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Holidays] WHERE [HolidayName] = 'Summer Break' AND YEAR([HolidayDate]) = 2026)
BEGIN
    INSERT INTO [dbo].[Holidays] ([HolidayName], [HolidayDate], [Description], [IsRecurring])
    VALUES ('Summer Break', '2026-05-20', 'Annual Summer Break', 1);
END

PRINT 'Company-Specific Holidays populated successfully.';

-- =====================================================
-- 6. DISPLAY SUMMARY
-- =====================================================
PRINT '';
PRINT '========================================';
PRINT 'MASTER DATA SUMMARY';
PRINT '========================================';

PRINT '';
PRINT 'Leave Types:';
SELECT COUNT(*) AS [Count], 'Leave Types' AS [Type] FROM [dbo].[LeaveTypes];

PRINT '';
PRINT 'Leave Statuses:';
SELECT COUNT(*) AS [Count], 'Leave Statuses' AS [Type] FROM [dbo].[LeaveStatuses];

PRINT '';
PRINT 'Holidays:';
SELECT COUNT(*) AS [Total], 
       SUM(CASE WHEN [IsRecurring] = 1 THEN 1 ELSE 0 END) AS [Recurring],
       SUM(CASE WHEN [IsRecurring] = 0 THEN 1 ELSE 0 END) AS [OneTime]
FROM [dbo].[Holidays];

PRINT '';
PRINT 'Holidays by Year:';
SELECT YEAR([HolidayDate]) AS [Year], COUNT(*) AS [Count]
FROM [dbo].[Holidays]
GROUP BY YEAR([HolidayDate])
ORDER BY [Year];

PRINT '';
PRINT 'Holiday List (Next 12 Months):';
SELECT [HolidayId], [HolidayName], [HolidayDate], DATENAME(WEEKDAY, [HolidayDate]) AS [Day], [IsRecurring]
FROM [dbo].[Holidays]
WHERE [HolidayDate] >= CAST(GETDATE() AS DATE)
ORDER BY [HolidayDate]
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

PRINT '';
PRINT '========================================';
PRINT 'Master Data Loaded Successfully!';
PRINT '========================================';
