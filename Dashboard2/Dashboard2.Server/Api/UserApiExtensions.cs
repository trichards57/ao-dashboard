// -----------------------------------------------------------------------
// <copyright file="UserApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client;
using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;

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

            group.MapGet("me", async ([FromServices] UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor, HttpContext context) =>
            {
                var options = optionsAccessor.Value;

                var userId = context.User.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
                var name = context.User.FindFirst(ClaimTypes.Name)?.Value;
                var email = context.User.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
                var role = context.User.FindFirst(options.ClaimsIdentity.RoleClaimType)?.Value;
                var amrUsed = context.User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
                var lastAuthenticated = context.User.FindFirst("auth_time")?.Value;

                await context.Response.WriteAsJsonAsync(new UserInfo
                {
                    UserId = userId!,
                    Email = email!,
                    Role = role ?? "None",
                    RealName = name ?? email!,
                    AmrUsed = amrUsed ?? "Unknown",
                    LastAuthenticated = lastAuthenticated != null ? DateTimeOffset.Parse(lastAuthenticated, CultureInfo.InvariantCulture) : null,
                    OtherClaims = context.User.Claims
                            .Where(c =>
                                c.Type != options.ClaimsIdentity.UserIdClaimType &&
                                c.Type != ClaimTypes.Name &&
                                c.Type != options.ClaimsIdentity.EmailClaimType &&
                                c.Type != options.ClaimsIdentity.RoleClaimType &&
                                c.Type != ClaimTypes.AuthenticationMethod &&
                                c.Type != "auth_time")
                            .ToDictionary(c => c.Type, c => c.Value),
                });
            })
                .RequireAuthorization()
                .Produces<UserInfo>(StatusCodes.Status200OK)
                .WithName("GetMe")
                .WithSummary("Gets information about the current user.");

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
