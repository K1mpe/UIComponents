namespace UIComponents.Defaults;

public static class FormatDefaults
{
    /// <summary>
    /// A function used for converting a timespan in a aproximation of the time passed. This is used by the <see cref="LoggerExtensions.LogFunction(Microsoft.Extensions.Logging.ILogger, string, bool, Action, Microsoft.Extensions.Logging.LogLevel, Microsoft.Extensions.Logging.EventId)"/>
    /// </summary>
    public static Func<TimeSpan, string> FormatTimespan { get; set; } = (timespan) =>
    {
        if(timespan >= TimeSpan.FromDays(2))
            return $"{timespan.Days} days";
        if (timespan >= TimeSpan.FromHours(2))
            return $"{timespan.Hours} hours and {timespan.Minutes} minutes";
        if (timespan >= TimeSpan.FromHours(1))
            return $"{timespan.Hours} hour and {timespan.Minutes} minutes";
        if(timespan >= TimeSpan.FromMinutes(2))
            return $"{timespan.Minutes} minutes and {timespan.Seconds} seconds";
        if (timespan >= TimeSpan.FromMinutes(1))
            return $"{timespan.Minutes} minute and {timespan.Seconds} seconds";
        if (timespan >= TimeSpan.FromMilliseconds(100))
            return $"{timespan.Seconds} seconds";
        return $"{timespan.Milliseconds} milliseconds";

    };

    /// <summary>
    /// This function takes a the filesize and should return a easy to understand string to give a size indication
    /// </summary>
    public static Func<long, string> FormatFileSize { get; set; } = (bytes) =>
    {
        if (bytes == null)
            return string.Empty;

        if (bytes > 100000000000)
            return $"{Math.Round(bytes / 1000000000000f, 3)} TB";
        if (bytes > 100000000)
            return $"{Math.Round(bytes / 1000000000f, 3)} GB";
        if (bytes > 100000)
            return $"{Math.Round(bytes / 1000000f, 3)} MB";
        if (bytes > 100)
            return $"{Math.Round(bytes / 1000f, 3)} KB";
        return $"{bytes} B";
    };
}
