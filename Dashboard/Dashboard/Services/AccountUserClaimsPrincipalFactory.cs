using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dashboard.Services;

public class AccountUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public AccountUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    /// <inheritdoc/>
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (identity.HasClaim(c => c.Type == "amr"))
        {
            var val = identity.FindFirst("amr")!.Value;
            identity.TryRemoveClaim(identity.FindFirst("amr"));
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, val));
        }

        identity.TryRemoveClaim(identity.FindFirst(ClaimTypes.Name));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.RealName));

        return identity;
    }
}
