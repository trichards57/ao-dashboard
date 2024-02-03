// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers.Filters;
using AODashboard.ApiControllers.Validation;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

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
    public async Task<IActionResult> PostEntry([FromBody] IEnumerable<VorIncident> incident)
    {
        await vehicleService.AddEntriesAsync(incident);

        foreach (var item in incident)
        {
            RequestLogging.Updated(logger, $"VOR Entry : {item.Registration}");
        }

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
    /// Gets the vor status of vehicles in the given place.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <param name="district">The district to search (optional).</param>
    /// <param name="hub">The hub to search (optional).</param>
    /// <returns>The response from the action.</returns>
    [HttpGet("byPlace")]
    [ProducesResponseType(typeof(IAsyncEnumerable<VorStatus>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IAsyncEnumerable<VorStatus>> GetByPlace([FromQuery][RequiredRegion] Region region, [FromQuery]string? district, [FromQuery]string? hub)
    {
        var vehicles = vehicleService.GetStatusesByPlace(region, district, hub);
        RequestLogging.Found(logger, $"Vehicles in {region} {district} {hub}");
        return Ok(vehicles);
    }

    /// <summary>
    /// Gets the VOR statistics for the vehicles in a given place.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <param name="district">The district to search (optional).</param>
    /// <param name="hub">The hub to search (optional).</param>
    /// <response code="200">Returns the requested vehicle statistics.</response>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    [HttpGet("byPlace/stats")]
    [ProducesResponseType(typeof(VorStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<ActionResult<VorStatistics>> GetStatisticsByPlace([FromQuery][RequiredRegion] Region region, [FromQuery] string? district, [FromQuery] string? hub)
    {
        var incomingEtag = Request.GetTypedHeaders().IfNoneMatch.FirstOrDefault(h => !h.IsWeak)?.Tag.Value?.Trim('=').Trim('"');
        var actualEtag = await vehicleService.GetStatisticsEtagByPlace(region, district, hub);

        if (!string.IsNullOrWhiteSpace(incomingEtag) && !string.IsNullOrWhiteSpace(actualEtag) && incomingEtag.Equals(actualEtag, StringComparison.Ordinal))
        {
            RequestLogging.NotModified(logger, $"Vehicle stats in {region} {district} {hub}");

            return StatusCode(StatusCodes.Status304NotModified);
        }

        if (!string.IsNullOrWhiteSpace(actualEtag))
        {
            Response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{actualEtag}\"", false);
        }

        var stats = await vehicleService.GetStatisticsByPlace(region, district, hub);
        RequestLogging.Found(logger, $"Vehicle stats in {region} {district} {hub}");
        return Ok(stats);
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
