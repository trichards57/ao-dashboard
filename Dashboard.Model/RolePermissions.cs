// -----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

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
