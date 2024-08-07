// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

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
    /// Gets or sets their role's ID.
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// Gets or sets the name of their role.
    /// </summary>
    public string? Role { get; set; }
}
