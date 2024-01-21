// -----------------------------------------------------------------------
// <copyright file="UserLogger.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Logging;

namespace AODashboard.ApiControllers;

/// <summary>
/// Logger to deal with controller events.
/// </summary>
public static partial class UserLogger
{
    /// <summary>
    /// Logs that a user has requested low-sensitivity information about their profile (e.g. their profile picture).
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="details">The details requested.</param>
    [LoggerMessage(EventId = EventIds.UserProfileDetailsRequested, EventName = nameof(EventIds.UserProfileDetailsRequested), Level = LogLevel.Information, Message = "A user requested some details from their profile. : {UserId}, {Details}")]
    public static partial void UserProfileDetailsRequested(ILogger logger, string userId, string[] details);
}
