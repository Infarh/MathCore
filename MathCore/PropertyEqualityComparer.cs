#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace MathCore;

public class PropertyEqualityComparer<T, TValue> : IEqualityComparer<T>
{
    private readonly Func<T, TValue> _Selector;

    public PropertyEqualityComparer(Func<T, TValue> Selector) => _Selector = Selector.NotNull();

    public bool Equals(T? x, T? y) => EqualityComparer<TValue>.Default.Equals(_Selector(x), _Selector(y));

    public int GetHashCode(T obj) => EqualityComparer<TValue>.Default.GetHashCode(_Selector(obj));
}

public class PropertyEqualityComparer : IEqualityComparer
{
    public static PropertyEqualityComparer<T, TValue> Create<T, TValue>(Func<T, TValue> Selector) => new(Selector);

    private readonly Func<object, object> _Selector;

    public PropertyEqualityComparer(Func<object, object> Selector) => _Selector = Selector.NotNull();

    public new bool Equals(object x, object y) => object.Equals(_Selector(x), _Selector(y));

    public int GetHashCode(object obj) => _Selector(obj).GetHashCode();
}