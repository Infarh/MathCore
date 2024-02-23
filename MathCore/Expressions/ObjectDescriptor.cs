#nullable enable
using System.ComponentModel;
using System.Reflection;

#if NET8_0_OR_GREATER
using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;
#else
using DisallowNullAttribute = MathCore.Annotations.DisallowNullAttribute;
#endif


// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class ObjectDescriptor(object obj) : ObjectDescriptor<object>(obj);

public class ObjectDescriptor<T>([DisallowNull] T obj)
{
    private const BindingFlags __BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly Type _ObjectType = obj.GetType();
    private DictionaryReadOnly<string, Property>? _Properties;
    private DictionaryReadOnly<string, Field>? _Fields;
    private Func<PropertyInfo, bool>? _PropertiesFilter;
    private Func<FieldInfo, bool>? _FieldsFilter;

    public T Obj => obj;

    public bool IsNotifyPropertyChanged { get; private set; } = obj is INotifyPropertyChanged;

    public DictionaryReadOnly<string, Property> Property
    {
        get
        {
            if(_Properties != null) return _Properties;
            var properties = _ObjectType.GetProperties(__BindingFlags)
               .Where(_PropertiesFilter ?? (_ => true))
               .Select(p => new Property(obj, p)).ToArray();
            _Properties = new(properties.ToDictionary(p => p.Name));
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

    public DictionaryReadOnly<string, Field> Fields
    {
        get
        {
            if(_Fields != null) return _Fields;
            var fields = _ObjectType.GetFields(__BindingFlags)
               .Where(_FieldsFilter ?? (_ => true))
               .Select(p => new Field(obj, p)).ToArray();
            _Fields = new(fields.ToDictionary(p => p.Name));
            return _Fields;
        }
    }
}