// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Data;
using Dashboard.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Dashboard.Services;

internal class VehicleService(ApplicationDbContext context) : IVehicleService, Dashboard.Client.Services.IVehicleService
{
    private readonly ApplicationDbContext context = context;
    private readonly string[] disposalMarkings = ["to be sold", "dispose", "disposal"];

    /// <inheritdoc/>
    public async Task AddEntriesAsync(IList<VorIncident> vorIncident)
    {
        using var scope = await context.Database.BeginTransactionAsync();

        var lastUpdate = context.KeyDates.OrderBy(k => k.Id).First();

        var fileDate = vorIncident.Max(i => i.UpdateDate);

        var updateVors = vorIncident.All(i => i.UpdateDate == fileDate) && fileDate >= lastUpdate.LastUpdateFile;

        if (updateVors)
        {
            await context.Vehicles.GetNotDeleted().ExecuteUpdateAsync(u => u.SetProperty(v => v.IsVor, false));
        }

        try
        {
            foreach (var i in vorIncident)
            {
                await AddSingleEntryAsync(i, updateVors);
            }

            if (updateVors)
            {
                lastUpdate.LastUpdateFile = fileDate;
            }

            await scope.CommitAsync();
        }
        catch
        {
            await scope.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place) => context.Vehicles
            .GetNotDeleted()
            .GetForPlace(place)
            .AsNoTracking()
            .Select(s => new VehicleSettings
            {
                CallSign = s.CallSign,
                District = s.District,
                ForDisposal = s.ForDisposal,
                Hub = s.Hub,
                Id = s.Id,
                Registration = s.Registration,
                Region = s.Region,
                VehicleType = s.VehicleType,
            })
            .AsAsyncEnumerable();

    /// <inheritdoc/>
    public Task<VehicleSettings?> GetSettingsAsync(Guid id) => context.Vehicles
            .GetNotDeleted()
            .Where(v => v.Id == id)
            .Select(s => new VehicleSettings
            {
                CallSign = s.CallSign,
                District = s.District,
                ForDisposal = s.ForDisposal,
                Hub = s.Hub,
                Id = s.Id,
                Registration = s.Registration,
                Region = s.Region,
                VehicleType = s.VehicleType,
            })
            .FirstOrDefaultAsync();

    /// <inheritdoc/>
    public async Task PutSettingsAsync(UpdateVehicleSettings settings)
    {
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(s => s.Registration == settings.Registration);

        if (vehicle == null)
        {
            vehicle = new Vehicle();
            context.Vehicles.Add(vehicle);
        }

        vehicle.CallSign = settings.CallSign;
        vehicle.District = settings.District;
        vehicle.ForDisposal = settings.ForDisposal;
        vehicle.Hub = settings.Hub;
        vehicle.Registration = settings.Registration;
        vehicle.Region = settings.Region;
        vehicle.VehicleType = settings.VehicleType;
        vehicle.Deleted = null;
        vehicle.LastModified = DateTimeOffset.UtcNow;
        vehicle.ETag = CalculateEtag(vehicle);
        await context.SaveChangesAsync();
    }

    private static string CalculateEtag(Vehicle vehicle) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(vehicle.ETagIdentifier)));

    private async Task AddSingleEntryAsync(VorIncident vorIncident, bool updateVors)
    {
        var trimmedReg = vorIncident.Registration.ToUpperInvariant().Trim().Replace(" ", "", StringComparison.OrdinalIgnoreCase);
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Registration == trimmedReg);

        if (vehicle == null)
        {
            vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                CallSign = vorIncident.CallSign.ToUpperInvariant().Trim().Replace(" ", "", StringComparison.OrdinalIgnoreCase),
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

        if (updateVors)
        {
            vehicle.IsVor = true;
        }

        if (Array.Exists(disposalMarkings, s => vorIncident.Comments.Contains(s, StringComparison.OrdinalIgnoreCase) || vorIncident.Description.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            vehicle.ForDisposal = true;
        }

        vehicle.LastModified = DateTimeOffset.UtcNow;
        vehicle.ETag = CalculateEtag(vehicle);

        await context.SaveChangesAsync();
    }
}
