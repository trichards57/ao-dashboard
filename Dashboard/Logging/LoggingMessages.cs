using Dashboard.Client.Logging;

namespace Dashboard.Logging;

public static partial class LoggingMessages
{
    [LoggerMessage(EventCode.User_ChangedPassword, LogLevel.Information, "User {id} has changed their password.")]
    public static partial void User_ChangedPassword(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_DisabledTwoFactor, LogLevel.Information, "User {id} has disabled two-factor authentication.")]
    public static partial void User_DisabledTwoFactor(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_EnabledTwoFactor, LogLevel.Information, "User {id} has enabled two-factor authentication.")]
    public static partial void User_EnabledTwoFactor(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_EmailChangeRequested, LogLevel.Information, "User {id} has requested a change to their email address.")]
    public static partial void User_EmailChangeRequested(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_RequestedRecoveryCodes, LogLevel.Information, "User {id} has generated new two-factor authentication codes.")]
    public static partial void User_RequestedRecoveryCodes(this ILogger logger, string id);

    [LoggerMessage(EventCode.User_UpdatedProfile, LogLevel.Information, "User {id} has updated their profile.")]
    public static partial void User_UpdatedProfile(this ILogger logger, string id);
}
