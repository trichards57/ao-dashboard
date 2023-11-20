// -----------------------------------------------------------------------
// <copyright file="VehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VorReceiver.Model;

/// <summary>
/// The details describing a vehicle.
/// </summary>
public class VehicleSettingsDetail
{
    /// <summary>
    /// Gets or sets the registration of the vehicle.
    /// </summary>
    [JsonProperty("reg")]
    public string Registration { get; set; }

    /// <summary>
    /// Gets or sets the radio call sign for the vehicle.
    /// </summary>
    [JsonProperty("callSign")]
    public string CallSign { get; set; }

    /// <summary>
    /// Gets or sets the owning district.
    /// </summary>
    [JsonProperty("district")]
    public string District { get; set; }

    /// <summary>
    /// Gets or sets the owning region.
    /// </summary>
    [JsonProperty("region")]
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the vehicle type.
    /// </summary>
    [JsonProperty("type")]
    public VehicleType Type { get; set; }
}

/// <summary>
/// Functions to manage vehicle settings.
/// </summary>
public class VehicleSettings
{
    private const string Partition = "VOR";
    private readonly CosmosClient cosmosClient;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleSettings"/> class.
    /// </summary>
    /// <param name="cosmosClient">The client used to access CosmosDB.</param>
    /// <param name="configuration">The function configuration files.</param>
    public VehicleSettings(CosmosClient cosmosClient, IConfiguration configuration)
    {
        this.cosmosClient = cosmosClient;
        this.configuration = configuration;
    }

    /// <summary>
    /// A function to change a vehicle's settings.
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <param name="log">The logger for the function.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [FunctionName("set-vehicle-settings")]
    public async Task<IActionResult> Set(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vehicle-settings")] HttpRequest req,
        ILogger log)
    {
        var body = new StreamReader(req.Body).ReadToEnd();

        var item = JsonConvert.DeserializeObject<VehicleSettingsDetail>(body);
        var container = cosmosClient.GetVorContainer(configuration);

        var valResults = new ValidationProblemDetails();

        if (string.IsNullOrWhiteSpace(item.Registration))
        {
            log.LogError("Registration is blank.");
            valResults.Errors["Registration"] = new[] { "Registration must be provided." };
        }

        if (!Enum.IsDefined(item.Region))
        {
            log.LogError("Region is invalid.");
            valResults.Errors["Region"] = new[] { "Region must be provided." };
        }

        if (!Enum.IsDefined(item.Type))
        {
            log.LogError("Type is invalid.");
            valResults.Errors["Type"] = new[] { "Type must be provided." };
        }

        if (valResults.Errors.Any())
        {
            valResults.Instance = req.Path;
            valResults.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            valResults.Title = "Bad Request";
            valResults.Status = StatusCodes.Status400BadRequest;

            log.LogError("Invalid data received.");
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
                Distict = item.District,
                Region = item.Region,
                VehicleType = item.Type,
            };

            await container.CreateItemAsync(newItem);

            log.LogInformation($"New item created : {item.Registration}");

            return new CreatedResult($"/api/vehicle-settings/{item.Registration}", item);
        }
        else
        {
            var newItem = originalItem.Resource;

            newItem.CallSign = item.CallSign;
            newItem.Registration = item.Registration;
            newItem.Distict = item.District;
            newItem.Region = item.Region;
            newItem.VehicleType = item.Type;

            await container.UpsertItemAsync(newItem);

            log.LogInformation($"Item updated : {item.Registration}");

            return new OkObjectResult(item);
        }
    }

    /// <summary>
    /// Function to get the set of vehicle settings.
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <param name="log">The logger for the function.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [FunctionName("get-vehicle-settings")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicle-settings")] HttpRequest req,
        ILogger log)
    {
        var district = req.Query["district"];
        var region = req.Query["region"];

        var container = cosmosClient.GetVorContainer(configuration);
        FeedIterator<VehicleSettingsDetail> items;

        if (string.IsNullOrWhiteSpace(region))
        {
            if (!string.IsNullOrWhiteSpace(district))
            {
                log.LogError("Region must not be empty if district is provided.");

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
            log.LogInformation("Getting all vehicles.");

            items = container.GetItemLinqQueryable<Vehicle>().Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.Distict ?? "Unknown",
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }).ToFeedIterator();
        }
        else if (string.IsNullOrWhiteSpace(district))
        {
            var actualRegion = JsonConvert.DeserializeObject<Region>($"\"{region}\"");

            log.LogInformation($"Getting all vehicles for {actualRegion}");

            items = container.GetItemLinqQueryable<Vehicle>().Where(v => v.Region == actualRegion).Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.Distict,
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }).ToFeedIterator();
        }
        else
        {
            var actualRegion = JsonConvert.DeserializeObject<Region>($"\"{region}\"");

            log.LogInformation($"Getting all vehicles for district {district} of region {actualRegion}.");

            items = container.GetItemLinqQueryable<Vehicle>().Where(v => v.Region == actualRegion && v.Distict == district).Select(v => new VehicleSettingsDetail
            {
                CallSign = v.CallSign,
                District = v.Distict,
                Region = v.Region,
                Registration = v.Registration,
                Type = v.VehicleType,
            }).ToFeedIterator();
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
