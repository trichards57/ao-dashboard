// -----------------------------------------------------------------------
// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;

namespace Dashboard.Services;

public interface IVehicleService
{
    /// <summary>
    /// Adds multiple VOR entries to the database in a single transaction.
    /// Also resets the VOR flags if any of the incidents are from today.
    /// </summary>
    /// <param name="vorIncident">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddEntriesAsync(IList<VorIncident> vorIncident);
}
