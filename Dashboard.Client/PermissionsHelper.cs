// -----------------------------------------------------------------------
// <copyright file="PermissionsHelper.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;

namespace Dashboard.Client;

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
    /// Gets the claim for the user's VOR Data permission level.
    /// </summary>
    public static string Permissions => "Permissions";

    /// <summary>
    /// Gets the value when the user has permission to read.
    /// </summary>
    public static string Read => "Read";

    /// <summary>
    /// Gets the claim for the user's vehicle configuration permission level.
    /// </summary>
    public static string VehicleConfiguration => "VehicleConfiguration";

    /// <summary>
    /// Gets the claim for the user's VOR Data permission level.
    /// </summary>
    public static string VorData => "VORData";
}

/// <summary>
/// Helper to set up common permissions behaviour on server and client.
/// </summary>
public static class PermissionsHelper
{
    /// <summary>
    /// Adds the standard permissions policies to the authorization options.
    /// </summary>
    /// <param name="options">The options to add the policies to.</param>
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy("CanEditVehicles", policy => policy.RequireClaim(UserClaims.VehicleConfiguration, UserClaims.Edit));
        options.AddPolicy("CanEditVOR", policy => policy.RequireClaim(UserClaims.VorData, UserClaims.Edit));
        options.AddPolicy("CanViewVOR", policy => policy.RequireClaim(UserClaims.VorData, UserClaims.Read, UserClaims.Edit));
        options.AddPolicy("CanViewUsers", policy => policy.RequireClaim(UserClaims.Permissions, UserClaims.Read, UserClaims.Edit));
        options.AddPolicy("CanEditUsers", policy => policy.RequireClaim(UserClaims.Permissions, UserClaims.Edit));
        options.AddPolicy("CanEditRoles", policy => policy.RequireRole("Administrator"));
    }
}
