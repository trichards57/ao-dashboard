// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers.Filters;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to handle VOR incident.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
/// <param name="logger">The controller's logger.</param>
[Route("api/vors")]
[ApiController]
[Authorize]
[OutputCache(NoStore = true)]
[ApiSecurityPolicy]
public class VorController(IVehicleService vehicleService, ILogger<VorController> logger) : ControllerBase
{
    /// <summary>
    /// Accepts a list of vor entries to update the database.
    /// </summary>
    /// <param name="incident">The incident to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostEntry([FromBody] VorIncident incident)
    {
        await vehicleService.AddEntryAsync(incident);

        RequestLogging.Updated(logger, $"VOR Entry : {incident.Registration}");

        return Ok();
    }

    /// <summary>
    /// Clears all of the current VOR flags, ready for the system to be updated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="204">VORs have been cleared.</response>
    /// <remarks>This should only be used immediately before uploading the latest VOR data.</remarks>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete()
    {
        await vehicleService.ClearVorsAsync();

        RequestLogging.Cleared(logger, $"VOR Entries");

        return NoContent();
    }

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
    [ProducesResponseType(typeof(VorStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VorStatus>> Get([FromQuery] string? callSign, [FromQuery] string? registration)
    {
        if (string.IsNullOrWhiteSpace(registration) && !string.IsNullOrWhiteSpace(callSign))
        {
            var vehicle = await vehicleService.GetStatusByCallSignAsync(callSign);

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
            var vehicle = await vehicleService.GetStatusByRegistrationAsync(registration);

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
}
