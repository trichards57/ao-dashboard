// -----------------------------------------------------------------------
// <copyright file="UserApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard2.Server.Api
{
    public static class UserApiExtensions
    {
        public static WebApplication MapUsers(this WebApplication app)
        {
            var group = app.MapGroup("/api/users").RequireAuthorization("CanViewUsers").WithTags("Users");

            group.MapGet("", async ([FromServices] IUserService userService, HttpContext context) =>
            {
                var users = userService.GetUsersWithRole();

                await context.Response.WriteAsJsonAsync(users);
            })
                .Produces<IEnumerable<UserWithRole>>(StatusCodes.Status200OK)
                .WithName("GetUsers")
                .WithSummary("Gets all of the users.");

            group.MapGet("{id}", async ([FromServices] IUserService userService, [FromRoute] string id, HttpContext context) =>
            {
                var user = await userService.GetUserWithRole(id);

                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.Headers.CacheControl = "no-cache";
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Detail = "No user found with the given ID.",
                        Instance = context.Request.Path,
                        Status = StatusCodes.Status404NotFound,
                        Title = "No user found",
                        Type = "https://httpstatuses.com/404",
                    });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(user);
                }
            })
                .Produces<UserWithRole>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetUser")
                .WithSummary("Gets the user with their role.");

            group.MapPost("{id}", async ([FromServices] IUserService userService, [FromServices] UserManager<ApplicationUser> userManager, [FromRoute] string id, [FromBody] UserRoleUpdate role, HttpContext context) =>
            {
                var currentId = userManager.GetUserId(context.User);

                if (id.Equals(currentId, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.Headers.CacheControl = "no-cache";
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Detail = "You cannot change your own account.",
                        Instance = context.Request.Path,
                        Status = StatusCodes.Status403Forbidden,
                        Title = "Cannot Change User",
                        Type = "https://httpstatuses.com/403",
                    });
                }

                if (await userService.SetUserRole(id, role))
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
                .RequireAuthorization("CanEditUsers")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("SetUserRole")
                .WithSummary("Sets the role for a user.");

            return app;
        }
    }
}
