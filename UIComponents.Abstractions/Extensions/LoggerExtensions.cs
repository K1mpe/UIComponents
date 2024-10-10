using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UIComponents.Abstractions.Extensions;

public static class LoggerExtensions
{
    /// <summary>
    /// Begin the scope with key value pairs
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="kvps"></param>
    /// <returns></returns>
    public static IDisposable BeginScopeKvp(this ILogger logger, params KeyValuePair<string, object>[] kvps)
    {
        if (logger == null)
            return default;
        return logger.BeginScope(new Dictionary<string, object>(kvps));
    }

    /// <summary>
    /// Begin a the scope by adding a single key with value
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IDisposable BeginScopeKvp(this ILogger logger, string key, object value) => BeginScopeKvp(logger, new KeyValuePair<string, object>(key, value));
    

    /// <summary>
    /// This method will start a log when before executing the action and afterwards.
    /// This will log when a exception occures but still throws the exceptions for more details
    /// </summary>
    /// <remarks>
    /// Starting <paramref name="name"/>...
    /// <br></br> Finished <paramref name="name"/>
    /// <br></br> Finished <paramref name="name"/> in ..ms
    /// <br></br> Failed <paramref name="name"/>!
    /// </remarks>
    public static void LogFunction(this ILogger logger, string name, bool logTime, Action action, LogLevel logLevel = LogLevel.Debug, EventId eventId = default)
    {
        try
        {
            if (logger == null)
                action();
            if (logTime)
            {
                var stopwatch = Stopwatch.StartNew();
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                action();
                stopwatch.Stop();
                var elapsed = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms";
                if (stopwatch.ElapsedMilliseconds > 10)
                    elapsed = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} sec";

                logger.Log(logLevel, eventId, "Finished {0} in {1}", name, elapsed);

            }
            else
            {
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                action();
                logger.Log(logLevel, eventId, "Finished {0}", name);
            }

        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, eventId, "Failed {0}!", name);
            throw;
        }
    }


    /// <inheritdoc cref="LogFunction(ILogger, string, bool, Action, LogLevel, EventId)"/>
    public static T LogFunction<T>(this ILogger logger, string name, bool logTime, Func<T> function, LogLevel logLevel = LogLevel.Debug, EventId eventId = default)
    {
        try
        {
            if (logger == null)
                return function();
            if (logTime)
            {
                var stopwatch = Stopwatch.StartNew();
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                var result = function();
                stopwatch.Stop();
                var elapsed = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms";
                if (stopwatch.ElapsedMilliseconds > 10)
                    elapsed = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} sec";

                logger.Log(logLevel, eventId, "Finished {0} in {1}", name, elapsed);
                return result;
            }
            else
            {
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                var result = function();
                logger.Log(logLevel, eventId, "Finished {0}", name);
                return result;
            }
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, eventId, "Failed {0}!", name);
            throw;
        }
    }

    /// <inheritdoc cref="LogFunction(ILogger, string, bool, Action, LogLevel, EventId)"/>
    public static async Task LogFunction(this ILogger logger, string name, bool logTime, Func<Task> function, LogLevel logLevel = LogLevel.Debug, EventId eventId = default)
    {
        try
        {
            if (logger == null)
            {
                await function();
                return;
            }
            if (logTime)
            {
                var stopwatch = Stopwatch.StartNew();
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                await function();
                stopwatch.Stop();
                var elapsed = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms";
                if (stopwatch.ElapsedMilliseconds > 10)
                    elapsed = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} sec";

                logger.Log(logLevel, eventId, "Finished {0} in {1}", name, elapsed);

            }
            else
            {
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                await function();
                logger.Log(logLevel, eventId, "Finished {0}", name);
            }

        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, eventId, "Failed {0}!", name);
            throw;
        }
    }

    /// <inheritdoc cref="LogFunction(ILogger, string, bool, Action, LogLevel, EventId)"/>
    public static async Task<T> LogFunction<T>(this ILogger logger, string name, bool logTime, Func<Task<T>> function, LogLevel logLevel = LogLevel.Debug, EventId eventId = default)
    {
        try
        {
            if(logger == null)
                return await function();
            if (logTime)
            {
                var stopwatch = Stopwatch.StartNew();
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                var result = await function();
                stopwatch.Stop();
                var elapsed = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms";
                if (stopwatch.ElapsedMilliseconds > 10)
                    elapsed = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} sec";

                logger.Log(logLevel, eventId, "Finished {0} in {1}", name, elapsed);
                return result;
            }
            else
            {
                logger.Log(logLevel, eventId, "Starting {0}...", name);
                var result = await function();
                logger.Log(logLevel, eventId, "Finished {0}", name);
                return result;
            }

        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, eventId, "Failed {0}!", name);
            throw;
        }
    }
}
