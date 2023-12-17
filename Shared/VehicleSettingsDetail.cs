// -----------------------------------------------------------------------
// <copyright file="VehicleSettingsDetail.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Shared;

public readonly record struct VehicleSettingsDetail
{
    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    [JsonPropertyName("reg")]
    public string Registration { get; init; }

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [JsonPropertyName("callSign")]
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    [JsonPropertyName("district")]
    public string District { get; init; }

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    [JsonPropertyName("region")]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    [JsonPropertyName("type")]
    public VehicleType Type { get; init; }
}
