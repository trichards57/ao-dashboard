// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace AODashboard.Services;

/// <summary>
/// Service to access information about places.
/// </summary>
/// <param name="context">The data context to access.</param>
public class PlaceService(ApplicationDbContext context) : IPlaceService
{
    /// <inheritdoc/>
    public async Task<Places> GetDistrictHubs(Region region, string district)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentException($"Region {region} is not defined.", nameof(region));
        }

        if (region == Region.All || region == Region.Unknown)
        {
            throw new ArgumentOutOfRangeException(nameof(region), "Region must be a specific region.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(district);

        var districts = await context.Vehicles
            .GetNotDeleted()
            .Where(p => p.Region == region && p.District == district)
            .Select(p => p.Hub)
            .Distinct()
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .OrderBy(d => d)
            .ToListAsync();

        return new Places { Names = districts.ToImmutableList() };
    }

    /// <inheritdoc/>
    public async Task<Places> GetDistrictNames(Region region)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentException($"Region {region} is not defined.", nameof(region));
        }

        if (region == Region.All || region == Region.Unknown)
        {
            throw new ArgumentOutOfRangeException(nameof(region), "Region must be a specific region.");
        }

        var districts = await context.Vehicles
            .GetNotDeleted()
            .Where(p => p.Region == region)
            .Select(p => p.District)
            .Distinct()
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .OrderBy(d => d)
            .ToListAsync();

        return new Places { Names = districts.ToImmutableList() };
    }
}
