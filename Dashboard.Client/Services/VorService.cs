// -----------------------------------------------------------------------
// <copyright file="VorService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the VOR API.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class VorService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IVorService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly JsonSerializerOptions jsonOptions = jsonOptions;

    /// <inheritdoc/>
    public Task<VorStatistics?> GetVorStatisticsAsync(Place place) => httpClient.GetFromJsonAsync<VorStatistics>($"api/vor/statistics{place.CreateQuery()}", jsonOptions);

    /// <inheritdoc/>
    public IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place)
        => httpClient.GetFromJsonAsAsyncEnumerable<VorStatus>($"api/vor{place.CreateQuery()}", jsonOptions).OfType<VorStatus>();
}
