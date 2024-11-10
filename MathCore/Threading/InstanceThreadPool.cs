using System.Diagnostics;

#nullable enable
namespace MathCore.Threading;

/// <summary>Пул с фикисрованным поличеством потоков</summary>
public class InstanceThreadPool : IDisposable
{
    private readonly ThreadPriority _Priority;
    private readonly Queue<(Action<object?> Work, object? Parameter)> _Works = [];
    private readonly Thread[] _Threads;
    private volatile bool _CanWork = true;
    private readonly string _Name;

    private readonly AutoResetEvent _ExecuteEvent = new(true);
    private readonly AutoResetEvent _WorkingEvent = new(false);

    /// <summary>Инициализирует новый экземпляр класса InstanceThreadPool с заданным количеством потоков, приоритетом и именем</summary>
    /// <param name="MaxThreadsCount">Максимальное количество потоков в пуле. Должно быть больше 0.</param>
    /// <param name="Priority">Приоритет потоков. По умолчанию - Normal.</param>
    /// <param name="Name">Имя пула потоков. Если не указано, используется хэш-код объекта в виде строки.</param>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если MaxThreadsCount меньше или равно 0.</exception>
    public InstanceThreadPool(int MaxThreadsCount, ThreadPriority Priority = ThreadPriority.Normal, string? Name = null)
    {
        if (MaxThreadsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(MaxThreadsCount), MaxThreadsCount, "Число рабочих потоков должно быть больше 0");

        _Priority = Priority;
        _Threads = new Thread[MaxThreadsCount];
        _Name = Name ?? GetHashCode().ToString("x");
        Initialize();
    }

        /// <summary>Инициализирует потоки пула</summary>
        /// <remarks>
        /// Создает <see cref="_Threads.Length"/> потоков, каждый из которых
        /// будет выполнять делегат <see cref="WorkingThread"/>, и запускает
        /// каждый поток.
        /// </remarks>
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

    /// <summary>Выполняет задание в пуле потоков</summary>
    /// <param name="Parameter">Параметр, передаваемый в задание</param>
    /// <param name="Work">Задание, которое должно быть выполнено</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если пул потоков остановлен</exception>
    public void Execute(object? Parameter, Action<object?> Work)
    {
        if (!_CanWork) throw new InvalidOperationException("Попытка передать задание остановленному пулу потоков");

        _ExecuteEvent.WaitOne();    // запрашиваем разрешение на доступ к списку 
        if (!_CanWork) throw new InvalidOperationException("Попытка передать задание остановленному пулу потоков");

        _Works.Enqueue((Work, Parameter));
        _ExecuteEvent.Set();        // разрешаем доступ к списку заданий

        _WorkingEvent.Set();        // разрешаем работу потоку
    }

    /// <summary>Метод, выполняемый каждым потоком пула</summary>
    /// <remarks>
    /// Метод работает в цикле, пока <see cref="_CanWork"/> имеет значение <c>true</c>.
    /// Метод ожидает событие <see cref="_WorkingEvent"/>, которое свидетельствует
    /// о получении разрешения на работу потока.
    /// Затем метод ожидает событие <see cref="_ExecuteEvent"/>, которое свидетельствует
    /// о доступности списка заданий.
    /// Если <see cref="_CanWork"/> имеет значение <c>false</c>, то метод
    /// завершает свою работу.
    /// </remarks>
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
