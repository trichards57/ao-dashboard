// -----------------------------------------------------------------------
// <copyright file="ClaimsTransform.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AODashboard.Services;

/// <summary>
/// Transform to add the role claims to the user's identity.
/// </summary>
/// <param name="userService">The user service to retrieve the user's extra claims.</param>
internal class RolesClaimsTransform(IUserService userService) : IClaimsTransformation
{
    /// <inheritdoc/>
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not available.");

        var claims = await userService.GetClaimsAsync(userId).ToListAsync();

        principal.AddIdentity(new ClaimsIdentity(claims, "Local"));

        return principal;
    }
}
