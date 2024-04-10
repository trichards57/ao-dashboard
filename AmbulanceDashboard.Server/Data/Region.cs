﻿// -----------------------------------------------------------------------
// <copyright file="Region.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AmbulanceDashboard.Data;

/// <summary>
/// Represents the region the vehicle 'belongs' to.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum Region
{
    /// <summary>
    /// An unknown region.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The North East region.
    /// </summary>
    NorthEast = 1,

    /// <summary>
    /// The North West region.
    /// </summary>
    NorthWest = 2,

    /// <summary>
    /// The East of England region.
    /// </summary>
    EastOfEngland = 3,

    /// <summary>
    /// The West Midlands region.
    /// </summary>
    WestMidlands = 4,

    /// <summary>
    /// The East Midlands region.
    /// </summary>
    EastMidlands = 5,

    /// <summary>
    /// The London region.
    /// </summary>
    London = 6,

    /// <summary>
    /// The South East region.
    /// </summary>
    SouthEast = 7,

    /// <summary>
    /// The South West region.
    /// </summary>
    SouthWest = 8,

    /// <summary>
    /// The South West region.
    /// </summary>
    All = 9,
}
