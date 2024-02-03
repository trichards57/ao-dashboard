﻿// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Data;
using AODashboard.Migrations;
using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace AODashboard.Services;

/// <summary>
/// Service to manage vehicles.
/// </summary>
/// <param name="context">The data context to manage.</param>
/// <param name="mapper">The mapper used to convert database types to communication types.</param>
public class VehicleService(ApplicationDbContext context, IMapper mapper) : IVehicleService
{
    /// <inheritdoc/>
    public async Task AddEntriesAsync(IEnumerable<VorIncident> vorIncident)
    {
        using var scope = context.Database.BeginTransaction();

        foreach (var i in vorIncident)
        {
            await AddSingleEntryAsync(i);
        }

        await context.SaveChangesAsync();
        await scope.CommitAsync();
    }

    /// <inheritdoc/>
    public async Task AddEntryAsync(VorIncident vorIncident)
    {
        await AddSingleEntryAsync(vorIncident);

        await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task ClearVorsAsync() => await context.Vehicles.GetNotDeleted().ExecuteUpdateAsync(u => u.SetProperty(v => v.IsVor, false));

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetByCallSignAsync(string callSign)
    {
        ArgumentException.ThrowIfNullOrEmpty(callSign);

        callSign = callSign.Trim().ToUpper();

        return await mapper
            .ProjectTo<VehicleSettings>(context.Vehicles.GetNotDeleted())
            .Where(v => v.CallSign.Equals(callSign))
            .Cast<VehicleSettings?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetByRegistrationAsync(string registration)
    {
        ArgumentException.ThrowIfNullOrEmpty(registration);

        registration = registration.Trim().ToUpper();

        return await mapper
            .ProjectTo<VehicleSettings>(context.Vehicles.GetNotDeleted())
            .Where(v => v.Registration.Equals(registration))
            .Cast<VehicleSettings?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<string?> GetEtagByCallSignAsync(string callSign)
    {
        ArgumentException.ThrowIfNullOrEmpty(callSign);

        callSign = callSign.Trim().ToUpper();

        return await context.Vehicles.GetNotDeleted()
            .Where(v => v.CallSign.Equals(callSign))
            .Select(v => v.ETag)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<string?> GetEtagByRegistrationAsync(string registration)
    {
        ArgumentException.ThrowIfNullOrEmpty(registration);

        registration = registration.Trim().ToUpper();

        return await context.Vehicles.GetNotDeleted()
            .Where(v => v.Registration.Equals(registration))
            .Select(v => v.ETag)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset?> GetLastUpdateByCallSignAsync(string callSign)
    {
        ArgumentException.ThrowIfNullOrEmpty(callSign);

        callSign = callSign.Trim().ToUpper();

        return await context.Vehicles.GetNotDeleted()
            .Where(v => v.CallSign.Equals(callSign))
            .Select(v => v.LastModified)
            .Cast<DateTimeOffset?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset?> GetLastUpdateByPlace(Region region, string? district, string? hub)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentOutOfRangeException(nameof(region));
        }

        var vehicles = context.Vehicles
           .GetNotDeleted()
           .GetForPlace(region, district, hub);

        return await vehicles
            .Select(s => s.LastModified)
            .OrderByDescending(s => s)
            .Cast<DateTimeOffset?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset?> GetLastUpdateByRegistration(string registration)
    {
        ArgumentException.ThrowIfNullOrEmpty(registration);

        registration = registration.Trim().ToUpper();

        return await context.Vehicles.GetNotDeleted()
            .Where(v => v.Registration.Equals(registration))
            .Select(v => v.LastModified)
            .Cast<DateTimeOffset?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<VorStatistics> GetStatisticsByPlace(Region region, string? district, string? hub)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentOutOfRangeException(nameof(region));
        }

        var vehicles = await context.Vehicles
            .GetNotDeleted()
            .GetForPlace(region, district, hub)
            .Select(v => new { v.IsVor })
            .AsNoTracking().ToListAsync();

        var totalVehicles = vehicles.Count;
        var vorVehicles = vehicles.Count(v => v.IsVor);
        var availableVehicles = totalVehicles - vorVehicles;

        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddYears(-1);

        var incidentsLastYear = await context.Incidents
            .GetForPlace(region, district, hub)
            .Where(i => i.StartDate <= DateOnly.FromDateTime(endDate) && i.EndDate >= DateOnly.FromDateTime(startDate))
            .Select(i => new { i.StartDate, i.EndDate })
            .AsNoTracking()
            .ToListAsync();

        var dateRange = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                              .Select(offset => startDate.AddDays(offset))
                              .ToList();

        var incidentCountPerDay = new Dictionary<DateOnly, int>();

        foreach (var date in dateRange)
        {
            var d = DateOnly.FromDateTime(date);
            incidentCountPerDay[d] = totalVehicles - incidentsLastYear.Count(i => i.StartDate <= d && i.EndDate >= d);
        }

        return new VorStatistics
        {
            AvailableVehicles = availableVehicles,
            TotalVehicles = totalVehicles,
            VorVehicles = vorVehicles,
            PastAvailability = incidentCountPerDay,
        };
    }

    /// <inheritdoc/>
    public async Task<string> GetStatisticsEtagByPlace(Region region, string? district, string? hub)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentOutOfRangeException(nameof(region));
        }

        var vehicles = await context.Vehicles
            .GetNotDeleted()
            .GetForPlace(region, district, hub)
            .OrderBy(v => v.Id)
            .Select(v => $"{v.Id}-{v.LastModified:O}")
            .ToListAsync();

        var combinedIds = string.Concat(vehicles);

        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(combinedIds)));
    }

    /// <inheritdoc/>
    public async Task<VorStatus?> GetStatusByCallSignAsync(string callSign)
    {
        var item = await context.Vehicles
            .GetNotDeleted()
            .FirstOrDefaultAsync(i => i.CallSign == callSign);

        if (item == null)
        {
            return null;
        }

        if (item.IsVor)
        {
            var incident = item.Incidents.OrderByDescending(s => s.StartDate).FirstOrDefault();

            return new VorStatus
            {
                IsVor = true,
                DueBack = incident?.EstimatedEndDate,
                Summary = incident?.Description,
            };
        }

        return new VorStatus
        {
            IsVor = false,
        };
    }

    /// <inheritdoc/>
    public Task<VorStatus?> GetStatusByRegistrationAsync(string registration) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IAsyncEnumerable<VorStatus> GetStatusesByPlace(Region region, string? district, string? hub)
    {
        if (!Enum.IsDefined(region))
        {
            throw new ArgumentOutOfRangeException(nameof(region));
        }

        var vehicles = context.Vehicles
           .GetNotDeleted()
           .GetForPlace(region, district, hub);

        return mapper.ProjectTo<VorStatus>(vehicles).AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public async Task UpdateSettingsAsync(UpdateVehicleSettings settings)
    {
        var vehicle = await context.Vehicles.Persist(mapper).InsertOrUpdateAsync(settings);

        vehicle.ETag = CalculateEtag(vehicle);

        await context.SaveChangesAsync();
    }

    private static string CalculateEtag(Vehicle vehicle) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(vehicle.ETagIdentifier)));

    private async Task AddSingleEntryAsync(VorIncident vorIncident)
    {
        var trimmedReg = vorIncident.Registration.ToUpper().Trim().Replace(" ", "");
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Registration == trimmedReg);

        if (vehicle == null)
        {
            vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                CallSign = vorIncident.CallSign.ToUpper().Trim().Replace(" ", ""),
                Registration = trimmedReg,
                BodyType = vorIncident.BodyType,
                Make = vorIncident.Make,
                Model = vorIncident.Model,
            };
            context.Vehicles.Add(vehicle);
        }
        else if (vehicle.Deleted != null)
        {
            vehicle.Deleted = null;
        }

        var incident = await context.Incidents.FirstOrDefaultAsync(i => i.VehicleId == vehicle.Id && i.StartDate == vorIncident.StartDate);

        if (incident == null)
        {
            incident = new Incident
            {
                VehicleId = vehicle.Id,
                StartDate = vorIncident.StartDate,
                Description = vorIncident.Description,
            };
            context.Incidents.Add(incident);
        }

        if (incident.EndDate < vorIncident.UpdateDate)
        {
            incident.EndDate = vorIncident.UpdateDate;
            incident.Description = vorIncident.Description;
            incident.Comments = vorIncident.Comments;
            incident.EstimatedEndDate = vorIncident.EstimatedRepairDate;
        }

        if (vorIncident.UpdateDate == DateOnly.FromDateTime(DateTime.UtcNow))
        {
            vehicle.IsVor = true;
        }

        vehicle.LastModified = DateTimeOffset.UtcNow;
        vehicle.ETag = CalculateEtag(vehicle);
    }
}
