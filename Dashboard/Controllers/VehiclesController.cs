// -----------------------------------------------------------------------
// <copyright file="VehiclesController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for managing vehicles.
/// </summary>
[Authorize(Policy = "CanViewVOR")]
public class VehiclesController(IVehicleService vehicleService) : Grpc.Vehicles.VehiclesBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    public override async Task GetAll(GetAllVehiclesRequest request, IServerStreamWriter<GetAllVehiclesResponse> responseStream, ServerCallContext context)
    {
        await foreach (var vehicle in vehicleService.GetSettingsAsync(request.Place))
        {
            await responseStream.WriteAsync(new GetAllVehiclesResponse
            {
                Vehicle = vehicle,
            });
        }
    }

    public override async Task<GetVehicleResponse> Get(GetVehicleRequest request, ServerCallContext context)
    {
        var vehicle = await vehicleService.GetSettingsAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));

        return new GetVehicleResponse { Vehicle = vehicle };
    }

    [Authorize(Policy = "CanEditVehicles")]
    public override async Task<UpdateVehiclesResponse> Update(UpdateVehiclesRequest request, ServerCallContext context)
        => new UpdateVehiclesResponse { Success = await vehicleService.PutSettingsAsync(request) };
}
