# Database Scripts - Quick Reference

## Overview
Complete database implementation for Leave Management System with schema, lookup data, and useful queries.

---

## 📁 File Locations

All files are in: `Database/`

```
Database/
├── 01_CreateLeaveManagementSchema.sql    [Schema creation]
├── 02_LoadMasterData.sql                 [Lookup data & holidays]
├── 03_QueryExamples.sql                  [Useful queries & reports]
└── README.md                             [Complete guide]
```

---

## 🚀 Quick Start (3 Steps)

### Step 1: Create Schema
Execute: `01_CreateLeaveManagementSchema.sql`

**Creates:**
- ✅ Holidays table
- ✅ LeaveRequests table  
- ✅ LeaveTypes lookup table
- ✅ LeaveStatuses lookup table
- ✅ 2 Views (vw_LeaveRequestDetails, vw_HolidayCalendar)
- ✅ 3 Stored Procedures

**Time:** ~5 seconds

---

### Step 2: Load Master Data
Execute: `02_LoadMasterData.sql`

**Populates:**
- ✅ 6 Leave Types (Annual, Sick, Casual, Maternity, Paternity, Other)
- ✅ 3 Leave Statuses (Pending, Approved, Rejected)
- ✅ 13+ Holidays (2026-2028) including:
  - Recurring holidays (New Year, Republic Day, etc.)
  - Festival holidays (Diwali, Holi)
  - Company holidays

**Time:** ~2 seconds

---

### Step 3: Verify Installation (Optional)
Execute any query from: `03_QueryExamples.sql`

---

## 📊 Database Schema Summary

### Tables Created

| Table | Records | Purpose |
|-------|---------|---------|
| `Holidays` | ~20 | Public/company holidays |
| `LeaveRequests` | 0 (ready for data) | Leave requests from associates |
| `LeaveTypes` | 6 | Leave type lookup (Annual, Sick, etc.) |
| `LeaveStatuses` | 3 | Status lookup (Pending, Approved, Rejected) |

### Views Created

| View | Purpose |
|------|---------|
| `vw_LeaveRequestDetails` | Leave requests with associate info |
| `vw_HolidayCalendar` | Holiday list with day-of-week |

### Stored Procedures Created

| Procedure | Purpose |
|-----------|---------|
| `sp_CalculateWorkingDays` | Calculate working days (excluding weekends/holidays) |
| `sp_GetAssociateLeaveSummary` | Get leave summary for an associate |
| `sp_CheckLeaveConflict` | Detect overlapping leave requests |

---

## 📋 File Contents

### 01_CreateLeaveManagementSchema.sql
**Lines:** ~350 | **Size:** ~12 KB

**Sections:**
1. Holidays table with indexes
2. LeaveRequests table with FK to Associates
3. LeaveTypes lookup table
4. LeaveStatuses lookup table
5. vw_LeaveRequestDetails view
6. vw_HolidayCalendar view
7. sp_CalculateWorkingDays procedure
8. sp_GetAssociateLeaveSummary procedure
9. sp_CheckLeaveConflict procedure

**Safety:** All operations wrapped in `IF NOT EXISTS` for idempotency

---

### 02_LoadMasterData.sql
**Lines:** ~180 | **Size:** ~7 KB

**Sections:**
1. LeaveTypes: 6 types with descriptions
2. LeaveStatuses: 3 statuses with descriptions
3. Recurring Holidays: For years 2026-2028
   - New Year Day (01-01)
   - Republic Day (01-26)
   - Independence Day (08-15)
   - Gandhi Jayanti (10-02)
   - Christmas (12-25)
4. Festival Holidays:
   - Diwali (2026: Nov 8, 2027: Oct 29, 2028: Nov 16)
   - Holi (2026: Mar 15, 2027: Mar 4, 2028: Mar 22)
5. Company Holidays:
   - Company Foundation Day (04-15)
   - Summer Break (05-20)

**Safety:** All inserts check for duplicates before inserting

---

### 03_QueryExamples.sql
**Lines:** ~450 | **Size:** ~18 KB

**Query Categories:**
1. **View All Data** (5 queries)
   - All leave requests
   - Approved leaves
   - Pending approvals
   - All holidays
   - Upcoming holidays

2. **Summaries** (5 queries)
   - Leave by associate
   - Leave by type
   - Leave statistics

3. **Revenue Forecasting** (3 queries)
   - Resource availability
   - Working days calculations
   - Leave impact analysis

4. **Conflict Detection** (1 query)
   - Find overlapping leaves

5. **Stored Procedure Examples** (3 examples)
   - Calculate working days
   - Get associate summary
   - Check conflicts

6. **Data Validation** (3 queries)
   - Invalid date ranges
   - Missing associates
   - Missing approval details

7. **Maintenance** (3 queries)
   - Archive old records
   - Pending action items
   - Holiday statistics

8. **Reporting** (3 queries)
   - Monthly trends
   - Utilization analysis
   - Top users

---

## 🔑 Key Features

### Auto-Calculation of Working Days
```sql
EXEC sp_CalculateWorkingDays 
    @StartDate = '2026-07-01',
    @EndDate = '2026-07-31',
    @WorkingDays = @WorkingDays OUTPUT;
```
Excludes: Weekends (Sat-Sun) + Holidays

---

### Leave Conflict Detection
```sql
EXEC sp_CheckLeaveConflict
    @AssociateId = 1,
    @StartDate = '2026-08-01',
    @EndDate = '2026-08-15',
    @HasConflict = @HasConflict OUTPUT;
```

---

### Resource Availability for Revenue Forecasting
Query calculates:
- Available working days per associate per month
- Deducts approved leaves
- Deducts holidays
- Shows capacity for forecasting

---

## 💾 Data Relationships

```
Associates (existing)
    ↓ (1:N)
LeaveRequests
    ├─ AssociateId (FK)
    ├─ LeaveType (references LeaveTypes)
    └─ Status (references LeaveStatuses)

Holidays (standalone)
    └─ Used by sp_CalculateWorkingDays
```

---

## 📈 Indexes Created

**Performance Optimized Indexes:**
- `IX_Holidays_Date` - Date range queries
- `IX_Holidays_Recurring` - Annual holiday lookups
- `IX_LeaveRequests_AssociateId` - Filter by associate
- `IX_LeaveRequests_Dates` - Date range queries
- `IX_LeaveRequests_Status` - Status filtering
- `IX_LeaveRequests_LeaveType` - Leave type analysis
- `IX_LeaveRequests_AssociateId_Dates` - Composite for common queries

---

## ✅ Testing Checklist

After running the scripts:

- [ ] Check `Holidays` table has ~20 records
- [ ] Check `LeaveTypes` table has 6 records
- [ ] Check `LeaveStatuses` table has 3 records
- [ ] Verify views exist: `vw_LeaveRequestDetails`, `vw_HolidayCalendar`
- [ ] Verify stored procedures: `sp_CalculateWorkingDays`, `sp_GetAssociateLeaveSummary`, `sp_CheckLeaveConflict`
- [ ] Run a sample query from `03_QueryExamples.sql`

---

## 🔧 Customization

### Add New Holiday
```sql
INSERT INTO [dbo].[Holidays] 
([HolidayName], [HolidayDate], [Description], [IsRecurring])
VALUES ('My Holiday', '2026-MM-DD', 'Description', 1);
```

### Add New Leave Type
```sql
INSERT INTO [dbo].[LeaveTypes]
([LeaveTypeName], [Description], [IsActive])
VALUES ('Custom Leave', 'Description', 1);
```

### Add New Leave Status
```sql
INSERT INTO [dbo].[LeaveStatuses]
([StatusName], [Description], [IsActive])
VALUES ('On Hold', 'Description', 1);
```

---

## 📞 Support

For detailed information:
1. See **README.md** - Complete guide with troubleshooting
2. See **Query Examples** - Specific query patterns for your use case
3. Review **Schema Creation** - Logic and relationships

---

## Summary Stats

| Metric | Value |
|--------|-------|
| **Total Lines of SQL** | ~980 |
| **Total File Size** | ~37 KB |
| **Tables Created** | 4 |
| **Views Created** | 2 |
| **Stored Procedures** | 3 |
| **Indexes Created** | 7 |
| **Sample Queries** | 30+ |
| **Holidays Prepopulated** | 15+ for 3 years |
| **Leave Types** | 6 |
| **Leave Statuses** | 3 |

---

**Status:** ✅ Ready for Production  
**Last Updated:** 2026-07-07  
**Compatibility:** SQL Server 2016+
