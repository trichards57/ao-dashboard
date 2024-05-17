// -----------------------------------------------------------------------
// <copyright file="IdentityEmailSender.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Dashboard.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Dashboard.Components.Account;

/// <summary>
/// Service to handle sending Identity related emails.
/// </summary>
internal sealed class IdentityEmailSender(IOptions<IdentityEmailSenderOptions> optionsAccessor) : IEmailSender
{
    private readonly IdentityEmailSenderOptions options = optionsAccessor.Value;

    /// <inheritdoc/>
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        Console.WriteLine(confirmationLink);

        var emailContent = @$"<p>Dear {user.RealName},<p>
<p>You have signed up for the Ambulance Operations dashboard. Please click the link below to confirm your email address.<p>
<p><a href=""{confirmationLink}"">Confirm your email</a></p>
<p>If that doesn't work, copy and paste the following link into your browser:</p>
<p>{confirmationLink}</p>
<p>Kind regards,</p>
<p>Tony</p>";

        return SendEmailAsync(email, "Confirm your email", emailContent);
    }

    /// <inheritdoc/>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var emailContent = @$"<p>Dear {user.RealName},<p>
<p>You have requested a link to reset your password. Please click the link below to confirm your email address.<p>
<p><a href=""{resetLink}"">Confirm your email</a></p>
<p>If that doesn't work, copy and paste the following link into your browser:</p>
<p>{resetLink}</p>
<p>Kind regards,</p>
<p>Tony</p>";

        return SendEmailAsync(email, "Reset your password", emailContent);
    }

    /// <inheritdoc/>
    public Task SendInviteLinkAsync(string email)
    {
        var emailContent = @$"<p>Hi,</p>
<p>You have been invited to the Ambulance Operations dashboard. Please click the link below to set up your account.</p>
<p><a href=""{options.RegisterLink}"">Set up your account</a></p>
<p>If that doesn't work, copy and paste the following link into your browser:</p>
<p>{options.RegisterLink}</p>
<p>Be sure to use this email address when you register.</p>
<p>Kind regards,</p>
<p>Tony</p>";

        return SendEmailAsync(email, "You have been invited to the Ambulance Operations dashboard", emailContent);
    }

    private async Task SendEmailAsync(string email, string subject, string htmlContent)
    {
        var client = new SendGridClient(options.SendGridKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("no-reply@crewlog.me.uk", "AO Dashboard - No Reply"),
            Subject = subject,
            HtmlContent = htmlContent,
        };
        msg.AddTo(new EmailAddress(email));
        msg.SetClickTracking(false, false);
        await client.SendEmailAsync(msg);
    }
}
