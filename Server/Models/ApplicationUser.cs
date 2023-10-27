using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<IdentityUserRole<string>> Roles { get; set; }
        public ICollection<IdentityUserClaim<string>> Claims { get; set; }
    }
}