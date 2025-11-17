#nullable enable

// ReSharper disable once CheckNamespace

using System.Runtime.InteropServices;

namespace System;

/// <summary>Класс методов-расширений для даты-времени <see cref="DateTime"/></summary>
public static class DateTimeExtensions
{
    /// <summary>Преобразование структуры <see cref="DateTime"/> в массив байт</summary>
    /// <param name="Time">Дата/время</param>
    /// <returns>Массив</returns>
    public static byte[] ToBytArray(this DateTime Time) => BitConverter.GetBytes(Time.ToBinary());

    /// <summary>Копирование значения структуры даты-времени в массив байт с заданным смещением</summary>
    /// <param name="Time">Структура даты-времени</param>
    /// <param name="Data">Массив байт, куда требуется записать значение</param>
    /// <param name="Offset">Смещение в массиве байт</param>
    public static void ToByteArray(this DateTime Time, byte[] Data, int Offset = 0)
    {
        if (Offset < 0) throw new ArgumentOutOfRangeException(nameof(Offset), Offset, "Смещение в массиве не может быть меньше нуля");
        if (Data.Length - Offset < 8) throw new InvalidOperationException("Процесс копирования данных выходит за пределы массива");

#if NET8_0_OR_GREATER
        MemoryMarshal.Cast<DateTime, byte>(new(ref Time)).CopyTo(Data.AsSpan(Offset));
#else
        long[] data = [Time.ToBinary()];
        Buffer.BlockCopy(data, 0, Data, Offset, 8);
#endif
    }

    /// <summary>Преобразование массива байт в <see cref="DateTime"/></summary>
    /// <param name="Data">Массив байт</param>
    /// <param name="Offset">Смещение в массиве</param>
    /// <returns>Структура <see cref="DateTime"/></returns>
    public static DateTime FromBytArray(byte[] Data, int Offset = 0)
    {
        if (Offset < 0) throw new ArgumentOutOfRangeException(nameof(Offset), Offset, "Смещение в массиве не может быть меньше нуля");
        if (Data.Length - Offset < 8) throw new InvalidOperationException("Процесс копирования данных выходит за пределы массива");

#if NET8_0_OR_GREATER
        return Data.AsSpan(Offset).Cast<DateTime>()[0];
#else
        return DateTime.FromBinary(BitConverter.ToInt64(Data, Offset));
#endif
    }

    extension(DateTime)
    {
        public static DateTime NowMSK => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        public static DateTime NowUTC => DateTime.UtcNow;
    }

    public static int GetCount(this DateTime FromTime, DateTime ToTime, DayOfWeek Day)
    {
        if (FromTime > ToTime) return 0;

        var count = 0;
        for (var dt = FromTime; dt <= ToTime; dt = dt.AddDays(1))
            if (dt.DayOfWeek == Day)
                count++;
        return count;
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


#if NET8_0_OR_GREATER
    public static int GetCount(this DateOnly FromDate, DateOnly ToDate, DayOfWeek Day)
    {
        if (FromDate > ToDate) return 0;
        var count = 0;
        for (var dt = FromDate; dt <= ToDate; dt = dt.AddDays(1))
            if (dt.DayOfWeek == Day)
                count++;
        return count;
    }
#endif
}
