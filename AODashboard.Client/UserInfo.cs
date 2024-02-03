// -----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;

namespace AODashboard.Client;

/// <summary>
/// Represents the user information passed by the server to the client.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Gets or sets the user's ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's other claims not covered by the other properties.
    /// </summary>
    public required IEnumerable<Claim> OtherClaims { get; set; }
}
