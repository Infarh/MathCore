using System.Diagnostics;
using System.Reflection;

namespace System.Linq.Expressions
{
    public class Field<T> : ItemBase
    {
        private readonly FieldInfo _FieldInfo;

        private readonly Action<T> _Writer;
        private readonly Func<T> _Reader;
        private AttributesExtractor _Attributes;

        public bool IsReadOnly => (_FieldInfo.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;

        public Action<T> Writer => _Writer;
        public Func<T> Reader => _Reader;

        public T Value
        {
            get { return _Reader(); }
            set
            {
                if(IsReadOnly) throw new NotSupportedException();
                _Writer(value);
            }
        }

        public AttributesExtractor Attribute => _Attributes ?? (_Attributes = new AttributesExtractor(_FieldInfo));


        // ReSharper disable once RedundantAssignment
        private static void Set(ref T field, T value) { field = value; }

        public Field(Type type, string Name, bool IsPublicOnly = true)
            : base(type, Name)
        {
            var lv_ValueType = typeof(T);
            _FieldInfo = _ObjectType.GetField(Name, BindingFlags.Static | (IsPublicOnly
                                                          ? BindingFlags.Public
                                                          : BindingFlags.Public | BindingFlags.NonPublic));
            Debug.Assert(_FieldInfo != null, "_FieldInfo != null");
            var field = Expression.Field(null, _FieldInfo);
            var ReaderExpr = Expression.Lambda<Func<T>>(field);
            _Reader = ReaderExpr.Compile();
            if(IsReadOnly) return;
            var value = Expression.Parameter(lv_ValueType, "value");
            var call = Expression.Call(null,
                typeof(Field<T>).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic), field, value);
            var expr = Expression.Lambda<Action<T>>(call, value);
            _Writer = expr.Compile();
        }

        public Field(object Obj, string Name, bool IsPublicOnly = true)
            : this(Obj, Obj.GetType().GetField(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic))) { }

        public Field(object Obj, FieldInfo info)
            : base(Obj, info.Name)
        {
            _FieldInfo = info;
            Debug.Assert(_FieldInfo != null, "_FieldInfo != null");
            var lv_ValueType = info.FieldType;
            var ObjConstant = Expression.Constant(Obj);
            var field = Expression.Field(ObjConstant, Name);
            var ReaderExpr = Expression.Lambda<Func<T>>(field);
            _Reader = ReaderExpr.Compile();
            if(IsReadOnly) return;
            var value = Expression.Parameter(lv_ValueType, "value");
            var call = Expression.Call(null, typeof(Field<T>).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic), field, value);
            var expr = Expression.Lambda<Action<T>>(call, value);
            _Writer = expr.Compile();
        }
    }

    public class Field : ItemBase
    {
        private readonly FieldInfo _FieldInfo;

        private readonly Action<object> _Writer;
        private readonly Func<object> _Reader;
        private AttributesExtractor _Attributes;

        public bool IsReadOnly => (_FieldInfo.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;

        public Action<object> Writer => _Writer;
        public Func<object> Reader => _Reader;

        public object Value
        {
            get { return _Reader(); }
            set
            {
                if(IsReadOnly) throw new NotSupportedException();
                _Writer(value);
            }
        }

        public AttributesExtractor Attribute => _Attributes ?? (_Attributes = new AttributesExtractor(_FieldInfo));

        // ReSharper disable once RedundantAssignment
        private static void Set(ref object field, object value) { field = value; }

        public Field(Type type, string Name, bool IsPublicOnly = true)
            : base(type, Name)
        {
            _FieldInfo = _ObjectType.GetField(Name, BindingFlags.Static | (IsPublicOnly
                                                          ? BindingFlags.Public
                                                          : BindingFlags.Public | BindingFlags.NonPublic));
            Debug.Assert(_FieldInfo != null, "_FieldInfo != null");

            var lv_ValueType = _FieldInfo.FieldType;

            var field = Expression.Field(null, _FieldInfo);
            var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(field, typeof(object)));
            _Reader = ReaderExpr.Compile();

            if(IsReadOnly) return;
            var value = Expression.Parameter(lv_ValueType, "value");
            var call = Expression.Call(null, typeof(Field).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic), field, value);
            var expr = Expression.Lambda<Action<object>>(call, value);
            _Writer = expr.Compile();
        }

        public Field(object Obj, string Name, bool IsPublicOnly = true)
            : this(Obj, Obj.GetType().GetField(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic))) { }

        public Field(object Obj, FieldInfo info)
            : base(Obj, info.Name)
        {
            _FieldInfo = info;
            Debug.Assert(_FieldInfo != null, "_FieldInfo != null");

            var lv_ValueType = info.FieldType;

            var ObjConstant = Expression.Constant(Obj);
            var field = Expression.Field(ObjConstant, Name);
            var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(field, typeof(object)));
            _Reader = ReaderExpr.Compile();
            if(IsReadOnly) return;
            var value = Expression.Parameter(lv_ValueType, "value");
            var call = Expression.Call(null, typeof(Field).GetMethod("Set", BindingFlags.Static | BindingFlags.NonPublic), field, value);
            var expr = Expression.Lambda<Action<object>>(call, value);
            _Writer = expr.Compile();
        }
    }
}