using System;
using System.Threading;

namespace MathCore
{
    /// <summary>Наблюдатель за сборщиком мусора</summary>
    public sealed class GCWacher
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
                    if(old_handlers == null) Start();
                }
            }
            remove
            {
                lock (__SyncRoot)
                {
                    __Approaches -= value;
                    if(__Approaches == null) Stop();
                }
            }
        }

        /// <summary>Событие завершения процесса сборки мусора</summary>
        private static event EventHandler __Complite;
        /// <summary>Событие завершения процесса сборки мусора</summary>
        public static event EventHandler Complite
        {
            add
            {
                lock (__SyncRoot)
                {
                    var old_handlers = __Complite;
                    __Complite += value;
                    if(old_handlers == null) Start();
                }
            }
            remove
            {
                lock (__SyncRoot)
                {
                    __Complite -= value;
                    if(__Complite == null) Stop();
                }
            }
        }

        /// <summary>Генерация осбытия начала сборки мусора</summary>
        private static void OnApproaches() => __Approaches.BeginInvoke(__GCWacher, EventArgs.Empty, null, null);
        /// <summary>Генерация осбытия окончания сборки мусора</summary>
        private static void OnComplite() => __Complite.BeginInvoke(__GCWacher, EventArgs.Empty, null, null);

        /// <summary>Объект-наблюдатель за сборщиком мусора</summary>
        private static readonly GCWacher __GCWacher = new GCWacher();
        /// <summary>Объект синхронизации потоков управления наблюдателем</summary>
        private static readonly object __SyncRoot = new object();
        /// <summary>Поток наблюдения с борщиком мусора</summary>
        private static Thread _WatcherThread;
        /// <summary>Признак активности наблюдателя</summary>
        private static bool __Enabled;

        /// <summary>Скрытая инициализация объекта-наблюдателя</summary>
        private GCWacher() { }

        /// <summary>Запуск процесса наблюдения</summary>
        private static void Start()
        {
            if(__Enabled) return;
            lock (__SyncRoot)
            {
                if(__Enabled) return;
                __Enabled = true;
                _WatcherThread = new Thread(Watch);
                _WatcherThread.IsBackground = true;
                _WatcherThread.Start();
            }
        }

        /// <summary>Остановка процесса наблюдения</summary>
        private static void Stop()
        {
            if(!__Enabled) return;
            lock (__SyncRoot)
            {
                if(!__Enabled) return;
                __Enabled = false;
                _WatcherThread = null;
            }
        }

        /// <summary>Проесс наблюдения</summary>
        private static void Watch()
        {
            GC.RegisterForFullGCNotification(50, 50);
            while(__Enabled)
            {
                GC.WaitForFullGCApproach();
                if(!__Enabled) return;
                OnApproaches();
                GC.WaitForFullGCComplete();
                if(!__Enabled) return;
                OnComplite();
            }
        }
    }
}
