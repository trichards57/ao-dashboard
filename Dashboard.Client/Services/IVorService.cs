﻿// -----------------------------------------------------------------------
// <copyright file="IVorService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;

namespace Dashboard.Client.Services;

/// <summary>
/// Represents a service for getting VOR statuses and statistics.
/// </summary>
public interface IVorService
{
    /// <summary>
    /// Gets the statistics for the VORs in a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>
    /// The VOR statistics for the place.
    /// </returns>
    Task<VorStatistics?> GetVorStatisticsAsync(Place place);

    /// <summary>
    /// Gets the VOR statuses for the vehicles in a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>
    /// The VOR statuses for the vehicles in the place.
    /// </returns>
    IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place);
}
