// -----------------------------------------------------------------------
// <copyright file="VorService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using System.Net.Http.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the VOR API.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class VorService(HttpClient httpClient) : IVorService
{
    private readonly HttpClient httpClient = httpClient;

    /// <inheritdoc/>
    public async Task<VorStatistics> GetVorStatisticsAsync(Place place)
    {
        var response = await httpClient.GetAsync($"api/vor/statistics{place.CreateQuery()}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VorStatistics>() ?? new();
        }

        return new();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place)
    {
        var response = await httpClient.GetAsync($"api/vor{place.CreateQuery()}");

        if (response.IsSuccessStatusCode)
        {
            var statuses = await response.Content.ReadFromJsonAsync<List<VorStatus>>();

            if (statuses != null)
            {
                foreach (var status in statuses)
                {
                    yield return status;
                }
            }
        }
    }
}
