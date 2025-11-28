using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using VenkataAllocationManagementSystem.Models;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class AccountManagementViewModel
    {
        public Account? Account { get; set; }
        public IEnumerable<Project>? Projects { get; set; }
        // public IEnumerable<Associate>? Associates { get; set; }
        // public IEnumerable<Allocation>? Allocations { get; set; }

        public IEnumerable<SelectListItem>? Accounts { get; set; }
    }
}