// -----------------------------------------------------------------------
// <copyright file="UserInviteCancel.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model;

/// <summary>
/// Represents a request to cancel a user's invite.
/// </summary>
public class UserInviteCancel
{
    /// <summary>
    /// Gets or sets the email address of the invite to cancel.
    /// </summary>
    public string Email { get; set; } = "";
}
