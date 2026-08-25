using VenkataAllocationManagementSystem.Models;
 using Microsoft.AspNetCore.Mvc.Rendering;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class LeaveManagementViewModel
    {
        public IEnumerable<Associate>? Associates { get; set; }

        public IEnumerable<LeaveRequest>? LeaveRequests { get; set; }

        public LeaveRequest? LeaveRequest { get; set; }

        public IEnumerable<Holiday>? Holidays { get; set; }

        public Holiday? Holiday { get; set; }
        public int AssociateId {get; set;}
        public int ProjectId {get; set;}
        public IEnumerable<Project>? Projects {get;set;}  
        public string? SelectedMonth { get; set; }

        public IEnumerable<SelectListItem>? MonthList {get; set; }
    }
}
