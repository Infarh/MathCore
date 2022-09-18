#nullable enable
using System.Text;

// ReSharper disable once CheckNamespace
namespace System;

public static class TimeSpanExtensions
{
    [DST]
    public static string ToShortString(this TimeSpan time)
    {
        var builder = new StringBuilder(50);

        var days = time.Days;
        if(days != 0) 
            builder.Append(days).Append('d');

        var hours = time.Hours;
        if (builder.Length > 0)
            builder.Append(hours.ToString("00"));
        else if(hours > 0) 
            builder.Append(hours);

        var minutes = time.Minutes;
        if(builder.Length > 0)
            builder.Append(minutes.ToString("00"));
        else if (minutes > 0) 
            builder.Append(minutes);

        var seconds = time.Seconds;
        var milliseconds = time.Milliseconds;

        if (builder.Length == 0)
            return seconds == 0 && milliseconds == 0 
                ? "0" 
                : $"{seconds + (double)milliseconds / 1000}";

        builder.Append(':');
        if (seconds < 10)
            builder.Append('0');

        builder.Append(seconds + (double)milliseconds / 1000);

        return builder.ToString();
    }
}