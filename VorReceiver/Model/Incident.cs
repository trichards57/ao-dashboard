// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace VorReceiver.Model;

/// <summary>
/// A VOR incident.
/// </summary>
public class Incident
{
    /// <summary>
    /// Gets or sets the comments associated with the incident.
    /// </summary>
    [JsonProperty("comments")]
    public string Comments { get; set; }

    /// <summary>
    /// Gets or sets the description of the incident.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the date the incident has ended.
    /// </summary>
    [JsonProperty("endDate")]
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the date the incident started.
    /// </summary>
    [JsonProperty("startDate")]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date the incident was expected to end.
    /// </summary>
    [JsonProperty("estimatedEndDate")]
    public DateOnly? EstimatedEndDate { get; set; }
}
