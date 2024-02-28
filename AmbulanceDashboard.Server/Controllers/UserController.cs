// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceDashboard.Controllers;

/// <summary>
/// Controller for handling user operations.
/// </summary>
[Route("api/user")]
[ApiController]
[Authorize]
public class UserController(IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>
    /// Gets the permissions for the current user.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    [HttpGet("permissions")]
    public async Task<ActionResult<UserPermissions>> GetPermissions()
    {
        var result = new UserPermissions
        {
            UserId = User.Identity?.Name ?? string.Empty,
            CanViewVehicles = (await authorizationService.AuthorizeAsync(User, "CanViewVehicles")).Succeeded,
            CanEditVehicles = (await authorizationService.AuthorizeAsync(User, "CanEditVehicles")).Succeeded,
            CanViewPlaces = (await authorizationService.AuthorizeAsync(User, "CanViewPlaces")).Succeeded,
            CanEditVOR = (await authorizationService.AuthorizeAsync(User, "CanEditVOR")).Succeeded,
            CanViewVOR = (await authorizationService.AuthorizeAsync(User, "CanViewVOR")).Succeeded,
        };

        return Ok(result);
    }
}
