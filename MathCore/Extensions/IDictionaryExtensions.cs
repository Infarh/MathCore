using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>Класс методов-расширений для интерфейса <see cref="IDictionary{T,V}"/></summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class IDictionaryExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> item, out TKey key, out TValue value)
        {
            key = item.Key;
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
            [NotNull] this IDictionary<TKey, IList<TValue>> dictionary,
            [NotNull] TKey key,
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
            [NotNull] this IDictionary<TKey, IList<TValue>> dictionary,
            TObject obj,
            [NotNull] Func<TObject, TKey> KeySelector,
            [NotNull] Func<TObject, TValue> ValueSelector
        ) =>
            dictionary.AddValue(KeySelector(obj), ValueSelector(obj));

        /// <summary>Метод добавления значения в словарь списков значений</summary>
        /// <param name="dictionary">Словарь списков <see cref="IList{TValue}"/> значений типа <typeparamref name="TValue"/></param>
        /// <param name="value">Значение, записываемое в словарь</param>
        /// <param name="KeySelector">Метод извлечения ключа из указанного значения</param>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения списка</typeparam>
        public static void AddValue<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, IList<TValue>> dictionary,
            TValue value,
            [NotNull] Func<TValue, TKey> KeySelector
        ) =>
            dictionary.AddValue(KeySelector(value), value);

        /// <summary>Добавление значений в словарь</summary>
        /// <param name="dictionary">Словарь в который надо добавить значения</param>
        /// <param name="collection">Коллекция добавляемух значений</param>
        /// <param name="converter">Метод определения ключа словаря для каждого из элементов коллекции</param>
        /// <typeparam name="TKey">ТИп ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        public static void AddValues<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] IEnumerable<TValue> collection,
            [NotNull] Func<TValue, TKey> converter
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
            [NotNull] this Dictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] Func<TValue> creator
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
            [NotNull] this Dictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> creator
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
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] Func<TValue> creator
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
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] Func<TKey, TValue> creator
        )
        {
            if (!dictionary.TryGetValue(key, out var value))
                dictionary.Add(key, value = creator(key));
            return value;
        }

        /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
        /// <param name="dictionary">Рассматриваемый словарь</param>
        /// <param name="key">Ключ, значение для которого требуется получить</param>
        /// <param name="DefaultValue">Значение по-умолчанию, которое будет добавлено в словарь с указанным ключём, если он отсутствует</param>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <returns>Значение словаря для указанного ключа, либо указанное значение по-умолчанию</returns>
        public static TValue GetValue<TKey, TValue>
        (
            [NotNull] this Dictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            TValue DefaultValue = default
        ) =>
            dictionary.TryGetValue(key, out var v) ? v : DefaultValue;

        /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
        /// <param name="dictionary">Рассматриваемый словарь</param>
        /// <param name="key">Ключ, значение для которого требуется получить</param>
        /// <param name="DefaultValue">Значение по-умолчанию, которое будет добавлено в словарь с указанным ключём, если он отсутствует</param>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <returns>Значение словаря для указанного ключа, либо указанное значение по-умолчанию</returns>
        public static TValue GetValue<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            TValue DefaultValue = default
        ) =>
            dictionary.TryGetValue(key, out var value) ? value : DefaultValue;

        /// <summary>Получить значение из словаря в случае его наличия, или добавить новое</summary>
        /// <param name="dictionary">Рассматриваемый словарь</param>
        /// <param name="name">Название объекта, значение для которого требуется получить</param>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <returns>Значение словаря для указанного ключа</returns>
        public static TValue GetValue<TValue>([NotNull] this Dictionary<string, object> dictionary, [NotNull] string name) => dictionary.TryGetValue(name, out var value) ? (TValue)value : default;

        /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
        /// <param name="dictionary">Инициализируемый словарь</param>
        /// <param name="count">Количество добавляемых элементов</param>
        /// <param name="initializer">Метод генерации новых элементов</param>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        /// <returns>Инициализированный словарь</returns>
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            int count,
            [NotNull] Func<KeyValuePair<TKey, TValue>> initializer
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
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            int count,
            [CanBeNull] TParameter parameter,
            [NotNull] Func<TParameter, KeyValuePair<TKey, TValue>> initializer
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
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            int count,
            [NotNull] Func<int, KeyValuePair<TKey, TValue>> initializer
        )
        {
            for (var i = 0; i < count; i++)
                dictionary.Add(initializer(i));

            return dictionary;
        }

        /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
        /// <param name="dictionary">Инициализируемый словарь</param>
        /// <param name="count">Количество добавляемых элементов</param>
        /// <param name="parameter">Параметр инициалализации</param>
        /// <param name="initializer">Метод генерации новых элементов</param>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
        /// <returns>Инициализированный словарь</returns>
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            int count,
            [NotNull] TParameter parameter,
            [NotNull] Func<int, TParameter, KeyValuePair<TKey, TValue>> initializer
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
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] IEnumerable<TKey> keys,
            [NotNull] Func<TKey, TValue> initializer
        )
        {
            foreach (var key in keys)
                dictionary.Add(key, initializer(key));

            return dictionary;
        }

        /// <summary>Инициализация словаря указанным методом для указанного числа значений</summary>
        /// <param name="dictionary">Инициализируемый словарь</param>
        /// <param name="keys">Коллекция ключей</param>
        /// <param name="parameter">Параметр инициалализации</param>
        /// <param name="initializer">Метод генерации новых элементов</param>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
        /// <returns>Инициализированный словарь</returns>
        [NotNull]
        public static IDictionary<TKey, TValue> Initialize<TKey, TValue, TParameter>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] IEnumerable<TKey> keys,
            [CanBeNull] TParameter parameter,
            [NotNull] Func<TKey, TParameter, TValue> initializer
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
        [NotNull]
        public static KeyValuePair<TKey, TValue>[] RemoveWhere<TKey, TValue>
        (
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] Func<KeyValuePair<TKey, TValue>, bool> selector
        )
        {
            var to_remove = dictionary.Where(selector).ToArray();
            foreach (var remove in to_remove)
                dictionary.Remove(remove.Key);
            return to_remove;
        }

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
    }
}