// -----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for managing roles.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class RoleService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IRoleService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly JsonSerializerOptions jsonOptions = jsonOptions;

    /// <inheritdoc/>
    public Task<RolePermissions?> GetRolePermissions(string id) => httpClient.GetFromJsonAsync<RolePermissions>($"api/roles/{id}", jsonOptions);

    /// <inheritdoc/>
    public IAsyncEnumerable<RolePermissions> GetRoles()
        => httpClient.GetFromJsonAsAsyncEnumerable<RolePermissions>($"api/roles", jsonOptions).OfType<RolePermissions>();

    /// <inheritdoc/>
    public async Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions)
    {
        var response = await httpClient.PostAsJsonAsync($"api/roles/{id}", permissions, jsonOptions);

        return response.IsSuccessStatusCode;
    }
}
