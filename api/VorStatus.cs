// -----------------------------------------------------------------------
// <copyright file="VorStatus.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API;

/// <summary>
/// Function to get the current VOR status of a vehicle.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VorStatus"/> class.
/// </remarks>
/// <param name="cosmosClient">The client used to access CosmosDB.</param>
/// <param name="configuration">The function configuration files.</param>
/// <param name="logger">The logger for the function.</param>
public class VorStatus(CosmosClient cosmosClient, IConfiguration configuration, ILogger<VorStatus> logger)
{
    /// <summary>
    /// Function to get the VOR status of the vehicle(s).
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Function("vor-status")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        double cost = 0;
        logger.LogInformation("Received request.");

        var callsignsValid = req.Query.TryGetValue("callsigns", out var callsigns);

        if (!callsignsValid || string.IsNullOrWhiteSpace(callsigns))
        {
            logger.LogError("No callsigns received.");

            return new BadRequestObjectResult(new ProblemDetails()
            {
                Detail = "No callsigns received.",
                Instance = req.Path,
                Status = StatusCodes.Status400BadRequest,
                Title = "No callsigns received.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            });
        }

        var parts = callsigns[0].Split(",").Select(s => s.Trim().ToUpperInvariant());

        logger.LogInformation($"Received call-signs {string.Join(", ", parts)}.");

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
                        DueBack = incident == default ? null : incident.EstimatedEndDate,
                        Summary = incident == default ? null : incident.Description,
                    };
                }
                else
                {
                    results[item.CallSign.ToUpperInvariant().Trim()] = new VorStatusResult { IsVor = false };
                }
            }
        }

        foreach (var p in parts.Where(p => !results.ContainsKey(p)))
        {
            results[p] = new VorStatusResult { IsVor = false };
        }

        logger.LogInformation($"Request completed.  {cost} RUs expended.");

        return new OkObjectResult(results);
    }
}
