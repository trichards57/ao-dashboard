// -----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client;

/// <summary>
/// The user information shared between the client and server.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Gets or sets the user's real name.
    /// </summary>
    public required string RealName { get; set; }

    /// <summary>
    /// Gets or sets the user's ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's role.
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// Gets or sets the user's most recent authentication method.
    /// </summary>
    public required string AmrUsed { get; set; }

    /// <summary>
    /// Gets or sets the user's last authenticated date and time.
    /// </summary>
    public required DateTimeOffset? LastAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets the user's other claims.
    /// </summary>
    public Dictionary<string, string> OtherClaims { get; set; }
}
