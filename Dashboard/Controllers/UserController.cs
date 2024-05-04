// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[Route("api/users")]
[Authorize(Policy = "CanViewUsers")]
[ApiController]
public class UserController(IUserService userService, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly IUserService userService = userService;

    /// <summary>
    /// Gets all the users and their roles.
    /// </summary>
    /// <returns>The users with their roles.</returns>
    [HttpGet]
    public IAsyncEnumerable<UserWithRole> Get() => userService.GetUsersWithRole();

    /// <summary>
    /// Gets the specific user with their role.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user with their role.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserWithRole>> Get([Required] string id)
    {
        var user = await userService.GetUserWithRole(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Updates the role of a user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="role">The user's role.</param>
    /// <returns>The outcome of the result.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "CanEditUsers")]
    public async Task<ActionResult> Put([Required] string id, [FromBody] UserRoleUpdate role)
    {
        var currentId = userManager.GetUserId(User);

        if (id.Equals(currentId, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (await userService.SetUserRole(id, role))
        {
            return Ok();
        }

        return NotFound();
    }
}
