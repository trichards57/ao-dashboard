// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Logging;
using AODashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller for handling user operations.
/// </summary>
/// <param name="userService">The service to manage users.</param>
/// <param name="logger">The controller's logger.</param>
[Route("api/user")]
[ApiController]
[Authorize]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    /// <summary>
    /// Gets the current user's profile picture.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the result of the action.
    /// </returns>
    /// <response code="404">The profile image wasn't found.</response>
    /// <response code="200">The user's profile image.</response>
    [HttpGet("picture")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("image/jpeg")]
    public async Task<IActionResult> ProfilePicture()
    {
        using var scope = logger.BeginScope("{Controller} {Name}", nameof(UserController), nameof(ProfilePicture));

        UserLogger.UserProfileDetailsRequested(logger, User.GetNameIdentifierId() ?? "", ["Profile Picture"]);

        var pic = await userService.GetProfilePictureAsync();

        if (pic == null)
        {
            RequestLogging.NotFound(logger, "Profile Picture");
            return NotFound();
        }

        RequestLogging.Found(logger, "Profile Picture");
        return File(pic, "image/jpeg");
    }
}
