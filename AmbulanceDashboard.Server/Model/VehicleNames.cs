// -----------------------------------------------------------------------
// <copyright file="VehicleNames.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AmbulanceDashboard.Model;

/// <summary>
/// Represents the naming details of a vehicle.
/// </summary>
public readonly record struct VehicleNames
{
    /// <summary>
    /// Gets the vehicle's registration number.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the vehicle's call sign.
    /// </summary>
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the vehicle's ID.
    /// </summary>
    public Guid Id { get; init; }
}
