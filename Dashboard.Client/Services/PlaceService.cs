// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Grpc;
using Grpc.Core;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for retrieving places.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class PlaceService(Places.PlacesClient client) : IPlaceService
{
    private readonly Places.PlacesClient client = client;

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetDistricts(Region region)
    {
        var response = client.GetDistricts(new GetDistrictsRequest { Region = region });

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current.District;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetHubs(Region region, string district)
    {
        var response = client.GetHubs(new GetHubsRequest { Region = region, District = district });

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current.Hub;
        }
    }
}
