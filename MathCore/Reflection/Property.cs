using System.ComponentModel;
using System.Diagnostics.Contracts;
using MathCore.Values;

namespace System.Reflection
{
    ///<summary>"Свойство" позднего связывания</summary>
    ///<typeparam name="TObject">Тип объекта, для которого определяется свойство</typeparam>
    ///<typeparam name="TValue">Тип значения свойства</typeparam>
    public class Property<TObject, TValue> : IValue<TValue>
    {
        /* ------------------------------------------------------------------------------------------ */

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs E) => ValueChanged.Start(this, EventArgs.Empty);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Информация о свойстве</summary>
        private PropertyInfo _PropertyInfo;
        /// <summary>Имя свйоства</summary>
        private string _Name;
        /// <summary>Объект, которому принадлежит свйоство</summary>
        private TObject _Object;
        /// <summary>Флаг приватности свйоства</summary>
        private bool _Private;

        private Action<TValue> _SetMethod;
        private Func<TValue> _GetMethod;

        private PropertyDescriptor _Descriptor;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>
        /// Тип значения свойства
        /// </summary>
        public Type PropertyType => _PropertyInfo.PropertyType;

        ///<summary>Имя свойства</summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value != "");
                Initialize(_Object, value, _Private);
            }
        }

        ///<summary>Объект, определяющий свойтсво</summary>
        public TObject Object
        {
            get { return _Object; }
            set
            {
                Contract.Requires(value != null);
                Initialize(value, _Name, _Private);
            }
        }

        ///<summary>Признак - является ли свойство пиватным</summary>
        public bool Private
        {
            get { return _Private; }
            set { Initialize(_Object, _Name, _Private = value); }
        }

        ///<summary>Признак </summary>
        public bool IsExist => _PropertyInfo != null;

        ///<summary>Значение свйоства</summary>
        public TValue Value { get { return _GetMethod(); } set { _SetMethod(value); } }

        ///<summary>Признак возможности читать значение</summary>
        public bool CanRead => _PropertyInfo != null && _PropertyInfo.CanRead;

        /// <summary>Признак возможности устанавливать значение</summary>
        public bool CanWrite => _PropertyInfo != null && _PropertyInfo.CanWrite;

        /// <summary>
        /// Поддерживает генерацию событий изменения значения
        /// </summary>
        public bool SupportsChangeEvents => _Descriptor != null && _Descriptor.SupportsChangeEvents;

        ///<summary>Атрибуты свойства</summary>
        public PropertyAttributes Attributes => _PropertyInfo.Attributes;

        /// <summary>
        /// Дескриптор свйоства объекта
        /// </summary>
        public PropertyDescriptor Descriptor => _Descriptor;

        public string DisplayName { get; private set; }

        public string DescriptionAttribute { get; private set; }

        /* ------------------------------------------------------------------------------------------ */

        ///<summary>Новый объект "Свойство" для позднего связывания</summary>
        ///<param name="Name">Имя свйосвта</param>
        ///<param name="Private">Является ли свойство скрытым</param>
        public Property(string Name, bool Private = false) : this(default, Name, Private) { }

        ///<summary>Новый объект "Свойство" для позднего связывания</summary>
        ///<param name="o">Объект, для которого определяется свойство</param>
        ///<param name="Name">Имя свйосвта</param>
        ///<param name="Private">Является ли свойство скрытым</param>
        public Property(TObject o, string Name, bool Private = false) => Initialize(o, Name, Private);

        /* ------------------------------------------------------------------------------------------ */

        private void Initialize(TObject o, string Name, bool Private)
        {
            //Contract.Requires(o != null);
            Contract.Requires(!string.IsNullOrEmpty(Name));

            if(_Descriptor != null
                && !ReferenceEquals(_Object, null)
                && !ReferenceEquals(o, _Object)
                && _Name != Name)
                _Descriptor.RemoveValueChanged(o, PropertyValueChanged);

            _Object = o;
            _Name = Name;
            _Private = Private;

            _Descriptor = _Object != null
                ? TypeDescriptor.GetProperties(_Object).Find(Name, true)
                : TypeDescriptor.GetProperties(typeof(TObject)).Find(Name, true);
            if(_Descriptor != null && !ReferenceEquals(_Object, null) && _Descriptor.SupportsChangeEvents)
                _Descriptor.AddValueChanged(_Object, PropertyValueChanged);


            _GetMethod = GetValue;
            _SetMethod = SetValue;

            var type = typeof(TObject);
            if(type == typeof(object) && !ReferenceEquals(o, null))
                type = o.GetType();

            var IsStatic = ReferenceEquals(o, null) ? BindingFlags.Static : BindingFlags.Instance;
            var IsPublic = Private ? BindingFlags.NonPublic : BindingFlags.Public;

            _PropertyInfo = type.GetProperty(Name, IsStatic | IsPublic);

            if(_PropertyInfo == null)
            {
                var lv_GetMethod = new Method<TObject, TValue>(o, $"get_{Name}", Private);
                var lv_SetMethod = new Method<TObject, object>(o, $"set_{Name}", Private);

                _GetMethod = () => lv_GetMethod.Invoke();
                _SetMethod = value => lv_SetMethod.Invoke(value);
            }

            if(!(o is ISynchronizeInvoke)) return;

            var obj = (ISynchronizeInvoke)o;
            _GetMethod = () => (TValue)obj.Invoke(_GetMethod, null);
            _SetMethod = value => obj.Invoke(_SetMethod, new object[] { value });

        }

        private void LoadAttributes()
        {
            var description_attributes = _PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(description_attributes.Length > 0)
                DescriptionAttribute = ((DescriptionAttribute)description_attributes[0]).Description;

            var name_attributes = _PropertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            DisplayName = name_attributes.Length > 0 ? ((DisplayNameAttribute)name_attributes[0]).DisplayName : Name;
        }

        private void PropertyValueChanged(object Sender, EventArgs Args) => OnValueChanged(EventArgs.Empty);

        private void SetValue(TValue value) { if(CanWrite) _PropertyInfo.SetValue(_Object, value, null); }

        private TValue GetValue() => CanRead ? (TValue)_PropertyInfo.GetValue(_Object, null) : default;

        /* ------------------------------------------------------------------------------------------ */

        [ContractInvariantMethod]
        private void CheckAccessMethods()
        {
            Contract.Invariant(_GetMethod != null);
            Contract.Invariant(_SetMethod != null);
        }

        /* ------------------------------------------------------------------------------------------ */

        public override string ToString()
        {
            var PropertyType = _Object == null ? "Static property" : "Property";

            if(_PropertyInfo == null)
                return $"Incorrect {PropertyType.ToLower()} of {typeof(TObject)} name {_Name}";

            var value = CanRead ? $" = {Value}" : "";
            var host = typeof(TObject).Name;
            return string.Format("{0}({4}{5}{6}): {1}.{2}{3}", PropertyType, host, _Name, value,
                CanRead ? "R" : "", CanWrite ? "W" : "", SupportsChangeEvents ? "E" : "");
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}