// -----------------------------------------------------------------------
// <copyright file="UserInviteRequest.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model;

/// <summary>
/// Represents a request for a user to be invited to the system.
/// </summary>
public class UserInviteRequest
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// Gets or sets the role they should be invited to.
    /// </summary>
    public string RoleId { get; set; } = "";
}
