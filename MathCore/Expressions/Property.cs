#nullable enable
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Reactive;
using System.Reflection;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

/// <summary>Свойство класса</summary>
/// <typeparam name="T">Тип значения свойства</typeparam>
public class Property<T> : ItemBase, INotifyPropertyChanged, IObservableEx<T>
{
    public static Expression<Func<TObject, T>> GetExtractorExpression<TObject>(string PropertyName, bool IsPublicOnly)
    {
        var type = typeof(TObject);
        var info = type.GetProperty(PropertyName, BindingFlags.Instance |
                (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic))
            ?? throw new InvalidOperationException($"Свойство {PropertyName} не найдено в типе {type}");

        var expr_object = Expression.Parameter(typeof(object), "obj");
        return expr_object
           .ConvertTo(type)
           .GetProperty(info)
           .CreateLambda<Func<TObject, T>>(expr_object);
    }

    public static Func<TObject, T> GetExtractor<TObject>(string PropertyName, bool IsPublicOnly) => GetExtractorExpression<TObject>(PropertyName, IsPublicOnly).Compile();


    /// <summary>Событие изменения свойства</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Метод генерации события изменения свойства</summary>
    /// <param name="PropertyName">Имя свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        _ObservableObject.OnNext(Value);
    }

    /// <summary>Информация о свойстве</summary>
    private readonly PropertyInfo _PropertyInfo;

    /// <summary>Метод чтения свойства</summary>
    private readonly Func<T> _Reader;

    /// <summary>Метод записи значения свойства</summary>
    private readonly Action<T> _Writer;

    private readonly SimpleObservableEx<T> _ObservableObject = new();

    private AttributesExtractor _Attributes;

    /// <summary>Признак возможности чтения значения свойства</summary>
    public bool CanRead => _Reader != null;

    /// <summary>Признак возможности записи значения свойства</summary>
    public bool CanWrite => _Writer != null;

    /// <summary>Метод записи значения свойства</summary>
    public Action<T> Writer => _Writer;

    /// <summary>Метод чтения значения свойства</summary>
    public Func<T> Reader => _Reader;

    /// <summary>Значение свойства</summary>
    public T Value
    {
        get
        {
            if(!CanRead) throw new NotSupportedException();
            return _Reader();
        }
        set
        {
            if(!CanWrite) throw new NotSupportedException();
            _Writer(value);
        }
    }

    /// <summary>Свойство доступа к значению атрибута <see cref="ComponentModel.DescriptionAttribute"/></summary>
    public string DescriptionAttribute { get; private set; }

    /// <summary>Отображаемое имя</summary>
    public string DisplayName { get; private set; }

    /// <summary>Признак реализации объектом-хозяином свойства интерфейса <see cref="INotifyPropertyChanged"/></summary>
    public bool IsNotifyPropertyChanged { get; }

    public AttributesExtractor Attributes => _Attributes ??= new AttributesExtractor(_PropertyInfo);

    public PropertyInfo Info => _PropertyInfo;

    public Type PropertyType => _PropertyInfo.PropertyType;

    /// <summary>Инициализация доступа к статическому свойству</summary>
    /// <param name="type">Рассматриваемый тип</param>
    /// <param name="Name">Имя статического свойства</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(Type type, string Name, bool IsPublicOnly = true)
        : base(type, Name)
    {
        IsNotifyPropertyChanged = false;
        _PropertyInfo = _ObjectType.GetProperty(Name, BindingFlags.Static
                | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic))
            ?? throw new InvalidOperationException($"Свойство {Name} не найдено в типе {_ObjectType}");

        LoadAttributes();

        var ValueParameter = Expression.Parameter(_PropertyInfo.PropertyType, "value");
        if(_PropertyInfo.CanRead)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var body        = Expression.Property(null, Name);
            var reader_expr = Expression.Lambda<Func<T>>(body);
            _Reader = reader_expr.Compile();
        }

        if(_PropertyInfo.CanWrite)
        {
            var set_method_info = _PropertyInfo.GetSetMethod(!IsPublicOnly);
            var writer_expr     = Expression.Lambda<Action<T>>(Expression.Call(null, set_method_info, ValueParameter), ValueParameter);
            _Writer = writer_expr.Compile();
            // ReSharper disable once UseNameofExpression
            _Writer += _ => OnPropertyChanged("Value");
        }
    }

    /// <summary>Инициализация доступа к свойству объекта</summary>
    /// <param name="Obj">Рассматриваемый объект</param>
    /// <param name="Name">Имя свойства</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(object Obj, string Name, bool IsPublicOnly = true)
        : this(Obj, Obj.GetType().GetProperty(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)) ?? throw new InvalidOperationException($"Свойство {Name} не найдено в типе {Obj.GetType()}"))
    { }


    /// <summary>Инициализация доступа к свойству объекта</summary>
    /// <param name="Obj">Рассматриваемый объект</param>
    /// <param name="info">Информация о свойстве</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(object Obj, PropertyInfo info, bool IsPublicOnly = true)
        : base(Obj, info.Name)
    {
        _PropertyInfo = info;
        Debug.Assert(_PropertyInfo != null, "_PropertyInfo != null");
        LoadAttributes();

        var ObjConstant    = Obj.ToExpression();
        var ValueParameter = Expression.Parameter(info.PropertyType, "value");
        if(_PropertyInfo.CanRead)
        {
            var body       = Expression.Property(ObjConstant, Name);
            var ReaderExpr = Expression.Lambda<Func<T>>(body);
            _Reader = ReaderExpr.Compile();
        }

        if(_PropertyInfo.CanWrite)
        {
            var set_method_info = _PropertyInfo.GetSetMethod(!IsPublicOnly);
            var writer_expr     = Expression.Lambda<Action<T>>(Expression.Call(ObjConstant, set_method_info, ValueParameter), ValueParameter);
            _Writer = writer_expr.Compile();
            // ReSharper disable once UseNameofExpression
            if(!IsNotifyPropertyChanged) _Writer += _ => OnPropertyChanged("Value");
        }

        // ReSharper disable once AssignmentInConditionalExpression
        if(IsNotifyPropertyChanged = Obj is INotifyPropertyChanged)
            ((INotifyPropertyChanged)Obj).PropertyChanged += (_, e) => { if(e.PropertyName == _Name) OnPropertyChanged(nameof(Value)); };
    }

    /// <summary>Чтение сведений из атрибутов</summary>
    private void LoadAttributes()
    {
        var description_attributes = _PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if(description_attributes.Length > 0)
            DescriptionAttribute = ((DescriptionAttribute)description_attributes[0]).Description;

        var name_attributes = _PropertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
        DisplayName = name_attributes.Length > 0 ? ((DisplayNameAttribute)name_attributes[0]).DisplayName : Name;
    }

    #region Implementation of IObservableEx<T>

    public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);
    public IDisposable Subscribe(IObserverEx<T> observer) => _ObservableObject.Subscribe(observer);

    #endregion
}

/// <summary>Свойство класса</summary>
public class Property : ItemBase, INotifyPropertyChanged, IObservable<object>
{

    /// <summary>Событие изменения свойства</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Метод генерации события изменения свойства</summary>
    /// <param name="PropertyName">Имя свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        _ObservableObject.OnNext(Value);
    }

    /// <summary>Информация о свойстве</summary>
    private readonly PropertyInfo _PropertyInfo;

    /// <summary>Метод чтения свойства</summary>
    private readonly Func<object> _Reader;

    /// <summary>Метод записи значения свойства</summary>
    private readonly Action<object> _Writer;

    private readonly SimpleObservableEx<object> _ObservableObject = new();

    private AttributesExtractor _Attributes;

    /// <summary>Признак возможности чтения значения свойства</summary>
    public bool CanRead => _Reader != null;

    /// <summary>Признак возможности записи значения свойства</summary>
    public bool CanWrite => _Writer != null;

    /// <summary>Метод записи значения свойства</summary>
    public Action<object> Writer => _Writer;

    /// <summary>Метод чтения значения свойства</summary>
    public Func<object> Reader => _Reader;

    /// <summary>Свойство доступа к значению атрибута <see cref="ComponentModel.DescriptionAttribute"/></summary>
    public string DescriptionAttribute { get; private set; }

    /// <summary>Отображаемое имя</summary>
    public string DisplayName { get; private set; }

    /// <summary>Признак реализации объектом-хозяином свойства интерфейса <see cref="INotifyPropertyChanged"/></summary>
    public bool IsNotifyPropertyChanged { get; }

    public PropertyInfo Info => _PropertyInfo;

    public Type PropertyType => _PropertyInfo.PropertyType;

    /// <summary>Значение свойства</summary>
    public object Value
    {
        get
        {
            if(!CanRead) throw new NotSupportedException();
            return _Reader();
        }
        set
        {
            if(!CanWrite) throw new NotSupportedException();
            var value_type = value.GetType();
            if(value_type != _PropertyInfo.PropertyType)
                value = _PropertyInfo.PropertyType.GetCasterFrom(value_type)(value);
            _Writer(value);
        }
    }

    public object ValueUnsafe
    {
        get
        {
            if(!CanRead) throw new NotSupportedException();
            return _Reader();
        }
        set
        {
            if(!CanWrite) throw new NotSupportedException();
            _Writer(value);
        }
    }

    public object ValueSafe
    {
        get
        {
            if(!CanRead) throw new NotSupportedException();
            return _Reader();
        }
        set
        {
            if(!CanWrite) throw new NotSupportedException();
            var value_type       = value.GetType();
            var destination_type = PropertyType;
            if(value_type != destination_type)
                if(value_type != typeof(string))
                    value = destination_type.GetCasterFrom(value_type)(value);
                else
                {
                    var converter = TypeDescriptor.GetConverter(destination_type);
                    if(!converter.CanConvertFrom(value_type))
                        throw new InvalidCastException(
                            $"Невозможно преобразовать значение типа {value_type} в {destination_type}");
                    value = converter.ConvertTo(value, destination_type);
                }
            _Writer(value);
        }
    }

    public AttributesExtractor Attribute => _Attributes ??= new AttributesExtractor(_PropertyInfo);

    /// <summary>Инициализация доступа к статическому свойству</summary>
    /// <param name="type">Рассматриваемый тип</param>
    /// <param name="Name">Имя статического свойства</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(Type type, string Name, bool IsPublicOnly = true)
        : base(type, Name)
    {
        IsNotifyPropertyChanged = false;

        _PropertyInfo = _ObjectType.GetProperty(Name, BindingFlags.Static
                | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic))
            ?? throw new InvalidOperationException($"Свойство {Name} не найдено в типе {type}");
        var p = Expression.Parameter(_PropertyInfo.PropertyType, "value");

        var object_type = typeof(object);
        if(_PropertyInfo.CanRead)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var body        = Expression.Property(null, Name);
            var reader_expr = Expression.Lambda<Func<object>>(Expression.Convert(body, object_type));
            _Reader = reader_expr.Compile();
        }

        if(_PropertyInfo.CanWrite)
        {
            var set_method_info = _PropertyInfo.GetSetMethod(!IsPublicOnly);
            var writer_expr = Expression.Lambda<Action<object>>(Expression.Call(null, set_method_info, Expression.Convert(p, _PropertyInfo.PropertyType)), p);
            _Writer = writer_expr.Compile();
            // ReSharper disable UseNameofExpression
            _Writer += _ => OnPropertyChanged("Value");
            _Writer += _ => OnPropertyChanged("ValueUnsafe");
            _Writer += _ => OnPropertyChanged("ValueSafe");
            // ReSharper restore UseNameofExpression
        }
    }

    /// <summary>Инициализация доступа к свойству объекта</summary>
    /// <param name="Obj">Рассматриваемый объект</param>
    /// <param name="Name">Имя свойства</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(object Obj, string Name, bool IsPublicOnly = true)
        : this(Obj, Obj.GetType().GetProperty(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)) ?? throw new InvalidOperationException($"Свойство {Name} не найдено в типе {Obj.GetType()}"))
    { }

    //private static T Cast<T>(object obj) { return typeof(T) (T)obj; }

    /// <summary>Инициализация доступа к свойству объекта</summary>
    /// <param name="Obj">Рассматриваемый объект</param>
    /// <param name="info">Информация о свойстве</param>
    /// <param name="IsPublicOnly">Признак публичности свойства</param>
    public Property(object Obj, PropertyInfo info, bool IsPublicOnly = true)
        : base(Obj, info.Name)
    {
        _PropertyInfo = info;
        Debug.Assert(_PropertyInfo != null, "_FieldInfo != null");
        LoadAttributes();

        var obj_constant = Expression.Constant(Obj);
        var object_type = typeof(object);
        if(_PropertyInfo.CanRead)
        {
            var body        = Expression.Property(obj_constant, Name);
            var reader_expr = Expression.Lambda<Func<object>>(Expression.Convert(body, object_type));
            _Reader = reader_expr.Compile();
        }

        //var q = typeof(double).Cast(4);

        if(_PropertyInfo.CanWrite)
        {
            var set_method_info = IsPublicOnly ? info.GetSetMethod() ?? info.GetSetMethod(true) : info.GetSetMethod(true);
            var p               = Expression.Parameter(typeof(object), "value");
            var call            = Expression.Call(Expression.Constant(Obj), set_method_info, Expression.Convert(p, info.PropertyType));
            var writer_expr     = Expression.Lambda<Action<object>>(call, p);
            _Writer = writer_expr.Compile();
            if(!IsNotifyPropertyChanged)
            {
                // ReSharper disable UseNameofExpression
                _Writer += _ => OnPropertyChanged("Value");
                _Writer += _ => OnPropertyChanged("ValueSafe");
                _Writer += _ => OnPropertyChanged("ValueUnsafe");
                // ReSharper restore UseNameofExpression
            }
        }

        // ReSharper disable once AssignmentInConditionalExpression
        if(IsNotifyPropertyChanged = Obj is INotifyPropertyChanged)
            ((INotifyPropertyChanged)Obj).PropertyChanged += (_, e) => { if(e.PropertyName == _Name) OnPropertyChanged(nameof(Value)); };
    }


    /// <summary>Чтение сведений из атрибутов</summary>
    private void LoadAttributes()
    {
        var description_attributes = _PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if(description_attributes.Length > 0)
            DescriptionAttribute = ((DescriptionAttribute)description_attributes[0]).Description;

        var name_attributes = _PropertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
        DisplayName = name_attributes.Length > 0 ? ((DisplayNameAttribute)name_attributes[0]).DisplayName : Name;
    }

    #region Implementation of IObservable<object>

    public IDisposable Subscribe(IObserver<object> observer) => _ObservableObject.Subscribe(observer);

    #endregion
}