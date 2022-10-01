using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore;

/// <summary>Наблюдатель за сборщиком мусора</summary>
public sealed class GCWatcher
{
    /// <summary>Событие запуска процесса сборки мусора</summary>
    private static event EventHandler ApproachesHandlers;
    /// <summary>Событие запуска процесса сборки мусора</summary>
    public static event EventHandler Approaches
    {
        add
        {
            lock (__SyncRoot)
            {
                var old_handlers = ApproachesHandlers;
                ApproachesHandlers += value;
                if (old_handlers is null) Start();
            }
        }
        remove
        {
            lock (__SyncRoot)
            {
                ApproachesHandlers -= value;
                if (ApproachesHandlers is null) Stop();
            }
        }
    }

    /// <summary>Событие завершения процесса сборки мусора</summary>
    private static event EventHandler CompleteHandlers;
    /// <summary>Событие завершения процесса сборки мусора</summary>
    public static event EventHandler Complete
    {
        add
        {
            lock (__SyncRoot)
            {
                var old_handlers = CompleteHandlers;
                CompleteHandlers += value;
                if (old_handlers is null) Start();
            }
        }
        remove
        {
            lock (__SyncRoot)
            {
                CompleteHandlers -= value;
                if (CompleteHandlers is null) Stop();
            }
        }
    }

    /// <summary>Генерация события начала сборки мусора</summary>
    private static void OnApproaches()
    {
        if(ApproachesHandlers is not { } handlers) return;
        _ = Task.Run(() => handlers(__GcWatcher, EventArgs.Empty));
    }

    /// <summary>Генерация события окончания сборки мусора</summary>
    private static void OnComplete()
    {
        if(CompleteHandlers is not { } handlers) return;
        _ = Task.Run(() => handlers(__GcWatcher, EventArgs.Empty));
    }

    /// <summary>Объект-наблюдатель за сборщиком мусора</summary>
    private static readonly GCWatcher __GcWatcher = new();

    /// <summary>Объект синхронизации потоков управления наблюдателем</summary>
    private static readonly object __SyncRoot = new();

    /// <summary>Поток наблюдения с сборщиком мусора</summary>
    private static Thread __WatcherThread;

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
            __Enabled      = true;
            __WatcherThread = new(Watch) { IsBackground = true };
            __WatcherThread.Start();
        }
    }

    /// <summary>Остановка процесса наблюдения</summary>
    private static void Stop()
    {
        if (!__Enabled) return;
        lock (__SyncRoot)
        {
            if (!__Enabled) return;
            __Enabled      = false;
            __WatcherThread = null;
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