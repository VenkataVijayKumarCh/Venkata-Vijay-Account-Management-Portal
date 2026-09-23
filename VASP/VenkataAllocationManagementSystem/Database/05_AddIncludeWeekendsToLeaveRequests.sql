-- Add per-request weekend policy for leave day calculation.
IF COL_LENGTH('dbo.LeaveRequests', 'IncludeWeekends') IS NULL
BEGIN
    ALTER TABLE [dbo].[LeaveRequests]
        ADD [IncludeWeekends] BIT NOT NULL CONSTRAINT [DF_LeaveRequests_IncludeWeekends] DEFAULT 0;
END