using Dashboard.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dashboard.Services;

public class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override Task SignInWithClaimsAsync(ApplicationUser user, AuthenticationProperties? authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        List<Claim> claims = [.. additionalClaims.Where(c => c.Type != "amr"), new Claim("auth_time", DateTimeOffset.UtcNow.ToString("o"))];

        if (additionalClaims.Any(c => c.Type == "amr"))
        {
            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, additionalClaims.First(c => c.Type == "amr").Value));
        }

        return base.SignInWithClaimsAsync(user, authenticationProperties, claims);
    }
}
