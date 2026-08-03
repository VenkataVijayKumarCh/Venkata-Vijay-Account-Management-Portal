using System.ComponentModel.DataAnnotations;

namespace VenkataAllocationManagementSystem.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }

        [Required]
        public int AssociateId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required, Range(0.5, 365)]
        public decimal RequestedDays { get; set; }

        [Required, StringLength(50)]
        public string LeaveType { get; set; } = "Annual";

        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public bool IsValid { get; set; } = true;

        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedBy { get; set; }

        public Associate? Associate { get; set; }
        public int LeaveStatusId { get; set; }
    }
}
