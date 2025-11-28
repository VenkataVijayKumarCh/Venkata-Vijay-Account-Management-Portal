using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenkataAllocationManagementSystem.Models
{
    public class AssociateStatus
    {
        public int AssociateStatusId { get; set; }

        public string? AssociateStatusName { get; set; }
    }
}