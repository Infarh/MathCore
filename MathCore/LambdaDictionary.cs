﻿#nullable enable
using System.Collections;
using System.Collections.ObjectModel;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore;

public class LambdaDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> _ElementsGetter;

    private readonly Action<TKey, TValue>? _ElementSetter;

    private readonly Action? _Clear;

    private readonly Func<TKey, bool>? _Remove;

    /// <inheritdoc />
    public bool IsReadOnly => _ElementSetter is null;

    /// <inheritdoc />
    public int Count => _ElementsGetter().Count();

    /// <inheritdoc />
    public ICollection<TKey> Keys => new Collection<TKey>(_ElementsGetter().Select(v => v.Key).ToList());

    /// <inheritdoc />
    public ICollection<TValue> Values => new Collection<TValue>(_ElementsGetter().Select(v => v.Value).ToList());

    /// <inheritdoc />
    public TValue? this[TKey key]
    {
        get => _ElementsGetter().Where(v => Equals(v.Key, key)).Select(v => v.Value).FirstOrDefault();
        set => Add(key, value);
    }

    public LambdaDictionary(
        Func<IEnumerable<KeyValuePair<TKey, TValue>>> ElementsGetter,
        Action<TKey, TValue>? ElementSetter = null,
        Action? Clear = null,
        Func<TKey, bool>? Remove = null
    )
    {
        _ElementsGetter = ElementsGetter ?? throw new ArgumentNullException(nameof(ElementsGetter), "Не задан метод получения значения");
        _ElementSetter  = ElementSetter;
        _Clear          = Clear;
        _Remove         = Remove;
    } 

    // ReSharper disable once UnusedParameter.Local
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    // ReSharper disable once AnnotateNotNullParameter
    private static void CheckSupported(Delegate action, string message)
    {
        if(action is null)
            throw new NotSupportedException(message);
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    /// <inheritdoc />
    public void Add(TKey key, TValue value) => _ElementSetter.NotNull("Словарь не поддерживает операции записи").Invoke(key, value);

    /// <inheritdoc />
    public void Clear() => _Clear.NotNull("Словарь не поддерживает операцию очистки")();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) => _ElementsGetter().Contains(item);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int ArrayIndex)
    {
        foreach (var item in _ElementsGetter())
            array[ArrayIndex++] = item;
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    /// <inheritdoc />
    public bool Remove(TKey key) => _Remove.NotNull("Словарь не поддерживает операцию удаления")(key);

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => _ElementsGetter().Contains(v => Equals(v.Key, key));

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value)
    {
        foreach (var v in _ElementsGetter().Where(v => Equals(v.Key, key)))
        {
            value = v.Value;
            return true;
        }
        value = default;
        return false;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _ElementsGetter().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}