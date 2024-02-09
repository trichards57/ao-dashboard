// -----------------------------------------------------------------------
// <copyright file="VehicleSettingsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using AODashboard.Middleware.ServerTiming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to handle vehicle settings operations.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
/// <param name="logger">The controller's logger.</param>
/// <param name="serverTiming">The server timing service to use.</param>
[Route("api/vehicle-settings")]
[ApiController]
[Authorize]
public class VehicleSettingsController(IVehicleService vehicleService, ILogger<VehicleSettingsController> logger, IServerTiming serverTiming) : ControllerBase
{
    /// <summary>
    /// Gets the settings associated with a vehicle, identified by either a call sign or registration.
    /// </summary>
    /// <param name="callSign">The call sign of the vehicle.</param>
    /// <param name="registration">The registration of the vehicle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested vehicle.</response>
    /// <response code="304">The requested vehicle hasn't been changed.</response>
    /// <response code="404">The requested vehicle wasn't found.</response>
    /// <response code="400">Either: neither callSign nor registration were provided, or both were provided.</response>
    [HttpGet]
    [ProducesResponseType(typeof(VehicleSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "The ETag for the response.")]
    [Authorize(Policy = "CanViewVehicles")]
    public async Task<ActionResult<VehicleSettings?>> Get([FromQuery] string? callSign, [FromQuery] string? registration)
    {
        using var scope = logger.RunningControllerScope(nameof(VehicleSettingsController), nameof(Get));

        Func<Task<string>> getEtag;
        Func<Task<VehicleSettings?>> getVehicle;
        string logParam;

        if (string.IsNullOrWhiteSpace(registration) && !string.IsNullOrWhiteSpace(callSign))
        {
            getEtag = () => vehicleService.GetEtagByCallSignAsync(callSign);
            getVehicle = () => vehicleService.GetByCallSignAsync(callSign);
            logParam = $"Vehicle {callSign}";
        }
        else if (!string.IsNullOrWhiteSpace(registration) && string.IsNullOrWhiteSpace(callSign))
        {
            getEtag = () => vehicleService.GetEtagByRegistrationAsync(registration);
            getVehicle = () => vehicleService.GetByRegistrationAsync(registration);
            logParam = $"Vehicle {registration}";
        }
        else
        {
            RequestLogging.BadParameters(logger, [nameof(callSign), nameof(registration)]);

            return BadRequest(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Bad Request",
                Detail = "Exactly one of callSign or registration must be provided.  You cannot provide neither or both.",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        return await this.CachedGet(getEtag, getVehicle, logger, logParam, serverTiming);
    }

    /// <summary>
    /// Sets the settings for the requested vehicle, identified by it's registration.  The vehicle will be created if it isn't found.
    /// </summary>
    /// <param name="vehicleSettings">The new vehicle settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the updated vehicle.</response>
    /// <response code="400">The provided data was invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "CanEditVehicles")]
    public async Task<ActionResult<VehicleSettings>> Post([FromBody] UpdateVehicleSettings vehicleSettings)
    {
        await vehicleService.UpdateSettingsAsync(vehicleSettings);

        RequestLogging.Updated(logger, $"Vehicle {vehicleSettings.Registration}");

        return Ok(await vehicleService.GetByRegistrationAsync(vehicleSettings.Registration));
    }
}
