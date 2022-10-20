using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MathCore.Annotations;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class ObjectDescriptor : ObjectDescriptor<object>
{
    public ObjectDescriptor([NotNull] object obj) : base(obj) { }

}

public class ObjectDescriptor<T>
{
    private const BindingFlags __BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly T _Object;
    private readonly Type _ObjectType;
    private DictionaryReadOnly<string, Property> _Properties;
    private DictionaryReadOnly<string, Field> _Fields;
    private Func<PropertyInfo, bool> _PropertiesFilter;
    private Func<FieldInfo, bool> _FieldsFilter;

    public T Obj => _Object;

    public bool IsNotifyPropertyChanged { get; private set; }

    [NotNull]
    public DictionaryReadOnly<string, Property> Property
    {
        get
        {
            if(_Properties != null) return _Properties;
            var properties = _ObjectType.GetProperties(__BindingFlags)
               .Where(_PropertiesFilter ?? (_ => true))
               .Select(p => new Property(_Object, p)).ToArray();
            _Properties = new DictionaryReadOnly<string, Property>(properties.ToDictionary(p => p.Name));
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
            _Properties       = null;
        }
    }
    public Func<FieldInfo, bool> FieldsFilter
    {
        get => _FieldsFilter;
        set
        {
            if(ReferenceEquals(_FieldsFilter, value)) return;
            _FieldsFilter = value;
            _Fields       = null;
        }
    }

    [NotNull]
    public DictionaryReadOnly<string, Field> Fields
    {
        get
        {
            if(_Fields != null) return _Fields;
            var fields = _ObjectType.GetFields(__BindingFlags)
               .Where(_FieldsFilter ?? (_ => true))
               .Select(p => new Field(_Object, p)).ToArray();
            _Fields = new DictionaryReadOnly<string, Field>(fields.ToDictionary(p => p.Name));
            return _Fields;
        }
    }


    public ObjectDescriptor([NotNull] T obj)
    {
        _Object                 = obj;
        _ObjectType             = obj.GetType();
        IsNotifyPropertyChanged = obj is INotifyPropertyChanged;
    }
}