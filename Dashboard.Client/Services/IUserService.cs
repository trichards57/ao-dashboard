// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;

namespace Dashboard.Client.Services;

/// <summary>
/// Represents a service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Cancels the invitation associated with <paramref name="email"/>.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CancelInvite(string email);

    /// <summary>
    /// Gets all of the invites currently in the system.
    /// </summary>
    /// <returns>The list of invites.</returns>
    IAsyncEnumerable<UserInviteSummary> GetAllInvites();

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
    /// Creates an invitation with the given user details.
    /// </summary>
    /// <param name="request">The requested user details.</param>
    /// <param name="invitingUserId">The user making the request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the result of the operation.</returns>
    Task<bool> InviteUser(UserInviteRequest request, string invitingUserId);

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

/// <summary>
/// Represents a user with their role.
/// </summary>
public sealed class UserWithRole
{
    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of their role.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets their role's ID.
    /// </summary>
    public string? RoleId { get; set; }
}

/// <summary>
/// Represents an update to a user's role.
/// </summary>
public sealed class UserRoleUpdate
{
    /// <summary>
    /// Gets or sets the ID of the new role.
    /// </summary>
    public string RoleId { get; set; } = "";
}
