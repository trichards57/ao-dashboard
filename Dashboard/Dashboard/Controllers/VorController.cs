// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Dashboard.Model;
using Dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for managing VOR incidents.
/// </summary>
/// <param name="vehicleService">Service to manage vehicles.</param>
[Route("api/vor")]
[ApiController]
[Authorize(Policy = "CanViewVOR")]
public class VorController(IVehicleService vehicleService, IVorService vorService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;
    private readonly IVorService vorService = vorService;

    /// <summary>
    /// Accepts VOR incidents.
    /// </summary>
    /// <param name="incidents">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [Authorize(Policy = "CanEditVOR")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] IEnumerable<VorIncident> incidents)
    {
        await vehicleService.AddEntriesAsync(incidents.ToList());

        return Ok();
    }

    /// <summary>
    /// Gets the VOR statistics for a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [HttpGet("statistics")]
    public Task<VorStatistics> GetStatistics([FromQuery] Place place) => vorService.GetVorStatisticsAsync(place);

    [HttpGet]
    public IAsyncEnumerable<VorStatus> Get([FromQuery] Place place) => vorService.GetVorStatusesAsync(place);
}
