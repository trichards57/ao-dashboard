// -----------------------------------------------------------------------
// <copyright file="IPlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;

namespace AODashboard.Client.Services;

/// <summary>
/// Represents a service for managing places.
/// </summary>
public interface IPlaceService
{
    /// <summary>
    /// Gets all of the district names for a region.
    /// </summary>
    /// <param name="region">The region to look in.</param>
    /// <param name="district">The district to look in.</param>
    /// <returns>The hub names as an immutable list.</returns>
    /// <exception cref="ArgumentException">If <paramref name="region" /> is not a defined value or <paramref name="district" /> is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="region" /> is not a specific region.</exception>
    Task<Places> GetDistrictHubs(Region region, string district);

    /// <summary>
    /// Gets the ETag for the list of District hubs.
    /// </summary>
    /// <param name="region">The region to look in.</param>
    /// <param name="district">The district to look in.</param>
    /// <returns>The ETag as a trimmed base 64 string.</returns>
    Task<string> GetDistrictHubsETag(Region region, string district);

    /// <summary>
    /// Gets all of the district names for a region.
    /// </summary>
    /// <param name="region">The region to look in.</param>
    /// <returns>The district names as an immutable list.</returns>
    /// <exception cref="ArgumentException">If <paramref name="region" /> is not a defined value.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="region" /> is not a specific region.</exception>
    Task<Places> GetDistrictNames(Region region);

    /// <summary>
    /// Gets the ETag for the list of District names.
    /// </summary>
    /// <param name="region">The region to look in.</param>
    /// <returns>The ETag as a trimmed base 64 string.</returns>
    Task<string> GetDistrictNamesETag(Region region);
}
