// -----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Dashboard.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Dashboard.Services;

/// <summary>
/// Some helpers to work with the vehicle data.
/// </summary>
internal static class Helpers
{
    /// <summary>
    /// Gets all of the vehicles that haven't been deleted or marked for disposal.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that aren't deleted or marked for disposal.</returns>
    public static IQueryable<Vehicle> GetActive(this IQueryable<Vehicle> vehicles)
        => vehicles.GetNotDeleted().Where(v => !v.ForDisposal);

    /// <summary>
    /// Gets the ETag string for the provided vehicles.
    /// </summary>
    /// <param name="vehicles">The vehicles to calculate the ETag for.</param>
    /// <param name="extraTag">An optional extra tag to add to the ETag.</param>
    /// <returns>The ETag as a string.</returns>
    public static async Task<string> GetEtagStringAsync(this IQueryable<Vehicle> vehicles, string? extraTag = null)
    {
        var strings = await vehicles
            .OrderBy(v => v.Id)
            .Select(v => $"{v.Id}-{v.LastModified:O}")
            .ToListAsync();

        var combinedIds = string.Concat(strings);

        if (!string.IsNullOrWhiteSpace(extraTag))
        {
            combinedIds = extraTag + combinedIds;
        }

        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(combinedIds))).Trim('=');
    }

    /// <summary>
    /// Gets all the vehicles at the specified place.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that are in the given place.</returns>
    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Region region, string? district = null, string? hub = null)
    {
        if (region == Region.All)
        {
            return vehicles;
        }

        var filteredVehicles = vehicles
            .Where(v => v.Region == region);

        if (!string.IsNullOrWhiteSpace(district))
        {
            filteredVehicles = filteredVehicles.Where(v => v.District == district);

            if (!string.IsNullOrWhiteSpace(hub))
            {
                filteredVehicles = filteredVehicles.Where(v => v.Hub == hub);
            }
        }

        return filteredVehicles;
    }

    /// <summary>
    /// Gets all the vehicles at the specified place.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <param name="place">The place to check for.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that are in the given place.</returns>
    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(vehicles, place.Region, district, hub);
    }

    /// <summary>
    /// Gets all the incidents at the specified place.
    /// </summary>
    /// <param name="incidents">The incident items to query.</param>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>All the incidents in <paramref name="incidents"/> that are in the given place.</returns>
    public static IQueryable<Incident> GetForPlace(this IQueryable<Incident> incidents, Region region, string? district = null, string? hub = null)
    {
        if (region == Region.All)
        {
            return incidents;
        }

        var filteredIncidents = incidents
            .Where(v => v.Vehicle.Region == region);

        if (!string.IsNullOrWhiteSpace(district))
        {
            filteredIncidents = filteredIncidents.Where(v => v.Vehicle.District == district);

            if (!string.IsNullOrWhiteSpace(hub))
            {
                filteredIncidents = filteredIncidents.Where(v => v.Vehicle.Hub == hub);
            }
        }

        return filteredIncidents;
    }

    /// <summary>
    /// Gets all the incidents at the specified place.
    /// </summary>
    /// <param name="incidents">The incident items to query.</param>
    /// <param name="place">The place to check for.</param>
    /// <returns>All the incidents in <paramref name="incidents"/> that are in the given place.</returns>
    public static IQueryable<Incident> GetForPlace(this IQueryable<Incident> incidents, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(incidents, place.Region, district, hub);
    }

    /// <summary>
    /// Gets all of the vehicles that are not deleted.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that aren't deleted.</returns>
    public static IQueryable<Vehicle> GetNotDeleted(this IQueryable<Vehicle> vehicles)
        => vehicles.Where(v => v.Deleted == null);
}
