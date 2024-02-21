// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;

namespace AmbulanceDashboard.Services;

/// <summary>
/// Represents a service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the claims for the provided user.
    /// </summary>
    /// <param name="userId">The ID of the user to check for.</param>
    /// <returns>The extra claims to add to the user.</returns>
    IAsyncEnumerable<Claim> GetClaimsAsync(string userId);
}
