// -----------------------------------------------------------------------
// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;

namespace Dashboard.Client.Services;

/// <summary>
/// Represents a service for getting and updating vehicle settings.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Gets the settings for the vehicles in a given place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{T}"/> of <see cref="VehicleSettings"/>.
    /// </returns>
    IAsyncEnumerable<VehicleSettings?> GetSettingsAsync(Place place);

    /// <summary>
    /// Gets the settings for a specific vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>
    /// The <see cref="VehicleSettings"/> for the vehicle.
    /// </returns>
    Task<VehicleSettings?> GetSettingsAsync(Guid id);

    /// <summary>
    /// Updates the settings for a vehicle.
    /// </summary>
    /// <param name="settings">The new vehicle settings.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// Matches the vehicles using the vehicle registration, not the ID.
    /// </remarks>
    Task PutSettingsAsync(UpdateVehicleSettings settings);
}
