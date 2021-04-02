using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using MathCore.Annotations;

namespace MathCore
{
    [Serializable, XmlRoot("Dictionary")]
    public class DictionaryKeySafe<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable
    {
        #region Поля

        [NotNull]
        private readonly IDictionary<TKey, TValue> _Dictionary;

        #endregion

        #region Свойства

        public int Count => _Dictionary.Count;
        public ICollection<TKey> Keys => _Dictionary.Keys;
        public ICollection<TValue> Values => _Dictionary.Values;

        public TValue this[TKey key]
        {
            get => _Dictionary.TryGetValue(key, out var v) ? v : default;
            set => _Dictionary[key] = value;
        }

        public bool IsReadOnly => _Dictionary.IsReadOnly;

        #endregion

        #region Конструктор

        public DictionaryKeySafe() => _Dictionary = new Dictionary<TKey, TValue>();

        public DictionaryKeySafe(int Capacity) => _Dictionary = new Dictionary<TKey, TValue>(Capacity);

        public DictionaryKeySafe(IEqualityComparer<TKey> Comparer) => _Dictionary = new Dictionary<TKey, TValue>(Comparer);

        public DictionaryKeySafe([NotNull] IDictionary<TKey, TValue> Dictionary) => _Dictionary = Dictionary;

        #endregion

        #region Методы

        public void Add(TKey key, TValue value) => _Dictionary.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => _Dictionary.Add(item);
        public bool Remove(KeyValuePair<TKey, TValue> item) => _Dictionary.Remove(item);

        public bool Remove(TKey key) => _Dictionary.Remove(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => _Dictionary.Contains(item);
        public bool ContainsKey(TKey key) => _Dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _Dictionary.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _Dictionary.CopyTo(array, arrayIndex);
        public void Clear() => _Dictionary.Clear();

        #endregion

        #region IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Dictionary).GetEnumerator();

        #endregion

        [NotNull] public static implicit operator DictionaryKeySafe<TKey, TValue>([NotNull] Dictionary<TKey, TValue> d) => new DictionaryKeySafe<TKey, TValue>(d);

        [NotNull] public static implicit operator Dictionary<TKey, TValue>([NotNull] DictionaryKeySafe<TKey, TValue> d) => d._Dictionary as Dictionary<TKey, TValue> ?? new Dictionary<TKey, TValue>(d._Dictionary);

        #region IXmlSerializable

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        [CanBeNull]
        private static XmlSerializer GetSerializer([NotNull] Type type) =>
            type.IsPrimitive || type == typeof(string)
                ? null
                : type.GetXmlSerializer();

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var key_serializer = GetSerializer(typeof(TKey));
            var value_serializer = GetSerializer(typeof(TValue));

            var was_empty = reader.IsEmptyElement;
            reader.Read();

            if (was_empty) return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                var key = default(TKey);
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
            var key_serializer = GetSerializer(typeof(TKey));
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
}
