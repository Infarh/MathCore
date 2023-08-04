#nullable enable
namespace MathCore.Extensions.Linq;

public static class LinqEx
{
    public static IEnumerable<Q> GetObjects<T, Q>(this IEnumerable<T> collection, Func<T, IEnumerable<Q>> selector) => collection.SelectMany(selector);

    public static Dictionary<TKey, T[]> ToDictionaryArrays<T, TKey>(
        this IEnumerable<T> collection,
        Func<T, TKey> KeySelector) =>
        collection.GroupBy(KeySelector).ToDictionary(v => v.Key, v => v.ToArray());

    public static Dictionary<TKey, TValue[]> ToDictionaryArrays<T, TKey, TValue>(
        this IEnumerable<T> collection,
        Func<T, TKey> KeySelector, Func<T, TValue> ValueSelector) =>
        collection.GroupBy(KeySelector).ToDictionary(v => v.Key, v => v.ToArray(ValueSelector));
}