// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace AODashboard.Client.Services;

/// <summary>
/// Service to manage places on the server.
/// </summary>
/// <param name="client">The client used to communicate with the server.</param>
internal class PlaceService(HttpClient client) : IPlaceService
{
    /// <inheritdoc/>
    public async Task<Places> GetDistrictHubs(Region region, string district)
    {
        var uri = $"/api/places/{region}/{district}/hubs";

        return await client.GetFromJsonAsync<Places>(uri);
    }

    /// <inheritdoc/>
    public async Task<Places> GetDistrictNames(Region region)
    {
        var uri = $"/api/places/{region}/districts";

        return await client.GetFromJsonAsync<Places>(uri);
    }
}
