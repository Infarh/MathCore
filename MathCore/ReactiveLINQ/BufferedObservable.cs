using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    internal abstract class BufferedObservable<T> : SimpleObservableEx<T[]>
    {
        private readonly int _BufferLength;
        // ReSharper disable once NotAccessedField.Local
        private readonly IObserver<T> _Observer;
        private readonly Queue<Queue<T>> _Buffer;
        protected readonly object _SyncRoot = new();

        protected BufferedObservable([NotNull] IObservable<T> ObservableObject, int QueueLength, int BufferLength = 0)
        {
            _BufferLength = BufferLength;
            _Observer = new LambdaObserver<T>(ObservableObject, OnNext, OnCompleted, OnReset, OnError);
            _Buffer = new Queue<Queue<T>>(QueueLength);
        }

        protected abstract void OnNext(T value);

        [CanBeNull]
        protected T[] AddValue(T Value)
        {
            lock (_SyncRoot)
            {
                foreach (var b in _Buffer) b.Enqueue(Value);
                var f = _Buffer.Peek();
                if (f.Count < _BufferLength) return null;
                _Buffer.Dequeue();
                return f.ToArray();
            }
        }

        protected void AddBuffer() { lock (_SyncRoot) _Buffer.Enqueue(new Queue<T>(_BufferLength)); }
    }
}