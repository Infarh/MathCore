
// ReSharper disable once CheckNamespace

namespace System;

public static class DateTimeOffsetExtensions
{
    extension(DateTimeOffset)
    {
        public static DateTimeOffset NowMSK => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        public static DateTimeOffset NowUTC => DateTimeOffset.UtcNow;
    }

    public static int GetCount(this DateTimeOffset FromTime, DateTimeOffset ToTime, DayOfWeek Day)
    {
        if (FromTime > ToTime) return 0;

        var count = 0;
        for (var dt = FromTime; dt <= ToTime; dt = dt.AddDays(1))
            if (dt.DayOfWeek == Day)
                count++;
        return count;
    }
}