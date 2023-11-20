// -----------------------------------------------------------------------
// <copyright file="VorReceiver.cs" company="Tony Richards">
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
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VorReceiver.Model;

namespace VorReceiver;

/// <summary>
/// Function to receive VOR data.
/// </summary>
public class VorReceiver
{
    private const string Partition = "VOR";
    private readonly int batchSize;
    private readonly CosmosClient cosmosClient;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VorReceiver"/> class.
    /// </summary>
    /// <param name="cosmosClient">The client used to access CosmosDB.</param>
    /// <param name="configuration">The function configuration files.</param>
    public VorReceiver(CosmosClient cosmosClient, IConfiguration configuration)
    {
        var licenseCode = configuration["SyncfusionLicenseCode"];

        batchSize = configuration.GetValue("BatchSize", 100);

        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseCode);

        this.cosmosClient = cosmosClient;
        this.configuration = configuration;
    }

    /// <summary>
    /// Function to accept a VOR status file.
    /// </summary>
    /// <param name="req">The HTTP Request received.</param>
    /// <param name="log">The logger for the function.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [FunctionName("vor-receiver")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        double cost = 0;
        log.LogInformation("Received request.");

        var form = await req.ReadFormAsync();

        if (form.Files.Count != 1)
        {
            log.LogError("No file received.");

            return new BadRequestObjectResult(new ProblemDetails()
            {
                Detail = "No file received.",
                Instance = req.Path,
                Status = StatusCodes.Status400BadRequest,
                Title = "No file received.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            });
        }

        log.LogInformation("Received uploaded file.");

        var file = form.Files[0];

        log.LogInformation($"Received file {file.FileName} of size {file.Length} bytes.");

        var parameters = req.GetQueryParameterDictionary();

        if (!parameters.TryGetValue("date", out var date) || !DateOnly.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fileDate))
        {
            try
            {
                fileDate = DateOnly.Parse(file.FileName.Split(" ")[0], CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                log.LogError($"No date provided and filename {file.FileName} does not start with a valid date.");

                return new BadRequestObjectResult(new ProblemDetails()
                {
                    Detail = $"No date provided and filename {file.FileName} does not start with a valid date.",
                    Instance = req.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid report date.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }
        }

        log.LogInformation($"File date is {fileDate}.");

        var updateVors = fileDate == DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);
        var forceUpdateValid = parameters.TryGetValue("update-vors", out var forceUpdate);

        if (updateVors)
        {
            log.LogInformation($"File date is from today.  Will update VOR status.");
        }
        else if (forceUpdateValid && forceUpdate.Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
            log.LogInformation($"'update-vors' is set to true.  Will update VOR status.");
            updateVors = true;
        }
        else
        {
            log.LogInformation($"Historic log.  Will not update VOR status.");
        }

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

        using var excelEngine = new ExcelEngine();
        var excelApp = excelEngine.Excel;

        excelApp.Workbooks.Open(file.OpenReadStream());

        var sheet = excelApp.Worksheets[0];

        var columns = new Dictionary<string, int>();

        var headerRow = sheet.Rows[0];

        var container = cosmosClient.GetVorContainer(configuration);

        if (updateVors)
        {
            var iterator = container.GetItemLinqQueryable<Vehicle>().Where(v => v.Partition == Partition).ToFeedIterator();
            var batchTasks = new List<Task<TransactionalBatchResponse>>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                cost += response.RequestCharge;

                var batch = container.CreateTransactionalBatch(new PartitionKey(Partition));

                var c = 0;

                foreach (var item in response)
                {
                    if (item.IsVor)
                    {
                        item.IsVor = false;
                        batch.ReplaceItem(item.Registration, item);
                        c += 1;

                        if (c >= batchSize)
                        {
                            batchTasks.Add(batch.ExecuteAsync());
                            c = 0;
                            batch = container.CreateTransactionalBatch(new PartitionKey(Partition));
                            await Task.Delay(250);
                        }
                    }
                }

                if (c > 0)
                {
                    batchTasks.Add(batch.ExecuteAsync());
                }
            }

            var res = await Task.WhenAll(batchTasks);

            if (Array.Exists(res, task => !task.IsSuccessStatusCode))
            {
                log.LogError("VOR Status Clear failed");
            }
            else
            {
                log.LogInformation("VOR Status Cleared");
                cost += res.Sum(t => t.RequestCharge);
            }
        }

        foreach (var c in headerRow.Cells)
        {
            columns[c.Text.Replace(" ", "")] = c.Column;
        }

        var updates = 0;

        foreach (var cols in sheet.Rows.Skip(1).Select(r => r.Columns))
        {
            var reg = cols[columns["VehicleReg"] - 1].Text.Trim().ToUpper();
            var fleetNum = cols[columns["FleetNumber"] - 1].Text?.Trim() ?? "";
            var bodyType = cols[columns["BodyType"] - 1].Text?.Trim() ?? "";
            var make = cols[columns["Make"] - 1].Text?.Trim() ?? "";
            var model = cols[columns["Model"] - 1].Text?.Trim() ?? "";
            var comments = cols[columns["Comments"] - 1].Text?.Trim() ?? "";
            var startDate = DateOnly.FromDateTime(cols[columns["StartDate"] - 1].DateTime);
            var description = cols[columns["Description"] - 1].Text?.Trim() ?? "";
            var estimatedReturnCol = columns.GetValueOrDefault("EstimatedRepairDate");
            if (estimatedReturnCol == default)
            {
                estimatedReturnCol = columns.GetValueOrDefault("ExpectedFinishDate");
            }

            DateOnly? estimatedReturn = null;

            if (estimatedReturnCol != default)
            {
                var estimatedReturnDateTime = cols[estimatedReturnCol - 1].DateTime;
                estimatedReturn = estimatedReturnDateTime == DateTime.MinValue ? (DateOnly?)null : DateOnly.FromDateTime(estimatedReturnDateTime);
            }

            Vehicle vehicle;

            var count = 0;
            var retry = false;

            do
            {
                retry = false;

                try
                {
                    vehicle = await container.ReadItemAsync<Vehicle>(reg, new(Partition));
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    vehicle = new Vehicle();
                }

                vehicle.CallSign = fleetNum;
                vehicle.Registration = reg;
                vehicle.BodyType = bodyType;
                vehicle.Make = make;
                vehicle.Model = model;

                var incident = vehicle.Incidents.Find(i => i.StartDate == startDate);

                if (incident == null)
                {
                    incident = new Incident { StartDate = startDate, Description = description };
                    vehicle.Incidents.Add(incident);
                }

                if (incident.EndDate < fileDate)
                {
                    incident.EndDate = fileDate;
                    incident.Description = description;
                    incident.Comments = comments;
                    incident.EstimatedEndDate = estimatedReturn;
                }

                if (updateVors)
                {
                    vehicle.IsVor = true;
                }

                ItemResponse<Vehicle> res = null;

                try
                {
                    res = await container.UpsertItemAsync(vehicle, new PartitionKey(vehicle.Partition), new ItemRequestOptions
                    {
                        IfMatchEtag = vehicle.Etag,
                    });
                    log.LogTrace($"Updated {reg}.");
                    updates++;
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                    log.LogWarning($"Concurrency clash found.");
                    retry = true;
                }
                catch (CosmosException)
                {
                    log.LogWarning($"Error accessing CosmosDb.  Retry {count}.");
                    retry = true;
                    count++;
                }

                cost += res?.RequestCharge ?? 0;
            }
            while (retry && count < 3);

            if (count == 3)
            {
                log.LogError($"Ran out of retries. Skipping {reg}.");
            }
        }

        log.LogInformation($"File completed.  {updates} items updated.  {cost} RUs expended.");

        excelApp.Workbooks.Close();

        return new OkResult();
    }
}
