// -----------------------------------------------------------------------
// <copyright file="ApplicationSignInManager.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dashboard.Services;

/// <summary>
/// Sign in manager for the application, including custom logic for SJA email addresses.
/// </summary>
public class ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<ApplicationSignInManager> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation) : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
    /// <inheritdoc/>
    public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        if (userName.EndsWith("@sja.org.uk"))
        {
            return Task.FromResult(SignInResult.NotAllowed);
        }

        return base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    /// <inheritdoc/>
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
