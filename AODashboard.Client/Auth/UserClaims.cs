// -----------------------------------------------------------------------
// <copyright file="UserClaims.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Client.Auth;

/// <summary>
/// Constants representing the user claims.
/// </summary>
public static class UserClaims
{
    /// <summary>
    /// Gets the value when the user has permission to read or edit.
    /// </summary>
    public static string Edit => "Edit";

    /// <summary>
    /// Gets the claim for the user's permissions permission level.
    /// </summary>
    public static string Permissions => "Permissions";

    /// <summary>
    /// Gets the value when the user has permission to read.
    /// </summary>
    public static string Read => "Read";

    /// <summary>
    /// Gets the claim for the user's sensitive permissions permission level.
    /// </summary>
    public static string SensitivePermissions => "SensitivePermissions";

    /// <summary>
    /// Gets the claim for the user's vehicle configuration permission level.
    /// </summary>
    public static string VehicleConfiguration => "VehicleConfiguration";

    /// <summary>
    /// Gets the claim for the user's VOR Data permission level.
    /// </summary>
    public static string VorData => "VORData";
}
