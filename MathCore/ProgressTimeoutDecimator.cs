using System;
using System.Diagnostics;

namespace MathCore
{
    /// <summary>Информатор прогресса операции, осуществляющий прореживание вызовов по времени</summary>
    /// <typeparam name="T">Тип данных информатора</typeparam>
    public class ProgressTimeoutDecimator<T> : IProgress<T>
    {
        private readonly IProgress<T> _ProgressSource;
        private readonly TimeSpan _Timeout;

        /// <summary>Таймаут между вызовами</summary>
        public TimeSpan Timeout { get => _Timeout; init => _Timeout = value; }

        public ProgressTimeoutDecimator(IProgress<T> ProgressSource) => _ProgressSource = ProgressSource;

        public ProgressTimeoutDecimator(IProgress<T> ProgressSource, TimeSpan Timeout) : this(ProgressSource) => _Timeout = Timeout;

        private readonly Stopwatch _Timer = Stopwatch.StartNew();
        public void Report(T value)
        {
            if (_Timer.Elapsed < _Timeout) return;
            _Timer.Restart();
            _ProgressSource.Report(value);
        }
    }
}
