// -----------------------------------------------------------------------
// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;

namespace AODashboard.Client.Services;

/// <summary>
/// Represents a service to manage vehicles.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Adds a VOR entry to the database.
    /// </summary>
    /// <param name="vorIncident">The incident to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddEntryAsync(VorIncident vorIncident);

    /// <summary>
    /// Adds multiple VOR entries to the database in a single transaction.
    /// </summary>
    /// <param name="vorIncident">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddEntriesAsync(IEnumerable<VorIncident> vorIncident);

    /// <summary>
    /// Clears the VOR status of all vehicles.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearVorsAsync();

    /// <summary>
    /// Finds the vehicles whose registration or callsign contains the given query.
    /// </summary>
    /// <param name="callSign">The query to search for.</param>
    /// <returns>
    /// An async enumerable containing the matching vehicles.
    /// </returns>
    Task<VehicleSettings?> GetByCallSignAsync(string callSign);

    /// <summary>
    /// Gets the vehicle with the given registration.
    /// </summary>
    /// <param name="registration">The registration to search for.</param>
    /// <returns>
    /// A <see cref="Task" /> representing the asynchronous operation.  Resolves to the matching
    /// vehicle, or <see langword="null"/> if not found.
    /// </returns>
    Task<VehicleSettings?> GetByRegistrationAsync(string registration);

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
    /// Updates the settings for the given vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>Will create the vehicle if it doesn't already exist.</remarks>
    Task UpdateSettingsAsync(UpdateVehicleSettings settings);

    /// <summary>
    /// Gets the VOR status of all vehicles at a place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>The VOR statuses as an asynchronous enumerable.</returns>
    IAsyncEnumerable<VorStatus> GetStatusesByPlace(Region region, string? district, string? hub);

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
    /// Gets the date of the last update to the vehicle with the given registration.
    /// </summary>
    /// <param name="registration">The registration to search for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a last update date for the vehicle.
    /// </returns>
    Task<DateTimeOffset?> GetLastUpdateByRegistration(string registration);

    /// <summary>
    /// Gets the date of the last update to the vehicle with the given call-sign.
    /// </summary>
    /// <param name="callSign">The call-sign to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a last update date for the vehicle.
    /// </returns>
    Task<DateTimeOffset?> GetLastUpdateByCallSignAsync(string callSign);

    /// <summary>
    /// Gets the date of the last update to the vehicles in the given place.
    /// </summary>
    /// <param name="region">The region to check for.</param>
    /// <param name="district">The district to check for.</param>
    /// <param name="hub">The hub to check for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a last update date for the vehicles.
    /// </returns>
    Task<DateTimeOffset?> GetLastUpdateByPlace(Region region, string? district, string? hub);

    /// <summary>
    /// Gets the ETag for the vehicle with the given registration.
    /// </summary>
    /// <param name="registration">The registration to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the vehicle.
    /// </returns>
    Task<string?> GetEtagByRegistrationAsync(string registration);

    /// <summary>
    /// Gets the ETag for the vehicle with the given call-sign.
    /// </summary>
    /// <param name="callSign">The call-sign to look up.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to the ETag for the vehicle.
    /// </returns>
    Task<string?> GetEtagByCallSignAsync(string callSign);
}
