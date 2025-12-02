using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class TimesheetPeriodsViewModel
    {
        public int TimesheetPeriodId { get; set; }
        public List<TimesheetPeriod>? TimesheetPeriods { get; set; } = new List<TimesheetPeriod>();  
    }
}