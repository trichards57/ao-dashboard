// -----------------------------------------------------------------------
// <copyright file="VorStatusResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace VorReceiver.Model;

/// <summary>
/// Represents the current status of a vehicle.
/// </summary>
[JsonConverter(typeof(VorStatusResultConverter))]
public readonly record struct VorStatusResult
{
    /// <summary>
    /// Gets a value indicating whether the vehicle is currently off-the-road.
    /// </summary>
    [JsonProperty("isVor")]
    public bool IsVor { get; init; }

    /// <summary>
    /// Gets a value indicating the day is expected to return.
    /// </summary>
    [JsonProperty("dueBack", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateOnly? DueBack { get; init; }

    /// <summary>
    /// Gets the summary of the most recent VOR incident.
    /// </summary>
    [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Summary { get; init; }
}
