using MathCore;
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace System
{
    public static class IProgressExtensions
    {
        /// <summary>Выполнить децимацию вызовов по времени</summary>
        /// <typeparam name="T">Тип данных прогресса</typeparam>
        /// <param name="progress">Исходный прогресс операции</param>
        /// <param name="TimeoutInSeconds">Таймаут между вызовами в секундах</param>
        /// <returns>Информатор прогресса с прореживанием вызовов по времени</returns>
        public static IProgress<T> DecimateByTimeSec<T>(this IProgress<T> progress, double TimeoutInSeconds) => progress.DecimateByTime(TimeSpan.FromSeconds(TimeoutInSeconds));

        /// <summary>Выполнить децимацию вызовов по времени</summary>
        /// <typeparam name="T">Тип данных прогресса</typeparam>
        /// <param name="progress">Исходный прогресс операции</param>
        /// <param name="TimeoutInMilliSeconds">Таймаут между вызовами в миллисекундах</param>
        /// <returns>Информатор прогресса с прореживанием вызовов по времени</returns>
        public static IProgress<T> DecimateByTimeMilliSec<T>(this IProgress<T> progress, double TimeoutInMilliSeconds) => progress.DecimateByTime(TimeSpan.FromMilliseconds(TimeoutInMilliSeconds));

        /// <summary>Выполнить децимацию вызовов по времени</summary>
        /// <typeparam name="T">Тип данных прогресса</typeparam>
        /// <param name="progress">Исходный прогресс операции</param>
        /// <param name="Timeout">Таймаут между вызовами</param>
        /// <returns>Информатор прогресса с прореживанием вызовов по времени</returns>
        public static IProgress<T> DecimateByTime<T>(this IProgress<T> progress, TimeSpan Timeout) => new ProgressTimeoutDecimator<T>(progress, Timeout);

        /// <summary>Выполнить децимацию вызовов по числу вызовов</summary>
        /// <typeparam name="T">Тип данных прогресса</typeparam>
        /// <param name="progress">Исходный прогресс операции</param>
        /// <param name="CallCount">Число пропускаемых вызовов</param>
        /// <returns>Информатор прогресса с прореживанием вызовов по количествоу</returns>
        public static IProgress<T> DecimateByCallCount<T>(this IProgress<T> progress, int CallCount) => new ProgressCallCountDecimator<T>(progress, CallCount);
    }
}
