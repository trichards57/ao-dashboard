// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using System.Net.Http;
using System.Net.Http.Json;

namespace Dashboard.Client.Services;

internal class VehicleService(HttpClient httpClient) : IVehicleService
{
    private readonly HttpClient httpClient = httpClient;

    public async IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place)
    {
        var response = await httpClient.GetAsync($"api/vehicles{place.CreateQuery()}");

        if (response.IsSuccessStatusCode)
        {
            var statuses = await response.Content.ReadFromJsonAsync<List<VehicleSettings>>();

            if (statuses != null)
            {
                foreach (var status in statuses)
                {
                    yield return status;
                }
            }
        }
    }

    public async Task<VehicleSettings?> GetSettingsAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"api/vehicles/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VehicleSettings>();
        }

        return null;
    }

    public Task PutSettingsAsync(UpdateVehicleSettings settings) => httpClient.PostAsJsonAsync("api/vehicles", settings);
}
