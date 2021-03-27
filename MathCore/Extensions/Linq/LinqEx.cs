using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Extensions.Linq
{
    public static class LinqEx
    {
        [NotNull]
        public static IEnumerable<Q> GetObjects<T, Q>(this IEnumerable<T> collection, Func<T, IEnumerable<Q>> selector) => collection.SelectMany(selector);

        [NotNull]
        public static Dictionary<TKey, T[]> ToDictionaryArrays<T, TKey>(
            [NotNull] this IEnumerable<T> collection,
            [NotNull] Func<T, TKey> KeySelector) =>
            collection.GroupBy(KeySelector).ToDictionary(v => v.Key, v => v.ToArray());

        [NotNull]
        public static Dictionary<TKey, TValue[]> ToDictionaryArrays<T, TKey, TValue>(
            [NotNull] this IEnumerable<T> collection,
            [NotNull] Func<T, TKey> KeySelector, [NotNull] Func<T, TValue> ValueSelector) =>
            collection.GroupBy(KeySelector).ToDictionary(v => v.Key, v => v.ToArray(ValueSelector));
    }
}