using System.Diagnostics;

#nullable enable
namespace MathCore.Threading;

public class InstanceThreadPool : IDisposable
{
    private readonly ThreadPriority _Priority;
    private readonly Queue<(Action<object?> Work, object? Parameter)> _Works = new();
    private readonly Thread[] _Threads;
    private volatile bool _CanWork = true;
    private readonly string _Name;

    private readonly AutoResetEvent _ExecuteEvent = new(true);
    private readonly AutoResetEvent _WorkingEvent = new(false);

    public InstanceThreadPool(int MaxThreadsCount, ThreadPriority Priority = ThreadPriority.Normal, string? Name = null)
    {
        if (MaxThreadsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(MaxThreadsCount), MaxThreadsCount, "Число рабочих потоков должно быть больше 0");

        _Priority = Priority;
        _Threads = new Thread[MaxThreadsCount];
        _Name = Name ?? GetHashCode().ToString("x");
        Initialize();
    }

    private void Initialize()
    {
        for (var i = 0; i < _Threads.Length; i++)
        {
            var thread_name = $"{nameof(InstanceThreadPool)}[{_Name}]-Thread[{i}]";
            var thread = new Thread(WorkingThread)
            {
                Name = thread_name,
                IsBackground = true,
                Priority = _Priority,
            };
            _Threads[i] = thread;
            thread.Start();
        }
    }

    public void Execute(Action Work) => Execute(null, _ => Work());

    public void Execute(object? Parameter, Action<object?> Work)
    {
        if (!_CanWork) throw new InvalidOperationException("Попытка передать задание остановленному пулу потоков");

        _ExecuteEvent.WaitOne();    // запрашиваем разрешение на доступ к списку 
        if (!_CanWork) throw new InvalidOperationException("Попытка передать задание остановленному пулу потоков");

        _Works.Enqueue((Work, Parameter));
        _ExecuteEvent.Set();        // разрешаем доступ к списку заданий

        _WorkingEvent.Set();        // разрешаем работу потоку
    }

    private void WorkingThread()
    {
        var thread_name = Thread.CurrentThread.Name;
        Trace.TraceInformation("Поток {0} id:{1} запущен", thread_name, Environment.CurrentManagedThreadId);

        try
        {
            _WorkingEvent.WaitOne(); // запрашиваем разрешение на работу потока

            while (_CanWork)
            {
                _ExecuteEvent.WaitOne(); // запрашиваем разрешение на доступ к списку 
                if (!_CanWork) break;

                while (_Works.Count == 0)
                {
                    _ExecuteEvent.Set(); // разрешаем доступ к списку заданий
                    _WorkingEvent.WaitOne(); // запрашиваем разрешение на работу потока
                    if (!_CanWork) return;

                    _ExecuteEvent.WaitOne(); // запрашиваем разрешение на доступ к списку 
                }

                if (!_CanWork) return;

                var (work, parameter) = _Works.Dequeue();
                if (_Works.Count > 0)
                    _WorkingEvent.Set();

                _ExecuteEvent.Set(); // разрешаем доступ к списку заданий

                Trace.TraceInformation("Поток {0} id:{1} выполняет задание", thread_name, Environment.CurrentManagedThreadId);
                try
                {
                    var timer = Stopwatch.StartNew();
                    work(parameter);
                    Trace.TraceInformation(
                        "Поток {0} id:{1} выполнил задание за {2}мс",
                        thread_name,
                        Environment.CurrentManagedThreadId,
                        timer.ElapsedMilliseconds);
                }
                catch (ThreadInterruptedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка выполнения задания в потоке {0}:{1}", thread_name, e);
                }
            }
        }
        catch (ThreadInterruptedException e)
        {
            Trace.TraceWarning("Поток {0} был принудительно прерван при завершении работы пула", thread_name);
        }
        finally
        {
            Trace.TraceInformation("Поток {0} id:{1} остановлен", thread_name, Environment.CurrentManagedThreadId);
            if (!_WorkingEvent.SafeWaitHandle.IsClosed)
                _WorkingEvent.Set();
        }
    }

    private const int __JoinTimeout = 100;
    public void Dispose()
    {
        _CanWork = false;

        _WorkingEvent.Set();
        foreach (var thread in _Threads)
            if (!thread.Join(__JoinTimeout))
                thread.Interrupt();

        _ExecuteEvent.Dispose();
        _WorkingEvent.Dispose();
    }
}
