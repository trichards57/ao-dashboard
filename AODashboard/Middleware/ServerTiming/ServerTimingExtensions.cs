// -----------------------------------------------------------------------
// <copyright file="ServerTimingExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Extension methods to handle server timing headers.
/// </summary>
internal static class ServerTimingExtensions
{
    /// <summary>
    /// Sets the server timing header.
    /// </summary>
    /// <param name="response">The response to add the header to.</param>
    /// <param name="metrics">The metrics to add to the header.</param>
    public static void SetServerTiming(this HttpResponse response, params ServerTimingMetric[] metrics)
    {
        var serverTiming = new ServerTimingHeaderValue();

        foreach (ServerTimingMetric metric in metrics)
        {
            serverTiming.Metrics.Add(metric);
        }

        response.Headers.Append("Server-Timing", serverTiming.ToString());
    }

    /// <summary>
    /// Adds the server timing service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to.</param>
    /// <returns>The service collection for chaining requests.</returns>
    public static IServiceCollection AddServerTiming(this IServiceCollection services)
    {
        services.AddScoped<IServerTiming, ServerTiming>();

        return services;
    }
}
