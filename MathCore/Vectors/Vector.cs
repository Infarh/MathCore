using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Vectors
{
    public class Vector<T> : IEnumerable<T>, ICloneable<Vector<T>>, IEquatable<Vector<T>>
    {
        /* ------------------------------------------------------------------------------------------ */

        private readonly T[] _Elements;

        /* ------------------------------------------------------------------------------------------ */

        public int Demention => _Elements.Length;

        public T this[int i] { get => _Elements[i]; set => _Elements[i] = value; }

        /* ------------------------------------------------------------------------------------------ */

        public Vector(int Demention) => _Elements = new T[Demention];

        public Vector(T[] Elements) => _Elements = (T[])Elements.Clone();

        public Vector(IEnumerable<T> Elements) : this(Elements.ToArray()) { }

        /* ------------------------------------------------------------------------------------------ */

        public override int GetHashCode() => _Elements.Aggregate(Consts.BigPrime_int, (V, v) => V ^ v.GetHashCode());

        public bool Equals(Vector<T> other)
        {
            if(ReferenceEquals(other, null)) return false;
            if(ReferenceEquals(other, this)) return true;
            if(other.Demention != Demention) return false;

            return !_Elements.Where((v, i) => !v.Equals(other[i])).Any();
        }

        public override bool Equals(object obj) => (obj is Vector<T>) && Equals(obj as Vector<T>);

        public Vector<T> Clone() => new Vector<T>(_Elements);

        object ICloneable.Clone() => Clone();

        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /* ------------------------------------------------------------------------------------------ */

        public static implicit operator Vector<T>(T[] e) { return new Vector<T>(e); }

        public static implicit operator Vector<T>(List<T> e) { return new Vector<T>(e); }

        /* ------------------------------------------------------------------------------------------ */
    }
}
