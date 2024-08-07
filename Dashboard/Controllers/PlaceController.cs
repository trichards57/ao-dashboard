// -----------------------------------------------------------------------
// <copyright file="PlaceController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for retrieving place information.
/// </summary>
/// <param name="placeService">The service to get the place information from.</param>
[ApiController]
[Route("api/places")]
[Authorize(Policy = "CanViewVOR")]
public class PlaceController(IPlaceService placeService) : ControllerBase
{
    /// <summary>
    /// Gets the districts for the specified region.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <returns>
    /// The district names in that region.
    /// </returns>
    [HttpGet("{region}")]
    public IAsyncEnumerable<string> GetDistricts(Region region) => placeService.GetDistricts(region);

    /// <summary>
    /// Gets the hubs in a given district.
    /// </summary>
    /// <param name="region">The region to search.</param>
    /// <param name="district">The district to search.</param>
    /// <returns>
    /// The hub names in that district.
    /// </returns>
    [HttpGet("{region}/{district}")]
    public IAsyncEnumerable<string> GetHubs(Region region, string district) => placeService.GetHubs(region, district);
}
