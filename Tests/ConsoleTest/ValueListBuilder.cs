using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConsoleTest
{
    //https://habr.com/ru/company/clrium/blog/420051/
    public ref partial struct ValueListBuilder<T>
    {
        private Span<T> _Span;
        private T[] _ArrayFromPool;
        private int _Pos;

        public ValueListBuilder(Span<T> InitialSpan)
        {
            _Span = InitialSpan;
            _ArrayFromPool = null;
            _Pos = 0;
        }

        public int Length
        {
            get => _Pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _Span.Length);
                _Pos = value;
            }
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index < _Pos);
                return ref _Span[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            var pos = _Pos;
            if (pos >= _Span.Length)
                Grow();

            _Span[pos] = item;
            _Pos = pos + 1;
        }

        public ReadOnlySpan<T> AsSpan() => _Span.Slice(0, _Pos);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_ArrayFromPool is null) return;
            ArrayPool<T>.Shared.Return(_ArrayFromPool);
            _ArrayFromPool = null;
        }

        private void Grow()
        {
            var array = ArrayPool<T>.Shared.Rent(_Span.Length * 2);

            var success = _Span.TryCopyTo(array);
            Debug.Assert(success);

            var to_return = _ArrayFromPool;
            _Span = _ArrayFromPool = array;
            if (to_return != null) ArrayPool<T>.Shared.Return(to_return);
        }
    }
}