using System;
using System.Collections;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore
{
    public class PropertyEqualityComparer<T, TValue> : IEqualityComparer<T>
    {
        [NotNull] private readonly Func<T, TValue> _Selector;

        public PropertyEqualityComparer([NotNull] Func<T, TValue> Selector) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));

        public bool Equals(T x, T y) => EqualityComparer<TValue>.Default.Equals(_Selector(x), _Selector(y));

        public int GetHashCode(T obj) => EqualityComparer<TValue>.Default.GetHashCode(_Selector(obj));
    }

    public class PropertyEqualityComparer : IEqualityComparer
    {
        [NotNull] public static PropertyEqualityComparer<T, TValue> Create<T, TValue>([NotNull] Func<T, TValue> Selector) => new(Selector);

        [NotNull] private readonly Func<object, object> _Selector;

        public PropertyEqualityComparer([NotNull] Func<object, object> Selector) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));

        public new bool Equals(object x, object y) => object.Equals(_Selector(x), _Selector(y));

        public int GetHashCode(object obj) => _Selector(obj).GetHashCode();
    }
}
