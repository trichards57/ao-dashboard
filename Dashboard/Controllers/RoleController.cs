// -----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for managing roles.
/// </summary>
[Authorize(Policy = "CanEditRoles")]
public class RoleController(IRoleService roleService) : Roles.RolesBase
{
    private readonly IRoleService roleService = roleService;

    /// <summary>
    /// Gets a specific user role.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the requested role.
    /// </returns>
    public override async Task<GetRoleResponse> Get(GetRoleRequest request, ServerCallContext context)
    {
        var role = await roleService.GetRolePermissions(request.Id) ??
            throw new RpcException(new Status(StatusCode.NotFound, "Role not found"));

        return new GetRoleResponse { Role = role };
    }

    /// <summary>
    /// Gets all of the registered user roles.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="responseStream">The stream to write responses to.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public override async Task GetAll(GetAllRolesRequest request, IServerStreamWriter<GetAllRolesResponse> responseStream, ServerCallContext context)
    {
        await foreach (var role in roleService.GetRoles())
        {
            await responseStream.WriteAsync(new GetAllRolesResponse { Role = role });
        }
    }

    /// <summary>
    /// Updates a user role.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the result of the role.
    /// </returns>
    public override async Task<UpdateRoleResponse> Update(UpdateRoleRequest request, ServerCallContext context)
        => new UpdateRoleResponse { Success = await roleService.SetRolePermissions(request) };
}
