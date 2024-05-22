// -----------------------------------------------------------------------
// <copyright file="PlaceController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Dashboard.Controllers;

[Authorize(Policy = "CanViewVOR")]
public class PlaceController(IPlaceService placeService) : Grpc.Places.PlacesBase
{
    public override async Task GetDistricts(Grpc.GetDistrictsRequest request, IServerStreamWriter<Grpc.GetDistrictsResponse> responseStream, ServerCallContext context)
    {
        await foreach (var district in placeService.GetDistricts(request.Region))
        {
            await responseStream.WriteAsync(new Grpc.GetDistrictsResponse { District = district });
        }
    }

    public override async Task GetHubs(Grpc.GetHubsRequest request, IServerStreamWriter<Grpc.GetHubsResponse> responseStream, ServerCallContext context)
    {
        await foreach (var hub in placeService.GetHubs(request.Region, request.District))
        {
            await responseStream.WriteAsync(new Grpc.GetHubsResponse { Hub = hub });
        }
    }
}
