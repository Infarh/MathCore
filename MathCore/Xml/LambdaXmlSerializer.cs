using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MathCore.Annotations;

namespace MathCore.Xml
{
    public interface ISerializer<T, TResult>
    {
        TResult Serialize(T value);
        TResult Serialize(string Name, T value);
    }

    public abstract class LambdaXmlSerializer
    {
        public static LambdaXmlSerializer<T> Create<T>([CanBeNull] string RootName = null) => new LambdaXmlSerializer<T>(RootName ?? typeof(T).Name);
    }

    public class LambdaXmlSerializer<T> : LambdaXmlSerializer, ISerializer<T, XElement>
    {
        private static string __EmptyName = "item";

        public static string EmptyName { get => __EmptyName; set => __EmptyName = value; }

        [CanBeNull] private readonly string _ElementName;

        [NotNull] private readonly List<Func<T, object>> _Attributes = new List<Func<T, object>>();

        [NotNull] private readonly List<Func<T, object>> _Elements = new List<Func<T, object>>();

        public LambdaXmlSerializer([CanBeNull] string ElementName = null) => _ElementName = ElementName;

        [NotNull] public XElement Serialize(T value) => new XElement(_ElementName ?? __EmptyName, Content(value).ToArray());

        [NotNull] public XElement Serialize([CanBeNull] string Name, T value) => new XElement(Name ?? _ElementName ?? __EmptyName, Content(value).ToArray());

        [NotNull]
        private IEnumerable<object> Content(T value) => _Attributes.Select(a => a(value)).Concat(_Elements.Select(e => e(value)));

        [NotNull]
        public LambdaXmlSerializer<T> Attribute<TValue>([CanBeNull] string Name, [NotNull] Func<T, TValue> Selector)
        {
            _Attributes.Add(v => new XAttribute(Name ?? __EmptyName, Selector(v)));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, string> NameSelector,
            [NotNull] Func<TItem, TValue> ValueSelector)
        {
            _Attributes.Add(items => Selector(items).ToArray(item => new XAttribute(NameSelector(item), ValueSelector(item))));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, string> NameSelector,
            [NotNull] Func<TItem, bool> NeedToSerialize,
            [NotNull] Func<TItem, TValue> ValueSelector)
        {
            _Attributes.Add(items => Selector(items).Where(NeedToSerialize).ToArray(item => new XAttribute(NameSelector(item), ValueSelector(item))));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, int, string> NameSelector,
            [NotNull] Func<TItem, TValue> ValueSelector)
        {
            _Attributes.Add(items => Selector(items).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item))));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, int, string> NameSelector,
            [NotNull] Func<TItem, bool> NeedToSerialize,
            [NotNull] Func<TItem, TValue> ValueSelector)
        {
            _Attributes.Add(items => Selector(items).Where(NeedToSerialize).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item))));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, string> NameSelector,
            [NotNull] Func<TItem, int, TValue> ValueSelector)
        {
            _Attributes.Add(element => Selector(element).ToArray((item, i) => new XAttribute(NameSelector(item), ValueSelector(item, i))));
            return this;
        }

        public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
            [NotNull] Func<T, IEnumerable<TItem>> Selector,
            [NotNull] Func<TItem, int, string> NameSelector,
            [NotNull] Func<TItem, bool> NeedToSerialize,
            [NotNull] Func<TItem, int, TValue> ValueSelector)
        {
            _Attributes.Add(element => Selector(element).Where(NeedToSerialize).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item, i))));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Element<TValue>(
            [CanBeNull] string Name,
            [NotNull] Func<T, TValue> Selector)
        {
            _Elements.Add(v => new XElement(Name ?? __EmptyName, Selector(v)));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Element<TValue>(
            [CanBeNull] string Name, 
            [NotNull] Func<T, TValue> Selector,
            [NotNull] Func<TValue, bool> NeedToSerialize)
        {
            _Elements.Add(item =>
            {
                var value = Selector(item);
                return NeedToSerialize(value) ? new XElement(Name ?? __EmptyName, value) : null;
            });
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Element<TValue>(
            [CanBeNull] string Name,
            [NotNull] Func<T, TValue> Selector,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>(Name);
            Configurator(serializer);
            _Elements.Add(value => serializer.Serialize(Selector(value)));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Element<TValue>(
            [CanBeNull] string Name,
            [NotNull] Func<T, TValue> Selector,
            [NotNull] Func<TValue, bool> NeedToSerialize,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>(Name);
            Configurator(serializer);
            _Elements.Add(item =>
            {
                var value = Selector(item);
                return NeedToSerialize(value) ? serializer.Serialize(value) : null;
            });
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [CanBeNull] string Name,
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>(Name);
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).ToArray(serializer.Serialize));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [CanBeNull] string Name,
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Func<TValue, bool> NeedToSerialize,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>(Name);
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray(serializer.Serialize));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Func<TValue, string> NameSelector,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>();
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).ToArray(v => serializer.Serialize(NameSelector(v), v)));
            return this;
        }  

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Func<TValue, string> NameSelector,
            [NotNull] Func<TValue, bool> NeedToSerialize,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>();
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray(v => serializer.Serialize(NameSelector(v), v)));
            return this;
        }

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Func<TValue, int, string> NameSelector,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>();
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).ToArray((v, i) => serializer.Serialize(NameSelector(v, i), v)));
            return this;
        } 

        [NotNull]
        public LambdaXmlSerializer<T> Elements<TValue>(
            [NotNull] Func<T, IEnumerable<TValue>> ElementsSelector,
            [NotNull] Func<TValue, int, string> NameSelector,
            [NotNull] Func<TValue, bool> NeedToSerialize,
            [NotNull] Action<LambdaXmlSerializer<TValue>> Configurator)
        {
            var serializer = new LambdaXmlSerializer<TValue>();
            Configurator(serializer);
            _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray((v, i) => serializer.Serialize(NameSelector(v, i), v)));
            return this;
        }

        public LambdaXmlSerializer<T> Value()
        {
            _Elements.Add(v => v);
            return this;
        } 

        public LambdaXmlSerializer<T> Value<TValue>(TValue Value)
        {
            _Elements.Add(v => Value);
            return this;
        } 

        public LambdaXmlSerializer<T> Value<TValue>(Func<T, TValue> Selector)
        {
            _Elements.Add(v => Selector(v));
            return this;
        }
    }
}
