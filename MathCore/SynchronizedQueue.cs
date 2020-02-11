using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public class SynchronizedQueue<T> : IDisposable
    {
        [NotNull] private readonly Queue<T> _Queue;
        [NotNull] private readonly AutoResetEvent _Event = new AutoResetEvent(false);

        // ReSharper disable once InconsistentlySynchronizedField
        public int Count => _Queue.Count;

        public SynchronizedQueue() => _Queue = new Queue<T>();

        public SynchronizedQueue(int Capacity) => _Queue = new Queue<T>(Capacity);

        public void Add(T value)
        {
            lock (_Queue)
            {
                _Queue.Enqueue(value);
                _Event.Set();
            }
        }

        [CanBeNull]
        private T GetValue()
        {
            lock (_Queue)
            {
                if(_Queue.Count == 0) return default;
                var value = _Queue.Dequeue();
                if(_Queue.Count > 0) _Event.Set();
                return value;
            }
        }

        [CanBeNull]
        public T Get() => _Event.WaitOne() ? GetValue() : default;

        [CanBeNull]
        public T Get(TimeSpan Timeout) => _Event.WaitOne(Timeout) ? GetValue() : default;

        [CanBeNull]
        public T Get(int Timeout) => _Event.WaitOne(Timeout) ? GetValue() : default;

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
        private readonly SynchronizedQueue<T> _Queue = new SynchronizedQueue<T>();
        private readonly Task[] _Tasks;
        private readonly CancellationTokenSource _Cancellation;

        public SynchronizedItemsProcessor(Action<T> action, int ThreadCount = 1)
        {
            _Cancellation = new CancellationTokenSource();
            var cancellation_token = _Cancellation.Token;
            _Tasks = Enumerable.Range(0, ThreadCount)
                .Select(i => Task.Factory.StartNew(() =>
                {
                    while(!cancellation_token.IsCancellationRequested)
                        action(_Queue.Get());
                }, cancellation_token))
                .ToArray();
        }

        public SynchronizedItemsProcessor(Action<T> action, Action<T, Exception> Catch, int ThreadCount = 1)
        {
            _Cancellation = new CancellationTokenSource();
            _Tasks = Enumerable.Range(0, ThreadCount)
                .Select(i => Task.Factory.StartNew(() =>
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
}