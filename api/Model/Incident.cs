// -----------------------------------------------------------------------
// <copyright file="Incident.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace API.Model;

/// <summary>
/// A VOR incident.
/// </summary>
public readonly record struct Incident
{
    /// <summary>
    /// Gets the comments associated with the incident.
    /// </summary>
    [JsonPropertyName("comments")]
    [JsonProperty("comments")]
    public string Comments { get; init; }

    /// <summary>
    /// Gets the description of the incident.
    /// </summary>
    [JsonPropertyName("description")]
    [JsonProperty("description")]
    public string Description { get; init; }

    /// <summary>
    /// Gets the date the incident has ended.
    /// </summary>
    [JsonPropertyName("endDate")]
    [JsonProperty("endDate")]
    public DateOnly EndDate { get; init; }

    /// <summary>
    /// Gets the date the incident started.
    /// </summary>
    [JsonPropertyName("startDate")]
    [JsonProperty("startDate")]
    public DateOnly StartDate { get; init; }

    /// <summary>
    /// Gets the date the incident was expected to end.
    /// </summary>
    [JsonPropertyName("estimatedEndDate")]
    [JsonProperty("estimatedEndDate")]
    public DateOnly? EstimatedEndDate { get; init; }
}
