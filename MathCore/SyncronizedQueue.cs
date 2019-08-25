using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public class SyncronizedQueue<T> : IDisposable
    {
        [NotNull] private readonly Queue<T> _Queue;
        [NotNull] private readonly AutoResetEvent _Event = new AutoResetEvent(false);

        // ReSharper disable once InconsistentlySynchronizedField
        public int Count => _Queue.Count;

        public SyncronizedQueue() => _Queue = new Queue<T>();

        public SyncronizedQueue(int Capacity)
        {
            Contract.Requires(Capacity > 0);
            _Queue = new Queue<T>(Capacity);
        }

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

        public void Dispose() => _Event.Dispose();
    }

    public class SyncronizedItemsProcessor<T> : IDisposable
    {
        private readonly SyncronizedQueue<T> _Queue = new SyncronizedQueue<T>();
        private readonly Task[] _Tasks;
        private readonly CancellationTokenSource _Cancelation;

        public SyncronizedItemsProcessor(Action<T> action, int ThreadCount = 1)
        {
            Contract.Requires(action != null);
            Contract.Requires(ThreadCount > 0);

            _Cancelation = new CancellationTokenSource();
            var cancelation_token = _Cancelation.Token;
            _Tasks = Enumerable.Range(0, ThreadCount)
                .Select(i => Task.Factory.StartNew(() =>
                {
                    while(!cancelation_token.IsCancellationRequested)
                        action(_Queue.Get());
                }, cancelation_token))
                .ToArray();
        }

        public SyncronizedItemsProcessor(Action<T> action, Action<T, Exception> Catch, int ThreadCount = 1)
        {
            Contract.Requires(action != null);
            Contract.Requires(ThreadCount > 0);

            _Cancelation = new CancellationTokenSource();
            _Tasks = Enumerable.Range(0, ThreadCount)
                .Select(i => Task.Factory.StartNew(() =>
                {
                    while(!_Cancelation.IsCancellationRequested)
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
                }, _Cancelation.Token))
                .ToArray();
        }

        public void Add(T Value) => _Queue.Add(Value);

        public void Dispose()
        {
            _Cancelation.Cancel();
            _Queue.Dispose();
            _Cancelation.Dispose();
        }
    }
}