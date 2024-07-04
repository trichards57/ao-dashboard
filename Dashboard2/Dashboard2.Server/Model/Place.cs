// -----------------------------------------------------------------------
// <copyright file="Place.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model;

/// <summary>
/// Represents a place.
/// </summary>
public class Place
{
    /// <summary>
    /// Gets or sets the name of the district the place is in.
    /// </summary>
    public string District { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of the hub the place is in.
    /// </summary>
    public string Hub { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of the regin the place is in.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Converts the place to a query string.
    /// </summary>
    /// <returns>The place as a query string.</returns>
    public string CreateQuery()
    {
        if (Region == Region.All)
        {
            return string.Empty;
        }

        if (District.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return $"?region={Region}";
        }

        if (Hub.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return $"?region={Region}&district={District}";
        }

        return $"?region={Region}&district={District}&hub={Hub}";
    }
}