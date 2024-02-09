// -----------------------------------------------------------------------
// <copyright file="RequestLogging.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Logging;

namespace AODashboard.Logging;

/// <summary>
/// Logger to deal with request events.
/// </summary>
public static partial class RequestLogging
{
    private static Func<ILogger, string, string, IDisposable?> runningControllerScope = LoggerMessage.DefineScope<string, string>("{Controller} {Name}");

    /// <summary>
    /// Logs that the a request was made with bad parameters.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameters that are bad.</param>
    [LoggerMessage(EventId = EventIds.RequestBadParameters, EventName = nameof(EventIds.RequestBadParameters), Level = LogLevel.Error, Message = "A request failed because some of the parameters were invalid : {Parameters}.")]
    public static partial void BadParameters(ILogger logger, string[] parameters);

    /// <summary>
    /// Logs that a request cleared the requested item.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="item">The item being cleared.</param>
    [LoggerMessage(EventId = EventIds.RequestCleared, EventName = nameof(EventIds.RequestCleared), Level = LogLevel.Information, Message = "A request succeeded when the item was cleared : {Item}.")]
    public static partial void Cleared(ILogger logger, string item);

    /// <summary>
    /// Logs that a request found the requested item.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="item">The item being looked for.</param>
    [LoggerMessage(EventId = EventIds.RequestFound, EventName = nameof(EventIds.RequestFound), Level = LogLevel.Debug, Message = "A request succeeded when the item was found : {Item}.")]
    public static partial void Found(ILogger logger, string item);

    /// <summary>
    /// Logs that the a request was made with missing parameters.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="parameters">The parameters that are missing.</param>
    [LoggerMessage(EventId = EventIds.RequestMissingParameters, EventName = nameof(EventIds.RequestMissingParameters), Level = LogLevel.Error, Message = "A user requested a page with required parameters missing : {Parameters}.")]
    public static partial void MissingParameters(ILogger logger, string[] parameters);

    /// <summary>
    /// Logs that a request did not find the requested item.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="item">The item being looked for.</param>
    [LoggerMessage(EventId = EventIds.RequestNotFound, EventName = nameof(EventIds.RequestNotFound), Level = LogLevel.Warning, Message = "A request failed when the item was not found : {Item}.")]
    public static partial void NotFound(ILogger logger, string item);

    /// <summary>
    /// Logs that a requested item hasn't been modified.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="item">The item being looked for.</param>
    [LoggerMessage(EventId = EventIds.RequestNotModified, EventName = nameof(EventIds.RequestNotModified), Level = LogLevel.Warning, Message = "A request succeeded because the item has not been modified : {Item}.")]
    public static partial void NotModified(ILogger logger, string item);

    /// <summary>
    /// Starts a logging scope for a given controller action.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="controller">The controller by run.</param>
    /// <param name="action">The action being run.</param>
    /// <returns>An IDisposable representing the scope.</returns>
    public static IDisposable? RunningControllerScope(this ILogger logger, string controller, string action) => runningControllerScope(logger, controller, action);

    /// <summary>
    /// Logs that a request encountered an unexpected error.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="message">The message for the log.</param>
    /// <param name="ex">The exception that captured this error.</param>
    [LoggerMessage(EventId = EventIds.RequestUnexpectedError, EventName = nameof(EventIds.RequestUnexpectedError), Level = LogLevel.Error, Message = "A request failed with an unexpected error : {Message}.")]
    public static partial void UnexpectedError(ILogger logger, string message, Exception? ex = null);

    /// <summary>
    /// Logs that a request found the requested item.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="item">The item being looked for.</param>
    [LoggerMessage(EventId = EventIds.RequestUpdated, EventName = nameof(EventIds.RequestUpdated), Level = LogLevel.Debug, Message = "A request succeeded when the item was updated : {Item}.")]
    public static partial void Updated(ILogger logger, string item);
}
