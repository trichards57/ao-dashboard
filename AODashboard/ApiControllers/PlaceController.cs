// -----------------------------------------------------------------------
// <copyright file="PlaceController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers.Filters;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to manage information about places.
/// </summary>
/// <param name="placeService">The service used to access places.</param>
/// <param name="logger">The controller's logger.</param>
[ApiController]
[Route("api/places")]
[Authorize]
[ApiSecurityPolicy]
public class PlaceController(IPlaceService placeService, ILogger<PlaceController> logger) : ControllerBase
{
    /// <summary>
    /// Gets the name of all the Districts defined for the region.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested districts.</response>
    /// <response code="400">The provided region was invalid.</response>
    [HttpGet("{region}/districts")]
    [ProducesResponseType(typeof(Places), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Places>> GetDistricts([FromRoute]Region region)
    {
        using var scope = logger.BeginScope("{Controller} {Name}", nameof(PlaceController), nameof(GetDistricts));

        if (!Enum.IsDefined(region))
        {
            RequestLogging.BadParameters(logger, [nameof(region)]);
            return BadRequest(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Bad Request",
                Detail = "The provided region was invalid.",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        var districts = await placeService.GetDistrictNames(region);
        RequestLogging.Found(logger, $"Region {region} Districts.");
        return Ok(districts);
    }

    /// <summary>
    /// Gets the name of all of the hubs defined for a district.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <param name="district">The district to search.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested hubs.</response>
    /// <response code="400">The provided region or district was invalid.</response>
    [HttpGet("{region}/{district}/hubs")]
    [ProducesResponseType(typeof(Places), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Places>> GetDistrictHubs([FromRoute]Region region, [FromRoute][Required]string district)
    {
        using var scope = logger.BeginScope("{Controller} {Name}", nameof(PlaceController), nameof(GetDistrictHubs));

        if (!Enum.IsDefined(region))
        {
            RequestLogging.BadParameters(logger, [nameof(region)]);
            return BadRequest(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Bad Request",
                Detail = "The provided region was invalid.",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        var hubs = await placeService.GetDistrictHubs(region, district);
        RequestLogging.Found(logger, $"Region {region} District {district} Hubs.");
        return Ok(hubs);
    }
}
