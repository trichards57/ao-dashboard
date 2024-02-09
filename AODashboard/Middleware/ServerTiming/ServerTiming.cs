// -----------------------------------------------------------------------
// <copyright file="ServerTiming.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Class to hold the server timing metrics.
/// </summary>
internal sealed class ServerTiming : IServerTiming
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerTiming"/> class.
    /// </summary>
    public ServerTiming()
    {
        Metrics = new List<ServerTimingMetric>();
    }

    /// <inheritdoc/>
    public ICollection<ServerTimingMetric> Metrics { get; }
}
