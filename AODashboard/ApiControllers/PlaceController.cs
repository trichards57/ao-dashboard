// -----------------------------------------------------------------------
// <copyright file="PlaceController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers.Validation;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
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
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [Authorize(Policy = "CanViewPlaces")]
    [ResponseCache(Location = ResponseCacheLocation.None)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "The ETag for the response.")]
    public async Task<ActionResult<Places>> GetDistricts([FromRoute][RequiredRegion] Region region)
    {
        using var scope = logger.RunningControllerScope(nameof(PlaceController), nameof(GetDistricts));

        return await this.CachedGet(() => placeService.GetDistrictNamesETag(region), () => placeService.GetDistrictNames(region), logger, $"Region {region} Districts.");
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
    [Authorize(Policy = "CanViewPlaces")]
    [ResponseCache(Location = ResponseCacheLocation.None)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "The ETag for the response.")]
    public async Task<ActionResult<Places>> GetDistrictHubs([FromRoute][RequiredRegion] Region region, [FromRoute][Required] string district)
    {
        using var scope = logger.RunningControllerScope(nameof(PlaceController), nameof(GetDistrictHubs));

        return await this.CachedGet(() => placeService.GetDistrictHubsETag(region, district), () => placeService.GetDistrictHubs(region, district), logger, $"Region {region} District {district} Hubs.");
    }
}
