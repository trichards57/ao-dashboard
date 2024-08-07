// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for retrieving and updating vehicle settings from the client side.
/// </summary>
/// <param name="httpClient">HTTP Client used to access the server.</param>
/// <param name="jsonOptions">Options for JSON serialization.</param>
internal class VehicleService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IVehicleService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly JsonSerializerOptions jsonOptions = jsonOptions;

    /// <inheritdoc/>
    public IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place) 
        => httpClient.GetFromJsonAsAsyncEnumerable<VehicleSettings>($"api/vehicles{place.CreateQuery()}", jsonOptions).OfType<VehicleSettings>();

    /// <inheritdoc/>
    public Task<VehicleSettings?> GetSettingsAsync(Guid id) => httpClient.GetFromJsonAsync<VehicleSettings>($"api/vehicles/{id}", jsonOptions);

    /// <inheritdoc/>
    public Task PutSettingsAsync(UpdateVehicleSettings settings) => httpClient.PostAsJsonAsync("api/vehicles", settings, jsonOptions);
}
