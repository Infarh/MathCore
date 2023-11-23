#nullable enable
using System.Collections.ObjectModel;

namespace MathCore;

/// <summary>Коллекция, ключи которой определяются указанным методом на основе значений элементов</summary>
/// <typeparam name="TKey">Тип ключа</typeparam>
/// <typeparam name="TValue">Тип значения элемента</typeparam>
// ReSharper disable once UnusedMember.Global
public class LambdaKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue> where TKey : notnull
{
    /// <summary>Метод определения ключа элемента</summary>
    private readonly Func<TValue, TKey> _KeyExtractor;

    /// <summary>Инициализация новой коллекции, ключи которой определяются на основе значений элементов</summary>
    /// <param name="KeyExtractor">Метод определения ключа элемента</param>
    public LambdaKeyedCollection(Func<TValue, TKey> KeyExtractor) => _KeyExtractor = KeyExtractor ?? throw new ArgumentNullException(nameof(KeyExtractor));

    /// <summary>Инициализация новой коллекции, ключи которой определяются на основе значений элементов</summary>
    /// <param name="KeyExtractor">Метод определения ключа элемента</param>
    /// <param name="Values">Коллекция элементов</param>
    public LambdaKeyedCollection(Func<TValue, TKey> KeyExtractor, IEnumerable<TValue> Values)
    {
        _KeyExtractor = KeyExtractor ?? throw new ArgumentNullException(nameof(KeyExtractor));
        foreach (var value in Values) 
            Add(value);
    }

    /// <inheritdoc />
    protected override TKey GetKeyForItem(TValue item) => _KeyExtractor(item);
}