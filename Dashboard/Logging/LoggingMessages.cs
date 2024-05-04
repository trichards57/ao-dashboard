// -----------------------------------------------------------------------
// <copyright file="LoggingMessages.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Logging;

namespace Dashboard.Logging;

/// <summary>
/// Strongly-typed logging messages.
/// </summary>
public static partial class LoggingMessages
{
    [LoggerMessage(EventCode.User_Email_BadConfirmationCode, LogLevel.Warning, "User {id} provided an invalid email confirmation code.")]
    public static partial void User_Email_BadConfirmationCode(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Email_ChangeRequested, LogLevel.Information, "User {id} has requested a change to their email address.")]
    public static partial void User_Email_ChangeRequested(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Email_Confirmed, LogLevel.Information, "User {id} confirmed their email address.")]
    public static partial void User_Email_Confirmed(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Password_Changed, LogLevel.Information, "User {id} has changed their password.")]
    public static partial void User_Password_Changed(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Password_ResetRequested, LogLevel.Information, "User {id} has asked for their password to be reset.")]
    public static partial void User_Password_ResetRequested(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Profile_Updated, LogLevel.Information, "User {id} has updated their profile.")]
    public static partial void User_Profile_Updated(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_TwoFactor_Reset, LogLevel.Information, "User {id} reset their two-factor authentication key.")]
    public static partial void User_ResetTwoFactor(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_TwoFactor_Disabled, LogLevel.Information, "User {id} has disabled two-factor authentication.")]
    public static partial void User_TwoFactor_Disabled(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_TwoFactor_Enabled, LogLevel.Information, "User {id} has enabled two-factor authentication.")]
    public static partial void User_TwoFactor_Enabled(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_TwoFactor_RequestedRecoveryCodes, LogLevel.Information, "User {id} has generated new two-factor authentication codes.")]
    public static partial void User_TwoFactor_RequestedRecoveryCodes(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Login_Success, LogLevel.Information, "User {id} has logged in.")]
    public static partial void User_Login_Success(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Login_ExternalError, LogLevel.Error, "User log in has failed due to an external error : {reason}.")]
    public static partial void User_Login_ExternalError(this ILogger logger, string reason);

    [LoggerMessage(EventCode.User_Login_BadRequest, LogLevel.Warning, "User log in has failed due to an incorrect username or password.")]
    public static partial void User_Login_BadRequest(this ILogger logger);

    [LoggerMessage(EventCode.User_Login_LockedOut, LogLevel.Warning, "User {id} log in has failed because the account is locked out.")]
    public static partial void User_Login_LockedOut(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_Login_NewAccount, LogLevel.Warning, "User {id} log has been created.")]
    public static partial void User_Login_NewAccount(this ILogger logger, string id);
}
