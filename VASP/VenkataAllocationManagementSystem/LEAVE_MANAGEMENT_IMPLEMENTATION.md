# Leave Management System - Implementation Summary

## Overview
A comprehensive Leave & Holiday Management module has been successfully integrated into the Venkata Allocation Management System. This feature allows managers to record associate leave requests and manage the holiday calendar, which can be used to compute accurate revenue forecasts based on resource availability.

---

## Files Created

### Models (LeaveManagement folder)

#### 1. **Holiday.cs** - [Models/LeaveManagement/Holiday.cs](Models/LeaveManagement/Holiday.cs)
Represents public holidays and company-wide days off.

**Properties:**
- `HolidayId` (int) - Primary key
- `HolidayName` (string, max 150) - Name of the holiday
- `HolidayDate` (DateOnly) - Date of the holiday
- `Description` (string, max 500, optional) - Holiday description
- `IsRecurring` (bool) - Whether the holiday repeats annually
- `CreatedOn` (DateTime) - Timestamp when created

#### 2. **LeaveRequest.cs** - [Models/LeaveManagement/LeaveRequest.cs](Models/LeaveManagement/LeaveRequest.cs)
Captures associate leave information with full audit trail.

**Properties:**
- `LeaveRequestId` (int) - Primary key
- `AssociateId` (int, FK) - Link to Associate
- `StartDate` (DateOnly) - Leave start date
- `EndDate` (DateOnly) - Leave end date
- `RequestedDays` (decimal) - Number of days (supports 0.5 for half-days)
- `LeaveType` (string) - Type of leave (Annual, Sick, Casual, Maternity, Paternity, Other)
- `Status` (string) - Status (Pending, Approved, Rejected)
- `Notes` (string, max 500, optional) - Additional notes
- `RequestedOn` (DateTime) - Timestamp when requested
- `ApprovedOn` (DateTime, optional) - Approval timestamp
- `ApprovedBy` (string, optional) - Who approved it
- `Associate` (navigation property) - Reference to Associate

#### 3. **LeaveManagementViewModel.cs** - [Models/LeaveManagement/LeaveManagementViewModel.cs](Models/LeaveManagement/LeaveManagementViewModel.cs)
ViewModel for managing both leave and holiday data.

**Properties:**
- `Associates` - List of all associates
- `LeaveRequests` - List of leave requests
- `LeaveRequest` - Current leave request being edited
- `Holidays` - List of holidays
- `Holiday` - Current holiday being edited

---

## Controller Changes

### LeaveManagementController.cs - [Controllers/LeaveManagementController.cs](Controllers/LeaveManagementController.cs)

**Authorization:** `[CustomAuthorize(Roles.Admin, Roles.Manager)]` - Only accessible to Admin and Manager roles

**Methods:**

#### Leave Management Methods
- `ManageLeaves()` - GET/POST - Display and filter leave requests
- `CreateLeave()` - POST - Save a new leave request
- `EditLeave(id)` - GET - Display leave edit form
- `EditLeave(model)` - POST - Update existing leave request
- `DeleteLeave(id)` - POST - Delete a leave request

**Features:**
- Auto-calculation of working days (excludes weekends and holidays)
- Support for half-day leaves (0.5 increments)
- Manual override of calculated days
- Leave type selection (Annual, Sick, Casual, Maternity, Paternity, Other)
- Status tracking (Pending, Approved, Rejected)
- Full date range validation

#### Holiday Management Methods
- `ManageHolidays()` - GET/POST - Display and manage holidays
- `CreateHoliday()` - POST - Add a new holiday
- `EditHoliday(id)` - GET - Display holiday edit form
- `EditHoliday(model)` - POST - Update holiday
- `DeleteHoliday(id)` - POST - Delete a holiday

**Features:**
- Recurring holiday support (for annual events like New Year, Christmas)
- Holiday summary statistics
- Day of week display

---

## Database Integration

### ApplicationDbContext.cs Changes - [Data/ApplicationDbContext.cs](Data/ApplicationDbContext.cs)

**New DbSets added:**
```csharp
public DbSet<LeaveRequest> LeaveRequests { get; set; }
public DbSet<Holiday> Holidays { get; set; }
```

These entities are fully integrated into EF Core and ready for database migrations.

---

## Views Created

### 1. ManageLeaves.cshtml - [Views/LeaveManagement/ManageLeaves.cshtml](Views/LeaveManagement/ManageLeaves.cshtml)

**Features:**
- Associate dropdown (auto-sorted by name)
- Leave type selector (6 types)
- Date range picker (start and end dates)
- Auto-calculate button for working days
- Manual day adjustment field
- Status selector (Pending, Approved, Rejected)
- Notes/comments field
- Comprehensive leave history table with:
  - Associate name
  - Leave type with badge
  - Date range
  - Number of days
  - Status with color-coded badges
  - Request timestamp
  - Edit and delete actions

### 2. ManageHolidays.cshtml - [Views/LeaveManagement/ManageHolidays.cshtml](Views/LeaveManagement/ManageHolidays.cshtml)

**Features:**
- Holiday name input
- Holiday date picker
- Description field
- Recurring toggle checkbox
- Holiday calendar table with:
  - Holiday name
  - Date
  - Day of week
  - Recurring status
  - Description
  - Edit and delete actions
- Summary statistics (Total, Recurring, One-time)

### 3. EditLeave.cshtml - [Views/LeaveManagement/EditLeave.cshtml](Views/LeaveManagement/EditLeave.cshtml)

**Features:**
- Full leave request edit form
- Associate name (read-only, disabled dropdown)
- Leave type selector
- Date range adjustment
- Days modification with auto-calculate support
- Status change capability
- Notes update

### 4. EditHoliday.cshtml - [Views/LeaveManagement/EditHoliday.cshtml](Views/LeaveManagement/EditHoliday.cshtml)

**Features:**
- Holiday name edit
- Date modification
- Description update
- Recurring toggle adjustment

---

## Navigation Integration

### _Layout.cshtml Changes - [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml)

Added menu items under "Management Dashboard" submenu:
- **Leave Management** - Links to `LeaveManagement/ManageLeaves`
- **Holiday Calendar** - Links to `LeaveManagement/ManageHolidays`

Both accessible to Admin and Manager roles.

---

## Key Features Implemented

### 1. **Auto-Calculation of Working Days**
- Automatically calculates business days between start and end dates
- Excludes weekends (Saturday & Sunday)
- Excludes company holidays
- Editable for manual adjustments
- Supports half-day entries (0.5 increments)

### 2. **Leave Request Management**
- Record leave requests with start/end dates
- Support for multiple leave types
- Status tracking (Pending, Approved, Rejected)
- Edit and delete capabilities
- Associate audit trail (RequestedOn, ApprovedOn, ApprovedBy)

### 3. **Holiday Calendar Management**
- Maintain company-wide holiday list
- Support for recurring holidays (annual events)
- Holiday descriptions
- Easy edit and delete operations

### 4. **Manager-Centric Interface**
- Managers can record leave for associates
- Dropdown-based associate selection
- Bulk holiday management
- Summary statistics and reporting

### 5. **Database Design Alignment**
- Models align with Timesheet patterns (DateOnly, FK references)
- Proper DateTime tracking (UTC timestamps)
- Decimal precision for flexible leave calculations
- String-based Status/LeaveType for extensibility

---

## Database Schema

```sql
-- Holidays Table
CREATE TABLE Holidays (
    HolidayId INT PRIMARY KEY IDENTITY(1,1),
    HolidayName NVARCHAR(150) NOT NULL,
    HolidayDate DATE NOT NULL,
    Description NVARCHAR(500),
    IsRecurring BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NOT NULL
);

-- LeaveRequests Table
CREATE TABLE LeaveRequests (
    LeaveRequestId INT PRIMARY KEY IDENTITY(1,1),
    AssociateId INT NOT NULL FOREIGN KEY REFERENCES Associates(AssociateId),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    RequestedDays DECIMAL(5,2) NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Notes NVARCHAR(500),
    RequestedOn DATETIME NOT NULL,
    ApprovedOn DATETIME,
    ApprovedBy NVARCHAR(MAX),
    INDEX IX_Associate (AssociateId),
    INDEX IX_Dates (StartDate, EndDate),
    INDEX IX_Status (Status)
);
```

---

## How to Use

### For Managers:

#### Recording Leave:
1. Navigate to **Management Dashboard** → **Leave Management**
2. Select associate from dropdown
3. Choose leave type from options
4. Select start and end dates
5. Click "Auto-Calculate" to compute working days (optional)
6. Adjust days manually if needed (e.g., for half-days, enter 0.5)
7. Select status (Pending, Approved, Rejected)
8. Add notes if required
9. Click "Save Leave Request"

#### Managing Holidays:
1. Navigate to **Management Dashboard** → **Holiday Calendar**
2. Enter holiday name (e.g., "New Year", "Christmas")
3. Select the date
4. Add description (optional)
5. Check "Recurring Holiday" if it happens every year
6. Click "Save Holiday"

---

## Integration with Revenue Forecasting

The Leave & Holiday data can now be used to:

1. **Calculate Resource Availability**
   - Query `LeaveRequests` for Approved status
   - Deduct leave days from allocation calculations
   - Use recurring holidays for annual planning

2. **Compute Revenue Impact**
   - Associate billable days = Total working days - Leave days - Holiday days
   - Apply allocation rate to remaining available days
   - Forecast revenue based on net availability

3. **Query Examples:**
   ```csharp
   // Get approved leaves for an associate in a date range
   var approvedLeaves = _dbContext.LeaveRequests
       .Where(l => l.AssociateId == associateId 
               && l.Status == "Approved"
               && l.StartDate <= endDate 
               && l.EndDate >= startDate)
       .Sum(l => l.RequestedDays);

   // Get recurring holidays for year
   var annualHolidays = _dbContext.Holidays
       .Where(h => h.IsRecurring)
       .ToList();
   ```

---

## Build Status

✅ **Build Successful** - No compilation errors  
Project builds with 2 pre-existing warnings unrelated to Leave Management module.

---

## Notes

- All date fields use `DateOnly` type for consistency with Timesheet module
- Models follow existing project naming conventions and patterns
- Authorization is tied to Manager and Admin roles only
- All timestamps are in UTC
- Leave days support decimal values for half-day leaves
- Holiday management is global (not associate-specific)
- UI uses Bootstrap styling consistent with project

---

## Future Enhancements (Optional)

1. **Leave Balances** - Track annual leave allocation and consumption
2. **Approval Workflow** - Manager approval pipeline
3. **Reports** - Generate leave usage reports by associate/department
4. **Notifications** - Alert managers when leave is approaching
5. **Bulk Upload** - Import holidays from CSV
6. **Carry Forward** - Handle leave carryover logic
7. **Leave Policies** - Define leave rules per associate type
8. **Email Notifications** - Notify stakeholders of leave approvals
