using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace VenkataAllocationManagementSystem.Common
{
    public class HelperClass()
    {

        // this method will return the Identity user details


        // public int GetCurrentUserId()
        // {
        //     if (User.Identity!.IsAuthenticated)
        //     {
        //         if (User.Identity != null & @User.Identity!.IsAuthenticated)
        //         {
        //             var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
        //             if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        //             {
        //                 return userId;
        //             }
        //         }
        //     }

        //     throw new UnauthorizedAccessException("User ID claim not found or invalid.");
        // }
    }
}