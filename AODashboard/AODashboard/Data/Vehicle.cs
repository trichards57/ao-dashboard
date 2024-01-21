// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;

namespace AODashboard.Data;

/// <summary>
/// A vehicle with it's reported incidents.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Gets or sets the vehicle's internal ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's call-sign.
    /// </summary>
    public string CallSign { get; set; } = "";

    /// <summary>
    /// Gets or sets the incidents associated with this vehicle.
    /// </summary>
    public List<Incident> Incidents { get; set; } = new List<Incident>();

    /// <summary>
    /// Gets or sets the vehicle's registration.
    /// </summary>
    public string Registration { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's body type.
    /// </summary>
    public string BodyType { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's make.
    /// </summary>
    public string Make { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's model.
    /// </summary>
    public string Model { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is currently on the off-the-road list.
    /// </summary>
    public bool IsVor { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's home district.
    /// </summary>
    public string District { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets the vehicle's home region.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's type.
    /// </summary>
    public VehicleType VehicleType { get; set; }
}
