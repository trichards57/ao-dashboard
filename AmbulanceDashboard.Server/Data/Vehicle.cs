// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceDashboard.Data;

/// <summary>
/// A vehicle with it's reported incidents.
/// </summary>
internal sealed class Vehicle
{
    /// <summary>
    /// Gets or sets the vehicle's body type.
    /// </summary>
    public string BodyType { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's call-sign.
    /// </summary>
    public string CallSign { get; set; } = "";

    /// <summary>
    /// Gets or sets the date and time the vehicle was deleted.
    /// </summary>
    public DateTimeOffset? Deleted { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's home district.
    /// </summary>
    public string District { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets the ETag for the vehicle.
    /// </summary>
    [StringLength(44)]
    public string ETag { get; set; } = "";

    /// <summary>
    /// Gets the identifier used to calculate eTags.
    /// </summary>
    [NotMapped]
    public string ETagIdentifier => $"{Id}-{LastModified:O}";

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's home hub.
    /// </summary>
    public string Hub { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets the vehicle's internal ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the incidents associated with this vehicle.
    /// </summary>
    public List<Incident> Incidents { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is currently on the off-the-road list.
    /// </summary>
    public bool IsVor { get; set; }

    /// <summary>
    /// Gets or sets the date and time the vehicle was last modified.
    /// </summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's make.
    /// </summary>
    public string Make { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's model.
    /// </summary>
    public string Model { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's home region.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's registration.
    /// </summary>
    [MaxLength(7)]
    public string Registration { get; set; } = "";

    /// <summary>
    /// Gets or sets the vehicle's type.
    /// </summary>
    public VehicleType VehicleType { get; set; }
}
