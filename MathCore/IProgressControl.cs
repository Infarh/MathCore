#nullable enable
using System;
using System.Threading;

namespace MathCore;

/// <summary>Контроль над асинхронной операцией</summary>
/// <typeparam name="T">Тип значения прогресса</typeparam>
public interface IProgressControl<in T>
{
    /// <summary>Прогресс операции</summary>
    IProgress<T>? Progress { get; }

    /// <summary>Флаг отмены операции</summary>
    CancellationToken Cancel { get; }
}

/// <summary>Контроль над асинхронной операцией</summary>
public interface IProgressControl : IProgressControl<double> { }

/// <summary>Методы-расширения над контролем асинхронной операцией</summary>
public static class ProgressControlEx
{
    /// <summary>Извещение о ходе прогресса выполнения асинхронной операции</summary>
    /// <typeparam name="T">Тип значения прогресса</typeparam>
    /// <param name="Control">Объект контроля асинхронной операции</param>
    /// <param name="Value">Значение прогресса</param>
    public static void Report<T>(this IProgressControl<T>? Control, T Value) => Control?.Progress?.Report(Value);

    /// <summary>Проверка прерывания асинхронной операции</summary>
    /// <typeparam name="T">Тип значения прогресса</typeparam>
    /// <param name="Control">Объект контроля асинхронной операции</param>
    /// <exception cref="OperationCanceledException">Возникает если было затребовано прерывание операции</exception>
    public static void ThrowIfCancellationRequested<T>(this IProgressControl<T>? Control) => Control?.Cancel.ThrowIfCancellationRequested();

    /// <summary>Сформировать объект контроля асинхронной операции</summary>
    /// <typeparam name="T">Тип значения прогресса</typeparam>
    /// <param name="Progress">Прогресс операции</param>
    /// <param name="Cancel">Флаг отмены операции</param>
    /// <returns>Возвращает объект, упаковывающий внутри себя систему извещения о прогрессе и отмене операции</returns>
    public static IProgressControl<T> GetControl<T>(this IProgress<T>? Progress, CancellationToken Cancel = default) => new ProgressControl<T>(Progress, Cancel);

    /// <summary>Сформировать объект контроля асинхронной операции</summary>
    /// <param name="Progress">Прогресс операции</param>
    /// <param name="Cancel">Флаг отмены операции</param>
    /// <returns>Возвращает объект, упаковывающий внутри себя систему извещения о прогрессе и отмене операции</returns>
    public static IProgressControl GetControl(this IProgress<double>? Progress, CancellationToken Cancel = default) => new ProgressControl(Progress, Cancel);
}

/// <summary>Реализация интерфейса контроля за асинхронной операцией</summary>
/// <typeparam name="T">Тип значения прогресса</typeparam>
public class ProgressControl<T> : IProgressControl<T>
{
    public ProgressControl(IProgress<T>? Progress, CancellationToken Cancel)
    {
        this.Progress = Progress;
        this.Cancel = Cancel;
    }

    public IProgress<T>? Progress { get; init; }

    public CancellationToken Cancel { get; init; }
}

/// <summary>Реализация интерфейса контроля за асинхронной операцией</summary>
public class ProgressControl : ProgressControl<double>, IProgressControl
{
    public ProgressControl(IProgress<double>? Progress, CancellationToken Cancel) : base(Progress, Cancel) { }
}