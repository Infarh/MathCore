namespace MathCore.Extensions;

public static class GroupingExtensions
{
    public static void Deconstruct<TKey, TValue>(this IGrouping<TKey, TValue> group, out TKey key, out IEnumerable<TValue> values)
    {
        key = group.Key;
        values = group;
    }
}
