// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the user API.
/// </summary>
/// <param name="httpClient">The HTTP client to use.</param>
/// <param name="logger">The logger to use.</param>
internal class UserService(HttpClient httpClient) : IUserService
{
    private readonly HttpClient httpClient = httpClient;

    /// <inheritdoc/>
    public async Task<UserWithRole?> GetUserWithRole(string id)
    {
        var response = await httpClient.GetAsync($"api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserWithRole>();
        }

        return null;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<UserWithRole> GetUsersWithRole()
    {
        var response = await httpClient.GetAsync($"api/users");

        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<UserWithRole>>();

            if (users != null)
            {
                foreach (var user in users)
                {
                    yield return user;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetUserRole(string id, UserRoleUpdate role)
    {
        var response = await httpClient.PutAsJsonAsync($"api/users/{id}", role);

        return response.IsSuccessStatusCode;
    }
}
