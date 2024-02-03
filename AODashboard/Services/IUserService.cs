// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;

namespace AODashboard.Services;

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

    /// <summary>
    /// Gets the current user's photograph.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a memory stream containing the photo as a JPEG, or null
    /// if none is available.
    /// </returns>
    Task<MemoryStream?> GetProfilePictureAsync();
}
