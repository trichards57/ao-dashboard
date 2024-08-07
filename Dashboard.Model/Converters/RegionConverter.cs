// -----------------------------------------------------------------------
// <copyright file="RegionConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model.Converters;

/// <summary>
/// Converts a <see cref="Region"/> to a string and back.
/// </summary>
public static class RegionConverter
{
    /// <summary>
    /// Gets the string representation of the region.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns>The shortened string.</returns>
    public static string ToString(Region region) => region switch
    {
        Region.All => "all",
        Region.London => "lon",
        Region.SouthWest => "sw",
        Region.SouthEast => "se",
        Region.WestMidlands => "wm",
        Region.EastMidlands => "em",
        Region.EastOfEngland => "ee",
        Region.NorthEast => "ne",
        Region.NorthWest => "nw",
        _ => "unk",
    };

    /// <summary>
    /// Gets the display string for the region.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns>The display string.</returns>
    public static string ToDisplayString(Region region) => region switch
    {
        Region.All => "All",
        Region.London => "London",
        Region.SouthWest => "South West",
        Region.SouthEast => "South East",
        Region.WestMidlands => "West Midlands",
        Region.EastMidlands => "East Midlands",
        Region.EastOfEngland => "East of England",
        Region.NorthEast => "North East",
        Region.NorthWest => "North West",
        _ => "Unknown",
    };

    /// <summary>
    /// Converts a string to a region.
    /// </summary>
    /// <param name="region">The string.</param>
    /// <returns>The region.</returns>
    public static Region ToRegion(string region) => region.ToLower() switch
    {
        "all" => Region.All,
        "lon" => Region.London,
        "sw" => Region.SouthWest,
        "se" => Region.SouthEast,
        "wm" => Region.WestMidlands,
        "em" => Region.EastMidlands,
        "ee" => Region.EastOfEngland,
        "ne" => Region.NorthEast,
        "nw" => Region.NorthWest,
        _ => Region.Unknown,
    };
}
