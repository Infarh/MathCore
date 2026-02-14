using System.Reflection;
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable AnnotateNotNullTypeMember

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class Field<T> : ItemBase
{
    private readonly FieldInfo _FieldInfo = null!;

    private readonly Action<T> _Writer = null!;
    private readonly Func<T> _Reader;
    private AttributesExtractor _Attributes = null;

    public bool IsReadOnly => (_FieldInfo.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;

    public Action<T> Writer => _Writer;
    public Func<T> Reader => _Reader;

    public T Value
    {
        get => _Reader();
        set
        {
            if (IsReadOnly) throw new NotSupportedException();
            _Writer(value);
        }
    }

    public AttributesExtractor Attribute => _Attributes ??= new(_FieldInfo);

    [Diagnostics.CodeAnalysis.SuppressMessage("Качество кода", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private static void Set(ref T field, T value) => field = value;

    public Field(Type type, string Name, bool IsPublicOnly = true)
        : base(type, Name)
    {
        var value_type = typeof(T);
        _FieldInfo = _ObjectType.GetField(Name, BindingFlags.Static | (IsPublicOnly
            ? BindingFlags.Public
            : BindingFlags.Public | BindingFlags.NonPublic)).NotNull();
        var field = Expression.Field(null, _FieldInfo);
        var ReaderExpr = Expression.Lambda<Func<T>>(field);
        _Reader = ReaderExpr.Compile();
        if (IsReadOnly) return;
        var value = Expression.Parameter(value_type, "value");
        var method_info = typeof(Field<T>).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Не найден метод Set");
        var call = Expression.Call(null,
            method_info, field, value);
        var expr = Expression.Lambda<Action<T>>(call, value);
        _Writer = expr.Compile();
    }

    public Field(object Obj, string Name, bool IsPublicOnly = true)
        : this(Obj, Obj.GetType().GetField(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)) ?? throw new InvalidOperationException($"Не найдена информация о поле {Name}")) { }

    public Field(object Obj, FieldInfo info)
        : base(Obj, info.Name)
    {
        _FieldInfo = info.NotNull();
        var value_type = info.FieldType;
        var ObjConstant = Expression.Constant(Obj);
        var field = Expression.Field(ObjConstant, Name);
        var ReaderExpr = Expression.Lambda<Func<T>>(field);
        _Reader = ReaderExpr.Compile();

        if (IsReadOnly) return;

        var value = Expression.Parameter(value_type, "value");
        var method_info = typeof(Field<T>).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Не найден метод Set");

        var call = Expression.Call(null, method_info, field, value);
        var expr = Expression.Lambda<Action<T>>(call, value);

        _Writer = expr.Compile();
    }
}

public class Field : ItemBase
{
    private readonly FieldInfo _FieldInfo = null!;

    private readonly Action<object> _Writer = null!;
    private readonly Func<object> _Reader;
    private AttributesExtractor _Attributes = null!;

    public bool IsReadOnly => (_FieldInfo.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;

    public Action<object> Writer => _Writer;

    public Func<object> Reader => _Reader;

    public object Value
    {
        get => _Reader();
        set
        {
            if (IsReadOnly) throw new NotSupportedException();
            _Writer(value);
        }
    }

    public AttributesExtractor Attribute => _Attributes ??= new(_FieldInfo);

    [Diagnostics.CodeAnalysis.SuppressMessage("Качество кода", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private static void Set(ref object field, object value) => field = value;

    public Field(Type type, string Name, bool IsPublicOnly = true)
        : base(type, Name)
    {
        _FieldInfo = _ObjectType.GetField(Name, BindingFlags.Static | (IsPublicOnly
            ? BindingFlags.Public
            : BindingFlags.Public | BindingFlags.NonPublic)).NotNull();

        var value_type = _FieldInfo.FieldType;

        var field = Expression.Field(null, _FieldInfo);
        var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(field, typeof(object)));
        _Reader = ReaderExpr.Compile();

        if (IsReadOnly) return;
        var value = Expression.Parameter(value_type, "value");
        var method_info = typeof(Field).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Не найден метод Set");
        var call = Expression.Call(null, method_info, field, value);
        var expr = Expression.Lambda<Action<object>>(call, value);
        _Writer = expr.Compile();
    }

    public Field(object Obj, string Name, bool IsPublicOnly = true)
        : this(Obj, Obj.GetType().GetField(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)) ?? throw new InvalidOperationException($"Не найдена информация о поле {Name}")) { }

    public Field(object Obj, FieldInfo info)
        : base(Obj, info.Name)
    {
        _FieldInfo = info.NotNull();

        var value_type = info.FieldType;

        var ObjConstant = Expression.Constant(Obj);
        var field = Expression.Field(ObjConstant, Name);
        var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(field, typeof(object)));
        _Reader = ReaderExpr.Compile();
        if (IsReadOnly) return;
        var value = Expression.Parameter(value_type, "value");
        var method_info = typeof(Field).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Не найден метод Set");
        var call = Expression.Call(null, method_info, field, value);
        var expr = Expression.Lambda<Action<object>>(call, value);
        _Writer = expr.Compile();
    }
}