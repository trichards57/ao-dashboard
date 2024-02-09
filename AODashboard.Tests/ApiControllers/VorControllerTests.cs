// -----------------------------------------------------------------------
// <copyright file="VorControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers;
using AODashboard.Client.Logging;
using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Middleware.ServerTiming;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace AODashboard.Tests.ApiControllers;

public class VorControllerTests
{
    private readonly Mock<IVehicleService> vehicleServiceMock;
    private readonly Mock<ILogger<VorController>> loggerMock;
    private readonly Mock<HttpContext> httpContextMock;
    private readonly VorController controller;
    private readonly Fixture fixture = new();
    private readonly Mock<IServerTiming> serverTimingMock;
    private readonly List<ServerTimingMetric> serverTimingMetrics = [];

    public VorControllerTests()
    {
        vehicleServiceMock = new Mock<IVehicleService>();
        loggerMock = new Mock<ILogger<VorController>>();
        httpContextMock = new Mock<HttpContext>();
        serverTimingMock = new Mock<IServerTiming>();
        serverTimingMock.Setup(s => s.Metrics).Returns(serverTimingMetrics);

        var userMock = new Mock<ClaimsPrincipal>();
        userMock.Setup(p => p.Claims)
                .Returns(new List<Claim> { new(ClaimTypes.NameIdentifier, "MockUserId") });
        httpContextMock.Setup(context => context.User).Returns(userMock.Object);

        loggerMock.Setup(s => s.IsEnabled(LogLevel.Debug)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Information)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Warning)).Returns(true);

        controller = new VorController(vehicleServiceMock.Object, loggerMock.Object, serverTimingMock.Object)
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
    public async Task PostEntry_ReturnsOkResult()
    {
        var mockIncident = new VorIncident
        {
            BodyType = fixture.Create<string>(),
            CallSign = fixture.Create<string>(),
            Comments = fixture.Create<string>(),
            Description = fixture.Create<string>(),
            EstimatedRepairDate = new DateOnly(2024, 1, 2),
            Make = fixture.Create<string>(),
            Model = fixture.Create<string>(),
            Registration = fixture.Create<string>(),
            StartDate = new DateOnly(2024, 1, 1),
            UpdateDate = new DateOnly(2024, 1, 4),
        };

        vehicleServiceMock.Setup(service => service.AddEntryAsync(mockIncident))
                          .Returns(Task.CompletedTask);

        var result = await controller.PostEntry([mockIncident]);

        result.Should().BeOfType<OkResult>();
        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestUpdated, nameof(EventIds.RequestUpdated)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
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

        vehicleServiceMock.Setup(x => x.GetStatusByCallSignAsync(callSign)).ReturnsAsync((VorStatus?)null);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status404NotFound);
        loggerMock.Verify(logger => logger.Log(LogLevel.Warning, new EventId(EventIds.RequestNotFound, nameof(EventIds.RequestNotFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenRegistrationIsNotFound()
    {
        var registration = fixture.Create<string>();
        var callSign = (string?)null;

        vehicleServiceMock.Setup(x => x.GetStatusByRegistrationAsync(registration)).ReturnsAsync((VorStatus?)null);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(StatusCodes.Status404NotFound);
        loggerMock.Verify(logger => logger.Log(LogLevel.Warning, new EventId(EventIds.RequestNotFound, nameof(EventIds.RequestNotFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithVorStatus_WhenCallsignIsValid()
    {
        var callSign = fixture.Create<string>();
        var registration = (string?)null;

        var expectedResult = new VorStatus
        {
            DueBack = new DateOnly(2024, 1, 2),
            IsVor = true,
            Summary = fixture.Create<string>(),
        };

        vehicleServiceMock.Setup(x => x.GetStatusByCallSignAsync(callSign)).ReturnsAsync(expectedResult);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedResult);

        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestFound, nameof(EventIds.RequestFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithVorStatus_WhenRegistrationIsValid()
    {
        var registration = fixture.Create<string>();
        var callSign = (string?)null;

        var expectedResult = new VorStatus
        {
            DueBack = new DateOnly(2024, 1, 2),
            IsVor = true,
            Summary = fixture.Create<string>(),
        };

        vehicleServiceMock.Setup(x => x.GetStatusByRegistrationAsync(registration)).ReturnsAsync(expectedResult);

        var result = await controller.Get(callSign, registration);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedResult);

        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestFound, nameof(EventIds.RequestFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
    }
}
