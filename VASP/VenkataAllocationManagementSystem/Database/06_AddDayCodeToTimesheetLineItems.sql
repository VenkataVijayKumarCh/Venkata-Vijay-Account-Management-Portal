-- Store H (holiday) or L (leave) without changing numeric hour calculations.
IF COL_LENGTH('dbo.TimesheetLineItems', 'DayCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[TimesheetLineItems]
        ADD [DayCode] NVARCHAR(1) NULL;
END