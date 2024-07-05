using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Dashboard.Model;
using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard2.Server.Api
{
    public static class VorApiExtensions
    {
        public static WebApplication MapVor(this WebApplication app)
        {
            var group = app.MapGroup("/api/vor").WithTags("VOR").RequireAuthorization("CanViewVOR");

            group.MapPost("", ([FromServices] IVehicleService vehicleService, [FromBody] IEnumerable<VorIncident> incidents, HttpContext context) =>
            {
                vehicleService.AddEntriesAsync(incidents.ToList());

                context.Response.StatusCode = StatusCodes.Status204NoContent;
                context.Response.Headers.CacheControl = "no-cache";
            })
                .RequireAuthorization("CanEditVOR")
                .Produces(StatusCodes.Status204NoContent)
                .WithName("AddVorIncident")
                .WithSummary("Accepts VOR reports from the uploader.");

            group.MapGet("statistics", async ([FromServices]IVorService vorService, [AsParameters]Place place, HttpContext context) =>
            {
                var statistics = await vorService.GetVorStatisticsAsync(place);

                await context.Response.WriteAsJsonAsync(statistics);
                context.Response.StatusCode = StatusCodes.Status200OK;
            })
                .Produces<VorStatistics>(StatusCodes.Status200OK)
                .WithName("GetVorStatistics")
                .WithSummary("Gets the VOR statistics for the given place.");

            group.MapGet("", async ([FromServices] IVorService vorService, [AsParameters] Place place, HttpContext context) =>
            {
                var statuses = vorService.GetVorStatusesAsync(place);

                await context.Response.WriteAsJsonAsync(statuses);
                context.Response.StatusCode = StatusCodes.Status200OK;
            })
                .Produces<IEnumerable<VorStatus>>(StatusCodes.Status200OK)
                .WithName("GetVorStatus")
                .WithSummary("Gets the VOR status of the vehicles in the given place.");

            return app;
        }
    }
}
