// -----------------------------------------------------------------------
// <copyright file="VehicleApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard2.Server.Api
{
    public static class VehicleApiExtensions
    {
        public static WebApplication MapVehicles(this WebApplication app)
        {
            var group = app.MapGroup("/api/vehicles").RequireAuthorization("CanViewUsers").WithTags("Vehicles");

            group.MapGet("", async ([FromServices] IVehicleService userService, [AsParameters] Place place, HttpContext context) =>
            {
                var users = userService.GetSettingsAsync(place);

                await context.Response.WriteAsJsonAsync(users);
            })
                .Produces<IEnumerable<VehicleSettings>>(StatusCodes.Status200OK)
                .WithName("GetVehicles")
                .WithSummary("Gets all of the vehicles for the given place.");

            group.MapGet("{id}", async ([FromServices] IVehicleService vehicleService, [FromRoute] Guid id, HttpContext context) =>
            {
                var vehicle = await vehicleService.GetSettingsAsync(id);

                if (vehicle == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.Headers.CacheControl = "no-cache";
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Detail = "No vehicle found with the given ID.",
                        Instance = context.Request.Path,
                        Status = StatusCodes.Status404NotFound,
                        Title = "No vehicle found",
                        Type = "https://httpstatuses.com/404",
                    });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(vehicle);
                }
            })
                .Produces<VehicleSettings>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName("GetVehicle")
                .WithSummary("Gets the vehicle's settings.");

            group.MapPost("", async ([FromServices] IVehicleService vehicleService, [FromBody] UpdateVehicleSettings settings, HttpContext context) =>
            {
                await vehicleService.PutSettingsAsync(settings);

                context.Response.StatusCode = StatusCodes.Status204NoContent;
                context.Response.Headers.CacheControl = "no-cache";
            })
                .RequireAuthorization("CanEditVehicles")
                .Produces(StatusCodes.Status204NoContent)
                .WithName("SetVehicleSettings")
                .WithSummary("Sets a vehicle's settings.");

            return app;
        }
    }
}
