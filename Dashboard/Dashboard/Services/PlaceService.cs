// -----------------------------------------------------------------------
// <copyright file="PlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Services;

internal class PlaceService(ApplicationDbContext context) : IPlaceService
{
    private readonly ApplicationDbContext context = context;

    public IAsyncEnumerable<string> GetDistricts(Region region) => context.Vehicles
            .Where(c => c.Region == region && !c.Deleted.HasValue)
            .Select(c => c.District).Distinct()
            .AsAsyncEnumerable();

    public IAsyncEnumerable<string> GetHubs(Region region, string district) => context.Vehicles
            .Where(c => c.Region == region && c.District == district && !c.Deleted.HasValue)
            .Select(c => c.District).Distinct()
            .AsAsyncEnumerable();
}
