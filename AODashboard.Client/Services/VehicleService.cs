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
internal sealed class VehicleService(HttpClient client) : IVehicleService
{
    /// <inheritdoc/>
    public Task AddEntriesAsync(IList<VorIncident> vorIncident) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddEntryAsync(VorIncident vorIncident) => throw new NotImplementedException();

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
    public Task<string> GetEtagByCallSignAsync(string callSign) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<string> GetEtagByRegistrationAsync(string registration) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<DateTimeOffset?> GetLastUpdateByCallSignAsync(string callSign) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<DateTimeOffset?> GetLastUpdateByPlace(Region region, string? district, string? hub) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<DateTimeOffset?> GetLastUpdateByRegistration(string registration) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<VorStatistics> GetStatisticsByPlace(Region region, string? district, string? hub)
    {
        var uri = $"/api/vors/byPlace/stats?region={region}&district={district ?? ""}&hub={hub ?? ""}";
        try
        {
           return await client.GetFromJsonAsync<VorStatistics>(uri);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public Task<string> GetStatisticsEtagByPlace(Region region, string? district, string? hub) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<VorStatus?> GetStatusByCallSignAsync(string callSign) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<VorStatus?> GetStatusByRegistrationAsync(string registration) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async IAsyncEnumerable<VorStatus> GetStatusesByPlace(Region region, string? district, string? hub)
    {
        var uri = $"/api/vors/byPlace?region={region}&district={district ?? ""}&hub={hub ?? ""}";

        var response = await client.GetFromJsonAsync<IEnumerable<VorStatus>>(uri) ?? [];

        foreach (var item in response)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public Task<string> GetStatusesEtagByPlace(Region region, string? district, string? hub) => throw new NotImplementedException();

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
