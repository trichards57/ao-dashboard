// -----------------------------------------------------------------------
// <copyright file="Place.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Client.Model;

/// <summary>
/// Represents a selected place.
/// </summary>
public readonly record struct Place
{
    /// <summary>
    /// Gets the region for the place.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the district for the place.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the hub for the place.
    /// </summary>
    public string Hub { get; init; }
}
