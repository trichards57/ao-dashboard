// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Validation;
using System.Net;
using System.Net.Http.Json;

namespace AODashboard.Client.Services;

/// <summary>
/// A service to manage vehicles on the server.
/// </summary>
/// <param name="client">The client used to communicate with the server.</param>
internal class VehicleService(HttpClient client) : IVehicleService
{
    /// <inheritdoc/>
    public Task AddEntryAsync(VorIncident vorIncident) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task ClearVorsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetByCallSignAsync(string callSign)
    {
        var uri = $"/api/vehicle-settings?callSign={Uri.EscapeDataString(callSign)}";

        try
        {
            return await client.GetFromJsonAsync<VehicleSettings?>(uri);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetByRegistrationAsync(string registration)
    {
        var uri = $"/api/vehicle-settings?registration={Uri.EscapeDataString(registration)}";

        try
        {
            return await client.GetFromJsonAsync<VehicleSettings?>(uri);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public Task<VorStatus?> GetStatusByCallSignAsync(string callSign) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<VorStatus?> GetStatusByRegistrationAsync(string registration) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task UpdateSettingsAsync(UpdateVehicleSettings settings)
    {
        var uri = "/api/vehicle-settings";

        var response = await client.PostAsJsonAsync(uri, settings);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new InvalidRequestException(await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(), "Validation errors were reported by the server.");
        }

        response.EnsureSuccessStatusCode();
    }
}
