// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
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
/// Controller for managing VOR incidents.
/// </summary>
/// <param name="vehicleService">Service to manage vehicles.</param>
[Authorize(Policy = "CanViewVOR")]
public class VorController(Services.IVehicleService vehicleService, IVorService vorService) : Vor.VorBase
{
    private readonly Services.IVehicleService vehicleService = vehicleService;
    private readonly IVorService vorService = vorService;

    /// <summary>
    /// Adds VOR incidents to the database.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the result of the request.
    /// </returns>
    [Authorize(Policy = "CanEditVOR")]
    public override async Task<AddVorIncidentResponse> AddIncident(AddVorIncidentRequest request, ServerCallContext context)
    {
        await vehicleService.AddEntriesAsync(request.Incidents);
        return new AddVorIncidentResponse() { Success = true };
    }

    /// <summary>
    /// Gets the VOR statistics for a given place.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the result of the request.
    /// </returns>
    public override async Task<GetVorStatisticsResponse> GetStatistics(GetVorStatisticsRequest request, ServerCallContext context)
        => await vorService.GetVorStatisticsAsync(request.Place);

    /// <summary>
    /// Gets the VOR statuses for a given place.
    /// </summary>
    /// <param name="request">The gRPC request.</param>
    /// <param name="responseStream">The response stream to write to.</param>
    /// <param name="context">The request context.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public override async Task GetStatus(GetVorStatusRequest request, IServerStreamWriter<GetVorStatusResponse> responseStream, ServerCallContext context)
    {
        await foreach (var status in vorService.GetVorStatusesAsync(request.Place))
        {
            await responseStream.WriteAsync(status);
        }
    }
}
