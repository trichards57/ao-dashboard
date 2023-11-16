using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VorReceiver.Model;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

namespace VorReceiver;

public class VorStatus
{
    private readonly CosmosClient cosmosClient;
    private readonly IConfiguration configuration;

    public VorStatus(CosmosClient cosmosClient, IConfiguration configuration)
    {
        this.cosmosClient = cosmosClient;
        this.configuration = configuration;
    }

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
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
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

                    results[item.CallSign.ToUpperInvariant().Trim()] = new VorStatusResult { 
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

public class VorStatusResultConverter : JsonConverter<VorStatusResult>
{
    public override VorStatusResult ReadJson(JsonReader reader, Type objectType, VorStatusResult existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

    public override void WriteJson(JsonWriter writer, VorStatusResult value, JsonSerializer serializer)
    {
        if (!value.IsVor)
        {
            writer.WriteValue(false);
        }
        else
        {
            writer.WriteStartObject();

            writer.WritePropertyName("isVor");
            writer.WriteValue(value.IsVor);

            if (value.DueBack.HasValue)
            {
                writer.WritePropertyName("dueBack");
                writer.WriteValue(value.DueBack);
            }

            if (!string.IsNullOrWhiteSpace(value.Summary))
            {
                writer.WritePropertyName("summary");
                writer.WriteValue(value.Summary.Trim());
            }

            writer.WriteEndObject();
        }
    }
}

[JsonConverter(typeof(VorStatusResultConverter))]
public readonly record struct VorStatusResult
{
    [JsonProperty("isVor")]
    public bool IsVor { get; init; }

    [JsonProperty("dueBack", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateOnly? DueBack { get; init; }

    [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Summary { get; init; }
}
