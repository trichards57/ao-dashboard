// -----------------------------------------------------------------------
// <copyright file="VorStatus.cs" company="Tony Richards">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VorReceiver.Model;

namespace VorReceiver;

/// <summary>
/// Function to get the current VOR status of a vehicle.
/// </summary>
public class VorStatus
{
    private readonly CosmosClient cosmosClient;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VorStatus"/> class.
    /// </summary>
    /// <param name="cosmosClient">The client used to access CosmosDB.</param>
    /// <param name="configuration">The function configuration files.</param>
    public VorStatus(CosmosClient cosmosClient, IConfiguration configuration)
    {
        this.cosmosClient = cosmosClient;
        this.configuration = configuration;
    }

    /// <summary>
    /// Function to get the VOR status of the vehicle(s).
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <param name="log">The logger for the function.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [FunctionName("vor-status")]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log)
    {
        double cost = 0;
        log.LogInformation("Received request.");

        var parameters = req.GetQueryParameterDictionary();
        var callsignsValid = parameters.TryGetValue("callsigns", out var callsigns);

        if (!callsignsValid || string.IsNullOrWhiteSpace(callsigns))
        {
            log.LogError("No callsigns received.");

            return new BadRequestObjectResult(new ProblemDetails()
            {
                Detail = "No callsigns received.",
                Instance = req.Path,
                Status = StatusCodes.Status400BadRequest,
                Title = "No callsigns received.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            });
        }

        var parts = callsigns.Split(",").Select(s => s.Trim().ToUpperInvariant());

        log.LogInformation($"Received call-signs {string.Join(", ", parts)}.");

        var container = cosmosClient.GetVorContainer(configuration);

        var items = container.GetItemLinqQueryable<Vehicle>().Where(v => parts.Contains(v.CallSign.ToUpper().Trim())).ToFeedIterator();

        var results = new Dictionary<string, VorStatusResult>();

        while (items.HasMoreResults)
        {
            var response = await items.ReadNextAsync();
            cost += response.RequestCharge;

            foreach (var item in response)
            {
                if (item.IsVor)
                {
                    var incident = item.Incidents.OrderByDescending(s => s.StartDate).FirstOrDefault();

                    results[item.CallSign.ToUpperInvariant().Trim()] = new VorStatusResult
                    {
                        IsVor = item.IsVor,
                        DueBack = incident?.EstimatedEndDate,
                        Summary = incident?.Description,
                    };
                }
                else
                {
                    results[item.CallSign.ToUpperInvariant().Trim()] = new VorStatusResult { IsVor = false };
                }
            }
        }

        foreach (var p in parts)
        {
            if (!results.ContainsKey(p))
            {
                results[p] = new VorStatusResult { IsVor = false };
            }
        }

        log.LogInformation($"Request completed.  {cost} RUs expended.");

        return new OkObjectResult(results);
    }
}
