// -----------------------------------------------------------------------
// <copyright file="UserPermissions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AmbulanceDashboard.Model;

/// <summary>
/// Represents the permissions held by the user.
/// </summary>
public readonly record struct UserPermissions
{
    /// <summary>
    /// Gets the user's ID.
    /// </summary>
    /// <remarks>
    /// This will be their SJA email address.
    /// </remarks>
    public string UserId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user can view vehicle information.
    /// </summary>
    public bool CanViewVehicles { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user can edit vehicle information.
    /// </summary>
    public bool CanEditVehicles { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user can view place data.
    /// </summary>
    public bool CanViewPlaces { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user can edit VOR data.
    /// </summary>
    public bool CanEditVOR { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user can view VOR data.
    /// </summary>
    public bool CanViewVOR { get; init; }
}