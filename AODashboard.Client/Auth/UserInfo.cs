// -----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Client.Auth;

/// <summary>
/// Represents the user information passed by the server to the client.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Gets the user's claims.
    /// </summary>
    public Dictionary<string, string> Claims { get; init; } = [];

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the user's ID.
    /// </summary>
    public required string UserId { get; set; }
}
