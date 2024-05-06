// -----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Controllers;

[ApiController]
[Authorize(Policy = "CanEditRoles")]
[Route("api/roles")]
public class RoleController(IRoleService roleService) : ControllerBase
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
            return NotFound();
        }

        return role;
    }

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
