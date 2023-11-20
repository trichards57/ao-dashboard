// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace VorReceiver.Model;

/// <summary>
/// A vehicle with it's reported VOR incidents.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Gets or sets the radio call sign for the vehicle.
    /// </summary>
    [JsonProperty("callSign")]
    public string CallSign { get; set; }

    /// <summary>
    /// Gets the item's partition key.
    /// </summary>
    [JsonProperty("partition")]
    public string Partition => "VOR";

    /// <summary>
    /// Gets or sets the incidents associated with the vehicle.
    /// </summary>
    [JsonProperty("incidents")]
    public List<Incident> Incidents { get; set; } = new List<Incident>();

    /// <summary>
    /// Gets or sets the registration of the vehicle.
    /// </summary>
    [JsonProperty("id")]
    public string Registration { get; set; }

    /// <summary>
    /// Gets or sets the database entrie's ETAG.
    /// </summary>
    [JsonProperty("_etag")]
    public string Etag { get; set; }

    /// <summary>
    /// Gets or sets the body type of the vehicle.
    /// </summary>
    [JsonProperty("body")]
    public string BodyType { get; set; }

    /// <summary>
    /// Gets or sets the make of the vehicle.
    /// </summary>
    [JsonProperty("make")]
    public string Make { get; set; }

    /// <summary>
    /// Gets or sets the model of the vehicle.
    /// </summary>
    [JsonProperty("model")]
    public string Model { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is currently off-the-road.
    /// </summary>
    [JsonProperty("isVor")]
    public bool IsVor { get; set; }

    /// <summary>
    /// Gets or sets the owning district.
    /// </summary>
    [JsonProperty("district", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue("Unknown")]
    public string Distict { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets the owning region.
    /// </summary>
    [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue("Unknown")]
    public Region Region { get; set; } = Region.Unknown;

    /// <summary>
    /// Gets or sets the vehicle type.
    /// </summary>
    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(VehicleType.Other)]
    public VehicleType VehicleType { get; set; } = VehicleType.Other;
}
