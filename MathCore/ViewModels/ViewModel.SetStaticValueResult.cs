using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        /// <summary>Объект, отвечающий за управление процессом установки значения свойства при статическом вызове метода Set</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        public readonly ref struct SetStaticValueResult<T>
        {
            /// <summary>Было ли значение свойства обновлено</summary>
            private readonly bool _Result;

            /// <summary>Прежнее значение свойства</summary>
            [CanBeNull] private readonly T _OldValue;

            /// <summary>Новое значение свойства</summary>
            [CanBeNull] private readonly T _NewValue;

            /// <summary>Действие по генерации события обновления свойства - делегат <see cref="System.ComponentModel.PropertyChangedEventHandler"/></summary>
            [NotNull] private readonly Action<string> _OnPropertyChanged;

            /// <summary>Инициализация нового экземпляра <see cref="SetStaticValueResult{T}"/></summary>
            /// <param name="Result">Было ли значение свойства обновлено</param>
            /// <param name="OldValue">Прежнее значение свойства</param>
            /// <param name="OnPropertyChanged">Действие по генерации события обновления свойства - делегат <see cref="System.ComponentModel.PropertyChangedEventHandler"/></param>
            internal SetStaticValueResult(bool Result, [CanBeNull] in T OldValue, [NotNull] in Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }

            /// <summary>Инициализация нового экземпляра <see cref="SetStaticValueResult{T}"/></summary>
            /// <param name="Result">Было ли значение свойства обновлено</param>
            /// <param name="OldValue">Прежнее значение свойства</param>
            /// <param name="NewValue">Новое значение свойства</param>
            /// <param name="OnPropertyChanged">Действие по генерации события обновления свойства - делегат <see cref="System.ComponentModel.PropertyChangedEventHandler"/></param>
            internal SetStaticValueResult(bool Result, [CanBeNull] in T OldValue, [CanBeNull] in T NewValue, [NotNull] in Action<string> OnPropertyChanged)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _OnPropertyChanged = OnPropertyChanged;
            }

            /// <summary>В случае если значение свойства было изменено вызывать указанное действие</summary>
            /// <param name="execute">Действие, выполняемое в случае обновления значения свойства</param>
            /// <returns>Признак того, что свойство изменило своё значение</returns>
            public bool Then([NotNull] in Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            /// <summary>В случае если значение свойства было изменено вызывать указанное действие над новым значением</summary>
            /// <param name="execute">Действие над новым значением, выполняемое в случае обновления значения свойства</param>
            /// <returns>Признак того, что свойство изменило своё значение</returns>
            public bool Then([NotNull] in Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            /// <summary>В случае если значение свойства было изменено вызывать указанное действие над старым и новым значением</summary>
            /// <param name="execute">Действие над старым и новым значением, выполняемое в случае обновления значения свойства</param>
            /// <returns>Признак того, что свойство изменило своё значение</returns>
            public bool Then([NotNull] in Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            /// <summary>Выполнить генерацию события изменения указанного свойства, если значение текущего свойства изменилось</summary>
            /// <param name="PropertyName">Имя обновившегося связанного свойства</param>
            /// <returns>Текущий объект <see cref="SetStaticValueResult{T}"/></returns>
            public SetStaticValueResult<T> Update([NotNull] in string PropertyName)
            {
                if (!_Result) return this;
                _OnPropertyChanged(PropertyName);
                return this;
            } 

            /// <summary>Выполнить генерацию события изменения указанного свойства даже если значение свойства не изменилось</summary>
            /// <param name="PropertyName">Имя обновившегося связанного свойства</param>
            /// <returns>Текущий объект <see cref="SetStaticValueResult{T}"/></returns>
            public SetStaticValueResult<T> AnywayUpdate([NotNull] in string PropertyName)
            {
                _OnPropertyChanged(PropertyName);
                return this;
            }

            /// <summary>Выполнить генерацию события изменения указанного набора свойств, если значение текущего свойства изменилось</summary>
            /// <param name="PropertyName">Имена обновившихся связанных свойства</param>
            /// <returns>Текущий объект <see cref="SetStaticValueResult{T}"/></returns>
            public SetStaticValueResult<T> Update([NotNull, ItemCanBeNull] params string[] PropertyName)
            {
                if (!_Result) return this;
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            /// <summary>Выполнить генерацию события изменения указанного набора свойств даже если значение свойства не изменилось</summary>
            /// <param name="PropertyName">Имена обновившихся связанных свойства</param>
            /// <returns>Текущий объект <see cref="SetStaticValueResult{T}"/></returns>
            public SetStaticValueResult<T> AnywayUpdate([NotNull, ItemCanBeNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            /// <summary>Выполнить указанное действие даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action execute)
            {
                execute();
                return _Result;
            }

            /// <summary>Выполнить указанное действие с признаком обновления свойства даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие с признаком обновления свойства</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }

            /// <summary>Выполнить указанное действие над новым значением свойства даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие над новым значением свойства</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }

            /// <summary>Выполнить указанное действие над новым значением и признаком изменения свойства даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие над новым значением и признаком изменения свойства</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }

            /// <summary>Выполнить указанное действие над старым и новым значением свойства даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие над старым и новым значением свойства</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }

            /// <summary>Выполнить указанное действие над старым, новым значением и признаком изменения свойства даже в случае если значение свойства не изменилось</summary>
            /// <param name="execute">Выполняемое действие над старым, новым значением и признаком изменения свойства</param>
            /// <returns>Истина, если значение свойства изменилось</returns>
            public bool AnywayThen([NotNull] in Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }

            /// <summary>Indicates whether this instance and a specified object are equal.</summary>
            /// <param name="other">The object to compare with the current instance.</param>
            public bool Equals(in SetStaticValueResult<T> other) =>
                _Result == other._Result
                && EqualityComparer<T>.Default.Equals(_OldValue, other._OldValue)
                && EqualityComparer<T>.Default.Equals(_NewValue, other._NewValue)
                && _OnPropertyChanged.Equals(other._OnPropertyChanged);

            /// <inheritdoc />
            //[Obsolete("Нельзя вызвать метод Equals у ref struct типа")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object obj) => throw new NotSupportedException("Невозможно выполнить метод Equals()");

            /// <inheritdoc />
            public override int GetHashCode()
            {
                
                unchecked
                {
                    var hash_code = _Result.GetHashCode();
                    var equality_comparer = EqualityComparer<T>.Default;
                    hash_code = (hash_code * 397) ^ equality_comparer.GetHashCode(_OldValue);
                    hash_code = (hash_code * 397) ^ equality_comparer.GetHashCode(_NewValue);
                    hash_code = (hash_code * 397) ^ _OnPropertyChanged.GetHashCode();
                    return hash_code;
                }
            }

            /// <summary>Оператор равенства двух значений</summary>
            public static bool operator ==(in SetStaticValueResult<T> left, in SetStaticValueResult<T> right) =>
                ReferenceEquals(left._OnPropertyChanged, right._OnPropertyChanged)
                && left._Result == right._Result
                && Equals(left._OldValue, right._OldValue)
                && Equals(left._NewValue, right._NewValue);

            /// <summary>Оператор неравенства двух значений</summary>
            public static bool operator !=(in SetStaticValueResult<T> left, in SetStaticValueResult<T> right) => !(left == right);

        }

        /// <summary>Установить новое значение свойства</summary>
        /// <param name="field">Ссылка на поле свойства</param>
        /// <param name="value">Устанавливаемое новое значение свойства</param>
        /// <param name="OnPropertyChanged">Метод извещения об изменении значения свойства</param>
        /// <param name="PropertyName">Имя изменяемого свойства</param>
        /// <typeparam name="T">ТИп значения свойства</typeparam>
        /// <returns>Объект контроля процесс обновления значения свойства</returns>
        public static SetStaticValueResult<T> SetValue<T>(
            [CanBeNull] ref T field,
            [CanBeNull] in T value, 
            [NotNull] in Action<string> OnPropertyChanged, 
            [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value)) 
                return new SetStaticValueResult<T>(false, field, field, OnPropertyChanged);

            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        /// <summary>Установить новое значение свойства</summary>
        /// <param name="field">Ссылка на поле свойства</param>
        /// <param name="value">Устанавливаемое новое значение свойства</param>
        /// <param name="Validator">Метод проверки корректности значения свойства</param>
        /// <param name="OnPropertyChanged">Метод извещения об изменении значения свойства</param>
        /// <param name="PropertyName">Имя изменяемого свойства</param>
        /// <typeparam name="T">ТИп значения свойства</typeparam>
        /// <returns>Объект контроля процесс обновления значения свойства</returns>
        public static SetStaticValueResult<T> SetValue<T>(
            [CanBeNull] ref T field,
            [CanBeNull] in T value, 
            in Func<T, bool> Validator,
            [NotNull] in Action<string> OnPropertyChanged, 
            [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value) || !Validator(value)) 
                return new SetStaticValueResult<T>(false, field, value, OnPropertyChanged);

            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }
    }
}
