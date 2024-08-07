// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;

namespace Dashboard.Client.Services;

/// <summary>
/// Represents a service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all of the users with their roles.
    /// </summary>
    /// <returns>The users with their roles.</returns>
    IAsyncEnumerable<UserWithRole> GetUsersWithRole();

    /// <summary>
    /// Gets a user with their role.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user and their role.</returns>
    Task<UserWithRole?> GetUserWithRole(string id);

    /// <summary>
    /// Updates the role of a user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="role">The new role.</param>
    /// <returns>
    /// <see langword="true"/> if the role was updated; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SetUserRole(string id, UserRoleUpdate role);
}
