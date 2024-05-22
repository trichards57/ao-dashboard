// -----------------------------------------------------------------------
// <copyright file="VorService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Grpc;
using Grpc.Core;

namespace Dashboard.Client.Services;

/// <summary>
/// Service for interacting with the VOR API.
/// </summary>
/// <param name="httpClient">The HTTP Client to use.</param>
internal class VorService(Vor.VorClient client) : IVorService
{
    private readonly Vor.VorClient client = client;

    /// <inheritdoc/>
    public async Task<GetVorStatisticsResponse> GetVorStatisticsAsync(Place place)
        => await client.GetStatisticsAsync(new GetVorStatisticsRequest { Place = place });

    /// <inheritdoc/>
    public async IAsyncEnumerable<GetVorStatusResponse> GetVorStatusesAsync(Place place)
    {
        var response = client.GetStatus(new GetVorStatusRequest { Place = place });

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current;
        }
    }
}
