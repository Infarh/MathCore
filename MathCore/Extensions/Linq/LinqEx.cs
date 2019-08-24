using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MathCore.Annotations;

namespace MathCore.Extentions.Linq
{
    public static class LinqEx
    {
        [NotNull]
        public static IEnumerable<Q> GetObjects<T, Q>(this IEnumerable<T> collection, Func<T, IEnumerable<Q>> selector) => collection.SelectMany(selector);

        [NotNull]
        public static Dictionary<TKey, T[]> ToDictionaryArrays<T, TKey>(
            [NotNull] this IEnumerable<T> collection,
            [NotNull] Func<T, TKey> KeySelector)
        {
            var d = new Dictionary<TKey, List<T>>();
            foreach(var value in collection)
            {
                var key = KeySelector(value);
                List<T> list;
                if(!d.ContainsKey(key))
                    d.Add(key, list = new List<T>());
                else
                    list = d[key];
                list.Add(value);
            }
            return d.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
        }

        [NotNull]
        public static Dictionary<TKey, TValue[]> ToDictionaryArrays<T, TKey, TValue>(
            [NotNull] this IEnumerable<T> collection,
            [NotNull] Func<T, TKey> KeySelector, [NotNull] Func<T, TValue> ValueSelector)
        {
            var d = new Dictionary<TKey, List<TValue>>();
            foreach(var v in collection)
            {
                var key = KeySelector(v);
                var value = ValueSelector(v);
                List<TValue> list;
                if(!d.ContainsKey(key))
                    d.Add(key, list = new List<TValue>());
                else
                    list = d[key];
                list.Add(value);
            }
            return d.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
        }
    }
}
