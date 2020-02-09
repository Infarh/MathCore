using MathCore;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Управление конструктором класса</summary>
    /// <typeparam name="T">Тип объектов, порождаемых конструктором</typeparam>
    public class Constructor<T> : IFactory<T>, IFactory<T, object[]>
    {
        /// <summary>Информация о конструкторе</summary>
        private ConstructorInfo _Info;

        /// <summary>Тип объекта, конструктор которого требуется контролировать</summary>
        private Type _ObjectType;

        /// <summary>Искать приватный конструктор?</summary>
        private bool _Private;

        /// <summary>Массив типов параметров конструктора</summary>
        private Type[] _ArgumentTypes;

        /// <summary>Конструктор найден</summary>
        public bool IsExist => _Info != null;

        /// <summary>Тип объекта, конструктор которого требуется контролировать</summary>
        public Type ObjectType
        {
            get => _ObjectType;
            set => Initialize(_ObjectType = value ?? throw new ArgumentNullException(nameof(value)), _ArgumentTypes, _Private);
        }

        /// <summary>Искать приватный конструктор?</summary>
        public bool Private
        {
            get => _Private;
            set => Initialize(_ObjectType, _ArgumentTypes, _Private = value);
        }

        /// <summary>Массив типов параметров конструктора</summary>
        public Type[] ArgumentTypes
        {
            get => _ArgumentTypes;
            set => Initialize(_ObjectType, _ArgumentTypes = value, _Private);
        }

        /// <summary>Инициализация нового экземпляра <see cref="Constructor{T}"/></summary>
        /// <param name="o">Объект, Конструктор которого используется</param>
        /// <param name="Private">Искать приватный конструктор?</param>
        /// <param name="ArgumentTypes">Массив типов параметров конструктора</param>
        public Constructor([NotNull] T o, bool Private = false, [NotNull] params Type[] ArgumentTypes) => Initialize(_ObjectType = o.GetType(), ArgumentTypes, _Private = Private);

        /// <summary>Инициализация нового экземпляра <see cref="Constructor{T}"/></summary>
        /// <param name="type">Тип, из которого извлекается конструктор</param>
        /// <param name="Private">Искать приватный конструктор?</param>
        /// <param name="ArgumentTypes">Массив типов параметров конструктора</param>
        public Constructor(Type type, bool Private = false, [NotNull] params Type[] ArgumentTypes) => Initialize(_ObjectType = type, ArgumentTypes, _Private = Private);

        /// <summary>Инициализация контроля конструктора</summary>
        /// <param name="Type">Тип из которого извлекается конструктор</param>
        /// <param name="Types">Массив типов аргументов конструктора</param>
        /// <param name="IsPrivate">Искать приватный конструктор?</param>
        private void Initialize([NotNull] Type Type, [NotNull] Type[] Types, bool IsPrivate) =>
            _Info =
                Type.GetConstructor(
                    bindingAttr: BindingFlags.Instance | (IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public),
                    binder: null,
                    types: Types,
                    modifiers: null);


        /// <inheritdoc />
        public T Create() => (T)_Info.Invoke(Array.Empty<object>());

        /// <inheritdoc />
        public T Create(params object[] Parameter) => (T)_Info.Invoke(Parameter);
    }
}