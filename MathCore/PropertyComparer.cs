using System;
using System.Collections;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore
{
    public class PropertyComparer<T, TValue> : IComparer<T>
    {
        [NotNull] private readonly Func<T, TValue> _Selector;

        public PropertyComparer([NotNull] Func<T, TValue> Selector) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));

        public int Compare(T x, T y) => Comparer<TValue>.Default.Compare(_Selector(x), _Selector(y));
    }

    public class PropertyComparer : IComparer
    {
        [NotNull] public static PropertyComparer<T, TValue> Create<T, TValue>([NotNull] Func<T, TValue> Selector) => new PropertyComparer<T, TValue>(Selector);

        [NotNull] private readonly Func<object, object> _Selector;

        public PropertyComparer([NotNull] Func<object, object> Selector) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));

        public int Compare(object x, object y) => Comparer.Default.Compare(_Selector(x), _Selector(y));
    }
}