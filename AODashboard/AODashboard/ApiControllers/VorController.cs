// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AODashboard.ApiControllers;

/// <summary>
/// Controller to handle VOR incident.
/// </summary>
/// <param name="vehicleService">The service used to manage vehicles.</param>
[Route("api/vors")]
[ApiController]
[Authorize]
public class VorController(IVehicleService vehicleService) : ControllerBase
{
    /// <summary>
    /// Accepts a list of vor entries to update the database.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the outcome of each entry.</response>
    [HttpPost]
    [Produces("application/x-ndjson")]
    public async Task PostEntry()
    {
        Response.ContentType = "application/x-ndjson";

        using var reader = new StreamReader(Request.Body);
        var outputStream = new StreamWriter(Response.Body);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
        };

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (line == null)
            {
                break;
            }

            try
            {
                // TODO : Validation
                var item = JsonSerializer.Deserialize<VorIncident>(line);

                await vehicleService.AddEntryAsync(item);
                var error = new VorError { Error = "None." };
                var result = JsonSerializer.Serialize(error, jsonOptions);
                await outputStream.WriteLineAsync(result);
            }
            catch (JsonException)
            {
                var error = new VorError { Error = "Received JSON was invalid." };
                var result = JsonSerializer.Serialize(error, jsonOptions);
                await outputStream.WriteLineAsync(result);
            }
        }

        await outputStream.FlushAsync();
    }

    /// <summary>
    /// Clears all of the current VOR flags, ready for the system to be updated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="204">VORs have been cleared.</response>
    /// <remarks>This should only be used immediately before uploading the latest VOR data.</remarks>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete()
    {
        await vehicleService.ClearVorsAsync();
        return NoContent();
    }

    /// <summary>
    /// Gets the settings associated with a vehicle, identified by either a call sign or registration.
    /// </summary>
    /// <param name="callSign">The call sign of the vehicle.</param>
    /// <param name="registration">The registration of the vehicle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    /// <response code="200">Returns the requested vehicle.</response>
    /// <response code="404">The requested vehicle wasn't found.</response>
    /// <response code="400">Either: neither callSign nor registration were provided, or both were provided.</response>
    [HttpGet]
    [ProducesResponseType(typeof(VorStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VorStatus>> Get([FromQuery] string? callSign, [FromQuery] string? registration)
    {
        if (string.IsNullOrWhiteSpace(registration) && !string.IsNullOrWhiteSpace(callSign))
        {
            var vehicle = await vehicleService.GetStatusByCallSignAsync(callSign);

            if (vehicle == null)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Not Found",
                    Detail = "The requested item was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = Request.GetDisplayUrl(),
                });
            }

            return Ok(vehicle);
        }

        if (!string.IsNullOrWhiteSpace(registration) && string.IsNullOrWhiteSpace(callSign))
        {
            var vehicle = await vehicleService.GetStatusByRegistrationAsync(registration);

            if (vehicle == null)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Not Found",
                    Detail = "The requested item was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = Request.GetDisplayUrl(),
                });
            }

            return Ok(vehicle);
        }

        return BadRequest(new ProblemDetails
        {
            Type = "about:blank",
            Title = "Bad Request",
            Detail = "Exactly one of callSign or registration must be provided.  You cannot provide neither or both.",
            Status = StatusCodes.Status400BadRequest,
        });
    }
}
