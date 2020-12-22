using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using MathCore;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс объектов, выполняющих некоторое циклическое действие в отдельном фоновом потоке</summary>
    public abstract class Processor : INotifyPropertyChanged, IDisposable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Объект-исключение, передаваемое в качестве параметра события ошибки при рассинхронизации потока процессора</summary>
        private static readonly Exception __AsyncException = new ApplicationException("Рассинхронизация обработки");

        /// <summary>Текущее время системы</summary>
        protected static DateTime Now { [DST] get => DateTime.Now; }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Событие изменения свойства объекта</summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized), DST]
            add => _PropertyChanged += value;
            [MethodImpl(MethodImplOptions.Synchronized), DST]
            remove => _PropertyChanged -= value;
        }
        private event PropertyChangedEventHandler _PropertyChanged;
        /// <summary>Вызов события изменения свойства объекта</summary>
        ///  <param name="e">Параметры события изменения свойства объекта, содержащие имя свойства</param>
        [DST]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => _PropertyChanged?.Invoke(this, e);

        /// <summary>Вызов события изменения свойства объекта с указанием имени свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        [DST, NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));

        /// <summary>Событие изменения свойства активности процессора</summary>
        public event EventHandler EnableChanged;
        /// <summary>Источник события изменения свойства активности процессора</summary><param name="e">Параметры события</param>
        [DST]
        protected virtual void OnEnableChanged(EventArgs e) => EnableChanged?.Invoke(this, e);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Событие запуска процессора</summary>
        public event EventHandler ProcessStarted;
        /// <summary>Источник события запуска процессора</summary><param name="e">Параметры события</param>
        [DST]
        protected virtual void OnProcessStarted(EventArgs e) => ProcessStarted?.Invoke(this, e);

        /// <summary>Событие завершения работы процессора</summary>
        public event EventHandler ProcessCompleted;
        /// <summary>Источник события завершения работы процессора</summary><param name="e">Параметры события</param>
        [DST]
        protected virtual void OnProcessCompleted(EventArgs e) => ProcessCompleted.FastStart(this, e);

        /// <summary>Событие, возникающие при возникновении исключений в процессе работы процессора</summary>
        public event ExceptionEventHandler<Exception> Error;
        /// <summary>Источник события возникновения исключительной ситуации в процессе работы процессора</summary>
        /// <param name="e">Аргумент события ошибки, содержащий объект исключения</param>
        [DST]
        protected virtual void OnError(ExceptionEventHandlerArgs<Exception> e) => Error.ThrowIfUnhandled(this, e, true);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>
        /// Таймаут времени ожидания синхронизации потока обработки при его завершении, 
        /// после которого поток прерывается методом Abort()
        /// По умолчанию 100 мс.
        /// </summary>
        protected int _JoinThreadTimeout = 100;

        /// <summary>Флаг активности потока обработки. Пока значение флага "истина" - поток выполняется</summary>
        protected volatile bool _Enabled;

        /// <summary>Объект синхронизации запуска/остановки процессора - только для чтения</summary>
        protected readonly object _StartStopSectionLocker = new();

        /// <summary>Основной поток работы процессора</summary>
        protected Thread _MainWorkThread;

        /// <summary>Время запуска</summary>
        private DateTime? _StartTime;

        /// <summary>Время остановки</summary>
        private DateTime? _StopTime;

        /// <summary>Объект-наблюдатель за состоянием процессора</summary>
        private readonly ProgressMonitor _Monitor = new("Ожидание");

        /// <summary>Базовый приоритет потока процессора</summary>
        private ThreadPriority _Priority = ThreadPriority.Normal;

        /// <summary>Таймаут выполнения между циклами процессора </summary>
        private int _ActionTimeout;

        /// <summary>Метод установки времени таймаута для работающего потока процессора</summary>
        private Action<int> _SetTimeout;

        /// <summary>Признак синхронной работы</summary>
        private volatile bool _IsSynchronous;

        /// <summary>Флаг, разрешающий вызов события ошибки в случае рассинхронизации потока</summary>
        private volatile bool _ErrorIfAsync;

        /// <summary>Количество выполненных циклов обработки</summary>
        private long _CyclesCount;

        // ReSharper disable FieldCanBeMadeReadOnly.Global
        /// <summary>Имя для генерируемой потока выполнения процессора</summary>
        protected string _NameForeNewMainThread;
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        // ReSharper disable NotAccessedField.Global
        /// <summary>Метод извлечения времени выполнения одного цикла основного метода процессора</summary>
        protected Func<TimeSpan> _GetLastDeltaTime;
        // ReSharper restore NotAccessedField.Global

        /// <summary>Объект синхронизации потоков по запуску процессора</summary>
        protected readonly EventWaitHandle _StartWaitHandle = new ManualResetEvent(false);

        /// <summary>Объект синхронизации потоков по остановке процессора</summary>
        protected readonly EventWaitHandle _StopWaitHandle = new ManualResetEvent(false);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Приоритет выполнения метода наблюдения</summary>
        public ThreadPriority Priority
        {
            [DST]
            get => _Priority;
            [MethodImpl(MethodImplOptions.Synchronized), DST]
            set
            {
                _Priority = value;
                var thread = _MainWorkThread;
                lock (_StartStopSectionLocker)
                    if(thread != null && (thread.IsAlive || thread.IsBackground))
                        thread.Priority = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Признак активности процессора</summary>
        public bool Enable { [DST] get => _Enabled; [DST] set { if(value) Start(); else Stop(); } }

        /// <summary>Основной поток работы процессора</summary>
        public Thread MainThread { [DST] get => _MainWorkThread; }

        /// <summary>Таймаут времени синхронизации основного потока процессора с потоком, завершившим его работу.</summary>
        public int JoinThreadTimeout
        {
            [DST]
            get => _JoinThreadTimeout;
            [DST]
            set
            {
                _JoinThreadTimeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Время запуска</summary>
        public DateTime? StartTime { [DST] get => _StartTime; [DST] protected set { _StartTime = value; OnPropertyChanged(); } }

        /// <summary>Время остановки</summary>
        public DateTime? StopTime { [DST] get => _StopTime; protected set { _StopTime = value; OnPropertyChanged(); } }

        /// <summary>Время, прошедшее после запуска</summary>
        public TimeSpan? ElapsedTime { [DST] get { var start = _StartTime; return start is null ? (TimeSpan?)null : Now - start.Value; } }

        /// <summary>Объект-наблюдатель за состоянием процессора</summary>
        public ProgressMonitor Monitor => _Monitor;

        /// <summary>Таймаут основной циклической операции в миллисекундах</summary>
        public int ActionTimeout
        {
            [DST]
            get => _ActionTimeout;
            [DST]
            set
            {
                if(_ActionTimeout == value) return;
                _ActionTimeout = value;
                _SetTimeout?.Invoke(value);
                OnPropertyChanged();
            }
        }

        /// <summary>Признак синхронной работы</summary>
        public bool IsSynchronous { [DST] get => _IsSynchronous; }

        /// <summary>Генерировать ошибку в случае рассинхронизации?</summary>
        public bool ErrorIfAsync { [DST] get => _ErrorIfAsync; [DST] set { _ErrorIfAsync = value; OnPropertyChanged(); } }

        /// <summary>Количество пройденных циклов</summary>
        public long CyclesCount { [DST] get => _CyclesCount; }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового экземпляра <see cref="Processor"/></summary>
        [DST]
        protected Processor() => _NameForeNewMainThread = $"{GetType().Name}Thread";

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Перезапуск</summary>
        [DST]
        public virtual void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>Запуск обработки</summary>
        [DST]
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void Start()
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            if(_Enabled) return;
            lock (_StartStopSectionLocker)
            {
                if(_Enabled) return;
                _Enabled = true;
                _MainWorkThread = new Thread(ThreadMethod)
                {
                    IsBackground = true,
                    Name = _NameForeNewMainThread,
                    Priority = _Priority
                };

                _MainWorkThread.Start();

                _StopWaitHandle.Reset();
                _StartWaitHandle.Set();
            }
            OnEnableChanged(EventArgs.Empty);
            OnPropertyChanged(nameof(Enable));
        }

        /// <summary>Остановка обработки</summary>
        [DST]
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void Stop()
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            if(!_Enabled) return;
            lock (_StartStopSectionLocker)
            {
                if(!_Enabled) return;
                _Enabled = false;
                if(!_MainWorkThread.Join(_JoinThreadTimeout))
                    _MainWorkThread.Abort();
                _MainWorkThread = null;
                _SetTimeout = null;
                _GetLastDeltaTime = null;

                _StartWaitHandle.Reset();
                _StopWaitHandle.Set();
            }
            OnEnableChanged(EventArgs.Empty);
            OnPropertyChanged(nameof(Enable));
        }

        /// <summary>Блокировать поток до запуска процессора</summary>
        [DST]
        public bool WaitToStart(TimeSpan? Timeout = null) => Timeout is null || Timeout.Value.Ticks == 0
            ? _StartWaitHandle.WaitOne()
            : _StartWaitHandle.WaitOne(Timeout.Value);

        /// <summary>Блокировать поток до остановки процессора</summary>
        [DST]
        public bool WaitToStop(TimeSpan? Timeout = null) => Timeout is null || Timeout.Value.Ticks == 0
            ? _StopWaitHandle.WaitOne()
            : _StopWaitHandle.WaitOne(Timeout.Value);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Основной метод процессора, выполняемый в отдельном потоке </summary>
        // ReSharper disable VirtualMemberNeverOverriden.Global
        //        [DST]
        protected virtual void ThreadMethod()
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            InitializeAction();

            #region Переменные

            var timeout = TimeSpan.FromMilliseconds(_ActionTimeout);
            _SetTimeout = new_timeout => timeout = TimeSpan.FromMilliseconds(new_timeout);


            var delta = new TimeSpan();
            // ReSharper disable AccessToModifiedClosure
            _GetLastDeltaTime = () => delta;
            // ReSharper restore AccessToModifiedClosure

            // ReSharper disable TooWideLocalVariableScope
            DateTime start_time;
            DateTime stop_time;
            TimeSpan time_to_sleep;
            bool is_synchronous;
            // ReSharper restore TooWideLocalVariableScope 

            #endregion

            #region Основное действие

            while(_Enabled)
            {
                start_time = Now;
                try { MainAction(); } catch(Exception Error)
                {
                    var args = new ExceptionEventHandlerArgs<Exception>(Error);
                    OnError(args);
                    if(args.NeedToThrow) throw;
                    if(Error is ThreadAbortException) Thread.ResetAbort();
                }
                stop_time = Now;
                _CyclesCount++;
                if(timeout.Ticks <= 0) continue;
                delta = stop_time - start_time;
                time_to_sleep = timeout - delta;
                is_synchronous = _IsSynchronous = timeout.Ticks > 0 && time_to_sleep.Ticks > 0;
                if(is_synchronous)
                    Thread.Sleep(time_to_sleep);
                else if(_ErrorIfAsync)
                    OnError(__AsyncException);
            }

            #endregion

            FinalizeAction();
        }

        /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
        protected abstract void MainAction();

        /// <summary>Инициализация процесса</summary>
        [DST]
        private void InitializeAction()
        {
            try { Initializer(); } catch(Exception Error)
            {
                var args = new ExceptionEventHandlerArgs<Exception>(Error);
                OnError(args);
                if(args.NeedToThrow) throw;
                if(Error is ThreadAbortException) Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Метод инициализации. Вызывается после запуска обработки перед началом основного цикла.
        /// По умолчанию вызывает генерацию события запуска процессора
        /// </summary>
        [DST]
        protected virtual void Initializer()
        {
            _StartTime = Now;
            _StopTime = null;
            _CyclesCount = 0;
            _IsSynchronous = true;
            _Monitor.Status = "Обработка";
            OnProcessStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Метод, завершающий процесс обработки. Вызывается после выхода процессора из основного цикла.
        /// По умолчанию вызывает генерацию события завершения работы процессора
        /// </summary>
        // ReSharper disable VirtualMemberNeverOverriden.Global
        [DST]
        protected virtual void Finalizer()
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            _StopTime = Now;
            _Monitor.Status = "Завершено";
            OnProcessCompleted(EventArgs.Empty);
        }

        /// <summary>Завершающее действие процесса</summary>
        [DST]
        private void FinalizeAction()
        {
            try { Finalizer(); } catch(Exception Error)
            {
                var args = new ExceptionEventHandlerArgs<Exception>(Error);
                OnError(args);
                if(args.NeedToThrow) throw;

                if(Error is ThreadAbortException) Thread.ResetAbort();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Освобождение ресурсов</summary>
        /// <param name="disposing">Признак того, что требуется выполнить освобождение управляемых ресурсов</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Stop();
            _StartWaitHandle.Dispose();
            _StopWaitHandle.Dispose();
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}