// -----------------------------------------------------------------------
// <copyright file="VehicleSettingsControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers;
using AODashboard.Client.Logging;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Services;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AODashboard.Tests.ApiControllers;

public class VehicleSettingsControllerTests
{
    private readonly VehicleSettingsController controller;
    private readonly Fixture fixture = new();
    private readonly Mock<ILogger<VehicleSettingsController>> loggerMock;
    private readonly Mock<IVehicleService> vehicleServiceMock;

    public VehicleSettingsControllerTests()
    {
        vehicleServiceMock = new Mock<IVehicleService>();
        loggerMock = new Mock<ILogger<VehicleSettingsController>>();

        loggerMock.Setup(s => s.IsEnabled(LogLevel.Debug)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Warning)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Error)).Returns(true);

        controller = new VehicleSettingsController(vehicleServiceMock.Object, loggerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    Request =
                    {
                        Scheme = "https",
                        Host = new HostString("localhost", 443),
                        PathBase = "/",
                    },
                },
            },
        };
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenBothCallSignAndRegistrationIsGiven()
    {
        var callSign = fixture.Create<string>();
        var registration = fixture.Create<string>();

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenNeitherCallSignNotRegistrationIsGiven()
    {
        var callSign = (string?)null;
        var registration = (string?)null;

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenCallSignIsNotFound()
    {
        var callSign = fixture.Create<string>();
        var registration = (string?)null;

        vehicleServiceMock.Setup(x => x.GetByCallSignAsync(callSign)).ReturnsAsync((VehicleSettings?)null);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status404NotFound);
        loggerMock.Verify(logger => logger.Log(LogLevel.Warning, new EventId(EventIds.RequestNotFound, nameof(EventIds.RequestNotFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenRegistrationIsNotFound()
    {
        var registration = fixture.Create<string>();
        var callSign = (string?)null;

        vehicleServiceMock.Setup(x => x.GetByRegistrationAsync(registration)).ReturnsAsync((VehicleSettings?)null);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status404NotFound);
        loggerMock.Verify(logger => logger.Log(LogLevel.Warning, new EventId(EventIds.RequestNotFound, nameof(EventIds.RequestNotFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithVehicleSettings_WhenCallsignIsValid()
    {
        var callSign = fixture.Create<string>();
        var registration = (string?)null;

        var expectedResult = new VehicleSettings
        {
            CallSign = "WR123",
            Registration = "X632NBK",
            VehicleType = VehicleType.AllWheelDrive,
            District = "Test District",
            Hub = "Test Hub",
            Region = Region.EastMidlands,
        };

        vehicleServiceMock.Setup(x => x.GetByCallSignAsync(callSign)).ReturnsAsync(expectedResult);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedResult);

        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestFound, nameof(EventIds.RequestFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithVehicleSettings_WhenRegistrationIsValid()
    {
        var registration = fixture.Create<string>();
        var callSign = (string?)null;

        var expectedResult = new VehicleSettings
        {
            CallSign = "WR123",
            Registration = "X632NBK",
            VehicleType = VehicleType.AllWheelDrive,
            District = "Test District",
            Hub = "Test Hub",
            Region = Region.EastMidlands,
        };

        vehicleServiceMock.Setup(x => x.GetByRegistrationAsync(registration)).ReturnsAsync(expectedResult);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedResult);

        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestFound, nameof(EventIds.RequestFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Post_ReturnsOkResult_WithUpdatedVehicle_WhenUpdateIsSuccessful()
    {
        var settings = fixture.Create<UpdateVehicleSettings>();
        var expectedResult = new VehicleSettings
        {
            CallSign = settings.CallSign,
            Registration = settings.Registration,
            VehicleType = settings.VehicleType,
            District = settings.District,
            Hub = settings.Hub,
            Region = settings.Region,
        };

        vehicleServiceMock.Setup(x => x.UpdateSettingsAsync(settings)).Returns(Task.CompletedTask);
        vehicleServiceMock.Setup(x => x.GetByRegistrationAsync(settings.Registration)).ReturnsAsync(expectedResult);

        var result = await controller.Post(settings);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedResult);
        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestUpdated, nameof(EventIds.RequestUpdated)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }
}
