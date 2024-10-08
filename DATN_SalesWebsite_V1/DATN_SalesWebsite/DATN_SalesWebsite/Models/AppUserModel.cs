using Microsoft.AspNetCore.Identity;

namespace DATN_SalesWebsite.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation {  get; set; }

        public string RoleId { get; set; } 
    }
}
