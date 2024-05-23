// -----------------------------------------------------------------------
// <copyright file="VorService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model.Converters;
using Dashboard.Client.Services;
using Dashboard.Data;
using Dashboard.Grpc;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Services;

/// <summary>
/// Service for managing VOR data.
/// </summary>
/// <param name="contextFactory">Factory to create the database context.</param>
internal class VorService(IDbContextFactory<ApplicationDbContext> contextFactory) : IVorService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextFactory = contextFactory;

    /// <inheritdoc/>
    public async Task<GetVorStatisticsResponse> GetVorStatisticsAsync(Place place)
    {
        if (!Enum.IsDefined(place.Region))
        {
            throw new ArgumentOutOfRangeException(nameof(place));
        }

        var context = await contextFactory.CreateDbContextAsync();

        var vehicles = await context.Vehicles
            .GetActive()
            .GetForPlace(place)
            .Select(v => new { v.IsVor })
            .AsNoTracking()
            .ToListAsync();

        var totalVehicles = vehicles.Count;
        var vorVehicles = vehicles.Count(v => v.IsVor);
        var availableVehicles = totalVehicles - vorVehicles;

        var endDate = DateTime.Now.Date;
        var startDate = endDate.AddYears(-1);

        var incidentsLastYear = await context.Incidents
            .GetForPlace(place)
            .Where(i => i.StartDate <= System.DateOnly.FromDateTime(endDate) && i.EndDate >= System.DateOnly.FromDateTime(startDate))
            .Select(i => new { i.StartDate, i.EndDate })
            .AsNoTracking()
            .ToListAsync();

        var dateRange = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                              .Select(offset => startDate.AddDays(offset))
                              .ToList();

        var incidentCountPerDay = new Dictionary<System.DateOnly, int>();

        foreach (var date in dateRange)
        {
            var d = System.DateOnly.FromDateTime(date);
            incidentCountPerDay[d] = totalVehicles - incidentsLastYear.Count(i => i.StartDate <= d && i.EndDate >= d);
        }

        var incidentsByMonth = incidentCountPerDay.GroupBy(i => new { i.Key.Year, i.Key.Month })
            .Select(g => new { Month = g.Key, Count = (int)Math.Round(g.Average(i => i.Value)) })
            .Select(i => new PastAvailabilityEntry { Date = new Grpc.DateOnly { Year = (uint)i.Month.Year, Month = (uint)i.Month.Month, Day = 1 }, AvailableVehicles = (uint)i.Count });

        var res = new GetVorStatisticsResponse
        {
            AvailableVehicles = (uint)availableVehicles,
            TotalVehicles = (uint)totalVehicles,
            VorVehicles = (uint)vorVehicles,
        };
        res.PastAvailability.AddRange(incidentsByMonth);
        return res;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<GetVorStatusResponse> GetVorStatusesAsync(Place place)
    {
        if (!Enum.IsDefined(place.Region))
        {
            throw new ArgumentOutOfRangeException(nameof(place));
        }

        return GetVorStatusesPrivateAsync(place);
    }

    private async IAsyncEnumerable<GetVorStatusResponse> GetVorStatusesPrivateAsync(Place place)
    {
        var context = await contextFactory.CreateDbContextAsync();

        foreach (var v in context.Vehicles
           .GetActive()
           .GetForPlace(place)
           .Select(s => new GetVorStatusResponse
           {
               CallSign = s.CallSign,
               District = s.District,
               DueBack = s.IsVor ? DateOnlyConverter.ToGrpc(s.Incidents.OrderByDescending(i => i.StartDate).First().EstimatedEndDate) : null,
               Hub = s.Hub,
               IsVor = s.IsVor,
               Registration = s.Registration,
               Region = RegionConverter.ToRegion(s.Region),
               Summary = s.IsVor ? s.Incidents.OrderByDescending(i => i.StartDate).First().Description : string.Empty,
               Id = s.Id.ToString(),
           }))
        {
            yield return v;
        }
    }
}