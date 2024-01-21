﻿// -----------------------------------------------------------------------
// <copyright file="PermissionsHelpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;

namespace AODashboard.Client;

/// <summary>
/// Helper to set up common permissions behaviour on server and client.
/// </summary>
public static class PermissionsHelpers
{
    /// <summary>
    /// Adds the standard permissions policies to the authorization options.
    /// </summary>
    /// <param name="options">The options to add the policies to.</param>
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy("CanEditVehicles", policy => policy.RequireClaim("VehicleConfiguration", "Edit"));
    }
}
