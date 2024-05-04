// -----------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1310 // Field names should not contain underscore

namespace Dashboard.Client.Logging;

/// <summary>
/// Event codes for logging.
/// </summary>
public static class EventCode
{
    /// <summary>
    /// The user has requested an email address change.
    /// </summary>
    public const int User_Email_ChangeRequested = 1000;

    /// <summary>
    /// The user has confirmed their email address.
    /// </summary>
    public const int User_Email_Confirmed = 1001;

    /// <summary>
    /// The user has tried to confirm their email address with an invalid code.
    /// </summary>
    public const int User_Email_BadConfirmationCode = 1002;

    /// <summary>
    /// The user has changed their password.
    /// </summary>
    public const int User_Password_Changed = 1010;

    /// <summary>
    /// The user has changed their password.
    /// </summary>
    public const int User_Password_ResetRequested = 1011;

    /// <summary>
    /// The user has updated their profile.
    /// </summary>
    public const int User_Profile_Updated = 1020;

    /// <summary>
    /// The user has disabled two-factor authentication.
    /// </summary>
    public const int User_TwoFactor_Disabled = 1030;

    /// <summary>
    /// The user has enabled two-factor authentication.
    /// </summary>
    public const int User_TwoFactor_Enabled = 1031;

    /// <summary>
    /// The user has requested new two-factor authentication recovery codes.
    /// </summary>
    public const int User_TwoFactor_RequestedRecoveryCodes = 1032;

    /// <summary>
    /// The user has reset their two-factor authentication key.
    /// </summary>
    public const int User_TwoFactor_Reset = 1033;

    /// <summary>
    /// The user has successfully logged in.
    /// </summary>
    public const int User_Login_Success = 1040;

    /// <summary>
    /// The user failed to log in due to an error involving external
    /// authentication.
    /// </summary>
    public const int User_Login_ExternalError = 1041;

    /// <summary>
    /// The user failed to log in due to an error involving external
    /// authentication.
    /// </summary>
    public const int User_Login_BadRequest = 1042;

    /// <summary>
    /// The user failed to log in due to an error involving external
    /// authentication.
    /// </summary>
    public const int User_Login_LockedOut = 1043;

    /// <summary>
    /// A new user account has been created.
    /// </summary>
    public const int User_Login_NewAccount = 1044;
}
