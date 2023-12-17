// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace API.Model;

/// <summary>
/// A vehicle with it's reported VOR incidents.
/// </summary>
public readonly record struct Vehicle
{
    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [JsonPropertyName("callSign")]
    [JsonProperty("callSign")]
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the item's partition key.
    /// </summary>
    [JsonPropertyName("partition")]
    [JsonProperty("partition")]
    public string Partition => "VOR";

    /// <summary>
    /// Gets the incidents associated with the vehicle.
    /// </summary>
    [JsonPropertyName("incidents")]
    [JsonProperty("incidents")]
    public ImmutableList<Incident> Incidents { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonProperty("id")]
    public string Registration { get; init; }

    /// <summary>
    /// Gets the database entries ETAG.
    /// </summary>
    [JsonPropertyName("_etag")]
    [JsonProperty("_etag")]
    public string Etag { get; init; }

    /// <summary>
    /// Gets the body type of the vehicle.
    /// </summary>
    [JsonPropertyName("body")]
    [JsonProperty("body")]
    public string BodyType { get; init; }

    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    [JsonPropertyName("make")]
    [JsonProperty("make")]
    public string Make { get; init; }

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    [JsonPropertyName("model")]
    [JsonProperty("model")]
    public string Model { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is currently off-the-road.
    /// </summary>
    [JsonPropertyName("isVor")]
    [JsonProperty("isVor")]
    public bool IsVor { get; init; }

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    [JsonProperty("district", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [JsonPropertyName("district")]
    [DefaultValue("Unknown")]
    public string District { get; init; }

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [JsonPropertyName("region")]
    [DefaultValue(Region.Unknown)]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [JsonPropertyName("type")]
    [DefaultValue(VehicleType.Other)]
    public VehicleType VehicleType { get; init; }
}
