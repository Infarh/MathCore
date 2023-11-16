#nullable enable
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MathCore;

/// <summary>Безопасный для доступа к ключам словарь</summary>
/// <typeparam name="TKey">Тип ключа</typeparam>
/// <typeparam name="TValue">Тип значения</typeparam>
[Serializable, XmlRoot("Dictionary")]
public class DictionaryKeySafe<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable
{
    #region Поля

    /// <summary>Исходный словарь</summary>
    private readonly IDictionary<TKey, TValue> _Dictionary;

    #endregion

    #region Свойства

    /// <summary>Число элементов словаря</summary>
    public int Count => _Dictionary.Count;

    /// <summary>Ключи словаря</summary>
    public ICollection<TKey> Keys => _Dictionary.Keys;

    /// <summary>Значения словаря</summary>
    public ICollection<TValue> Values => _Dictionary.Values;

    /// <summary>Элемент словаря, либо default, если ключ отсутствует</summary>
    /// <param name="key">Ключ требуемого значения</param>
    /// <returns>Значение словаря по указанному ключу, либо default</returns>
    public TValue? this[TKey key]
    {
        get => _Dictionary.TryGetValue(key, out var v) ? v : default;
        set => _Dictionary[key] = value!;
    }

    public bool IsReadOnly => _Dictionary.IsReadOnly;

    #endregion

    #region Конструктор

    /// <summary>инициализация нового пустого безопасного словаря</summary>
    public DictionaryKeySafe() => _Dictionary = new Dictionary<TKey, TValue>();

    /// <summary>инициализация нового пустого безопасного словаря</summary>
    /// <param name="Capacity">Базовая ёмкость</param>
    public DictionaryKeySafe(int Capacity) => _Dictionary = new Dictionary<TKey, TValue>(Capacity);

    /// <summary>инициализация нового пустого безопасного словаря</summary>
    /// <param name="Comparer">Объект сравнения ключей</param>
    public DictionaryKeySafe(IEqualityComparer<TKey> Comparer) => _Dictionary = new Dictionary<TKey, TValue>(Comparer);

    /// <summary>инициализация нового пустого безопасного словаря</summary>
    /// <param name="Dictionary">Исходный словарь</param>
    public DictionaryKeySafe(IDictionary<TKey, TValue> Dictionary) => _Dictionary = Dictionary;

    #endregion

    #region Методы

    /// <inheritdoc />
    public void Add(TKey key, TValue value) => _Dictionary.Add(key, value);

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item) => _Dictionary.Add(item);

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) => _Dictionary.Remove(item);

    /// <inheritdoc />
    public bool Remove(TKey key) => _Dictionary.Remove(key);

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) => _Dictionary.Contains(item);

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => _Dictionary.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue? value) => _Dictionary.TryGetValue(key, out value);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int Index) => _Dictionary.CopyTo(array, Index);

    /// <inheritdoc />
    public void Clear() => _Dictionary.Clear();

    #endregion

    #region IEnumerable

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Dictionary).GetEnumerator();

    #endregion

    public static implicit operator DictionaryKeySafe<TKey, TValue>(Dictionary<TKey, TValue> d) => new(d);

    public static implicit operator Dictionary<TKey, TValue>(DictionaryKeySafe<TKey, TValue> d) => d._Dictionary as Dictionary<TKey, TValue> ?? new Dictionary<TKey, TValue>(d._Dictionary);

    #region IXmlSerializable

    /// <inheritdoc />
    XmlSchema? IXmlSerializable.GetSchema() => null;

    private static XmlSerializer? GetSerializer(Type type) =>
        type.IsPrimitive || type == typeof(string)
            ? null
            : type.GetXmlSerializer();

    /// <inheritdoc />
    void IXmlSerializable.ReadXml(XmlReader reader)
    {
        var key_serializer   = GetSerializer(typeof(TKey));
        var value_serializer = GetSerializer(typeof(TValue));

        var was_empty = reader.IsEmptyElement;
        reader.Read();

        if (was_empty) return;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            var key   = default(TKey);
            var value = default(TValue);

            if (key_serializer is null)
                key = (TKey)Convert.ChangeType(reader.GetAttribute("key"), typeof(TKey));
            if (value_serializer is null)
                value = (TValue)Convert.ChangeType(reader.GetAttribute("value"), typeof(TValue));

            if (reader.HasValue)
            {
                reader.ReadStartElement("item");

                if (key_serializer != null)
                {
                    reader.ReadStartElement("key");
                    key = (TKey)key_serializer.Deserialize(reader);
                    reader.ReadEndElement();
                }

                if (value_serializer != null)
                {
                    reader.ReadStartElement("value");
                    value = (TValue)value_serializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
            else reader.Skip();

            if (key != null) Add(key, value);

            reader.MoveToContent();
        }
        reader.ReadEndElement();

    }

    /// <inheritdoc />
    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
        var key_serializer   = GetSerializer(typeof(TKey));
        var value_serializer = GetSerializer(typeof(TValue));

        foreach (var key in Keys)
        {
            writer.WriteStartElement("item");

            if (key_serializer is null)
                writer.WriteAttributeString("key", Convert.ToString(key));
            else
            {
                writer.WriteStartElement("key");
                key_serializer.Serialize(writer, key);
                writer.WriteEndElement();
            }

            if (value_serializer is null)
                writer.WriteAttributeString("value", Convert.ToString(this[key]));
            else
            {
                writer.WriteStartElement("value");
                value_serializer.Serialize(writer, this[key]);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }

    #endregion
}