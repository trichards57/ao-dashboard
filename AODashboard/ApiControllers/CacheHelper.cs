// -----------------------------------------------------------------------
// <copyright file="CacheHelper.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Logging;
using AODashboard.Middleware.ServerTiming;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;

namespace AODashboard.ApiControllers;

/// <summary>
/// Helper to handle cached requests.
/// </summary>
public static class CacheHelper
{
    /// <summary>
    /// Checks one or more items against an ETag and returns a 304 if the ETag matches.  Otherwise, returns the items with an updated ETag.
    /// </summary>
    /// <typeparam name="T">The type of item to return.</typeparam>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="getEtag">Accessor to get the ETag.</param>
    /// <param name="getItems">Accessor to get the items.</param>
    /// <param name="logger">Logger for the operation.</param>
    /// <param name="logParam">The log phrase to use.</param>
    /// <param name="serverTiming">The server timing service to use.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    public static async Task<ActionResult<T>> CachedGet<T>(this ControllerBase controller, Func<Task<string>> getEtag, Func<Task<T>> getItems, ILogger logger, string logParam, IServerTiming? serverTiming = null)
    {
        var stopWatch = new Stopwatch();

        var incomingEtag = controller.Request.GetTypedHeaders().IfNoneMatch.FirstOrDefault(h => !h.IsWeak)?.Tag.Value?.Trim('=').Trim('"');

        stopWatch.Start();

        var actualEtag = await getEtag();

        stopWatch.Stop();

        serverTiming?.Metrics.Add(new ServerTimingMetric("etag", stopWatch.ElapsedMilliseconds, "Get Etag"));

        if (!string.IsNullOrWhiteSpace(incomingEtag) && !string.IsNullOrWhiteSpace(actualEtag) && incomingEtag.Equals(actualEtag, StringComparison.Ordinal))
        {
            serverTiming?.Metrics.Add(new ServerTimingMetric("hit", description: "Cache Hit"));

            RequestLogging.NotModified(logger, logParam);

            return controller.StatusCode(StatusCodes.Status304NotModified);
        }

        serverTiming?.Metrics.Add(new ServerTimingMetric("miss", description: "Cache Miss"));

        stopWatch.Restart();

        var items = await getItems();

        stopWatch.Stop();

        serverTiming?.Metrics.Add(new ServerTimingMetric("items", stopWatch.ElapsedMilliseconds, "Get Items"));

        if (items == null)
        {
            RequestLogging.NotFound(logger, logParam);

            return controller.NotFound(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Not Found",
                Detail = "The requested item was not found.",
                Status = StatusCodes.Status404NotFound,
                Instance = controller.Request.GetDisplayUrl(),
            });
        }

        if (!string.IsNullOrWhiteSpace(actualEtag))
        {
            controller.Response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{actualEtag}\"", false);
        }

        RequestLogging.Found(logger, logParam);
        return controller.Ok(items);
    }

    /// <summary>
    /// Checks one or more items against an ETag and returns a 304 if the ETag matches.  Otherwise, returns the items with an updated ETag.
    /// </summary>
    /// <typeparam name="T">The type of item to return.</typeparam>
    /// <param name="controller">The controller handling the request.</param>
    /// <param name="getEtag">Accessor to get the ETag.</param>
    /// <param name="getItems">Accessor to get the items.</param>
    /// <param name="logger">Logger for the operation.</param>
    /// <param name="logParam">The log phrase to use.</param>
    /// <param name="serverTiming">The server timing service to use.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the response from the action.</returns>
    public static async Task<ActionResult<T>> CachedGet<T>(this ControllerBase controller, Func<Task<string>> getEtag, Func<T> getItems, ILogger logger, string logParam, IServerTiming? serverTiming = null)
    {
        var stopWatch = new Stopwatch();

        var incomingEtag = controller.Request.GetTypedHeaders().IfNoneMatch.FirstOrDefault(h => !h.IsWeak)?.Tag.Value?.Trim('=').Trim('"');

        stopWatch.Start();

        var actualEtag = await getEtag();

        stopWatch.Stop();

        serverTiming?.Metrics.Add(new ServerTimingMetric("Get ETag", stopWatch.ElapsedMilliseconds));

        if (!string.IsNullOrWhiteSpace(incomingEtag) && !string.IsNullOrWhiteSpace(actualEtag) && incomingEtag.Equals(actualEtag, StringComparison.Ordinal))
        {
            serverTiming?.Metrics.Add(new ServerTimingMetric("hit", description: "Cache Hit"));

            RequestLogging.NotModified(logger, logParam);

            return controller.StatusCode(StatusCodes.Status304NotModified);
        }

        serverTiming?.Metrics.Add(new ServerTimingMetric("miss", description: "Cache Miss"));

        stopWatch.Restart();

        var items = getItems();

        stopWatch.Stop();

        serverTiming?.Metrics.Add(new ServerTimingMetric("Get Items", stopWatch.ElapsedMilliseconds));

        if (items == null)
        {
            RequestLogging.NotFound(logger, logParam);

            return controller.NotFound(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Not Found",
                Detail = "The requested item was not found.",
                Status = StatusCodes.Status404NotFound,
                Instance = controller.Request.GetDisplayUrl(),
            });
        }

        if (!string.IsNullOrWhiteSpace(actualEtag))
        {
            controller.Response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{actualEtag}\"", false);
        }

        RequestLogging.Found(logger, logParam);
        return controller.Ok(items);
    }
}
