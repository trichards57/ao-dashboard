// -----------------------------------------------------------------------
// <copyright file="VehiclesController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Middleware.ServerTiming;
using AmbulanceDashboard.Model;
using AmbulanceDashboard.Services;
using AODashboard.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AmbulanceDashboard.Controllers;

/// <summary>
/// Controller to handle vehicle operations.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
/// <param name="logger">The controller's logger.</param>
/// <param name="serverTiming">The server timing service to use.</param>
[Route("api/vehicles")]
[ApiController]
[Authorize]
public class VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger, IServerTiming serverTiming) : ControllerBase
{
    /// <summary>
    /// Gets the settings for a specific vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>The result of the action.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "The ETag for the response.")]
    [Authorize(Policy = "CanViewVehicles")]
    public async Task<ActionResult<VehicleSettings?>> Get([FromRoute] Guid id)
    {
        using var scope = logger.RunningControllerScope(nameof(VehiclesController), nameof(Get));

        return await this.CachedGet(
            () => vehicleService.GetEtagByIdAsync(id),
            () => vehicleService.GetByIdAsync(id),
            logger,
            $"Vehicles names",
            serverTiming);
    }

    /// <summary>
    /// Gets the names of all the registered vehicles.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested list.</response>
    /// <response code="304">The requested list hasn't been changed.</response>
    [HttpGet("names")]
    [Authorize(Policy = "CanViewVehicles")]
    [ProducesResponseType(typeof(IAsyncEnumerable<VehicleNames>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ResponseCache(Location = ResponseCacheLocation.None)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "The ETag for the response.")]
    public async Task<ActionResult<IAsyncEnumerable<VehicleNames>>> GetNames()
    {
        using var scope = logger.RunningControllerScope(nameof(VehiclesController), nameof(GetNames));

        return await this.CachedGet(
            vehicleService.GetNamesEtagAsync,
            vehicleService.GetNamesAsync,
            logger,
            $"Vehicles names",
            serverTiming);
    }

    /// <summary>
    /// Accepts an update to the vehicle's basic settings.
    /// </summary>
    /// <param name="settings">The updated settings.</param>
    /// <returns>The result of the action.</returns>
    /// <response code="204">The item has been updated.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "CanEditVehicles")]
    public async Task<ActionResult> Post([FromBody] UpdateVehicleSettings settings)
    {
        using var scope = logger.RunningControllerScope(nameof(VehiclesController), nameof(Post));

        await vehicleService.UpdateSettingsAsync(settings);

        RequestLogging.Updated(logger, $"Vehicle {settings.Registration}");

        return NoContent();
    }
}
