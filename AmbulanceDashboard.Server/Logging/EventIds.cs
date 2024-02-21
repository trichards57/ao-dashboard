// -----------------------------------------------------------------------
// <copyright file="EventIds.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AmbulanceDashboard.Logging;

/// <summary>
/// Holds the event logging IDs.
/// </summary>
public static class EventIds
{
    /// <summary>
    /// Event ID when a request has gets bad parameters.
    /// </summary>
    public const int RequestBadParameters = 2004;

    /// <summary>
    /// Event ID when a request successfully clears an item.
    /// </summary>
    public const int RequestCleared = 2006;

    /// <summary>
    /// Event ID when a request successfully finds an item.
    /// </summary>
    public const int RequestFound = 2000;

    /// <summary>
    /// Event ID when a request has required parameters that are missing.
    /// </summary>
    public const int RequestMissingParameters = 2001;

    /// <summary>
    /// Event ID when a request failed with an item not found.
    /// </summary>
    public const int RequestNotFound = 2003;

    /// <summary>
    /// Event ID when a request succeeded because the item hasn't been changed.
    /// </summary>
    public const int RequestNotModified = 2007;

    /// <summary>
    /// Event ID when a request failed with an unexpected error.
    /// </summary>
    public const int RequestUnexpectedError = 2002;

    /// <summary>
    /// Event ID when a request successfully updates an item.
    /// </summary>
    public const int RequestUpdated = 2005;

    /// <summary>
    /// Event ID when low-impact user profile information is requested.
    /// </summary>
    public const int UserProfileDetailsRequested = 3001;
}
