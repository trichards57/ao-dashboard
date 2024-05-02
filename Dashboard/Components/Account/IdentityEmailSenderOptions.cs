// -----------------------------------------------------------------------
// <copyright file="IdentityEmailSenderOptions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Components.Account;

/// <summary>
/// Options to control the authentication message sender.
/// </summary>
public class IdentityEmailSenderOptions
{
    /// <summary>
    /// Gets or sets the SendGrid API key.
    /// </summary>
    /// <value>
    /// The SendGrid API key.
    /// </value>
    public string? SendGridKey { get; set; }
}
