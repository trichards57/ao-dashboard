// -----------------------------------------------------------------------
// <copyright file="RoleApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard2.Server.Api
{
    public static class RoleApiExtensions
    {
        public static WebApplication MapRoles(this WebApplication app)
        {
            var group = app.MapGroup("/api/roles").WithTags("Roles");

            group.MapGet("", async ([FromServices] IRoleService roleService, HttpContext context) =>
            {
                var roles = roleService.GetRoles();

                await context.Response.WriteAsJsonAsync(roles);
            })
                .Produces<IEnumerable<RolePermissions>>(StatusCodes.Status200OK)
                .WithName("GetRoles")
                .WithSummary("Gets all of the roles.")
                .RequireAuthorization("CanViewUsers");

            group.MapGet("{id}", async ([FromServices] IRoleService roleService, [FromRoute] string id, HttpContext context) =>
            {
                var role = await roleService.GetRolePermissions(id);

                if (role == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.Headers.CacheControl = "no-cache";
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Detail = "No hubs found for the specified region and district.",
                        Instance = context.Request.Path,
                        Status = StatusCodes.Status404NotFound,
                        Title = "No hubs found",
                        Type = "https://httpstatuses.com/404",
                    });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(role);
                }
            })
                .Produces<RolePermissions>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetRole")
                .WithSummary("Gets the permissions for a role.")
                .RequireAuthorization("CanViewUsers");

            group.MapPost("{id}", async ([FromServices] IRoleService roleService, [FromRoute] string id, [FromBody] RolePermissionsUpdate permissions, HttpContext context) =>
            {
                if (await roleService.SetRolePermissions(id, permissions))
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    context.Response.Headers.CacheControl = "no-cache";
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.Headers.CacheControl = "no-cache";
                }
            })
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("SetRolePermissions")
                .WithSummary("Sets the permissions for a role.")
                .RequireAuthorization("CanEditRoles");

            return app;
        }
    }
}
