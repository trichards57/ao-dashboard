// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Data;
using Dashboard.Model;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Services;

/// <summary>
/// Service to manage places.
/// </summary>
/// <param name="contextFactory">Factory to create the database context.</param>
internal class PlaceService(IDbContextFactory<ApplicationDbContext> contextFactory) : IPlaceService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextFactory = contextFactory;

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetDistricts(Region region)
    {
        var context = await contextFactory.CreateDbContextAsync();

        foreach (var d in context.Vehicles
            .Where(c => c.Region == region && !c.Deleted.HasValue)
            .Select(c => c.District).Distinct())
        {
            yield return d;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetHubs(Region region, string district)
    {
        var context = await contextFactory.CreateDbContextAsync();

        foreach (var h in context.Vehicles
            .Where(c => c.Region == region && c.District == district && !c.Deleted.HasValue)
            .Select(c => c.Hub).Distinct())
        {
            yield return h;
        }
    }
}
