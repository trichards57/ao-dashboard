// -----------------------------------------------------------------------
// <copyright file="VehicleSettingsTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using API.Model;
using API.Support;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using Shared;
using System.Text;
using System.Text.Json;

namespace API.Tests;

public class VehicleSettingsTests
{
    private static List<Vehicle> TestData => new()
    {
        new() { CallSign = "WR111", Region = Region.SouthWest, District = "BAS" },
        new() { CallSign = "WR112", Region = Region.SouthWest, District = "Devon" },
        new() { CallSign = "WR113", Region = Region.SouthWest, District = "BAS" },
        new() { CallSign = "WR114", Region = Region.SouthEast, District = "District 1" },
        new() { CallSign = "WR115", Region = Region.SouthEast, District = "District 2" },
        new() { CallSign = "WR116", Region = Region.SouthEast, District = "District 1" },
    };

    [Fact]
    public async Task Get_RejectsCallSignWithDistrict()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "BAS" },
            { "region", "" },
            { "callsign", "WR111" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>()
            .Which.Detail.Should().Be("Call Sign must not be specified if District or Region is provided.");
    }

    [Fact]
    public async Task Get_RejectsCallSignWithRegion()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "" },
            { "region", "sw" },
            { "callsign", "WR111" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>()
            .Which.Detail.Should().Be("Call Sign must not be specified if District or Region is provided.");
    }

    [Fact]
    public async Task Get_RejectsDistrictWithoutRegion()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "BAS" },
            { "region", "" },
            { "callsign", "" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>()
            .Which.Detail.Should().Be("Region must not be empty if district is provided.");
    }

    [Fact]
    public async Task Get_ReturnsAllWithNoParameters()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "" },
            { "region", "" },
            { "callsign", "" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");
        var helper = new Mock<ICosmosLinqQuery>(MockBehavior.Strict);
        helper.Setup(s => s.GetFeedIterator(It.IsAny<IQueryable<VehicleSettingsDetail>>()))
            .Returns(new Func<IQueryable<VehicleSettingsDetail>, FeedIterator<VehicleSettingsDetail>>(GetFeedIterator));

        var settingsFunction = new VehicleSettings(client, helper.Object, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<VehicleSettingsDetail>>()
            .Which.Should().BeEquivalentTo(TestData, c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Get_ReturnsDistrictItems()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "BAS" },
            { "region", "sw" },
            { "callsign", "" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");
        var helper = new Mock<ICosmosLinqQuery>(MockBehavior.Strict);
        helper.Setup(s => s.GetFeedIterator(It.IsAny<IQueryable<VehicleSettingsDetail>>()))
            .Returns(new Func<IQueryable<VehicleSettingsDetail>, FeedIterator<VehicleSettingsDetail>>(GetFeedIterator));

        var settingsFunction = new VehicleSettings(client, helper.Object, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<VehicleSettingsDetail>>()
            .Which.Should().BeEquivalentTo(TestData.Where(r => r.Region == Region.SouthWest && r.District == "BAS"), c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Get_ReturnsRegionItems()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "" },
            { "region", "sw" },
            { "callsign", "" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");
        var helper = new Mock<ICosmosLinqQuery>(MockBehavior.Strict);
        helper.Setup(s => s.GetFeedIterator(It.IsAny<IQueryable<VehicleSettingsDetail>>()))
            .Returns(new Func<IQueryable<VehicleSettingsDetail>, FeedIterator<VehicleSettingsDetail>>(GetFeedIterator));

        var settingsFunction = new VehicleSettings(client, helper.Object, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<VehicleSettingsDetail>>()
            .Which.Should().BeEquivalentTo(TestData.Where(r => r.Region == Region.SouthWest), c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Get_ReturnsSingleWithCallSign()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "district", "" },
            { "region", "" },
            { "callsign", "WR111" },
        };

        var queryCollection = new QueryCollection(query);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Query).Returns(queryCollection);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");
        var helper = GetFeedIteratorHelper();

        var settingsFunction = new VehicleSettings(client, helper, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Run(request.Object);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<VehicleSettingsDetail>>()
            .Which.Should().BeEquivalentTo(TestData.Where(r => r.CallSign == "WR111"), c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Set_CreatesNewItem()
    {
        var testData = new VehicleSettingsDetail
        {
            Region = Region.SouthWest,
            CallSign = "WR123",
            District = "BAS",
            Registration = "X632NBK",
            Type = VehicleType.FrontLineAmbulance,
        };
        var testVehicle = new Vehicle
        {
            CallSign = testData.CallSign,
            Registration = testData.Registration,
            District = testData.District,
            Region = testData.Region,
            VehicleType = testData.Type,
        };

        var testDataString = JsonSerializer.Serialize(testData);
        var testDataBytes = Encoding.UTF8.GetBytes(testDataString);
        var bodyStream = new MemoryStream(testDataBytes);

        var response = new Mock<ItemResponse<Vehicle>>();
        response.Setup(s => s.StatusCode).Returns(System.Net.HttpStatusCode.NotFound);
        var newResponse = new Mock<ItemResponse<Vehicle>>();
        var container = new Mock<Container>(MockBehavior.Strict);
        container.Setup(s => s.ReadItemAsync<Vehicle>(testData.Registration, new PartitionKey("VOR"), null, default))
            .ReturnsAsync(response.Object);
        container.Setup(s => s.CreateItemAsync(testVehicle, null, null, default))
            .ReturnsAsync(newResponse.Object);
        var client = new Mock<CosmosClient>(MockBehavior.Strict);
        client.Setup(s => s.GetContainer("vehicle-data", "vor-data")).Returns(container.Object);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Body).Returns(bodyStream);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client.Object, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Set(request.Object);

        result.Should().BeOfType<CreatedResult>()
            .Which.Value.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task Set_UpdatesItem()
    {
        var testData = new VehicleSettingsDetail
        {
            Region = Region.SouthWest,
            CallSign = "WR123",
            District = "BAS",
            Registration = "X632NBK",
            Type = VehicleType.FrontLineAmbulance,
        };
        var testVehicle = new Vehicle
        {
            CallSign = "WR132",
            Registration = "X632NBK",
            District = "Devon",
            Region = Region.SouthEast,
            VehicleType = VehicleType.OffRoadAmbulance,
            Incidents =
            [
                new Incident { Comments = "Test Comments", Description = "Test Description", EndDate = new DateOnly(2022, 12, 2), EstimatedEndDate = new DateOnly(2022, 12, 12), StartDate = new DateOnly(2022, 12, 1) },
            ],
        };
        var newVehicle = new Vehicle
        {
            CallSign = testData.CallSign,
            Registration = testData.Registration,
            District = testData.District,
            Region = testData.Region,
            VehicleType = testData.Type,
            Incidents = testVehicle.Incidents,
        };

        var testDataString = JsonSerializer.Serialize(testData);
        var testDataBytes = Encoding.UTF8.GetBytes(testDataString);
        var bodyStream = new MemoryStream(testDataBytes);

        var response = new Mock<ItemResponse<Vehicle>>();
        response.Setup(s => s.StatusCode).Returns(System.Net.HttpStatusCode.OK);
        response.Setup(s => s.Resource).Returns(testVehicle);
        var newResponse = new Mock<ItemResponse<Vehicle>>();
        var container = new Mock<Container>(MockBehavior.Strict);
        container.Setup(s => s.ReadItemAsync<Vehicle>(testData.Registration, new PartitionKey("VOR"), null, default))
            .ReturnsAsync(response.Object);
        container.Setup(s => s.UpsertItemAsync(newVehicle, null, null, default))
            .ReturnsAsync(newResponse.Object);
        var client = new Mock<CosmosClient>(MockBehavior.Strict);
        client.Setup(s => s.GetContainer("vehicle-data", "vor-data")).Returns(container.Object);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Body).Returns(bodyStream);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client.Object, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Set(request.Object);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task Set_RejectsBlankRegistration()
    {
        var testData = new VehicleSettingsDetail
        {
            Region = Region.SouthWest,
            CallSign = "WR123",
            District = "BAS",
            Registration = "",
            Type = VehicleType.FrontLineAmbulance,
        };
        var testDataString = JsonSerializer.Serialize(testData);
        var testDataBytes = Encoding.UTF8.GetBytes(testDataString);
        var bodyStream = new MemoryStream(testDataBytes);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Body).Returns(bodyStream);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Set(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>()
            .Which.Errors.Should().ContainEquivalentOf(new KeyValuePair<string, string[]>("Registration", ["Registration must be provided."]));
    }

    [Fact]
    public async Task Set_RejectsInvalidRegion()
    {
        var testData = new VehicleSettingsDetail
        {
            Region = (Region)43,
            CallSign = "WR123",
            District = "BAS",
            Registration = "X632NBK",
            Type = VehicleType.FrontLineAmbulance,
        };
        var testDataString = JsonSerializer.Serialize(testData);
        var testDataBytes = Encoding.UTF8.GetBytes(testDataString);
        var bodyStream = new MemoryStream(testDataBytes);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Body).Returns(bodyStream);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Set(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>()
            .Which.Errors.Should().ContainEquivalentOf(new KeyValuePair<string, string[]>("Region", ["Region must be provided."]));
    }

    [Fact]
    public async Task Set_RejectsInvalidType()
    {
        var testData = new VehicleSettingsDetail
        {
            Region = Region.SouthWest,
            CallSign = "WR123",
            District = "BAS",
            Registration = "X632NBK",
            Type = (VehicleType)42,
        };
        var testDataString = JsonSerializer.Serialize(testData);
        var testDataBytes = Encoding.UTF8.GetBytes(testDataString);
        var bodyStream = new MemoryStream(testDataBytes);

        var client = GetCosmosClient(TestData);
        var config = GetConfiguration();
        var request = new Mock<HttpRequest>(MockBehavior.Strict);
        request.Setup(s => s.Body).Returns(bodyStream);
        request.Setup(s => s.Path).Returns("/api/vehicle-settings");

        var settingsFunction = new VehicleSettings(client, null, config, NullLogger<VehicleSettings>.Instance);

        var result = await settingsFunction.Set(request.Object);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>()
            .Which.Errors.Should().ContainEquivalentOf(new KeyValuePair<string, string[]>("Type", ["Type must be provided."]));
    }

    private static IConfiguration GetConfiguration()
    {
        var configSection = new Mock<IConfigurationSection>(MockBehavior.Strict);
        configSection.Setup(s => s.Value).Returns(() => null);
        var config = new Mock<IConfiguration>(MockBehavior.Strict);
        config.Setup(s => s.GetSection("CosmosDbDatabase")).Returns(configSection.Object);
        config.Setup(s => s.GetSection("CosmosDbContainer")).Returns(configSection.Object);
        return config.Object;
    }

    private static CosmosClient GetCosmosClient(List<Vehicle> data)
    {
        var container = new Mock<Container>(MockBehavior.Strict);
        container.Setup(s => s.GetItemLinqQueryable<Vehicle>(false, null, null, null))
            .Returns(data.AsQueryable().OrderBy(v => v.CallSign));
        var client = new Mock<CosmosClient>(MockBehavior.Strict);
        client.Setup(s => s.GetContainer("vehicle-data", "vor-data")).Returns(container.Object);
        return client.Object;
    }

    private FeedIterator<VehicleSettingsDetail> GetFeedIterator(IEnumerable<VehicleSettingsDetail> items)
    {
        var feedResponse = new Mock<FeedResponse<VehicleSettingsDetail>>();
        feedResponse.Setup(s => s.GetEnumerator()).Returns(() => items.GetEnumerator());
        var feedIterator = new Mock<FeedIterator<VehicleSettingsDetail>>();
        feedIterator.SetupSequence(s => s.HasMoreResults)
            .Returns(true)
            .Returns(false);
        feedIterator.Setup(s => s.ReadNextAsync(default))
            .ReturnsAsync(feedResponse.Object);

        return feedIterator.Object;
    }

    private ICosmosLinqQuery GetFeedIteratorHelper()
    {
        var helper = new Mock<ICosmosLinqQuery>(MockBehavior.Strict);
        helper.Setup(s => s.GetFeedIterator(It.IsAny<IQueryable<VehicleSettingsDetail>>()))
            .Returns(new Func<IQueryable<VehicleSettingsDetail>, FeedIterator<VehicleSettingsDetail>>(GetFeedIterator));
        return helper.Object;
    }
}
