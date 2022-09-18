#nullable enable
using Microsoft.Extensions.Logging;

namespace ConsoleTest.Extensions;

public static class LoggerEx
{
    public static void Trace<T>(this ILogger logger, string Message, T arg)
    {
        if(logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace(Message, arg);
    }

    public static void Trace<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace(Message, arg1, arg2);
    }

    public static void Trace<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace(Message, arg1, arg2, arg3);
    }

    public static void Debug<T>(this ILogger logger, string Message, T arg)
    {
        if(logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(Message, arg);
    }

    public static void Debug<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(Message, arg1, arg2);
    }

    public static void Debug<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(Message, arg1, arg2, arg3);
    }

    public static void Information<T>(this ILogger logger, string Message, T? arg)
    {
        if(logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(Message, arg);
    }

    public static void Information<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(Message, arg1, arg2);
    }

    public static void Information<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(Message, arg1, arg2, arg3);
    }

    public static void Warning<T>(this ILogger logger, string Message, T? arg)
    {
        if(logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(Message, arg);
    }

    public static void Warning<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(Message, arg1, arg2);
    }

    public static void Warning<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(Message, arg1, arg2, arg3);
    }

    public static void Error<T>(this ILogger logger, string Message, T arg)
    {
        if(logger.IsEnabled(LogLevel.Error))
            logger.LogError(Message, arg);
    }

    public static void Error<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Error))
            logger.LogError(Message, arg1, arg2);
    }

    public static void Error<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Error))
            logger.LogError(Message, arg1, arg2, arg3);
    }

    public static void Critical<T>(this ILogger logger, string Message, T arg)
    {
        if(logger.IsEnabled(LogLevel.Critical))
            logger.LogCritical(Message, arg);
    }

    public static void Critical<T1, T2>(this ILogger logger, string Message, T1? arg1, T2? arg2)
    {
        if(logger.IsEnabled(LogLevel.Critical))
            logger.LogCritical(Message, arg1, arg2);
    }

    public static void Critical<T1, T2, T3>(this ILogger logger, string Message, T1? arg1, T2? arg2, T3? arg3)
    {
        if(logger.IsEnabled(LogLevel.Critical))
            logger.LogCritical(Message, arg1, arg2, arg3);
    }
}