using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Server.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }

        public ApplicationRole(string roleName)
            : base(roleName) { }

        public ICollection<IdentityRoleClaim<string>> Claims { get; set; }
    }
}