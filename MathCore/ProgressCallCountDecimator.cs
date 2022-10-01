#nullable enable
using System;
using System.Threading;

namespace MathCore;

/// <summary>Информатор прогресса операции, осуществляющий прореживание вызовов к информатору по числу вызовов</summary>
/// <typeparam name="T">Тип данных информатора</typeparam>
public class ProgressCallCountDecimator<T> : IProgress<T>
{
    private readonly IProgress<T> _ProgressSource;
    private readonly int _CallCount = 10;

    /// <summary>Число пропускаемых вызовов</summary>
    public int CallCount { get => _CallCount; init => _CallCount = value; }

    public ProgressCallCountDecimator(IProgress<T> ProgressSource) => _ProgressSource = ProgressSource;

    public ProgressCallCountDecimator(IProgress<T> ProgressSource, int CallCount) : this(ProgressSource) => _CallCount = CallCount;

    private volatile int _Counter;
    public void Report(T value)
    {
        if (Interlocked.Increment(ref _Counter) < _CallCount) return;
        while (Interlocked.Exchange(ref _Counter, 0) != 0) Thread.SpinWait(100);
        _ProgressSource.Report(value);
    }
}