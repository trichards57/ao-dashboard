// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model.Converters;
using Dashboard.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Dashboard.Services;

/// <summary>
/// A service to manage vehicles.
/// </summary>
internal class VehicleService(ApplicationDbContext context) : IVehicleService, Client.Services.IVehicleService
{
    private readonly ApplicationDbContext context = context;
    private readonly string[] disposalMarkings = ["to be sold", "dispose", "disposal"];

    /// <inheritdoc/>
    public async Task AddEntriesAsync(IList<Grpc.VorIncident> vorIncident)
    {
        using var scope = await context.Database.BeginTransactionAsync();

        var lastUpdate = context.KeyDates.OrderBy(k => k.Id).First();

        var fileDate = vorIncident.Max(i => DateOnlyConverter.ToData(i.UpdateDate));

        var updateVors = vorIncident.All(i => DateOnlyConverter.ToData(i.UpdateDate) == fileDate) && fileDate >= lastUpdate.LastUpdateFile;

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
    public IAsyncEnumerable<Grpc.Vehicle> GetSettingsAsync(Grpc.Place place) => context.Vehicles
            .GetNotDeleted()
            .GetForPlace(place)
            .AsNoTracking()
            .Select(s => new Grpc.Vehicle
            {
                CallSign = s.CallSign,
                District = s.District,
                ForDisposal = s.ForDisposal,
                Hub = s.Hub,
                Id = s.Id.ToString(),
                Registration = s.Registration,
                Region = RegionConverter.ToRegion(s.Region),
                VehicleType = VehicleTypeConverter.ToGrpc(s.VehicleType),
            })
            .AsAsyncEnumerable();

    /// <inheritdoc/>
    public Task<Grpc.Vehicle?> GetSettingsAsync(Guid id) => context.Vehicles
            .GetNotDeleted()
            .Where(v => v.Id == id)
            .Select(s => new Grpc.Vehicle
            {
                CallSign = s.CallSign,
                District = s.District,
                ForDisposal = s.ForDisposal,
                Hub = s.Hub,
                Id = s.Id.ToString(),
                Registration = s.Registration,
                Region = RegionConverter.ToRegion(s.Region),
                VehicleType = VehicleTypeConverter.ToGrpc(s.VehicleType),
            })
            .FirstOrDefaultAsync();

    /// <inheritdoc/>
    public async Task<bool> PutSettingsAsync(Grpc.UpdateVehiclesRequest settings)
    {
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(s => s.Registration == settings.Registration);

        if (vehicle == null)
        {
            vehicle = new Vehicle();
            context.Vehicles.Add(vehicle);
        }

        if (settings.HasCallSign)
        {
            vehicle.CallSign = settings.CallSign;
        }

        if (settings.HasDistrict)
        {
            vehicle.District = settings.District;
        }

        if (settings.HasForDisposal)
        {
            vehicle.ForDisposal = settings.ForDisposal;
        }

        if (settings.HasHub)
        {
            vehicle.Hub = settings.Hub;
        }

        if (settings.HasRegion)
        {
            vehicle.Region = RegionConverter.ToRegion(settings.Region);
        }

        if (settings.HasVehicleType)
        {
            vehicle.VehicleType = VehicleTypeConverter.ToData(settings.VehicleType);
        }

        vehicle.Deleted = null;
        vehicle.LastModified = DateTimeOffset.UtcNow;
        vehicle.ETag = CalculateEtag(vehicle);
        await context.SaveChangesAsync();

        return true;
    }

    private static string CalculateEtag(Vehicle vehicle) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(vehicle.ETagIdentifier)));

    private async Task AddSingleEntryAsync(Grpc.VorIncident vorIncident, bool updateVors)
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

        var incident = await context.Incidents.FirstOrDefaultAsync(i => i.VehicleId == vehicle.Id && i.StartDate == DateOnlyConverter.ToData(vorIncident.StartDate));

        if (incident == null)
        {
            incident = new Incident
            {
                VehicleId = vehicle.Id,
                StartDate = DateOnlyConverter.ToData(vorIncident.StartDate),
                Description = vorIncident.Description,
            };
            context.Incidents.Add(incident);
        }

        if (incident.EndDate < DateOnlyConverter.ToData(vorIncident.UpdateDate))
        {
            incident.EndDate = DateOnlyConverter.ToData(vorIncident.UpdateDate);
            incident.Description = vorIncident.Description;
            incident.Comments = vorIncident.Comments;
            incident.EstimatedEndDate = DateOnlyConverter.ToData(vorIncident.EstimatedRepairDate);
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
