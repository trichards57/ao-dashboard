// -----------------------------------------------------------------------
// <copyright file="VehicleSettingsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using AODashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to handle vehicle settings operations.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
/// <param name="logger">The controller's logger.</param>
[Route("api/vehicle-settings")]
[ApiController]
[Authorize]
public class VehicleSettingsController(IVehicleService vehicleService, ILogger<VehicleSettingsController> logger) : ControllerBase
{
    /// <summary>
    /// Gets the settings associated with a vehicle, identified by either a call sign or registration.
    /// </summary>
    /// <param name="callSign">The call sign of the vehicle.</param>
    /// <param name="registration">The registration of the vehicle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested vehicle.</response>
    /// <response code="404">The requested vehicle wasn't found.</response>
    /// <response code="400">Either: neither callSign nor registration were provided, or both were provided.</response>
    [HttpGet]
    [ProducesResponseType(typeof(VehicleSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleSettings>> Get([FromQuery] string? callSign, [FromQuery] string? registration)
    {
        using var scope = logger.BeginScope("{Controller} {Name}", nameof(VehicleSettingsController), nameof(Get));

        if (string.IsNullOrWhiteSpace(registration) && !string.IsNullOrWhiteSpace(callSign))
        {
            var vehicle = await vehicleService.GetByCallSignAsync(callSign);

            if (vehicle == null)
            {
                RequestLogging.NotFound(logger, $"Vehicle {callSign}");

                return NotFound(new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Not Found",
                    Detail = "The requested item was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = Request.GetDisplayUrl(),
                });
            }

            RequestLogging.Found(logger, $"Vehicle {callSign}");
            return Ok(vehicle);
        }

        if (!string.IsNullOrWhiteSpace(registration) && string.IsNullOrWhiteSpace(callSign))
        {
            var vehicle = await vehicleService.GetByRegistrationAsync(registration);

            if (vehicle == null)
            {
                RequestLogging.NotFound(logger, $"Vehicle {registration}");

                return NotFound(new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Not Found",
                    Detail = "The requested item was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = Request.GetDisplayUrl(),
                });
            }

            RequestLogging.Found(logger, $"Vehicle {registration}");
            return Ok(vehicle);
        }

        RequestLogging.BadParameters(logger, [nameof(callSign), nameof(registration)]);

        return BadRequest(new ProblemDetails
        {
            Type = "about:blank",
            Title = "Bad Request",
            Detail = "Exactly one of callSign or registration must be provided.  You cannot provide neither or both.",
            Status = StatusCodes.Status400BadRequest,
        });
    }

    /// <summary>
    /// Sets the settings for the requested vehicle, identified by it's registration.  The vehicle will be created if it isn't found.
    /// </summary>
    /// <param name="vehicleSettings">The new vehicle settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the updated vehicle.</response>
    /// <response code="400">Either: the provided data was invalid, or the call-sign is already being used.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleSettings>> Post([FromBody] UpdateVehicleSettings vehicleSettings)
    {
        try
        {
            await vehicleService.UpdateSettingsAsync(vehicleSettings);

            RequestLogging.Updated(logger, $"Vehicle {vehicleSettings.Registration}");

            return Ok(await vehicleService.GetByRegistrationAsync(vehicleSettings.Registration));
        }
        catch (DuplicateCallSignException)
        {
            RequestLogging.BadParameters(logger, ["CallSign"]);

            return BadRequest(new ValidationProblemDetails
            {
                Type = "about:blank",
                Title = "Bad Request",
                Detail = "The provided call-sign for the vehicle is already in use.",
                Status = StatusCodes.Status400BadRequest,
                Errors =
                {
                    { "CallSign", ["The provided call-sign is already in use."] },
                },
            });
        }
    }
}
