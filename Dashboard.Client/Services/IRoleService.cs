// -----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Services;

/// <summary>
/// Represents the permission levels for a role's permissions.
/// </summary>
public enum ReadWrite
{
    /// <summary>
    /// The role may read this data.
    /// </summary>
    Read,

    /// <summary>
    /// The role may read and update this data.
    /// </summary>
    Write,

    /// <summary>
    /// The role may not see this data.
    /// </summary>
    Deny,
}

/// <summary>
/// Represents a service for managing roles.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Gets the permissions for a role.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <returns>The permissions.</returns>
    Task<RolePermissions?> GetRolePermissions(string id);

    /// <summary>
    /// Gets all of the roles.
    /// </summary>
    /// <returns>The roles.</returns>
    IAsyncEnumerable<RolePermissions> GetRoles();

    /// <summary>
    /// Updates the permissions for a role.
    /// </summary>
    /// <param name="id">The ID of the role to update.</param>
    /// <param name="permissions">The new permissions.</param>
    /// <returns>
    /// <see langword="true" /> if the permissions were updated; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions);
}

/// <summary>
/// The permissions for a role.
/// </summary>
public sealed class RolePermissions
{
    /// <summary>
    /// Gets or sets the ID of the role.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the Permissions permission for the role.
    /// </summary>
    public ReadWrite Permissions { get; set; }

    /// <summary>
    /// Gets or sets the Vehicle Configuration permission for the role.
    /// </summary>
    public ReadWrite VehicleConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the VOR Data permission for the role.
    /// </summary>
    public ReadWrite VorData { get; set; }
}

/// <summary>
/// Represents an update to a role's permissions.
/// </summary>
public sealed class RolePermissionsUpdate
{
    /// <summary>
    /// Gets or sets the new Permissions permission for the role.
    /// </summary>
    public ReadWrite Permissions { get; set; }

    /// <summary>
    /// Gets or sets the new Vehicle Configuration permission for the role.
    /// </summary>
    public ReadWrite VehicleConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the new VOR Data permission for the role.
    /// </summary>
    public ReadWrite VorData { get; set; }
}