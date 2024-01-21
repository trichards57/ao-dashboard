// -----------------------------------------------------------------------
// <copyright file="VorStatus.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Client.Model;

/// <summary>
/// Represents the current VOR Status of a vehicle.
/// </summary>
public readonly record struct VorStatus
{
    /// <summary>
    /// Gets the summary of the vehicle's VOR incident.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is currently marked VOR.
    /// </summary>
    public bool IsVor { get; init; }

    /// <summary>
    /// Gets the date the vehicle is expected back.
    /// </summary>
    public DateOnly? DueBack { get; init; }
}
