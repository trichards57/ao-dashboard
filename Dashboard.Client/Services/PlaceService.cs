// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using System.Net.Http.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for retrieving places.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class PlaceService(HttpClient httpClient) : IPlaceService
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetDistricts(Region region)
    {
        var response = await httpClient.GetAsync($"api/places/{region}");

        if (response.IsSuccessStatusCode)
        {
            var districts = await response.Content.ReadFromJsonAsync<List<string>>();

            if (districts != null)
            {
                foreach (var district in districts)
                {
                    yield return district;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetHubs(Region region, string district)
    {
        var response = await httpClient.GetAsync($"api/places/{region}/{district}");

        if (response.IsSuccessStatusCode)
        {
            var hubs = await response.Content.ReadFromJsonAsync<List<string>>();

            if (hubs != null)
            {
                foreach (var hub in hubs)
                {
                    yield return hub;
                }
            }
        }
    }
}
