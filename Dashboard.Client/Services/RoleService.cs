// -----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Grpc;
using Grpc.Core;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for managing roles.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class RoleService(Roles.RolesClient client) : IRoleService
{
    private readonly Roles.RolesClient client = client;

    /// <inheritdoc/>
    public async Task<RolePermissions?> GetRolePermissions(string id)
    {
        try
        {
            var response = await client.GetAsync(new GetRoleRequest { Id = id });

            return response.Role;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<RolePermissions> GetRoles()
    {
        var response = client.GetAll(new GetAllRolesRequest());

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current.Role;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetRolePermissions(UpdateRoleRequest permissions)
    {
        var response = await client.UpdateAsync(permissions);

        return response.Success;
    }
}
