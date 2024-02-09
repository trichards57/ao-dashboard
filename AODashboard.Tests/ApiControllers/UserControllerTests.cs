// -----------------------------------------------------------------------
// <copyright file="UserControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.ApiControllers;
using AODashboard.Client.Logging;
using AODashboard.Middleware.ServerTiming;
using AODashboard.Services;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Moq;
using System.Security.Claims;

namespace AODashboard.Tests.ApiControllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> userServiceMock;
    private readonly Mock<ILogger<UserController>> loggerMock;
    private readonly Mock<HttpContext> httpContextMock;
    private readonly Mock<HttpResponse> httpResponseMock;
    private readonly UserController controller;
    private readonly Fixture fixture = new();
    private readonly HeaderDictionary headerDictionary = [];

    public UserControllerTests()
    {
        userServiceMock = new Mock<IUserService>();
        loggerMock = new Mock<ILogger<UserController>>();
        httpContextMock = new Mock<HttpContext>();
        httpResponseMock = new Mock<HttpResponse>();
        httpResponseMock.Setup(s => s.Headers).Returns(headerDictionary);
        httpContextMock.Setup(s => s.Response).Returns(httpResponseMock.Object);

        var userMock = new Mock<ClaimsPrincipal>();
        userMock.Setup(p => p.Claims)
                .Returns(new List<Claim> { new(ClaimTypes.NameIdentifier, "MockUserId") });
        httpContextMock.Setup(context => context.User).Returns(userMock.Object);

        loggerMock.Setup(s => s.IsEnabled(LogLevel.Debug)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Information)).Returns(true);
        loggerMock.Setup(s => s.IsEnabled(LogLevel.Warning)).Returns(true);

        controller = new UserController(userServiceMock.Object, loggerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            },
        };
    }

    [Fact]
    public async Task ProfilePicture_RetrievesPicture()
    {
        var mockImageData = fixture.Create<byte[]>();
        var mockImageDataStream = new MemoryStream(mockImageData);
        userServiceMock.Setup(service => service.GetProfilePictureAsync())
                       .ReturnsAsync(mockImageDataStream);

        // Act
        var result = await controller.ProfilePicture();

        // Assert
        result.Should().BeOfType<FileStreamResult>();
        var fileResult = (FileStreamResult)result;
        fileResult.ContentType.Should().Be("image/jpeg");
        fileResult.FileStream.Should().BeSameAs(mockImageDataStream);
        userServiceMock.Verify(service => service.GetProfilePictureAsync(), Times.Once);
        loggerMock.Verify(logger => logger.Log(LogLevel.Information, new EventId(EventIds.UserProfileDetailsRequested, nameof(EventIds.UserProfileDetailsRequested)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
        loggerMock.Verify(logger => logger.Log(LogLevel.Debug, new EventId(EventIds.RequestFound, nameof(EventIds.RequestFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
        headerDictionary.Should().Contain("Cache-Control", "no-store, private");
    }

    [Fact]
    public async Task ProfilePicture_PictureNotFound()
    {
        userServiceMock.Setup(service => service.GetProfilePictureAsync())
                       .Returns(Task.FromResult<MemoryStream?>(null));

        // Act
        var result = await controller.ProfilePicture();

        // Assert
        result.Should().BeOfType<VirtualFileResult>();
        var fileResult = (VirtualFileResult)result;
        fileResult.FileName.Should().Be("user.jpg");
        fileResult.ContentType.Should().Be("image/jpeg");
        userServiceMock.Verify(service => service.GetProfilePictureAsync(), Times.Once);
        loggerMock.Verify(logger => logger.Log(LogLevel.Information, new EventId(EventIds.UserProfileDetailsRequested, nameof(EventIds.UserProfileDetailsRequested)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
        loggerMock.Verify(logger => logger.Log(LogLevel.Warning, new EventId(EventIds.RequestNotFound, nameof(EventIds.RequestNotFound)), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
        headerDictionary.Should().Contain("Cache-Control", "no-store, private");
    }
}
