// -----------------------------------------------------------------------
// <copyright file="IdentityUserAccessor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.Components.Account;

/// <summary>
/// Helper class to access the current user.
/// </summary>
internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
{
    /// <summary>
    /// Gets the user from the current context, or redirects to InvalidUser if not available.
    /// </summary>
    /// <param name="context">The HTTP Context to access.</param>
    /// <returns>The user.</returns>
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
