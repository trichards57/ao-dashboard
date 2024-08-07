// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for retrieving places.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class PlaceService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IPlaceService
{
    /// <inheritdoc/>
    public IAsyncEnumerable<string> GetDistricts(Region region)
        => httpClient.GetFromJsonAsAsyncEnumerable<string>($"api/places/{region}", jsonOptions).OfType<string>();

    /// <inheritdoc/>
    public IAsyncEnumerable<string> GetHubs(Region region, string district)
        => httpClient.GetFromJsonAsAsyncEnumerable<string>($"api/places/{region}/{district}", jsonOptions).OfType<string>();
}
