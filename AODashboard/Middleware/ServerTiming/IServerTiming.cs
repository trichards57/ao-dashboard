// -----------------------------------------------------------------------
// <copyright file="IServerTiming.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Represents a service to handle server timing metrics.
/// </summary>
public interface IServerTiming
{
    /// <summary>
    /// Gets the list of server timing metrics.
    /// </summary>
    ICollection<ServerTimingMetric> Metrics { get; }
}
