// -----------------------------------------------------------------------
// <copyright file="RegionConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Dashboard.Client.Model.Converters;

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
    public static Region ToRegion(string region) => region switch
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

    /// <summary>
    /// Converts the gRPC representation of the region.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns>The converted value.</returns>
    public static Region ToRegion(Grpc.Region region) => region switch
    {
        Grpc.Region.All => Region.All,
        Grpc.Region.London => Region.London,
        Grpc.Region.SouthWest => Region.SouthWest,
        Grpc.Region.SouthEast => Region.SouthEast,
        Grpc.Region.WestMidlands => Region.WestMidlands,
        Grpc.Region.EastMidlands => Region.EastMidlands,
        Grpc.Region.EastOfEngland => Region.EastOfEngland,
        Grpc.Region.NorthEast => Region.NorthEast,
        Grpc.Region.NorthWest => Region.NorthWest,
        _ => Region.Unknown,
    };

    /// <summary>
    /// Gets the region as a gRPC region.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns>The converted value.</returns>
    public static Grpc.Region ToRegion(Region region) => region switch
    {
        Region.All => Grpc.Region.All,
        Region.London => Grpc.Region.London,
        Region.SouthWest => Grpc.Region.SouthWest,
        Region.SouthEast => Grpc.Region.SouthEast,
        Region.WestMidlands => Grpc.Region.WestMidlands,
        Region.EastMidlands => Grpc.Region.EastMidlands,
        Region.EastOfEngland => Grpc.Region.EastOfEngland,
        Region.NorthEast => Grpc.Region.NorthEast,
        Region.NorthWest => Grpc.Region.NorthWest,
        _ => Grpc.Region.Undefined,
    };
}
