// -----------------------------------------------------------------------
// <copyright file="AccountUserClaimsPrincipalFactory.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dashboard.Services;

/// <summary>
/// Claims principal factory that standardises the authentication method claim and adds the user's real name claim.
/// </summary>
public class AccountUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
{
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
