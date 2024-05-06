// -----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for managing roles.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class RoleService(HttpClient httpClient) : IRoleService
{
    private readonly HttpClient httpClient = httpClient;

    /// <inheritdoc/>
    public async Task<RolePermissions?> GetRolePermissions(string id)
    {
        var response = await httpClient.GetAsync($"api/roles/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RolePermissions>();
        }

        return null;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<RolePermissions> GetRoles()
    {
        var response = await httpClient.GetAsync($"api/roles");

        if (response.IsSuccessStatusCode)
        {
            var roles = await response.Content.ReadFromJsonAsync<List<RolePermissions>>();

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    yield return role;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions)
    {
        var response = await httpClient.PostAsJsonAsync($"api/roles/{id}", permissions);

        return response.IsSuccessStatusCode;
    }
}
