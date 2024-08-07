// -----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Controllers;

/// <summary>
/// Controller to manage user roles.
/// </summary>
/// <param name="roleService">The service to manage user roles.</param>
[ApiController]
[Authorize(Policy = "CanEditRoles")]
[Route("api/roles")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService roleService = roleService;

    /// <summary>
    /// Gets all the roles and their permissions.
    /// </summary>
    /// <returns>
    /// The list of roles with their permissions.
    /// </returns>
    [HttpGet]
    public IAsyncEnumerable<RolePermissions?> Get() => roleService.GetRoles();

    /// <summary>
    /// Gets the permissions for a specific role.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <returns>
    /// The permissions for the role, or NotFound if the role does not exist.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RolePermissions>> Get([Required] string id)
    {
        var role = await roleService.GetRolePermissions(id);

        if (role == null)
        {
            return NotFound();
        }

        return role;
    }

    /// <summary>
    /// Updates the permissions for a role.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <param name="permissions">The new permissions for the role.</param>
    /// <returns>
    /// Ok if the permissions were updated, NotFound if the role does not exist.
    /// </returns>
    [HttpPost("{id}")]
    public async Task<ActionResult> Put([Required] string id, [FromBody] RolePermissionsUpdate permissions)
    {
        if (await roleService.SetRolePermissions(id, permissions))
        {
            return Ok();
        }

        return NotFound();
    }
}
