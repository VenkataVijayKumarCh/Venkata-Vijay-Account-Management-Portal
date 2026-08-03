using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Associate> Associates { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<AllocationRate> AllocationRates { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<AssociateTypes> AssociateTypes { get; set; }

        public DbSet<AssociateStatus> AssociateStatus { get; set; }

        public DbSet<BillabilityTypes> BillabilityTypes { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<TimesheetPeriod> TimesheetPeriods { get; set; }

        public DbSet<Timesheet> Timesheets { get; set; }

        public DbSet<TimesheetLineItem> TimesheetLineItems { get; set; }

        public DbSet<TimesheetStatus> TimesheetStatus { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<Holiday> Holidays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Allocation>()
                .Property(al => al.AllocationPercentage)
                .HasPrecision(7, 2);

            builder.Entity<Project>()
                .Property(p => p.SOWValue)
                .HasPrecision(10, 2);

            builder.Entity<AllocationRate>()
                .Property(ar => ar.AllocationBillRate)
                .HasPrecision(18, 2);

            builder.Entity<AllocationRate>()
                .Property(al => al.AllocationPercentage)
                .HasPrecision(7, 2);

            builder.Entity<Timesheet>()
                .Property(t => t.TotalHours)
                .HasPrecision(5, 2);

            builder.Entity<TimesheetLineItem>()
                .Property(tl => tl.HoursWorked)
                .HasPrecision(5, 2);

            // Example: Configure relationships, constraints, etc.
            // builder.Entity<Project>()
            //     .HasOne(p => p.Account)
            //     .WithMany(a => a.Projects)
            //     .HasForeignKey(p => p.AccountId);

            // Add further configuration as needed for your schema
        }

        // public async List<T> GetProjectInfo(int projectId)
        // {
        //     return await Database.ExecuteSqlAsync("EXEC dbo.GetProjectInfo", (int)projectId).ToListAsync();
        // }
        
        public required string CurrentController { get; set; }
        public required string CurrentAction { get; set; }
        public required string CurrentUser { get; set; }

        // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     var auditEntries = new List<AuditLog>();
            
        //     ChangeTracker.DetectChanges(); // Ensure EF has the latest state


        //     foreach (var entry in ChangeTracker.Entries())
        //     {
        //         if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //             continue;

        //         if (entry.Entity.GetType().Name == "User")
        //         {
        //             ApplicationDbContext _dbContext = this;
        //             var exisitingInfo = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId == ((User)entry.Entity).UserId);
        //         }

        //         var audit = new AuditLog
        //         {
        //             TableName = entry.Entity.GetType().Name,
        //             OperationType = entry.State.ToString(),
        //             Timestamp = DateTime.UtcNow,
        //             KeyValues = string.Join(";", entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => $"{p.Metadata.Name}:{p.CurrentValue}")),
        //             ControllerName = CurrentController,
        //             ActionName = CurrentAction,
        //             UserName = CurrentUser
        //         };

        //         // if (entry.State == EntityState.Modified)
        //         // {
        //         //     System.Diagnostics.EventLog.WriteEntry("Application", "Audit Log - Update Operation Detected");
        //         //     System.Diagnostics.EventLog.WriteEntry("Application", "Changed Properties: " + string.Join(", ", entry.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name)));

        //         //     var changedProperties = entry.Properties
        //         //         .Where(p => p.IsModified)
        //         //         .ToList();

        //         //     // List of changed field names
        //         //     var changedFieldNames = changedProperties.Select(p => p.Metadata.Name).ToList();

        //         //     // Old values (before change)
        //         //     var oldValues = string.Join(";", changedProperties.Select(p => $"{p.Metadata.Name}:{p.OriginalValue}"));

        //         //     // New values (after change)
        //         //     var newValues = string.Join(";", changedProperties.Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));

        //         //     System.Diagnostics.EventLog.WriteEntry("Application", "Changed Fields: " + string.Join(", ", changedFieldNames));
        //         //     System.Diagnostics.EventLog.WriteEntry("Application", "Old Values: " + oldValues);
        //         //     System.Diagnostics.EventLog.WriteEntry("Application", "New Values: " + newValues);

        //         //     audit.OldValues = string.Join(";", entry.Properties.Where(p => p.IsModified).Select(p => $"{p.Metadata.Name}:{p.OriginalValue}"));
        //         //     audit.NewValues = string.Join(";", entry.Properties.Where(p => p.IsModified).Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));
        //         // }
        //         // else if (entry.State == EntityState.Added)
        //         // {
        //         //     audit.NewValues = string.Join(";", entry.Properties.Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));
        //         // }
        //         // else if (entry.State == EntityState.Deleted)
        //         // {
        //         //     audit.OldValues = string.Join(";", entry.Properties.Select(p => $"{p.Metadata.Name}:{p.OriginalValue}"));
        //         // }

        //         switch (entry.State)
        //         {
        //             case EntityState.Modified:
        //                 var modifiedProps = entry.Properties.Where(p => p.IsModified).ToList();

        //                 audit.OldValues = string.Join(";", modifiedProps.Select(p => $"{p.Metadata.Name}:{p.OriginalValue}"));
        //                 audit.NewValues = string.Join(";", modifiedProps.Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));

        //                 System.Diagnostics.EventLog.WriteEntry("Application", $"Audit Log - Update Operation Detected");
        //                 System.Diagnostics.EventLog.WriteEntry("Application", $"Changed Fields: {string.Join(", ", modifiedProps.Select(p => p.Metadata.Name))}");
        //                 System.Diagnostics.EventLog.WriteEntry("Application", $"Old Values: {audit.OldValues}");
        //                 System.Diagnostics.EventLog.WriteEntry("Application", $"New Values: {audit.NewValues}");
        //                 break;

        //             case EntityState.Added:
        //                 audit.NewValues = string.Join(";", entry.Properties
        //                     .Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));
        //                 break;

        //             case EntityState.Deleted:
        //                 audit.OldValues = string.Join(";", entry.Properties
        //                     .Select(p => $"{p.Metadata.Name}:{p.OriginalValue}"));
        //                 break;
        //         }


        //         auditEntries.Add(audit);
        //     }

        //     var result = await base.SaveChangesAsync(cancellationToken);

        //     if (auditEntries.Any())
        //     {
        //         AuditLogs.AddRange(auditEntries);
        //         await base.SaveChangesAsync(cancellationToken);
        //     }

        //     return result;
        // }
    }
}