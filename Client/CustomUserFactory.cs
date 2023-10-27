using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorApp1.Client
{
    public class CustomUserFactory
        : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public CustomUserFactory(IAccessTokenProviderAccessor accessor)
            : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var roleClaims = identity.FindAll(identity.RoleClaimType).ToArray();

                if (roleClaims.Any())
                {
                    foreach (var existingClaim in roleClaims)
                    {
                        identity.RemoveClaim(existingClaim);
                    }

                    var rolesElem =
                        account.AdditionalProperties[identity.RoleClaimType];

                    if (options.RoleClaim is not null && rolesElem is JsonElement roles)
                    {
                        if (roles.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var role in roles.EnumerateArray())
                            {
                                var roleValue = role.GetString();

                                if (!string.IsNullOrEmpty(roleValue))
                                {
                                    identity.AddClaim(
                                      new Claim(options.RoleClaim, roleValue));
                                }

                            }
                        }
                        else
                        {
                            var roleValue = roles.GetString();

                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                identity.AddClaim(
                                  new Claim(options.RoleClaim, roleValue));
                            }
                        }
                    }
                }
            }

            return user;
        }
    }
}
