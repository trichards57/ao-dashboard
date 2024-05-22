// -----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Services;

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
    Task<Grpc.RolePermissions?> GetRolePermissions(string id);

    /// <summary>
    /// Gets all of the roles.
    /// </summary>
    /// <returns>The roles.</returns>
    IAsyncEnumerable<Grpc.RolePermissions> GetRoles();

    /// <summary>
    /// Updates the permissions for a role.
    /// </summary>
    /// <param name="permissions">The new permissions.</param>
    /// <returns>
    /// <see langword="true" /> if the permissions were updated; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SetRolePermissions(Grpc.UpdateRoleRequest permissions);
}
