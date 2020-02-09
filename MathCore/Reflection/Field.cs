using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Объект управления полем</summary>
    /// <typeparam name="TObject">Тип объекта, в котором определено управляемое полек</typeparam>
    /// <typeparam name="TValue">Тип поля</typeparam>
    public class Field<TObject, TValue>
    {
        /// <summary>Информация о поле</summary>
        private FieldInfo _FieldInfo;

        /// <summary>Имя поля</summary>
        private string _Name;

        /// <summary>Объект, в котором определено поле</summary>
        private TObject _Object;

        /// <summary>Поле является приватным?</summary>
        private bool _Private;

        /// <summary>Поле найдено и может быть управляемо</summary>
        public bool IsExist => _FieldInfo != null;

        /// <summary>Имя поля</summary>
        public string Name { get => _Name; set => Initialize(_Object, _Name = value, _Private); }

        /// <summary>Объект, в котором определено поле</summary>
        public TObject Object { get => _Object; set => Initialize(_Object = value, _Name, _Private); }

        /// <summary>Поле является приватным?</summary>
        public bool Private { get => _Private; set => Initialize(_Object, _Name, _Private = value); }

        /// <summary>Значение поля</summary>
        public TValue Value { get => (TValue)_FieldInfo.GetValue(_Object); set => _FieldInfo.SetValue(_Object, value); }

        /// <summary>Инициализация нового экземпляра <see cref="Field{TObject,TValue}"/></summary>
        /// <param name="o">Объект, в котором определено поле</param>
        /// <param name="Name">Имя поля</param>
        /// <param name="Private">Поле является приватным?</param>
        public Field(TObject o, string Name, bool Private = false) => Initialize(_Object = o, _Name = Name, _Private = Private);

        /// <summary> Инициализация состояния <see cref="Field{TObject,TValue}"/></summary>
        /// <param name="o">Объект, в котором определено поле</param>
        /// <param name="FieldName">Имя поля</param>
        /// <param name="IsPrivate">Поле является приватным?</param>
        private void Initialize([CanBeNull] TObject o, [NotNull] string FieldName, bool IsPrivate)
        {
            var type = typeof(TObject);
            if (type == typeof(TObject) && o != null)
                type = o.GetType();

            var is_private = IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
            var is_static = o is null ? BindingFlags.Static : BindingFlags.Instance;

            _FieldInfo = type.GetField(FieldName, is_private | is_static);
        }
    }
}