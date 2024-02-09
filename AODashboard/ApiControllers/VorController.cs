// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers.Validation;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using AODashboard.Middleware.ServerTiming;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to handle VOR incident.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
/// <param name="logger">The controller's logger.</param>
/// <param name="serverTiming">The server timing service to use.</param>
[Route("api/vors")]
[ApiController]
[Authorize]
public class VorController(IVehicleService vehicleService, ILogger<VorController> logger, IServerTiming serverTiming) : ControllerBase
{
    /// <summary>
    /// Accepts a list of vor entries to update the database.
    /// </summary>
    /// <param name="incident">The incident to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CanEditVOR")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostEntry([FromBody] IEnumerable<VorIncident> incident)
    {
        using var scope = logger.RunningControllerScope(nameof(VorController), nameof(PostEntry));

        var incidentList = incident.ToList();

        await vehicleService.AddEntriesAsync(incidentList);

        foreach (var item in incidentList)
        {
            RequestLogging.Updated(logger, $"VOR Entry : {item.Registration}");
        }

        return Ok();
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
    [Authorize(Policy = "CanViewVOR")]
    public async Task<ActionResult<IAsyncEnumerable<VorStatus>>> GetByPlace([FromQuery][RequiredRegion] Region region, [FromQuery] string? district, [FromQuery] string? hub)
    {
        using var scope = logger.RunningControllerScope(nameof(VorController), nameof(GetByPlace));

        return await this.CachedGet(
            () => vehicleService.GetStatusesEtagByPlace(region, district, hub),
            () => vehicleService.GetStatusesByPlace(region, district, hub),
            logger,
            $"Vehicles in {region} {district} {hub}",
            serverTiming);
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
    [Authorize(Policy = "CanViewVOR")]
    public async Task<ActionResult<VorStatistics>> GetStatisticsByPlace([FromQuery][RequiredRegion] Region region, [FromQuery] string? district, [FromQuery] string? hub)
    {
        using var scope = logger.RunningControllerScope(nameof(VorController), nameof(GetStatisticsByPlace));

        return await this.CachedGet(
                () => vehicleService.GetStatisticsEtagByPlace(region, district, hub),
                () => vehicleService.GetStatisticsByPlace(region, district, hub),
                logger,
                $"Vehicle stats in {region} {district} {hub}",
                serverTiming);
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
    [Authorize(Policy = "CanViewVOR")]
    public async Task<ActionResult<VorStatus?>> Get([FromQuery] string? callSign, [FromQuery] string? registration)
    {
        using var scope = logger.RunningControllerScope(nameof(VorController), nameof(Get));

        Func<Task<string>> getEtag;
        Func<Task<VorStatus?>> getVehicle;
        string logParam;

        if (string.IsNullOrWhiteSpace(registration) && !string.IsNullOrWhiteSpace(callSign))
        {
            getEtag = () => vehicleService.GetEtagByCallSignAsync(callSign);
            getVehicle = () => vehicleService.GetStatusByCallSignAsync(callSign);
            logParam = $"Vehicle {callSign}";
        }
        else if (!string.IsNullOrWhiteSpace(registration) && string.IsNullOrWhiteSpace(callSign))
        {
            getEtag = () => vehicleService.GetEtagByRegistrationAsync(registration);
            getVehicle = () => vehicleService.GetStatusByRegistrationAsync(registration);
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
}
