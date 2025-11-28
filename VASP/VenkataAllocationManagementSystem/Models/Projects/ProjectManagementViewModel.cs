using System.Collections.Generic;
using VenkataAllocationManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class ProjectManagementViewModel
    {
        public Project? Project { get; set; } 
        public IEnumerable<Project>? Projects { get; set; }
        // public IEnumerable<Associate>? Associates { get; set; }
        // public IEnumerable<Allocation>? Allocations { get; set; }

        public string? AccountName { get; set; }
        public string? AccountId { get; set; }

        public int? SelectedAccountId { get; set; } = 0;
        // public IEnumerable<SelectListItem>? Accounts { get; set; }

        public IEnumerable<Account>? Accounts { get; set; }

        public IEnumerable<Associate>? Associates { get; set; }
        
        public IEnumerable<Allocation>? Allocations { get; set; }
    }
}