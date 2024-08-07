// -----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

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