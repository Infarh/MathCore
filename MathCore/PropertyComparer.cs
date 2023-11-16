#nullable enable
using System.Collections;

namespace MathCore;

public class PropertyComparer<T, TValue> : IComparer<T>
{
    private readonly Func<T, TValue> _Selector;

    public PropertyComparer(Func<T, TValue> Selector) => _Selector = Selector.NotNull();

    public int Compare(T? x, T? y) => Comparer<TValue>.Default.Compare(_Selector(x), _Selector(y));
}

public class PropertyComparer : IComparer
{
    public static PropertyComparer<T, TValue> Create<T, TValue>(Func<T, TValue> Selector) => new(Selector);

    private readonly Func<object, object> _Selector;

    public PropertyComparer(Func<object, object> Selector) => _Selector = Selector.NotNull();

    public int Compare(object? x, object? y) => Comparer.Default.Compare(_Selector(x), _Selector(y));
}