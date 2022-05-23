using System.ComponentModel;
using MathCore.Annotations;
using MathCore.Values;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    ///<summary>"Свойство" позднего связывания</summary>
    ///<typeparam name="TObject">Тип объекта, для которого определяется свойство</typeparam>
    ///<typeparam name="TValue">Тип значения свойства</typeparam>
    public class Property<TObject, TValue> : IValue<TValue>
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Событие возникает если свойство <see cref="Value"/> изменило своё значение</summary>
        public event EventHandler ValueChanged;

        /// <summary>Генерация события <see cref="ValueChanged"/></summary>
        /// <param name="E">Аргумент события</param>
        protected virtual void OnValueChanged([CanBeNull] EventArgs E = null) => ValueChanged.Start(this, E ?? EventArgs.Empty);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Информация о свойстве</summary>
        private PropertyInfo _PropertyInfo;
        /// <summary>Имя свойства</summary>
        private string _Name;
        /// <summary>Объект, которому принадлежит свойство</summary>
        private TObject _Object;
        /// <summary>Флаг приватности свойства</summary>
        private bool _Private;

        /// <summary>Действие, осуществляющее установку значения свойства</summary>
        private Action<TValue> _SetMethod;

        /// <summary>Функция, вычисляющая значение свойства</summary>
        private Func<TValue> _GetMethod;

        /// <summary>Описание свойства</summary>
        private PropertyDescriptor _Descriptor;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Тип значения свойства</summary>
        [NotNull]
        public Type PropertyType => _PropertyInfo.PropertyType;

        ///<summary>Имя свойства</summary>
        public string Name { get => _Name; set => Initialize(_Object, value, _Private); }

        ///<summary>Объект, определяющий свойство</summary>
        public TObject Object { get => _Object; set => Initialize(value, _Name, _Private); }

        ///<summary>Признак - является ли свойство приватным</summary>
        public bool Private { get => _Private; set => Initialize(_Object, _Name, _Private = value); }

        ///<summary>Признак </summary>
        public bool IsExist => _PropertyInfo != null;

        ///<summary>Значение свойства</summary>
        public TValue Value { get => _GetMethod(); set => _SetMethod(value); }

        ///<summary>Признак возможности читать значение</summary>
        public bool CanRead => _PropertyInfo != null && _PropertyInfo.CanRead;

        /// <summary>Признак возможности устанавливать значение</summary>
        public bool CanWrite => _PropertyInfo != null && _PropertyInfo.CanWrite;

        /// <summary>Поддерживает генерацию событий изменения значения</summary>
        public bool SupportsChangeEvents => _Descriptor != null && _Descriptor.SupportsChangeEvents;

        ///<summary>Атрибуты свойства</summary>
        public PropertyAttributes Attributes => _PropertyInfo.Attributes;

        /// <summary>Дескриптор свойства объекта</summary>
        public PropertyDescriptor Descriptor => _Descriptor;

        /// <summary>Значение <see cref="DisplayNameAttribute.DisplayName"/></summary>
        public string DisplayName { get; private set; }

        /// <summary>Значение <see cref="DescriptionAttribute.Description"/></summary>
        public string Description { get; private set; }

        /* ------------------------------------------------------------------------------------------ */

        ///<summary>Новый объект "Свойство" для позднего связывания</summary>
        ///<param name="Name">Имя свойства</param>
        ///<param name="Private">Является ли свойство скрытым</param>
        public Property([NotNull] string Name, bool Private = false) : this(default, Name, Private) { }

        ///<summary>Новый объект "Свойство" для позднего связывания</summary>
        ///<param name="o">Объект, для которого определяется свойство</param>
        ///<param name="Name">Имя свойства</param>
        ///<param name="Private">Является ли свойство скрытым</param>
        public Property(TObject o, [NotNull] string Name, bool Private = false) => Initialize(o, Name, Private);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация данных о свойстве</summary>
        /// <param name="o">Объект, свойство которого требуется контролировать</param>
        /// <param name="PropertyName">Имя запрашиваемого свойства</param>
        /// <param name="IsPrivate">Искать непубличные свойства</param>
        private void Initialize([CanBeNull] TObject o, [NotNull] string PropertyName, bool IsPrivate)
        {
            if (_Descriptor != null
                && _Object != null
                && !ReferenceEquals(o, _Object)
                && _Name != PropertyName)
                _Descriptor.RemoveValueChanged(o.IsNotNull(), PropertyValueChanged);

            _Object = o;
            _Name = PropertyName;
            _Private = IsPrivate;

            _Descriptor = _Object != null
                ? TypeDescriptor.GetProperties(_Object).Find(PropertyName, true)
                : TypeDescriptor.GetProperties(typeof(TObject)).Find(PropertyName, true);
            if (_Descriptor != null && _Object != null && _Descriptor.SupportsChangeEvents)
                _Descriptor.AddValueChanged(_Object, PropertyValueChanged);


            _GetMethod = GetValue;
            _SetMethod = SetValue;

            var type = typeof(TObject);
            if (type == typeof(object) && o != null)
                type = o.GetType();

            var is_static = o is null ? BindingFlags.Static : BindingFlags.Instance;
            var is_public = IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;

            _PropertyInfo = type.GetProperty(PropertyName, is_static | is_public);

            if (_PropertyInfo is null)
            {
                var get_method = new Method<TObject, TValue>(o, $"get_{PropertyName}", IsPrivate);
                var set_method = new Method<TObject, object>(o, $"set_{PropertyName}", IsPrivate);

                _GetMethod = () => get_method.Invoke();
                _SetMethod = value => set_method.Invoke(value);
            }

            if (o is not ISynchronizeInvoke obj) return;

            _GetMethod = () => (TValue)obj.Invoke(_GetMethod, null);
            _SetMethod = value => obj.Invoke(_SetMethod, new object[] { value });

            Description = _PropertyInfo?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            DisplayName = _PropertyInfo?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? _PropertyInfo?.Name;
        }

        //private void LoadAttributes()
        //{
        //    var description_attributes = _PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    if(description_attributes.Length > 0)
        //        DescriptionAttribute = ((DescriptionAttribute)description_attributes[0]).Description;

        //    var name_attributes = _PropertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
        //    DisplayName = name_attributes.Length > 0 ? ((DisplayNameAttribute)name_attributes[0]).DisplayName : Name;
        //}

        /// <summary>Обработчик события изменения значения свойства</summary>
        /// <param name="Sender">Источник события - объект, свойство которого контролируется</param>
        /// <param name="Args">Аргумент события</param>
        private void PropertyValueChanged(object Sender, EventArgs Args) => OnValueChanged(EventArgs.Empty);

        /// <summary>Метод, позволяющий установить значение свойства</summary>
        /// <param name="value">Устанавливаемое значение свойства</param>
        private void SetValue(TValue value) { if (CanWrite) _PropertyInfo.SetValue(_Object, value, null); }

        /// <summary>Метод, позволяющий получить значение свойства</summary>
        /// <returns>Значение свойства</returns>
        [CanBeNull]
        private TValue GetValue() => CanRead ? (TValue)_PropertyInfo.GetValue(_Object, null) : default;

        /* ------------------------------------------------------------------------------------------ */

        /// <inheritdoc />
        [NotNull]
        public override string ToString()
        {
            var property_type = _Object is null ? "Static property" : "Property";

            if (_PropertyInfo is null)
                return $"Incorrect {property_type.ToLower()} of {typeof(TObject)} name {_Name}";

            var value = CanRead ? $" = {Value}" : string.Empty;
            var host = typeof(TObject).Name;
            return string.Format("{0}({4}{5}{6}): {1}.{2}{3}", property_type, host, _Name, value,
                CanRead ? "R" : string.Empty, CanWrite ? "W" : string.Empty, SupportsChangeEvents ? "E" : string.Empty);
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}