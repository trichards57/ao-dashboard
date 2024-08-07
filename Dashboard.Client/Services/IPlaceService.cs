// -----------------------------------------------------------------------
// <copyright file="IPlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;

namespace Dashboard.Client.Services;

/// <summary>
/// Represents a service for retrieving place information.
/// </summary>
public interface IPlaceService
{
    /// <summary>
    /// Gets the districts for the specified region.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns>The name of the districts.</returns>
    IAsyncEnumerable<string> GetDistricts(Region region);

    /// <summary>
    /// Gets the hubs for the specified region and district.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <param name="district">The district.</param>
    /// <returns>The name of the hubs.</returns>
    IAsyncEnumerable<string> GetHubs(Region region, string district);
}
