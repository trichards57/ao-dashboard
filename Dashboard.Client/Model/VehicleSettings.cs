﻿// -----------------------------------------------------------------------
// <copyright file="VehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model;

/// <summary>
/// Represents the settings for a vehicle.
/// </summary>
public class VehicleSettings
{
    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    public string CallSign { get; init; } = "";

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    public string District { get; init; } = "";

    /// <summary>
    /// Gets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; init; }

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    public string Hub { get; init; } = "";

    /// <summary>
    /// Gets the internal ID for the vehicle.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    public string Registration { get; init; } = "";

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    public VehicleType VehicleType { get; init; }
}
