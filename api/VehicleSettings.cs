// -----------------------------------------------------------------------
// <copyright file="VehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using API.Model;
using API.Support;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API;

/// <summary>
/// Functions to manage vehicle settings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VehicleSettings"/> class.
/// </remarks>
/// <param name="cosmosClient">The client used to access CosmosDB.</param>
/// <param name="cosmosLinqQuery">Helper function to enable GetFeedIterator to be tested.</param>
/// <param name="configuration">The function configuration files.</param>
/// <param name="logger">The logger for this function.</param>
public class VehicleSettings(CosmosClient cosmosClient, ICosmosLinqQuery cosmosLinqQuery, IConfiguration configuration, ILogger<VehicleSettings> logger)
{
    private const string Partition = "VOR";

    /// <summary>
    /// A function to change a vehicle's settings.
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Function("set-vehicle-settings")]
    public async Task<IActionResult> Set([HttpTrigger(AuthorizationLevel.Function, "post", Route = "vehicle-settings")] HttpRequest req)
    {
        using var bodyReader = new StreamReader(req.Body);

        var body = await bodyReader.ReadToEndAsync();

        var item = JsonSerializer.Deserialize<VehicleSettingsDetail>(body);
        var container = cosmosClient.GetVorContainer(configuration);

        var valResults = new ValidationProblemDetails();

        if (string.IsNullOrWhiteSpace(item.Registration))
        {
            logger.LogError("Registration is blank.");
            valResults.Errors["Registration"] = new[] { "Registration must be provided." };
        }

        if (!Enum.IsDefined(item.Region))
        {
            logger.LogError("Region is invalid.");
            valResults.Errors["Region"] = new[] { "Region must be provided." };
        }

        if (!Enum.IsDefined(item.Type))
        {
            logger.LogError("Type is invalid.");
            valResults.Errors["Type"] = new[] { "Type must be provided." };
        }

        if (valResults.Errors.Any())
        {
            valResults.Instance = req.Path;
            valResults.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            valResults.Title = "Bad Request";
            valResults.Status = StatusCodes.Status400BadRequest;

            logger.LogError("Invalid data received.");
            return new BadRequestObjectResult(valResults);
        }

        item.District ??= "Unknown";
        item.CallSign ??= "";

        var originalItem = await container.ReadItemAsync<Vehicle>(item.Registration, new PartitionKey(Partition));

        if (originalItem.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var newItem = new Vehicle
            {
                CallSign = item.CallSign,
                Registration = item.Registration,
                District = item.District,
                Region = item.Region,
                VehicleType = item.Type,
            };

            await container.CreateItemAsync(newItem);

            logger.LogInformation($"New item created : {item.Registration}");

            return new CreatedResult($"/api/vehicle-settings/{item.Registration}", item);
        }
        else
        {
            var newItem = originalItem.Resource with
            {
                CallSign = item.CallSign,
                Registration = item.Registration,
                District = item.District,
                Region = item.Region,
                VehicleType = item.Type,
            };

            await container.UpsertItemAsync(newItem);

            logger.LogInformation($"Item updated : {item.Registration}");

            return new OkObjectResult(item);
        }
    }

    /// <summary>
    /// Function to get the set of vehicle settings.
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Function("get-vehicle-settings")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicle-settings")] HttpRequest req)
    {
        var district = req.Query["district"];
        var region = req.Query["region"];
        var callsign = req.Query["callsign"];

        var container = cosmosClient.GetVorContainer(configuration);
        FeedIterator<VehicleSettingsDetail> items;

        if (!string.IsNullOrWhiteSpace(callsign))
        {
            if (!string.IsNullOrWhiteSpace(district) || !string.IsNullOrWhiteSpace(region))
            {
                logger.LogError("Call Sign must not be specified if District or Region is provided.");

                return new BadRequestObjectResult(new ProblemDetails
                {
                    Detail = "Call Sign must not be specified if District or Region is provided.",
                    Instance = req.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid parameter combination.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            items = cosmosLinqQuery.GetFeedIterator(container.GetItemLinqQueryable<Vehicle>().Where(v => v.CallSign.Equals(callsign, StringComparison.InvariantCultureIgnoreCase)).Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.District,
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }));
        }
        else if (string.IsNullOrWhiteSpace(region))
        {
            if (!string.IsNullOrWhiteSpace(district))
            {
                logger.LogError("Region must not be empty if district is provided.");

                return new BadRequestObjectResult(new ProblemDetails
                {
                    Detail = "Region must not be empty if district is provided.",
                    Instance = req.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid parameter combination.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            // Get all vehicles.
            logger.LogInformation("Getting all vehicles.");

            items = cosmosLinqQuery.GetFeedIterator(container.GetItemLinqQueryable<Vehicle>().Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.District ?? "Unknown",
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }));
        }
        else if (string.IsNullOrWhiteSpace(district))
        {
            var actualRegion = JsonSerializer.Deserialize<Region>($"\"{region}\"");

            logger.LogInformation($"Getting all vehicles for {actualRegion}");

            items = cosmosLinqQuery.GetFeedIterator(container.GetItemLinqQueryable<Vehicle>().Where(v => v.Region == actualRegion).Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.District,
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }));
        }
        else
        {
            var actualRegion = JsonSerializer.Deserialize<Region>($"\"{region}\"");

            logger.LogInformation($"Getting all vehicles for district {district} of region {actualRegion}.");

            items = cosmosLinqQuery.GetFeedIterator(container.GetItemLinqQueryable<Vehicle>().Where(v => v.Region == actualRegion && v.District == district).Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.District,
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }));
        }

        var result = new List<VehicleSettingsDetail>();

        while (items.HasMoreResults)
        {
            var response = await items.ReadNextAsync();

            result.AddRange(response);
        }

        return new OkObjectResult(result);
    }
}
