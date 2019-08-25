
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    public class ObjectDescriptor : ObjectDescriptor<object>
    {
        public ObjectDescriptor(object obj) : base(obj) { }

    }

    public class ObjectDescriptor<T>
    {
        private const BindingFlags c_BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly T _Object;
        private readonly Type _ObjectType;
        private DictionaryReadOnly<string, Property> _Properties;
        private DictionaryReadOnly<string, Field> _Fields;
        private Func<PropertyInfo, bool> _PropertiesFilter;
        private Func<FieldInfo, bool> _FieldsFilter;

        public T Obj => _Object;

        public bool IsNotifyPropertyChanged { get; private set; }

        public DictionaryReadOnly<string, Property> Property
        {
            get
            {
                if(_Properties != null) return _Properties;
                var lv_Properties = _ObjectType.GetProperties(c_BindingFlags)
                            .Where(_PropertiesFilter ?? (p => true))
                            .Select(p => new Property(_Object, p)).ToArray();
                _Properties = new DictionaryReadOnly<string, Property>(lv_Properties.ToDictionary(p => p.Name));
                return _Properties;
            }
        }

        public Func<PropertyInfo, bool> PropertiesFilter
        {
            get => _PropertiesFilter;
            set
            {
                if(ReferenceEquals(_PropertiesFilter, value)) return;
                _PropertiesFilter = value;
                _Properties = null;
            }
        }
        public Func<FieldInfo, bool> FieldsFilter
        {
            get => _FieldsFilter;
            set
            {
                if(ReferenceEquals(_FieldsFilter, value)) return;
                _FieldsFilter = value;
                _Fields = null;
            }
        }

        public DictionaryReadOnly<string, Field> Fields
        {
            get
            {
                if(_Fields != null) return _Fields;
                var lv_Fields = _ObjectType.GetFields(c_BindingFlags)
                            .Where(_FieldsFilter ?? (f => true))
                            .Select(p => new Field(_Object, p)).ToArray();
                _Fields = new DictionaryReadOnly<string, Field>(lv_Fields.ToDictionary(p => p.Name));
                return _Fields;
            }
        }


        public ObjectDescriptor(T obj)
        {
            _Object = obj;
            _ObjectType = obj.GetType();
            IsNotifyPropertyChanged = obj is INotifyPropertyChanged;
        }
    }
}
