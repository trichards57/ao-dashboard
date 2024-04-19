// -----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Dashboard.Controllers;

[ApiController]
[Authorize(Policy = "CanEditRoles")]
[Route("api/roles")]
public class RoleController(IRoleService roleService, ILogger<RoleController> logger) : ControllerBase
{
    private readonly IRoleService roleService = roleService;

    [HttpGet]
    public IAsyncEnumerable<RolePermissions> Get() => roleService.GetRoles();

    [HttpGet("{id}")]
    public async Task<ActionResult<RolePermissions>> Get([Required] string id)
    {
        var role = await roleService.GetRolePermissions(id);

        if (role == null)
        {
            logger.LogWarning("Role {Id} not found", id);
            return NotFound();
        }

        return role;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put([Required] string id, [FromBody] RolePermissionsUpdate permissions)
    {
        if (await roleService.SetRolePermissions(id, permissions))
        {
            return Ok();
        }

        return NotFound();
    }
}
