// -----------------------------------------------------------------------
// <copyright file="ServerTimingMiddleware.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Middleware to inject the server timing header into the response.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ServerTimingMiddleware"/> class.
/// </remarks>
/// <param name="next">The next item in the middleware chain.</param>
public class ServerTimingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The request's HTTP Context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Invoke(HttpContext context)
    {
        HandleServerTiming(context);

        return next(context);
    }

    private static void HandleServerTiming(HttpContext context) => context.Response.OnStarting(() =>
    {
        var serverTiming = context.RequestServices.GetRequiredService<IServerTiming>();

        if (serverTiming.Metrics.Count > 0)
        {
            context.Response.SetServerTiming(serverTiming.Metrics.ToArray());
        }

        return Task.CompletedTask;
    });
}
