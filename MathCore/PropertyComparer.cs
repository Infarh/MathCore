#nullable enable
using System.Collections;

namespace MathCore;

public class PropertyComparer<T, TValue>(Func<T, TValue> Selector) : IComparer<T>
{
    public int Compare(T? x, T? y) => Comparer<TValue>.Default.Compare(Selector(x), Selector(y));
}

public class PropertyComparer(Func<object, object> Selector) : IComparer
{
    public static PropertyComparer<T, TValue> Create<T, TValue>(Func<T, TValue> Selector) => new(Selector);

    public int Compare(object? x, object? y) => Comparer.Default.Compare(Selector(x), Selector(y));
}