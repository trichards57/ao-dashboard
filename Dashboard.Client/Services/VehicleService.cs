// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Grpc;
using Grpc.Core;

namespace Dashboard.Client.Services;

/// <summary>
/// Service to manage vehicles.
/// </summary>
/// <param name="client">The gRPC client to use.</param>
internal class VehicleService(Vehicles.VehiclesClient client) : IVehicleService
{
    private readonly Vehicles.VehiclesClient client = client;

    /// <inheritdoc/>
    public async IAsyncEnumerable<Vehicle> GetSettingsAsync(Place place)
    {
        var response = client.GetAll(new GetAllVehiclesRequest { Place = place });

        while (await response.ResponseStream.MoveNext())
        {
            yield return response.ResponseStream.Current.Vehicle;
        }
    }

    /// <inheritdoc/>
    public async Task<Vehicle?> GetSettingsAsync(Guid id)
    {
        try
        {
            return (await client.GetAsync(new GetVehicleRequest { Id = id.ToString() })).Vehicle;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> PutSettingsAsync(UpdateVehiclesRequest settings)
    {
        var result = await client.UpdateAsync(settings);

        return result.Success;
    }
}