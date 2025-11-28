using System;
using Microsoft.EntityFrameworkCore;

namespace VenkataAllocationManagementSystem.Models
{
    public class AuditLog
    {
//        [PrimaryKey]
        public int AuditLogId { get; set; }
        public required string TableName { get; set; }
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? UserName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? OperationType { get; set; } // Insert, Update, Delete
    }
}