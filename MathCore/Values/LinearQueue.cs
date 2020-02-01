using System;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Values
{
    /// <summary>Очередь с линейным доступом</summary>
    /// <typeparam name="T">Тип элементов очереди</typeparam>
    public class LinearQueue<T>
    {
        private readonly T[] _Buffer;
        private int _Offset;

        public int Length { get; }

        public ref T this[int i] => ref _Buffer[(i + _Offset) % Length];

        public LinearQueue(int Length) => _Buffer = new T[this.Length = Length];

        public void Add(T t)
        {
            _Buffer[_Offset] = t;
            _Offset++;
            _Offset %= Length;
        }

        [NotNull]
        public T[] ToArray()
        {
            var result = new T[Length];
            Array.Copy(_Buffer, _Offset, result, 0, Length - _Offset);
            Array.Copy(_Buffer, 0, result, Length - _Offset, _Offset);
            return result;
        }
    }
}