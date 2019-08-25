using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Reactive;
using System.Reflection;
using MathCore.Annotations;
using MathCore.Extentions.Expressions;

namespace System.Linq.Expressions
{
    /// <summary>Свойство класса</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    public class Property<T> : ItemBase, INotifyPropertyChanged, IObservable<T>
    {
        public static Expression<Func<TObject, T>> GetExtractorExpression<TObject>(string PropertyName, bool IsPublicOnly)
        {
            var type = typeof(TObject);
            var info = type.GetProperty(PropertyName, BindingFlags.Instance |
                (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic));

            var expr_object = Expression.Parameter(typeof(object), "obj");
            return expr_object
                .ConvertTo(type)
                .GetProperty(info)
                .CreateLambda<Func<TObject, T>>(expr_object);
        }

        public static Func<TObject, T> GetExtractor<TObject>(string PropertyName, bool IsPublicOnly)
        {
            return GetExtractorExpression<TObject>(PropertyName, IsPublicOnly).Compile();
        }


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

        private readonly SimpleObservableEx<T> _ObservableObject = new SimpleObservableEx<T>();

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

        /// <summary>Свойство доступа к значению аттрибута <see cref="ComponentModel.DescriptionAttribute"/></summary>
        public string DescriptionAttribute { get; private set; }

        /// <summary>Отображаемое имя</summary>
        public string DisplayName { get; private set; }

        /// <summary>Признак реализации объектом-хозяином свойства интерфейса <see cref="INotifyPropertyChanged"/></summary>
        public bool IsNotifyPropertyChanged { get; }

        public AttributesExtractor Attributes => _Attributes ?? (_Attributes = new AttributesExtractor(_PropertyInfo));

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
                | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic));

            LoadAttributes();

            var ValueParameter = Expression.Parameter(_PropertyInfo.PropertyType, "value");
            if(_PropertyInfo.CanRead)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var body = Expression.Property(null, Name);
                var ReaderExpr = Expression.Lambda<Func<T>>(body);
                _Reader = ReaderExpr.Compile();
            }

            if(_PropertyInfo.CanWrite)
            {
                var SetMethodInfo = _PropertyInfo.GetSetMethod(!IsPublicOnly);
                var WrieExpr = Expression.Lambda<Action<T>>(Expression.Call(null, SetMethodInfo, ValueParameter), ValueParameter);
                _Writer = WrieExpr.Compile();
                // ReSharper disable once UseNameofExpression
                _Writer += t => OnPropertyChanged("Value");
            }
        }

        /// <summary>Инициализация доступа к свойству объекта</summary>
        /// <param name="Obj">Рассматриваемый объект</param>
        /// <param name="Name">Имя свойства</param>
        /// <param name="IsPublicOnly">Признак публичности свойства</param>
        public Property(object Obj, string Name, bool IsPublicOnly = true)
            : this(Obj, Obj.GetType().GetProperty(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)))
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

            var ObjConstant = Obj.ToExpression();
            var ValueParameter = Expression.Parameter(info.PropertyType, "value");
            if(_PropertyInfo.CanRead)
            {
                var body = Expression.Property(ObjConstant, Name);
                var ReaderExpr = Expression.Lambda<Func<T>>(body);
                _Reader = ReaderExpr.Compile();
            }

            if(_PropertyInfo.CanWrite)
            {
                var SetMethodInfo = _PropertyInfo.GetSetMethod(!IsPublicOnly);
                var WrieExpr = Expression.Lambda<Action<T>>(Expression.Call(ObjConstant, SetMethodInfo, ValueParameter), ValueParameter);
                _Writer = WrieExpr.Compile();
                // ReSharper disable once UseNameofExpression
                if(!IsNotifyPropertyChanged) _Writer += t => OnPropertyChanged("Value");
            }

            // ReSharper disable once AssignmentInConditionalExpression
            if(IsNotifyPropertyChanged = Obj is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Obj).PropertyChanged += (s, e) => { if(e.PropertyName == _Name) OnPropertyChanged(nameof(Value)); };
        }

        /// <summary>Чтение сведений из аттрибутов</summary>
        private void LoadAttributes()
        {
            var description_attributes = _PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(description_attributes.Length > 0)
                DescriptionAttribute = ((DescriptionAttribute)description_attributes[0]).Description;

            var name_attributes = _PropertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            DisplayName = name_attributes.Length > 0 ? ((DisplayNameAttribute)name_attributes[0]).DisplayName : Name;
        }

        #region Implementation of IObservable<T>

        public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);

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

        private readonly SimpleObservableEx<object> _ObservableObject = new SimpleObservableEx<object>();

        private AttributesExtractor _Attributes;

        /// <summary>Признак возможности чтения значения свойства</summary>
        public bool CanRead => _Reader != null;

        /// <summary>Признак возможности записи значения свойства</summary>
        public bool CanWrite => _Writer != null;

        /// <summary>Метод записи значения свойства</summary>
        public Action<object> Writer => _Writer;

        /// <summary>Метод чтения значения свойства</summary>
        public Func<object> Reader => _Reader;

        /// <summary>Свойство доступа к значению аттрибута <see cref="ComponentModel.DescriptionAttribute"/></summary>
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
                var value_type = value.GetType();
                var lv_DestinationType = PropertyType;
                if(value_type != lv_DestinationType)
                    if(value_type != typeof(string))
                        value = lv_DestinationType.GetCasterFrom(value_type)(value);
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(lv_DestinationType);
                        if(!converter.CanConvertFrom(value_type))
                            throw new InvalidCastException(
                                        $"Невозможно преобразовать значение типа {value_type} в {lv_DestinationType}");
                        value = converter.ConvertTo(value, lv_DestinationType);
                    }
                _Writer(value);
            }
        }

        public AttributesExtractor Attribute => _Attributes ?? (_Attributes = new AttributesExtractor(_PropertyInfo));

        /// <summary>Инициализация доступа к статическому свойству</summary>
        /// <param name="type">Рассматриваемый тип</param>
        /// <param name="Name">Имя статического свойства</param>
        /// <param name="IsPublicOnly">Признак публичности свойства</param>
        public Property(Type type, string Name, bool IsPublicOnly = true)
            : base(type, Name)
        {
            IsNotifyPropertyChanged = false;

            _PropertyInfo = _ObjectType.GetProperty(Name, BindingFlags.Static
                  | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic));
            var p = Expression.Parameter(_PropertyInfo.PropertyType, "value");

            var object_type = typeof(object);
            if(_PropertyInfo.CanRead)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var body = Expression.Property(null, Name);
                var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(body, object_type));
                _Reader = ReaderExpr.Compile();
            }

            if(_PropertyInfo.CanWrite)
            {
                var SetMethodInfo = _PropertyInfo.GetSetMethod(!IsPublicOnly);
                var WriteExpr = Expression.Lambda<Action<object>>(Expression.Call(null, SetMethodInfo, Expression.Convert(p, _PropertyInfo.PropertyType)), p);
                _Writer = WriteExpr.Compile();
                // ReSharper disable UseNameofExpression
                _Writer += t => OnPropertyChanged("Value");
                _Writer += t => OnPropertyChanged("ValueUnsafe");
                _Writer += t => OnPropertyChanged("ValueSafe");
                // ReSharper restore UseNameofExpression
            }
        }

        /// <summary>Инициализация доступа к свойству объекта</summary>
        /// <param name="Obj">Рассматриваемый объект</param>
        /// <param name="Name">Имя свойства</param>
        /// <param name="IsPublicOnly">Признак публичности свойства</param>
        public Property(object Obj, string Name, bool IsPublicOnly = true)
            : this(Obj, Obj.GetType().GetProperty(Name, BindingFlags.Instance | (IsPublicOnly ? BindingFlags.Public : BindingFlags.Public | BindingFlags.NonPublic)))
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

            var ObjConstant = Expression.Constant(Obj);
            var object_type = typeof(object);
            if(_PropertyInfo.CanRead)
            {
                var body = Expression.Property(ObjConstant, Name);
                var ReaderExpr = Expression.Lambda<Func<object>>(Expression.Convert(body, object_type));
                _Reader = ReaderExpr.Compile();
            }

            //var q = typeof(double).Cast(4);

            if(_PropertyInfo.CanWrite)
            {
                var SetMethodInfo = IsPublicOnly ? info.GetSetMethod() ?? info.GetSetMethod(true) : info.GetSetMethod(true);
                var p = Expression.Parameter(typeof(object), "value");
                var call = Expression.Call(Expression.Constant(Obj), SetMethodInfo, Expression.Convert(p, info.PropertyType));
                var WriteExpr = Expression.Lambda<Action<object>>(call, p);
                _Writer = WriteExpr.Compile();
                if(!IsNotifyPropertyChanged)
                {
                    // ReSharper disable UseNameofExpression
                    _Writer += t => OnPropertyChanged("Value");
                    _Writer += t => OnPropertyChanged("ValueSafe");
                    _Writer += t => OnPropertyChanged("ValueUnsafe");
                    // ReSharper restore UseNameofExpression
                }
            }

            // ReSharper disable once AssignmentInConditionalExpression
            if(IsNotifyPropertyChanged = Obj is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Obj).PropertyChanged += (s, e) => { if(e.PropertyName == _Name) OnPropertyChanged(nameof(Value)); };
        }


        /// <summary>Чтение сведений из аттрибутов</summary>
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

    public class AttributesExtractor
    {
        private readonly MemberInfo _Info;
        public bool Inherit { get; set; }

        public Attribute this[string Name] => GetAttributes(Name, Inherit).FirstOrDefault();
        public Attribute this[string Name, bool Inherit] => GetAttributes(Name, Inherit).FirstOrDefault();

        public object this[string Name, string ValueName]
        {
            get
            {
                var attribute = this[Name];
                if(attribute == null) return null;
                var type = attribute.GetType();
                var property = type.GetProperty(ValueName, BindingFlags.Instance | BindingFlags.Public);
                if(property == null || !property.CanRead) return null;
                return property.GetValue(attribute, null);
            }
        }

        public AttributesExtractor(MemberInfo Info) => _Info = Info;

        public IEnumerable<Attribute> GetAttributes(string Name) { return GetAttributes(Name, Inherit); }

        public IEnumerable<Attribute> GetAttributes(string Name, bool Inherit)
        {
            return _Info.GetCustomAttributes(Inherit)
                            .Cast<Attribute>()
                            .Where(a => a.GetType().Name.StartsWith(Name));
        }
    }
}