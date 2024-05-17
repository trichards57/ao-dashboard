// -----------------------------------------------------------------------
// <copyright file="IEmailSender.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;

namespace Dashboard.Services;

/// <summary>
/// Represents a service for sending emails.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends the user an email with a link to confirm their email address.
    /// </summary>
    /// <param name="user">The user to email.</param>
    /// <param name="email">The email address to send to.</param>
    /// <param name="confirmationLink">The confirmation link to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink);

    /// <summary>
    /// SEnds the user an invitation email.
    /// </summary>
    /// <param name="email">The email address to send to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendInviteLinkAsync(string email);

    /// <summary>
    /// Sends the user a password reset link.
    /// </summary>
    /// <param name="user">The user to email.</param>
    /// <param name="email">The email address to send to.</param>
    /// <param name="resetLink">The reset link to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink);
}
