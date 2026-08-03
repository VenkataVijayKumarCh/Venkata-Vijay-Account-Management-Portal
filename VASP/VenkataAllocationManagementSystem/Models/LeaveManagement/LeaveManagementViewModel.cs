using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class LeaveManagementViewModel
    {
        public IEnumerable<Associate>? Associates { get; set; }

        public IEnumerable<LeaveRequest>? LeaveRequests { get; set; }

        public LeaveRequest? LeaveRequest { get; set; }

        public IEnumerable<Holiday>? Holidays { get; set; }

        public Holiday? Holiday { get; set; }
    }
}
