namespace MathCore.Threading;

/// <summary>Контекст синхронизации с максимально допустимой степенью параллелизма</summary>
public sealed class MaxConcurrencySynchronizationContext : SynchronizationContext
{
    /// <summary>Семафор, ограничивающий число выполняемых задач</summary>
    private readonly SemaphoreSlim _Semaphore;

    /// <summary>Инициализация нового контекста синхронизации с ограничением числа параллельно выполняемых задач</summary>
    /// <param name="MaxConcurrencyLevel">Максимальное число выполняемых в контексте задач</param>
    public MaxConcurrencySynchronizationContext(int MaxConcurrencyLevel) => _Semaphore = new SemaphoreSlim(MaxConcurrencyLevel);

    /// <summary>Метод, вызываемый при освобождении семафора</summary>
    /// <param name="SemaphoreWaitTask">Задача ожидания освобождения семафора</param>
    /// <param name="CallState">Массив с параметрами продолжения, хранящий в первом параметре делегат, который надо вызвать, а во втором - параметр вызова делегата</param>
    private void OnSemaphoreReleased(Task SemaphoreWaitTask, object CallState)
    {
        var d     = (SendOrPostCallback)((object[])CallState)[0];
        var state = ((object[])CallState)[1];
        try
        {
            d(state);
        }
        finally
        {
            _Semaphore.Release();
        }
    }

    /// <inheritdoc />
    public override void Post(SendOrPostCallback d, object state) =>
        _Semaphore
           .WaitAsync()
           .ContinueWith(
                OnSemaphoreReleased, 
                new [] { d, state }, 
                default, 
                TaskContinuationOptions.None, 
                TaskScheduler.Default);

    /// <inheritdoc />
    public override void Send(SendOrPostCallback d, object state)
    {
        _Semaphore.Wait();
        try
        {
            d(state);
        }
        finally
        {
            _Semaphore.Release();
        }
    }
}