namespace MathCore.Extensions;

/// <summary>Предоставляет методы расширения для группировочных операций</summary>
public static class GroupingExtensions
{
    /// <summary>Разбирает <see cref="IGrouping{TKey, TElement}"/> на ключ и значения</summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значений.</typeparam>
    /// <param name="group">Группировка для разбора.</param>
    /// <param name="key">Ключ группировки.</param>
    /// <param name="values">Значения группировки.</param>
    public static void Deconstruct<TKey, TValue>(this IGrouping<TKey, TValue> group, out TKey key, out IEnumerable<TValue> values)
    {
        // Назначить ключ группировки параметру 'key' out
        key = group.Key;
        
        // Назначить значения группировки параметру 'values' out
        values = group;
    }
}
