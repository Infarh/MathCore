using System;

namespace MathCore.Time
{
    /// <summary>Измеритель времени</summary>
    public sealed class TimeCounter
    {
        /// <summary>Флаг состояния</summary>
        private bool _Started;

        /// <summary>Объект межпотоковой синхронизации</summary>
        private readonly object _LockObject = new object();

        //private readonly List<TimeEvent> _Events = new List<TimeEvent>();
        private TimeSpan _StartTime;
        private TimeSpan _TotalTime;

        /// <summary>Состояние измерителя</summary>
        public bool IsStarted { get => _Started;
            set { if(value) Start(); else Stop(); } }

        /// <summary>Прошло времени</summary>
        public TimeSpan TotalTime => _TotalTime;

        /// <summary>Запуск</summary>
        public void Start()
        {
            if(_Started) return;
            lock(_LockObject)
            {
                if(_Started) return;

                _StartTime = DateTime.Now.TimeOfDay;
                //_Events.Add(new TimeEvent {StartTime = _StartTime });

                _Started = true;
            }
        }

        /// <summary>Остановка</summary>
        public void Stop()
        {
            if(!_Started) return;
            lock(_LockObject)
            {
                if(!_Started) return;

                var now = DateTime.Now.TimeOfDay;
                //_Events[_Events.Count - 1].StopTime = now;
                _TotalTime += now - _StartTime;


                _Started = false;
            }
        }

        /// <summary>Сброс измерителя</summary>
        public void Reset()
        {
            lock(_LockObject)
            {
                _Started = false;
                _StartTime = default;
                _TotalTime = default;
            }
        }
    }
}