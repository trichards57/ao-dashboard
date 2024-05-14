// -----------------------------------------------------------------------
// <copyright file="AppInsightsTelemetryInitializer.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Security.Claims;

namespace Dashboard.Services;

/// <summary>
/// Initializer for the application insights telemetry.
/// </summary>
public class AppInsightsTelemetryInitializer(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : ITelemetryInitializer
{
    private readonly IWebHostEnvironment environment = environment;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public void Initialize(ITelemetry telemetry)
    {
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            telemetry.Context.Cloud.RoleName = "AO-Dashboard-Server";
            telemetry.Context.Cloud.RoleInstance = environment.IsDevelopment() ? "Development" : "Production";
        }

        var context = httpContextAccessor.HttpContext;

        if (context != null)
        {
            telemetry.Context.User.AuthenticatedUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
