// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the user API.
/// </summary>
/// <param name="httpClient">The HTTP client to use.</param>
/// <param name="logger">The logger to use.</param>
internal class UserService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IUserService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly JsonSerializerOptions jsonOptions = jsonOptions;

    /// <inheritdoc/>
    public Task<UserWithRole?> GetUserWithRole(string id) => httpClient.GetFromJsonAsync<UserWithRole?>($"api/users/{id}", jsonOptions);

    /// <inheritdoc/>
    public IAsyncEnumerable<UserWithRole?> GetUsersWithRole() => httpClient.GetFromJsonAsAsyncEnumerable<UserWithRole>($"api/users", jsonOptions);

    /// <inheritdoc/>
    public async Task<bool> SetUserRole(string id, UserRoleUpdate role)
    {
        var response = await httpClient.PostAsJsonAsync($"api/users/{id}", role, jsonOptions);

        return response.IsSuccessStatusCode;
    }
}
