#nullable enable

// ReSharper disable once CheckNamespace
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
        if(Offset < 0) throw new ArgumentOutOfRangeException(nameof(Offset), Offset, "Смещение в массиве не может быть меньше нуля");
        if(Data.Length - Offset < 8) throw new InvalidOperationException("Процесс копирования данных выходит за пределы массива");
        long[] data = { Time.ToBinary() };
        Buffer.BlockCopy(data, 0, Data, Offset, 8);
    }

    /// <summary>Преобразование массива байт в <see cref="DateTime"/></summary>
    /// <param name="Data">Массив байт</param>
    /// <param name="Offset">Смещение в массиве</param>
    /// <returns>Структура <see cref="DateTime"/></returns>
    public static DateTime FromBytArray(byte[] Data, int Offset = 0)
    {
        if (Offset < 0) throw new ArgumentOutOfRangeException(nameof(Offset), Offset, "Смещение в массиве не может быть меньше нуля");
        if (Data.Length - Offset < 8) throw new InvalidOperationException("Процесс копирования данных выходит за пределы массива");
        return DateTime.FromBinary(BitConverter.ToInt64(Data, Offset));
    }
}