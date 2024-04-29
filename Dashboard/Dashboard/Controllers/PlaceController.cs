// -----------------------------------------------------------------------
// <copyright file="PlaceController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers;

[ApiController]
[Route("api/places")]
[Authorize(Policy = "CanViewVOR")]
public class PlaceController(IPlaceService placeService) : ControllerBase
{
    [HttpGet("{region}")]
    public IAsyncEnumerable<string> GetDistricts(Region region) => placeService.GetDistricts(region);

    [HttpGet("{region}/{district}")]
    public IAsyncEnumerable<string> GetHubs(Region region, string district) => placeService.GetHubs(region, district);
}
