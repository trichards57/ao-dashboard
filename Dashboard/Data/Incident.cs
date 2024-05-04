// -----------------------------------------------------------------------
// <copyright file="Incident.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Dashboard.Data;

/// <summary>
/// An incident resulting in the vehicle becoming VOR.
/// </summary>
internal sealed class Incident
{
    /// <summary>
    /// Gets or sets the comments associated with the incident.
    /// </summary>
    public string Comments { get; set; } = "";

    /// <summary>
    /// Gets or sets the description of the incident.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets the date the incident ended.
    /// </summary>
    /// <remarks>
    /// This will only be correct if the incident has actually ended.
    /// </remarks>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the date the incident was expected to end.
    /// </summary>
    public DateOnly? EstimatedEndDate { get; set; }

    /// <summary>
    /// Gets or sets the ID of the incident.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date the incident started.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the vehicle the incident is associated with.
    /// </summary>
    [NotNull]
    public Vehicle? Vehicle { get; set; }

    /// <summary>
    /// Gets or sets the ID of the vehicle the incident is associated with.
    /// </summary>
    public Guid VehicleId { get; set; }
}
