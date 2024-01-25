// -----------------------------------------------------------------------
// <copyright file="Region.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AODashboard.Client.Model;

/// <summary>
/// Represents the region the vehicle 'belongs' to.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum Region
{
    /// <summary>
    /// An unknown region.
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown = 0,

    /// <summary>
    /// The North East region.
    /// </summary>
    [EnumMember(Value = "ne")]
    NorthEast = 1,

    /// <summary>
    /// The North West region.
    /// </summary>
    [EnumMember(Value = "nw")]
    NorthWest = 2,

    /// <summary>
    /// The East of England region.
    /// </summary>
    [EnumMember(Value = "ee")]
    EastOfEngland = 3,

    /// <summary>
    /// The West Midlands region.
    /// </summary>
    [EnumMember(Value = "wm")]
    WestMidlands = 4,

    /// <summary>
    /// The East Midlands region.
    /// </summary>
    [EnumMember(Value = "em")]
    EastMidlands = 5,

    /// <summary>
    /// The London region.
    /// </summary>
    [EnumMember(Value = "lo")]
    London = 6,

    /// <summary>
    /// The South East region.
    /// </summary>
    [EnumMember(Value = "se")]
    SouthEast = 7,

    /// <summary>
    /// The South West region.
    /// </summary>
    [EnumMember(Value = "sw")]
    SouthWest = 8,
}
