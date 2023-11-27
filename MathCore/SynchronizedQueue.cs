#nullable enable
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore;

public class SynchronizedQueue<T> : IDisposable
{
    private readonly Queue<T> _Queue;
    private readonly AutoResetEvent _Event = new(false);

    // ReSharper disable once InconsistentlySynchronizedField
    public int Count => _Queue.Count;

    public SynchronizedQueue() => _Queue = [];

    public SynchronizedQueue(int Capacity) => _Queue = new Queue<T>(Capacity);

    public void Add(T value)
    {
        lock (_Queue)
        {
            _Queue.Enqueue(value);
            _Event.Set();
        }
    }

    private T? GetValue()
    {
        lock (_Queue)
        {
            if(_Queue.Count == 0) return default;
            var value = _Queue.Dequeue();
            if(_Queue.Count > 0) _Event.Set();
            return value;
        }
    }

    public T? Get() => _Event.WaitOne() ? GetValue() : default;

    public T? Get(TimeSpan Timeout) => _Event.WaitOne(Timeout) ? GetValue() : default;

    public T? Get(int Timeout) => _Event.WaitOne(Timeout) ? GetValue() : default;

    public void Clear() { lock (_Queue) _Queue.Clear(); }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _Disposed;

    /// <summary>Освобождение ресурсов</summary>
    /// <param name="disposing">Требуется выполнить освобождение управляемых ресурсов</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _Disposed) return;
        _Disposed = true;
        _Event.Dispose();
    }
}

public class SynchronizedItemsProcessor<T> : IDisposable
{
    private readonly SynchronizedQueue<T> _Queue = new();
    private readonly Task[] _Tasks;
    private readonly CancellationTokenSource _Cancellation;

    public SynchronizedItemsProcessor(Action<T?> action, int ThreadCount = 1)
    {
        _Cancellation = new CancellationTokenSource();
        var cancellation_token = _Cancellation.Token;
        _Tasks = Enumerable.Range(0, ThreadCount)
           .Select(_ => Task.Run(() =>
            {
                while(!cancellation_token.IsCancellationRequested)
                    action(_Queue.Get());
            }, cancellation_token))
           .ToArray();
    }

    public SynchronizedItemsProcessor(Action<T?> action, Action<T?, Exception> Catch, int ThreadCount = 1)
    {
        _Cancellation = new CancellationTokenSource();
        _Tasks = Enumerable.Range(0, ThreadCount)
           .Select(_ => Task.Run(() =>
            {
                while(!_Cancellation.IsCancellationRequested)
                {
                    var t = _Queue.Get();
                    try
                    {
                        action(t);
                        // ReSharper disable once CatchAllClause
                    } catch(Exception e)
                    {
                        Catch(t, e);
                    }
                }
            }, _Cancellation.Token))
           .ToArray();
    }

    public void Add(T Value) => _Queue.Add(Value);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _Disposed;

    /// <summary>Освобождение ресурсов</summary>
    /// <param name="disposing">Требуется выполнить освобождение управляемых ресурсов</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _Disposed) return;
        _Disposed = true;
        _Cancellation.Cancel();
        _Queue.Dispose();
        _Cancellation.Dispose();
    }
}