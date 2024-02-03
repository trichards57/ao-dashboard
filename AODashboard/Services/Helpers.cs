// -----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Data;

namespace AODashboard.Services;

/// <summary>
/// Some helpers to work with the vehicle data.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Gets all of the vehicles that are not deleted.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that aren't deleted.</returns>
    public static IQueryable<Vehicle> GetNotDeleted(this IQueryable<Vehicle> vehicles)
        => vehicles.Where(v => v.Deleted == null);

    /// <summary>
    /// Gets all the vehicles at the specified place.
    /// </summary>
    /// <param name="vehicles">The vehicle items to query.</param>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>All the vehicles in <paramref name="vehicles"/> that are in the given place.</returns>
    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Region region, string? district, string? hub)
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
    /// Gets all the incidents at the specified place.
    /// </summary>
    /// <param name="incidents">The incident items to query.</param>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>All the incidents in <paramref name="incidents"/> that are in the given place.</returns>
    public static IQueryable<Incident> GetForPlace(this IQueryable<Incident> incidents, Region region, string? district, string? hub)
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
}
