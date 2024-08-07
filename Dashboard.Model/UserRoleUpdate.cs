// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

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
