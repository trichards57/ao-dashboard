// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for managing VOR incidents.
/// </summary>
/// <param name="vehicleService">Service to manage vehicles.</param>
/// TODO : Reinstate this when you work out authentication for apps.
// [Authorize(Policy = "CanEditVOR")]
[Route("api/vor")]
[ApiController]
public class VorController(IVehicleService vehicleService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    /// <summary>
    /// Accepts VOR incidents.
    /// </summary>
    /// <param name="incidents">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] IEnumerable<VorIncident> incidents)
    {
        await vehicleService.AddEntriesAsync(incidents.ToList());

        return Ok();
    }
}
