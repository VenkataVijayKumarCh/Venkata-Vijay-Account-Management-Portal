using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;



namespace VenkataAllocationManagementSystem.ViewModels
{
    public class LoginViewModel
    {

        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? UserEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        
    }   
}