// -----------------------------------------------------------------------
// <copyright file="IdentityEmailSender.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Dashboard.Components.Account;

/// <summary>
/// Service to handle sending Identity related emails.
/// </summary>
internal sealed class IdentityEmailSender(IOptions<IdentityEmailSenderOptions> optionsAccessor, ILogger<IdentityEmailSender> logger) : IEmailSender<ApplicationUser>
{
    private readonly IdentityEmailSenderOptions options = optionsAccessor.Value;
    private readonly ILogger<IdentityEmailSender> logger = logger;

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
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        Console.WriteLine(resetCode);

        var emailContent = @$"<p>Dear {user.RealName},<p>
<p>You have requested a code to reset your password. This is it:<p>
<p>{resetCode}</p>
<p>Kind regards,</p>
<p>Tony</p>";

        return SendEmailAsync(email, "Reset your password", emailContent);
    }

    /// <inheritdoc/>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        Console.WriteLine(resetLink);

        var emailContent = @$"<p>Dear {user.RealName},<p>
<p>You have requested a link to reset your password. Please click the link below to confirm your email address.<p>
<p><a href=""{resetLink}"">Confirm your email</a></p>
<p>If that doesn't work, copy and paste the following link into your browser:</p>
<p>{resetLink}</p>
<p>Kind regards,</p>
<p>Tony</p>";

        return SendEmailAsync(email, "Reset your password", emailContent);
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
