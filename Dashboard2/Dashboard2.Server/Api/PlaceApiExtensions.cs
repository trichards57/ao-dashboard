// -----------------------------------------------------------------------
// <copyright file="PlaceApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Dashboard2.Server.Api;

internal static class PlaceApiExtensions
{
    internal static WebApplication MapPlaces(this WebApplication app)
    {
        var group = app.MapGroup("/api/places").RequireAuthorization("CanViewVOR").WithTags("Places");

        group.MapGet("{region}", async ([FromServices] IPlaceService placeService, [FromRoute] string region, HttpContext context) =>
        {
            var regionValid = Enum.TryParse<Region>(region, out var actualRegion);

            if (!regionValid || !Enum.IsDefined(actualRegion) || actualRegion is Region.Unknown or Region.All)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.Headers.CacheControl = "no-cache";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Detail = "region must be a valid region that is not Unknown or All.",
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid region",
                    Type = "https://httpstatuses.com/400",
                });
            }

            await context.Response.WriteAsJsonAsync(placeService.GetDistricts(actualRegion));
        })
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
            .WithName("GetDistricts")
            .WithSummary("Gets all of the districts in a region.");

        group.MapGet("{region}/{district}", async ([FromServices] IPlaceService placeService, [FromRoute] string region, [FromRoute] string district, HttpContext context) =>
        {
            var regionValid = Enum.TryParse<Region>(region, out var actualRegion);

            if (!regionValid || !Enum.IsDefined(actualRegion) || actualRegion is Region.Unknown or Region.All)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.Headers.CacheControl = "no-cache";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Detail = "region must not be Unknown or All.",
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid region",
                    Type = "https://httpstatuses.com/400",
                });
            }

            var items = await placeService.GetHubs(actualRegion, district).ToListAsync();

            if (items.Count == 0)
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

            await context.Response.WriteAsJsonAsync(items);
        })
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
            .WithSummary("Gets all of the hubs in a district.");

        return app;
    }
}
