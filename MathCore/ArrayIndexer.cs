using System;
using MathCore.Annotations;

namespace MathCore
{
    public class ArrayIndexer<T>
    {
        [NotNull] private T[] _Array;

        private int _Index;

        public int Length => _Array.Length;

        public int Index { get => _Index; set => _Index = value; }

        public T Value { get => _Array[_Index]; set => _Array[_Index] = value; }

        public T this[int index] { get => _Array[index]; set => _Array[index] = value; }

        [NotNull] public T[] Array { get => _Array; set => _Array = value ?? throw new ArgumentNullException(nameof(value)); }

        public ArrayIndexer() => _Array = new T[0];

        public ArrayIndexer([NotNull] T[] Array, int Index = 0)
        {
            _Array = Array ?? throw new ArgumentNullException(nameof(Array));
            _Index = Index;
        }

        [NotNull] public static implicit operator T[]([NotNull] ArrayIndexer<T> Indexer) => Indexer._Array;
    }
}