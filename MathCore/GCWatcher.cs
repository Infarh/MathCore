using System;
using System.Threading;

namespace MathCore
{
    /// <summary>Наблюдатель за сборщиком мусора</summary>
    public sealed class GCWatcher
    {
        /// <summary>Событие запуска процесса сборки мусора</summary>
        private static event EventHandler __Approaches;
        /// <summary>Событие запуска процесса сборки мусора</summary>
        public static event EventHandler Approaches
        {
            add
            {
                lock (__SyncRoot)
                {
                    var old_handlers = __Approaches;
                    __Approaches += value;
                    if (old_handlers is null) Start();
                }
            }
            remove
            {
                lock (__SyncRoot)
                {
                    __Approaches -= value;
                    if (__Approaches is null) Stop();
                }
            }
        }

        /// <summary>Событие завершения процесса сборки мусора</summary>
        private static event EventHandler __Complete;
        /// <summary>Событие завершения процесса сборки мусора</summary>
        public static event EventHandler Complete
        {
            add
            {
                lock (__SyncRoot)
                {
                    var old_handlers = __Complete;
                    __Complete += value;
                    if (old_handlers is null) Start();
                }
            }
            remove
            {
                lock (__SyncRoot)
                {
                    __Complete -= value;
                    if (__Complete is null) Stop();
                }
            }
        }

        /// <summary>Генерация события начала сборки мусора</summary>
        private static void OnApproaches() => __Approaches.BeginInvoke(__GcWatcher, EventArgs.Empty, null, null);
        /// <summary>Генерация события окончания сборки мусора</summary>
        private static void OnComplete() => __Complete.BeginInvoke(__GcWatcher, EventArgs.Empty, null, null);

        /// <summary>Объект-наблюдатель за сборщиком мусора</summary>
        private static readonly GCWatcher __GcWatcher = new();
        /// <summary>Объект синхронизации потоков управления наблюдателем</summary>
        private static readonly object __SyncRoot = new();
        /// <summary>Поток наблюдения с сборщиком мусора</summary>
        private static Thread _WatcherThread;
        /// <summary>Признак активности наблюдателя</summary>
        private static bool __Enabled;

        /// <summary>Скрытая инициализация объекта-наблюдателя</summary>
        private GCWatcher() { }

        /// <summary>Запуск процесса наблюдения</summary>
        private static void Start()
        {
            if (__Enabled) return;
            lock (__SyncRoot)
            {
                if (__Enabled) return;
                __Enabled = true;
                _WatcherThread = new Thread(Watch) { IsBackground = true };
                _WatcherThread.Start();
            }
        }

        /// <summary>Остановка процесса наблюдения</summary>
        private static void Stop()
        {
            if (!__Enabled) return;
            lock (__SyncRoot)
            {
                if (!__Enabled) return;
                __Enabled = false;
                _WatcherThread = null;
            }
        }

        /// <summary>Процесс наблюдения</summary>
        private static void Watch()
        {
            GC.RegisterForFullGCNotification(50, 50);
            while (__Enabled)
            {
                GC.WaitForFullGCApproach();
                if (!__Enabled) return;
                OnApproaches();
                GC.WaitForFullGCComplete();
                if (!__Enabled) return;
                OnComplete();
            }
        }
    }
}