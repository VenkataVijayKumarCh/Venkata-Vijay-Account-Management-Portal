using Microsoft.AspNetCore.Authorization;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.CustomClass
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(params Roles[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}