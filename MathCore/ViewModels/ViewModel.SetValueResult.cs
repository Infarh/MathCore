using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        /// <summary>Установщик значения</summary>
        /// <typeparam name="T">Тип устанавливаемого значения</typeparam>
        public readonly struct SetValueResult<T>
        {
            /// <summary>Результат установки значения</summary>
            private readonly bool _Result;

            /// <summary>Предыдущее значение свойства</summary>
            [CanBeNull] private readonly T _OldValue;

            /// <summary>Новое значение свойства</summary>
            [CanBeNull] private readonly T _NewValue;

            /// <summary>Модель-представления, для которой производится установка значения</summary>
            [NotNull] private readonly ViewModel _Model;

            /// <summary>Инициализация нового экземпляра <see cref="SetValueResult{T}"/></summary>
            /// <param name="Result">Было ли значение установлено для свойства</param>
            /// <param name="OldValue">Старое значение</param>
            /// <param name="model">Модель-представления, для свойство которой изменилось</param>
            internal SetValueResult(bool Result, [CanBeNull] in T OldValue, [NotNull] in ViewModel model) : this(Result, OldValue, OldValue, model) { }

            /// <summary>Инициализация нового экземпляра <see cref="SetValueResult{T}"/></summary>
            /// <param name="Result">Было ли значение установлено для свойства</param>
            /// <param name="OldValue">Старое значение</param>
            /// <param name="NewValue">Новое (установленное) значение</param>
            /// <param name="model">Модель-представления, для свойство которой изменилось</param>
            internal SetValueResult(bool Result, [CanBeNull] in T OldValue, [CanBeNull] in T NewValue, [NotNull] in ViewModel model)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _Model = model;
            }

            /// <summary>В случае если значение было установлено, выполнить указанное действие</summary>
            /// <param name="execute">Действие, которое требуется выполнить в случае если значение свойства было установлено</param>
            /// <returns>Истина, если значение свойства было установлено</returns>
            public bool Then([NotNull] in Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            /// <summary>В случае если значение было установлено, выполнить указанное действие над новым значением</summary>
            /// <param name="execute">Действие над значением, которое требуется выполнить в случае если значение свойства было установлено</param>
            /// <returns>Истина, если значение свойства было установлено</returns>
            public bool Then([NotNull] in Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            /// <summary>В случае если значение было установлено, выполнить указанное действие над старым и новым значением</summary>
            /// <param name="execute">Действие над старым и новым значением, которое требуется выполнить в случае если значение свойства было установлено</param>
            /// <returns>Истина, если значение свойства было установлено</returns>
            public bool Then([NotNull] in Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);

                return _Result;
            }

            /// <summary>Выполнить генерацию события обновления указанного свойства</summary>
            /// <param name="PropertyName">Имя свойства, которое также изменилось</param>
            /// <returns>Текущий объект установки значения</returns>
            public SetValueResult<T> Update([NotNull] in string PropertyName)
            {
                _Model.OnPropertyChanged(PropertyName);
                return this;
            }

            /// <summary>Выполнить генерацию события обновления для указанных свойств</summary>
            /// <param name="PropertyName">Массив имён изменившихся свойств</param>
            /// <returns>Текущий объект установки значения</returns>
            public SetValueResult<T> Update([NotNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
                return this;
            }

            /// <summary>Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет</summary>
            /// <param name="execute">Действие, которое требуется выполнить</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action execute)
            {
                execute();
                return _Result;
            }

            /// <summary>
            /// Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет.
            /// Параметр действия - флаг, сигнализирующий о том, было ли изменено свойство
            /// </summary>
            /// <param name="execute">Действие, которое требуется выполнить</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }

            /// <summary>Выполнить действие с новым значением вне зависимости от того, изменилось оно, или нет</summary>
            /// <param name="execute">Действие, которое требуется выполнить с параметром в виде значения свойства</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }

            /// <summary>
            /// Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет.
            /// Параметры действия - флаг, сигнализирующий о том, было ли изменено свойство, и значение свойства
            /// </summary>
            /// <param name="execute">Действие, которое требуется выполнить над значением свойства</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }

            /// <summary>
            /// Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет.
            /// Параметры действия - предыдущее и текущее значение свойства
            /// </summary>
            /// <param name="execute">Действие, которое требуется выполнить над значением свойства</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }

            /// <summary>
            /// Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет.
            /// Параметры действия - предыдущее и текущее значение свойства, а также признак изменения значения
            /// </summary>
            /// <param name="execute">Действие, которое требуется выполнить над значением свойства</param>
            /// <returns>Истина, если свойство было изменено</returns>
            public bool AnywayThen([NotNull] in Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }

            /// <summary>Indicates whether this instance and a specified object are equal.</summary>
            /// <param name="other">The object to compare with the current instance.</param>
            public bool Equals(in SetValueResult<T> other) =>
                _Result == other._Result 
                && EqualityComparer<T>.Default.Equals(_OldValue, other._OldValue) 
                && EqualityComparer<T>.Default.Equals(_NewValue, other._NewValue) 
                && _Model.Equals(other._Model);

            /// <inheritdoc />
            public override bool Equals(object obj) => obj is SetValueResult<T> setter && Equals(setter);

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hash_code = _Result.GetHashCode();
                    hash_code = (hash_code * 397) ^ EqualityComparer<T>.Default.GetHashCode(_OldValue);
                    hash_code = (hash_code * 397) ^ EqualityComparer<T>.Default.GetHashCode(_NewValue);
                    hash_code = (hash_code * 397) ^ _Model.GetHashCode();
                    return hash_code;
                }
            }

            /// <summary>Оператор равенства двух значений</summary>
            public static bool operator ==(in SetValueResult<T> left, in SetValueResult<T> right) => left.Equals(right);

            /// <summary>Оператор неравенства двух значений</summary>
            public static bool operator !=(in SetValueResult<T> left, in SetValueResult<T> right) => !left.Equals(right);
        }

        /// <summary>Установить значение свойства</summary>
        /// <param name="field">Поле свойства</param>
        /// <param name="value">Устанавливаемое значение</param>
        /// <param name="PropertyName">Имя свойства (если пусто, то будет взято имя метода, откуда выполняется запуск)</param>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <returns>Объект, отвечающий за обработку результата установки значения</returns>
        [NotifyPropertyChangedInvocator]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] in T value, [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value)) return new SetValueResult<T>(false, field, field, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        /// <summary>Установить значение свойства</summary>
        /// <param name="field">Поле свойства</param>
        /// <param name="value">Устанавливаемое значение</param>
        /// <param name="Validator">Метод проверки корректности значения, передаваемого в свойство</param>
        /// <param name="PropertyName">Имя свойства (если пусто, то будет взято имя метода, откуда выполняется запуск)</param>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <returns>Объект, отвечающий за обработку результата установки значения</returns>
        [NotifyPropertyChangedInvocator]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] in T value, [NotNull] in Func<T, bool> Validator, [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value) || !Validator(value)) return new SetValueResult<T>(false, field, value, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }
    }
}
