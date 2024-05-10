// -----------------------------------------------------------------------
// <copyright file="UserInviteSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model;

/// <summary>
/// Represents an invitation for the user.
/// </summary>
public class UserInviteSummary
{
    /// <summary>
    /// Gets or sets the ID of the invitation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address for the invited user.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether the invite has been redeemed.
    /// </summary>
    public bool Redeemed { get; set; }

    /// <summary>
    /// Gets or sets the date the invite was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the user that created the invite.
    /// </summary>
    public string CreatedBy { get; set; } = "";
}
