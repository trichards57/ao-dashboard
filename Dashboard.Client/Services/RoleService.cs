// -----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http.Json;

namespace Dashboard.Client.Services;

internal class RoleService(HttpClient httpClient, ILogger<RoleService> logger) : IRoleService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<RoleService> logger = logger;

    public async Task<RolePermissions?> GetRolePermissions(string id)
    {
        var response = await httpClient.GetAsync($"api/roles/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RolePermissions>();
        }

        return null;
    }

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

    public async Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions)
    {
        var response = await httpClient.PutAsJsonAsync($"api/roles/{id}", permissions);

        return response.IsSuccessStatusCode;
    }
}
