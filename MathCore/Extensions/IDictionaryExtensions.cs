#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

using MathCore;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Класс методов-расширений для интерфейса <see cref="IDictionary{T,V}"/></summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class IDictionaryExtensions
{
    /// <summary>Деконструктор пары ключ-значение на составляющие</summary>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="item">Деконструируемое значение</param>
    /// <param name="key">Значение ключа</param>
    /// <param name="value">Значение значения</param>
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> item, out TKey key, out TValue value)
    {
        key   = item.Key;
        value = item.Value;
    }

    /// <summary>Метод добавления значения в словарь списков значений</summary>
    /// <param name="dictionary">Словарь списков <see cref="IList{TValue}"/> значений типа <typeparamref name="TValue"/></param>
    /// <param name="key">Ключ словаря типа <typeparamref name="TKey"/></param>
    /// <param name="value">Значение списка типа <typeparamref name="TValue"/></param>
    /// <typeparam name="TKey">Тип ключа словаря <paramref name="key"/></typeparam>
    /// <typeparam name="TValue">Тип значения списка значений <paramref name="value"/></typeparam>
    public static void AddValue<TKey, TValue>
    (
        this IDictionary<TKey, IList<TValue>> dictionary,
        [DisallowNull] TKey key,
        TValue value
    ) => dictionary.GetValueOrAddNew(key, () => new List<TValue>()).Add(value);

    /// <summary>Метод добавления значения в словарь списков значений</summary>
    /// <param name="dictionary">Словарь списков <see cref="IList{TValue}"/> значений типа <typeparamref name="TValue"/></param>
    /// <param name="obj">Объект-ключ словаря типа <typeparamref name="TObject"/></param>
    /// <param name="KeySelector">Метод образования ключа типа <typeparamref name="TKey"/> словаря из объекта типа <typeparamref name="TObject"/></param>
    /// <param name="ValueSelector">Метод образования значения типа <typeparamref name="TValue"/> из объекта типа <typeparamref name="TObject"/></param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TObject">Тип входного объекта</typeparam>
    /// <typeparam name="TValue">Тип значения списка</typeparam>
    public static void AddValue<TKey, TObject, TValue>
    (
        this IDictionary<TKey, IList<TValue>> dictionary,
        TObject obj,
        Func<TObject, TKey> KeySelector,
        Func<TObject, TValue> ValueSelector
    ) =>
        dictionary.AddValue(KeySelector(obj)!, ValueSelector(obj));

    /// <summary>Метод добавления значения в словарь списков значений</summary>
    /// <param name="dictionary">Словарь списков <see cref="IList{TValue}"/> значений типа <typeparamref name="TValue"/></param>
    /// <param name="value">Значение, записываемое в словарь</param>
    /// <param name="KeySelector">Метод извлечения ключа из указанного значения</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения списка</typeparam>
    public static void AddValue<TKey, TValue>
    (
        this IDictionary<TKey, IList<TValue>> dictionary,
        TValue value,
        Func<TValue, TKey> KeySelector
    ) =>
        dictionary.AddValue(KeySelector(value)!, value);

    /// <summary>Добавление значений в словарь</summary>
    /// <param name="dictionary">Словарь в который надо добавить значения</param>
    /// <param name="collection">Коллекция добавляемых значений</param>
    /// <param name="converter">Метод определения ключа словаря для каждого из элементов коллекции</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    public static void AddValues<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TValue> collection,
        Func<TValue, TKey> converter
    ) =>
        collection.AddToDictionary(dictionary, converter);

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ значения, которое надо получить</param>
    /// <param name="creator">Метод получения нового значения, заносимого в словарь при отсутствии в нём указанного ключа</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Полученное из словаря по указанному ключу значение, либо созданное вновь и помещённое значение указанным методом</returns>
    public static TValue GetValueOrAddNew<TKey, TValue>
    (
        this Dictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        Func<TValue> creator
    )
    {
        if (!dictionary.TryGetValue(key, out var value))
            dictionary.Add(key, value = creator());
        return value;
    }

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ значения, которое надо получить</param>
    /// <param name="creator">Метод получения нового значения по указанному ключу, заносимого в словарь при отсутствии в нём указанного ключа</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Полученное из словаря по указанному ключу значение, либо созданное вновь и помещённое значение указанным методом</returns>
    public static TValue GetValueOrAddNew<TKey, TValue>
    (
        this Dictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        Func<TKey, TValue> creator
    )
    {
        if (!dictionary.TryGetValue(key, out var value))
            dictionary.Add(key, value = creator(key));
        return value;
    }

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ значения, которое надо получить</param>
    /// <param name="creator">Метод получения нового значения, заносимого в словарь при отсутствии в нём указанного ключа</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Полученное из словаря по указанному ключу значение, либо созданное вновь и помещённое значение указанным методом</returns>
    public static TValue GetValueOrAddNew<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        Func<TValue> creator
    )
    {
        if (!dictionary.TryGetValue(key, out var value))
            dictionary.Add(key, value = creator());
        return value;
    }

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ значения, которое надо получить</param>
    /// <param name="creator">Метод получения нового значения, заносимого в словарь при отсутствии в нём указанного ключа</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Полученное из словаря по указанному ключу значение, либо созданное вновь и помещённое значение указанным методом</returns>
    public static TValue GetValueOrAddNew<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        Func<TKey, TValue> creator
    )
    {
        if (!dictionary.TryGetValue(key, out var value))
            dictionary.Add(key, value = creator(key));
        return value;
    }

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ, значение для которого требуется получить</param>
    /// <param name="DefaultValue">Значение по-умолчанию, которое будет добавлено в словарь с указанным ключом, если он отсутствует</param>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Значение словаря для указанного ключа, либо указанное значение по-умолчанию</returns>
    public static TValue? GetValue<TKey, TValue>
    (
        this Dictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        TValue? DefaultValue = default
    ) =>
        dictionary.TryGetValue(key, out var v) ? v : DefaultValue;

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="key">Ключ, значение для которого требуется получить</param>
    /// <param name="DefaultValue">Значение по-умолчанию, которое будет добавлено в словарь с указанным ключом, если он отсутствует</param>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Значение словаря для указанного ключа, либо указанное значение по-умолчанию</returns>
    public static TValue? GetValue<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        TValue? DefaultValue = default
    ) =>
        dictionary.TryGetValue(key, out var value) ? value : DefaultValue;

    /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="name">Название объекта, значение для которого требуется получить</param>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Значение словаря для указанного ключа</returns>
    public static TValue? GetValue<TValue>(this Dictionary<string, object> dictionary, string name) => dictionary.TryGetValue(name, out var value) ? (TValue)value : default;

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="count">Количество добавляемых элементов</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        int count,
        Func<KeyValuePair<TKey, TValue>> initializer
    )
    {
        for (var i = 0; i < count; i++)
            dictionary.Add(initializer());

        return dictionary;
    }

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="count">Количество добавляемых элементов</param>
    /// <param name="parameter">Параметр инициализации</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <typeparam name="TParameter">Тип параметра</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
    (
        this IDictionary<TKey, TValue> dictionary,
        int count,
        TParameter? parameter,
        Func<TParameter?, KeyValuePair<TKey, TValue>> initializer
    )
    {
        for (var i = 0; i < count; i++)
            dictionary.Add(initializer(parameter));

        return dictionary;
    }

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="count">Количество добавляемых элементов</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        int count,
        Func<int, KeyValuePair<TKey, TValue>> initializer
    )
    {
        for (var i = 0; i < count; i++)
            dictionary.Add(initializer(i));

        return dictionary;
    }

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="count">Количество добавляемых элементов</param>
    /// <param name="parameter">Параметр инициализации</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
    (
        this IDictionary<TKey, TValue> dictionary,
        int count,
        [DisallowNull] TParameter parameter,
        Func<int, TParameter, KeyValuePair<TKey, TValue>> initializer
    )
    {
        for (var i = 0; i < count; i++)
            dictionary.Add(initializer(i, parameter));

        return dictionary;
    }

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="keys">Коллекция ключей</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        Func<TKey, TValue> initializer
    )
    {
        foreach (var key in keys)
            dictionary.Add(key, initializer(key));

        return dictionary;
    }

    /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
    /// <param name="dictionary">Инициализируемый словарь</param>
    /// <param name="keys">Коллекция ключей</param>
    /// <param name="parameter">Параметр инициализации</param>
    /// <param name="initializer">Метод генерации новых элементов</param>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения словаря</typeparam>
    /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
    /// <returns>Инициализированный словарь</returns>
    public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
    (
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        TParameter? parameter,
        Func<TKey, TParameter?, TValue> initializer
    )
    {
        foreach (var key in keys)
            dictionary.Add(key, initializer(key, parameter));

        return dictionary;
    }

    /// <summary>Удаление из словаря элементов, удовлетворяющих предикату</summary>
    /// <param name="dictionary">Рассматриваемый словарь</param>
    /// <param name="selector">Метод отбора элементов</param>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Массив удалённых пар ключ-значение</returns>
    public static KeyValuePair<TKey, TValue>[] RemoveWhere<TKey, TValue>
    (
        this IDictionary<TKey, TValue> dictionary,
        Func<KeyValuePair<TKey, TValue>, bool> selector
    )
    {
        var to_remove = dictionary.Where(selector).ToArray();
        foreach (var remove in to_remove)
            dictionary.Remove(remove.Key);
        return to_remove;
    }

    /// <summary>Удалить записи с перечисленными ключами</summary>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="dictionary">Словарь, из которого требуется удалить данные</param>
    /// <param name="keys">Перечисление удаляемых ключей</param>
    /// <returns>Массив удалённых значений</returns>
    public static TValue[] RemoveItems<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
    {
        var result = new List<TValue>(dictionary.Count);
        foreach (var key in keys)
            if (dictionary.TryGetValue(key, out var value))
            {
                result.Add(value);
                dictionary.Remove(key);
            }
        return result.ToArray();
    }

    /// <summary>Преобразование перечисления кортежей двух элементов в словарь</summary>
    /// <typeparam name="TKey">Тип первого элемента кортежа - тип ключа</typeparam>
    /// <typeparam name="TValue">Тип второго элемента кортежа - тип значения</typeparam>
    /// <param name="items">Перечисление кортежей двух элементов</param>
    /// <returns>Словарь, составленный из ключей - первых элементов кортежа и значений - вторых элементов</returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> items) =>
        items.ToDictionary(value => value.Key, value => value.Value);

    /// <summary>Преобразовать в безопасный для ключей словарь</summary>
    /// <typeparam name="TKey">Тип ключа словаря</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Dictionary">Исходный словарь</param>
    /// <returns></returns>
    public static DictionaryKeySafe<TKey, TValue> ToSafeDictionary<TKey, TValue>(this IDictionary<TKey, TValue> Dictionary) =>
        Dictionary as DictionaryKeySafe<TKey, TValue> ?? new(Dictionary);

    /// <summary>Добавить значение в словарь, если ключ отсутствует</summary>
    /// <typeparam name="TKey">Тип ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="dictionary">Словарь</param>
    /// <param name="key">Ключ</param>
    /// <param name="value">Значение</param>
    /// <returns>Истина, если значение было добавлено</returns>
    public static bool AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key)) return false;
        dictionary.Add(key, value);
        return true;
    }

    /// <summary>Преобразование словаря значений в строку по заданному шаблону</summary>
    /// <typeparam name="TValue">Тип значений словаря</typeparam>
    /// <param name="dictionary">Словарь с данными для заполнения шаблона</param>
    /// <param name="Pattern">Шаблон строки</param>
    /// <param name="FormatProvider">Формат</param>
    /// <returns>Строка по заданному шаблону</returns>
    public static string ToPatternString<TValue>(
        this IDictionary<string, TValue> dictionary,
        string Pattern, 
        IFormatProvider? FormatProvider = null) =>
        dictionary is null 
            ? throw new ArgumentNullException(nameof(dictionary)) 
            : Regex.Replace(
                Pattern ?? throw new ArgumentNullException(nameof(Pattern)),
                @"{(?<name>\w+)(?::(?<format>.+?))?}",
                Match => !dictionary.TryGetValue(Match.Groups["name"].Value, out var value)
                    ? Match.Value
                    : value is IFormattable formattable_value && Match.Groups["format"].Success
                        ? formattable_value.ToString(Match.Groups["format"].Value, FormatProvider ?? CultureInfo.CurrentCulture)
                        : value?.ToString()
            );

    public static DictionaryKeySafe<TKey, TValue> ToKeySafeDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new(dictionary);
}