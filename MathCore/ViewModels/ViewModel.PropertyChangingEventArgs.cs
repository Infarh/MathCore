using System.ComponentModel;
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoProperty

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        /// <summary>Аргумент события процесса изменения значения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        public class PropertyChangingEventArgs<T> : PropertyChangingEventArgs
        {
            /// <summary>Предыдущее значение свойства</summary>
            private readonly T _OldValue;

            /// <summary>Новое значение свойства</summary>
            private T _NewValue;

            /// <summary>Предыдущее значение свойства</summary>
            public T OldValue => _OldValue;

            /// <summary>Новое значение свойства (которое может быть изменено)</summary>
            public T NewValue { get => _NewValue; set => _NewValue = value; }

            /// <summary>Признак того, что новое значение свойства отличается от старого</summary>
            public bool IsChangingValue => !Equals(_OldValue, _NewValue);

            /// <summary>Инициализация нового экземпляра <see cref="PropertyChangingEventArgs"/></summary>
            /// <param name="OldValue">Предыдущее значение свойства</param>
            /// <param name="NewValue">Новое значение свойства</param>
            /// <param name="PropertyName">Имя изменяющегося свойства</param>
            public PropertyChangingEventArgs(in T OldValue, in T NewValue, in string PropertyName) : base(PropertyName)
            {
                _OldValue = OldValue;
                _NewValue = NewValue;
            }

            /// <summary>Установить новое значение свойства</summary>
            /// <param name="Value">Новое значение свойства</param>
            /// <returns>Истина, Если новое значение свойства отличается от старого</returns>
            public bool Set(in T Value)
            {
                _NewValue = Value;
                return IsChangingValue;
            }
        }
    }
}
