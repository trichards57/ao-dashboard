// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Grpc;
using Grpc.Core;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the user API.
/// </summary>
/// <param name="httpClient">The HTTP client to use.</param>
/// <param name="logger">The logger to use.</param>
internal class UserService(Users.UsersClient client) : IUserService
{
    private readonly Users.UsersClient client = client;

    /// <inheritdoc/>
    public async IAsyncEnumerable<UserWithRole> GetUsersWithRole()
    {
        var response = client.GetAll(new GetAllUsersRequest());

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current.User;
        }
    }

    /// <inheritdoc/>
    public async Task<UserWithRole?> GetUserWithRole(string id)
    {
        try
        {
            return (await client.GetAsync(new GetUserRequest { Id = id })).User;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetUserRole(UpdateUserRequest update)
    {
        var response = await client.UpdateAsync(update);

        return response.Success;
    }
}
