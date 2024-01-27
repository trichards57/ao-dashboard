// -----------------------------------------------------------------------
// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AODashboard.Client.Services;
using AODashboard.Data;
using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AODashboard.Services;

/// <summary>
/// Service to manage vehicles.
/// </summary>
/// <param name="context">The data context to manage.</param>
/// <param name="mapper">The mapper used to convert database types to communication types.</param>
public class VehicleService(ApplicationDbContext context, IMapper mapper) : IVehicleService
{
    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetByCallSignAsync(string callSign)
    {
        ArgumentException.ThrowIfNullOrEmpty(callSign);

        callSign = callSign.Trim().ToUpper();

        return await mapper
            .ProjectTo<VehicleSettings>(context.Vehicles)
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
            .ProjectTo<VehicleSettings>(context.Vehicles)
            .Where(v => v.Registration.Equals(registration))
            .Cast<VehicleSettings?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateSettingsAsync(UpdateVehicleSettings settings)
    {
        try
        {
            await context.Vehicles.Persist(mapper).InsertOrUpdateAsync(settings);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("CallSign") == true)
            {
                throw new DuplicateCallSignException($"The call-sign {settings.CallSign} is already in use.", ex);
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task ClearVorsAsync() => await context.Vehicles.ExecuteUpdateAsync(u => u.SetProperty(v => v.IsVor, false));

    /// <inheritdoc/>
    public async Task AddEntryAsync(VorIncident vorIncident)
    {
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Registration == vorIncident.Registration);

        if (vehicle == null)
        {
            vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                CallSign = vorIncident.CallSign.ToUpper().Trim().Replace(" ", ""),
                Registration = vorIncident.Registration.ToUpper().Trim().Replace(" ", ""),
                BodyType = vorIncident.BodyType,
                Make = vorIncident.Make,
                Model = vorIncident.Model,
            };
            context.Vehicles.Add(vehicle);
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

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601 && sqlEx.Message.Contains("CallSign"))
        {
            throw new DuplicateCallSignException($"The call-sign {vehicle.CallSign} is already in use by a different call-sign.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<VorStatus?> GetStatusByCallSignAsync(string callSign)
    {
        var item = await context.Vehicles.FirstOrDefaultAsync(i => i.CallSign == callSign);

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
}

/// <summary>
/// Represents an error that occurs when a call-sign that is already being used is entered into the system.
/// </summary>
public sealed class DuplicateCallSignException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateCallSignException"/> class.
    /// </summary>
    public DuplicateCallSignException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateCallSignException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DuplicateCallSignException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateCallSignException"/> class with a
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public DuplicateCallSignException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
