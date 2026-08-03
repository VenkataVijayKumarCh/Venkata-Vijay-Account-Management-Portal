using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace VenkataAllocationManagementSystem.Models
{
    public class TimesheetStatus
    {
        public int TimesheetStatusId { get; set; }
        public string TimesheetStatusName { get; set; } = string.Empty;
    }
}