// -----------------------------------------------------------------------
// <copyright file="ServerTimingMetric.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace AODashboard.Middleware.ServerTiming;

/// <summary>
/// Represents a metric in the Server-Timing header.
/// </summary>
public readonly record struct ServerTimingMetric
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerTimingMetric"/> struct.
    /// </summary>
    /// <param name="name">The name of the metric.</param>
    /// <param name="value">The value of the metric.</param>
    /// <param name="description">The description of the metric.</param>
    public ServerTimingMetric(string name, decimal? value = null, string description = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        Value = value;
        Description = description ?? "";
    }

    /// <summary>
    /// Gets the name of the metric.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value of the metric.
    /// </summary>
    public decimal? Value { get; }

    /// <summary>
    /// Gets the description of the metric.
    /// </summary>
    public string Description { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var res = Name;

        if (Value.HasValue)
        {
            res = res + ";dur=" + Value.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrEmpty(Description))
        {
            res = res + ";desc=\"" + Description + "\"";
        }

        return res;
    }
}
