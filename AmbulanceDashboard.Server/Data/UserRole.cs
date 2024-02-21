// -----------------------------------------------------------------------
// <copyright file="UserRole.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceDashboard.Data;

/// <summary>
/// Defines the standard permission level for the given permission.
/// </summary>
/// <remarks>
/// The highest permission a user holds will apply.
/// </remarks>
public enum PermissionLevel
{
    /// <summary>
    /// The user may not use this permission.
    /// </summary>
    Forbid = 0,

    /// <summary>
    /// The user may read this data.
    /// </summary>
    Read = 1,

    /// <summary>
    /// The user may read and write this data.
    /// </summary>
    ReadWrite = 2,
}

/// <summary>
/// Represents a role available to a user.
/// </summary>
public class Role
{
    /// <summary>
    /// Gets or sets the unique ID of the role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the permission level for user permissions for the role.
    /// </summary>
    public PermissionLevel Permissions { get; set; } = PermissionLevel.Forbid;

    /// <summary>
    /// Gets or sets the permission level for sensitive user permissions for the role.
    /// </summary>
    public PermissionLevel SensitivePermissions { get; set; } = PermissionLevel.Forbid;

    /// <summary>
    /// Gets or sets the permission level for vehicle configurations for the role.
    /// </summary>
    public PermissionLevel VehicleConfiguration { get; set; } = PermissionLevel.Read;

    /// <summary>
    /// Gets or sets the permissions for VOR data for the role.
    /// </summary>
    public PermissionLevel VorData { get; set; } = PermissionLevel.Read;
}

/// <summary>
/// Represents a role assignment to a user.
/// </summary>
public class UserRole
{
    /// <summary>
    /// Gets or sets the role associated with this link.
    /// </summary>
    public Role? Role { get; set; }

    /// <summary>
    /// Gets or sets the role ID.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the user's ID.
    /// </summary>
    public string UserId { get; set; } = "";
}
