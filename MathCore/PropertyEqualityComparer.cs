#nullable enable
using System.Collections;

namespace MathCore;

public class PropertyEqualityComparer<T, TValue>(Func<T, TValue> Selector) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => EqualityComparer<TValue>.Default.Equals(Selector(x), Selector(y));

    public int GetHashCode(T obj) => EqualityComparer<TValue>.Default.GetHashCode(Selector(obj)!);
}

public class PropertyEqualityComparer(Func<object, object> Selector) : IEqualityComparer
{
    public static PropertyEqualityComparer<T, TValue> Create<T, TValue>(Func<T, TValue> Selector) => new(Selector);

    public new bool Equals(object x, object y) => object.Equals(Selector(x), Selector(y));

    public int GetHashCode(object obj) => Selector(obj).GetHashCode();
}