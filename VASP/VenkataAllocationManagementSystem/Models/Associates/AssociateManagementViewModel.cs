using System.Collections.Generic;
using VenkataAllocationManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VenkataAllocationManagementSystem.ViewModels
{
    public class AssociateManagementViewModel
    {
        public Associate? Associate { get; set; }
        public IEnumerable<Associate>? Associates { get; set; }

        public string? AssociateType { get; set; }

        public string? AssociateStatusName { get; set; }

        public IEnumerable<AssociateStatus>? AssociateStatus { get; set; }

        public IEnumerable<AssociateTypes>? AssociateTypes { get; set; }
    }
}