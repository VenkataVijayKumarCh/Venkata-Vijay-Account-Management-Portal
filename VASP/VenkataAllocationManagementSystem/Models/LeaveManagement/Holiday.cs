using System.ComponentModel.DataAnnotations;

namespace VenkataAllocationManagementSystem.Models
{
    public class Holiday
    {
        public int HolidayId { get; set; }

        [Required, StringLength(150)]
        public string HolidayName { get; set; } = string.Empty;

        [Required]
        public DateOnly HolidayDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsRecurring { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
