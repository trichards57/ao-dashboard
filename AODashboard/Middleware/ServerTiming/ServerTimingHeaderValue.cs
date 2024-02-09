// -----------------------------------------------------------------------
// <copyright file="ServerTimingHeaderValue.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Represents a value of the Server-Timing header.
/// </summary>
internal sealed class ServerTimingHeaderValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerTimingHeaderValue"/> class.
    /// </summary>
    public ServerTimingHeaderValue()
    {
        Metrics = new List<ServerTimingMetric>();
    }

    /// <summary>
    /// Gets the list of server timing metrics to include in the header.
    /// </summary>
    public ICollection<ServerTimingMetric> Metrics { get; }

    /// <inheritdoc/>
    public override string ToString() => string.Join(",", Metrics);
}
