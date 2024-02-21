// -----------------------------------------------------------------------
// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Data;
using AmbulanceDashboard.Model;
using AODashboard.Client.Model;

namespace AmbulanceDashboard.Services;

/// <summary>
/// Represents a service to manage vehicles.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Adds multiple VOR entries to the database in a single transaction.
    /// Also resets the VOR flags if any of the incidents are from today.
    /// </summary>
    /// <param name="vorIncident">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddEntriesAsync(IList<VorIncident> vorIncident);

    /// <summary>
    /// Gets the vehicle with the given ID.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the the vehicle, or null if not found.
    /// </returns>
    Task<VehicleSettings?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets the ETag for the vehicle with the given call-sign.
    /// </summary>
    /// <param name="callSign">The call-sign to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the vehicle.
    /// </returns>
    Task<string> GetEtagByCallSignAsync(string callSign);

    /// <summary>
    /// Gets teh ETAg for the vehicle with the given ID.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the vehicle.
    /// </returns>
    Task<string> GetEtagByIdAsync(Guid id);

    /// <summary>
    /// Gets the ETag for the vehicle with the given registration.
    /// </summary>
    /// <param name="registration">The registration to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the vehicle.
    /// </returns>
    Task<string> GetEtagByRegistrationAsync(string registration);

    /// <summary>
    /// Gets the names of all the vehicles that have not been deleted.
    /// </summary>
    /// <returns>
    /// The names of all the vehicles.
    /// </returns>
    IAsyncEnumerable<VehicleNames> GetNamesAsync();

    /// <summary>
    /// Gets the ETag for the list of vehicle names.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the list.
    /// </returns>
    Task<string> GetNamesEtagAsync();

    /// <summary>
    /// Gets the VOR statistics for a place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the VOR statistics.</returns>
    Task<VorStatistics> GetStatisticsByPlace(Region region, string? district, string? hub);

    /// <summary>
    /// Gets the ETAG for the VOR statistics for a place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the VOR statistics ETag.</returns>
    Task<string> GetStatisticsEtagByPlace(Region region, string? district, string? hub);

    /// <summary>
    /// Gets the VOR status of the provided call-sign.
    /// </summary>
    /// <param name="callSign">The call-sign to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a summary of the vehicle's status.
    /// </returns>
    Task<VorStatus?> GetStatusByCallSignAsync(string callSign);

    /// <summary>
    /// Gets the VOR status of the vehicle with the provided registration.
    /// </summary>
    /// <param name="registration">The registration to search for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a summary of the vehicle's status.
    /// </returns>
    Task<VorStatus?> GetStatusByRegistrationAsync(string registration);

    /// <summary>
    /// Gets the VOR status of all vehicles at a place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>The VOR statuses as an asynchronous enumerable.</returns>
    IAsyncEnumerable<VorStatus> GetStatusesByPlace(Region region, string? district, string? hub);

    /// <summary>
    /// Gets the ETag for the VOR statuses in the given place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the query.
    /// </returns>
    Task<string> GetStatusesEtagByPlace(Region region, string? district, string? hub);

    /// <summary>
    /// Updates the settings for the given vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>Will create the vehicle if it doesn't already exist.</remarks>
    Task UpdateSettingsAsync(UpdateVehicleSettings settings);
}
