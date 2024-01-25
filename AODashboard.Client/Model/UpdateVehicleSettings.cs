// -----------------------------------------------------------------------
// <copyright file="UpdateVehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AODashboard.Client.Model;

/// <summary>
/// Represents an update to the a vehicle's settings.
/// </summary>
public record class UpdateVehicleSettings
{
    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    [JsonPropertyName("reg")]
    [Required]
    public string Registration { get; init; } = "";

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    [JsonPropertyName("hub")]
    [Required]
    public string Hub { get; init; } = "";

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [JsonPropertyName("callSign")]
    [Required]
    public string CallSign { get; init; } = "";

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    [JsonPropertyName("district")]
    [Required]
    public string District { get; init; } = "";

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    [JsonPropertyName("region")]
    [EnumDataType(typeof(Region))]
    [Required]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    [JsonPropertyName("type")]
    [EnumDataType(typeof(VehicleType))]
    [Required]
    public VehicleType VehicleType { get; init; }
}
