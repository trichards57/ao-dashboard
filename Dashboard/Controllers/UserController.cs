// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Tony Richards">
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
/// Controller for managing users.
/// </summary>
[Authorize(Policy = "CanViewUsers")]
public class UserController(IUserService userService) : Users.UsersBase
{
    private readonly IUserService userService = userService;

    public override async Task GetAll(GetAllUsersRequest request, IServerStreamWriter<GetAllUsersResponse> responseStream, ServerCallContext context)
    {
        await foreach (var user in userService.GetUsersWithRole())
        {
            await responseStream.WriteAsync(new GetAllUsersResponse
            {
                User = user,
            });
        }
    }

    public override async Task<GetUserResponse> Get(GetUserRequest request, ServerCallContext context)
    {
        var user = await userService.GetUserWithRole(request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        return new GetUserResponse { User = user };
    }

    [Authorize(Policy = "CanEditUsers")]
    public override async Task<UpdateUserResponse> Update(UpdateUserRequest request, ServerCallContext context)
        => new UpdateUserResponse { Success = await userService.SetUserRole(request) };
}
