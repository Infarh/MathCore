#nullable enable
using System.Collections;

namespace MathCore.Vectors;

public class Vector<T> : IEnumerable<T>, ICloneable<Vector<T>>, IEquatable<Vector<T>>
{
    /* ------------------------------------------------------------------------------------------ */

    private readonly T[] _Elements;

    /* ------------------------------------------------------------------------------------------ */

    public int Dimension => _Elements.Length;

    public ref T this[int i] => ref _Elements[i];

    /* ------------------------------------------------------------------------------------------ */

    public Vector(int Dimension) => _Elements = new T[Dimension];

    public Vector(T[] Elements) => _Elements = (T[])Elements.Clone();

    public Vector(IEnumerable<T> Elements) : this(Elements.ToArray()) { }

    /* ------------------------------------------------------------------------------------------ */

    /// <inheritdoc />
    public override int GetHashCode() => _Elements.Aggregate(Consts.BigPrime_int, (V, v) => V ^ v.GetHashCode());

    /// <inheritdoc />
    public bool Equals(Vector<T>? other)
    {
        if(other is null) return false;
        if(ReferenceEquals(other, this)) return true;
        if(other.Dimension != Dimension) return false;

        return !_Elements.Where((v, i) => !v.Equals(other[i])).Any();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => (obj is Vector<T> vector) && Equals(vector);

    /// <inheritdoc />
    public Vector<T> Clone() => new(_Elements);

    /// <inheritdoc />
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_Elements.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /* ------------------------------------------------------------------------------------------ */

    public static implicit operator Vector<T>(T[] e) => new(e);

    public static implicit operator Vector<T>(List<T> e) => new(e);

    /* ------------------------------------------------------------------------------------------ */
}