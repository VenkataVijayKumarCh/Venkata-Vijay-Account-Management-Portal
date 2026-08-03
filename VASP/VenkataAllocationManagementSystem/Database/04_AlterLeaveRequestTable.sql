-- ============================================================================
-- ALTER SCRIPT: Add Comments and IsValid fields to LeaveRequest table
-- Purpose: Add new columns to support enhanced leave approval workflow
-- Date: 2026-07-07
-- ============================================================================

USE [VenkataAllocationManagementSystem]
GO

-- Check if Comments column exists before adding
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'LeaveRequest' 
    AND COLUMN_NAME = 'Comments'
)
BEGIN
    ALTER TABLE dbo.LeaveRequest
    ADD Comments NVARCHAR(1000) NULL;
    
    PRINT 'Column [Comments] added to LeaveRequest table.'
END
ELSE
BEGIN
    PRINT 'Column [Comments] already exists in LeaveRequest table. Skipping...'
END
GO

-- Check if IsValid column exists before adding
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'LeaveRequest' 
    AND COLUMN_NAME = 'IsValid'
)
BEGIN
    ALTER TABLE dbo.LeaveRequest
    ADD IsValid BIT DEFAULT 1 NOT NULL;
    
    PRINT 'Column [IsValid] added to LeaveRequest table with default value 1.'
END
ELSE
BEGIN
    PRINT 'Column [IsValid] already exists in LeaveRequest table. Skipping...'
END
GO

-- Verify the new columns
EXEC sp_help 'dbo.LeaveRequest';
GO

PRINT '============================================================================'
PRINT 'ALTER TABLE script completed successfully.'
PRINT '============================================================================'
PRINT 'New columns added:'
PRINT '- Comments (NVARCHAR(1000), nullable): For manager decision notes'
PRINT '- IsValid (BIT, default 1): For soft delete functionality'
PRINT '============================================================================'
GO
