using System.Diagnostics;

namespace MathCore;

/// <summary>Структура таймера, размещаемая на стеке. Служит для измерения времени.</summary>
public readonly ref struct StopWatchValue
{
    private static readonly double __TimestampToTicks = 10000000.0 / Stopwatch.Frequency;
    private readonly long _StartTimestamp;

    /// <summary>Структура может быть использована для определения времени</summary>
    public bool IsActive => (ulong)_StartTimestamp > 0UL;

    /// <summary>Инициализация структуры с захватом текущего времени</summary>
    /// <param name="StartTimestamp"></param>
    private StopWatchValue(long StartTimestamp) => _StartTimestamp = StartTimestamp;

    /// <summary>Запуск измерения (захват текущего времени)</summary>
    /// <returns>Запущенный таймер</returns>
    public static StopWatchValue StartNew() => new(Stopwatch.GetTimestamp());

    /// <summary>Прошедшее время</summary>
    /// <returns>Интервал времени, прошедший с момента создания структуры</returns>
    public TimeSpan GetElapsedTime() =>
        IsActive
            ? new TimeSpan((long)(__TimestampToTicks * (Stopwatch.GetTimestamp() - _StartTimestamp)))
            : throw new InvalidOperationException("Тип структуры не инициализирован, или используется значение по умолчанию default(StopWatchValue), данный экземпляр StopWatchValue не может быть использован для определения промежутка времени.");
}