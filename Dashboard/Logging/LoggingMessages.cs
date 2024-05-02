using Dashboard.Client.Logging;

namespace Dashboard.Logging;

public static partial class LoggingMessages
{
    [LoggerMessage(EventCode.User_ChangedPassword, LogLevel.Information, "User {id} has changed their password.")]
    public static partial void User_ChangedPassword(this ILogger logger, string id);
}
