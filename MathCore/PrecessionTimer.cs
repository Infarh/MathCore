using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedType.Global

namespace MathCore
{
    public class PrecisionTimer : IDisposable
    {
        private static readonly PrecisionTimerCaps __Caps;
        private volatile bool _Disposed;
        private volatile PrecisionTimerMode _Mode;
        private volatile int _Period;
        private volatile int _Resolution;
        private ISynchronizeInvoke _SynchronizingObject;
        private EventRaiser _TickRaiser;
        private TimeProc _TimeProcOneShot;
        private TimeProc _TimeProcPeriodic;
        private int _TimerId;

        // Methods
        static PrecisionTimer() => TimeGetDevCaps(ref __Caps, Marshal.SizeOf(__Caps));

        /// <exception cref="PlatformNotSupportedException">В случае если платформа не Win32NT</exception>
        public PrecisionTimer() => Initialize();

        public static PrecisionTimerCaps Capabilities => __Caps;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(false)]
        public bool IsRunning { get; private set; }

        [DefaultValue(typeof(PrecisionTimerMode), "Periodic")]
        public PrecisionTimerMode Mode
        {
            get
            {
                DisposeCheck();
                return _Mode;
            }
            set
            {
                DisposeCheck();
                _Mode = value;
                if(!IsRunning) return;
                Stop();
                Start();
            }
        }

        public int Period
        {
            get
            {
                DisposeCheck();
                return _Period;
            }
            set
            {
                DisposeCheck();
                if(value < Capabilities.PeriodMin || value > Capabilities.PeriodMax)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Multimedia Timer period out of range.");
                _Period = value;
                if(!IsRunning) return;
                Stop();
                Start();
            }
        }

        public int Resolution
        {
            get
            {
                DisposeCheck();
                return _Resolution;
            }
            set
            {
                DisposeCheck();
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "timer resolution out of range.");
                _Resolution = value;
                if(!IsRunning) return;
                Stop();
                Start();
            }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                DisposeCheck();
                return _SynchronizingObject;
            }
            set
            {
                DisposeCheck();
                _SynchronizingObject = value;
            }
        }

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event EventHandler Tick;

        public void Dispose()
        {
            if(_Disposed) return;
            if(IsRunning) Stop();
            _Disposed = true;
        }

        private void DisposeCheck() { if(_Disposed) throw new ObjectDisposedException("Timer"); }

        ~PrecisionTimer() { if(IsRunning) TimeKillEvent(_TimerId); }

        /// <exception cref="PlatformNotSupportedException">В случае если платформа не Win32NT</exception>
        private void Initialize()
        {
            if(Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException($"Платформа {Environment.OSVersion.Platform} не поддерживается");
            _Mode = PrecisionTimerMode.Periodic;
            _Period = Capabilities.PeriodMin;
            _Resolution = 1;
            IsRunning = false;
            _TimeProcPeriodic = TimerPeriodicEventCallback;
            _TimeProcOneShot = TimerOneShotEventCallback;
            _TickRaiser = OnTick;
        }

        private void OnStarted(EventArgs e) => Started?.Invoke(this, e);

        private void OnStopped(EventArgs e) => Stopped?.Invoke(this, e);

        private void OnTick(EventArgs e) => Tick?.Invoke(this, e);

        public void Start()
        {
            DisposeCheck();
            if(IsRunning) return;
            _TimerId = TimeSetEvent(Period, Resolution,
                                     Mode == PrecisionTimerMode.Periodic ? _TimeProcPeriodic : _TimeProcOneShot,
                                     0, Mode);
            if(_TimerId == 0)
                throw new TimerException("Unable to start timer.");
            IsRunning = true;
            if(SynchronizingObject?.InvokeRequired == true)
                SynchronizingObject.BeginInvoke(new EventRaiser(OnStarted), new object[] { EventArgs.Empty });
            else
                OnStarted(EventArgs.Empty);
        }

        public void Stop()
        {
            DisposeCheck();
            if(!IsRunning) return;
            TimeKillEvent(_TimerId);
            IsRunning = false;
            if(SynchronizingObject?.InvokeRequired == true)
                SynchronizingObject.BeginInvoke(new EventRaiser(OnStopped), new object[] { EventArgs.Empty });
            else
                OnStopped(EventArgs.Empty);
        }

        [DllImport("winmm.dll", EntryPoint = "timeGetDevCaps")]
        private static extern int TimeGetDevCaps(ref PrecisionTimerCaps Caps, int SizeOfTimerCaps);

        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        private static extern int TimeKillEvent(int ID);

        private void TimerOneShotEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if(_SynchronizingObject != null)
            {
                _SynchronizingObject.BeginInvoke(_TickRaiser, new object[] { EventArgs.Empty });
                Stop();
            }
            else
            {
                OnTick(EventArgs.Empty);
                Stop();
            }
        }

        private void TimerPeriodicEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if(_SynchronizingObject != null)
                _SynchronizingObject.BeginInvoke(_TickRaiser, new object[] { EventArgs.Empty });
            else
                OnTick(EventArgs.Empty);
        }

        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        private static extern int TimeSetEvent(int Delay, int Resolution, TimeProc Proc, int User,
                                               PrecisionTimerMode Mode);

        // Properties

        // Nested Types

        #region Nested type: EventRaiser

        private delegate void EventRaiser(EventArgs e);

        #endregion

        #region Nested type: TimeProc

        private delegate void TimeProc(int ID, int Msg, int User, int Param1, int Param2);

        #endregion
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PrecisionTimerCaps { public int PeriodMin, PeriodMax; }

    [Serializable]
    public enum PrecisionTimerMode : byte { OneShot = 0, Periodic = 1 }

    [Serializable]
    public class TimerException : ApplicationException
    {
        // Methods
        public TimerException() { }

        public TimerException(string message) : base(message) { }

        protected TimerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TimerException(string message, Exception inner) : base(message, inner) { }
    }
}