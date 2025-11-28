using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenkataAllocationManagementSystem.Models
{
    public class BillabilityTypes
    {
        [Key]
        public int BillabilityTypeId { get; set; }

        public string? BillabilityTypeName { get; set; }
    }
}