using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenkataAllocationManagementSystem.Models
{
    public class AssociateTypes
    {
        [Key]
        public int AssociateTypeId { get; set; }

        public string? AssociateType { get; set; }
    }
}