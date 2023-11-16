﻿#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.ViewModels;

public partial class ViewModel
{
    /// <summary>Установщик значения</summary>
    /// <typeparam name="T">Тип устанавливаемого значения</typeparam>
    public readonly ref struct SetValueResult<T>
    {
        /// <summary>Результат установки значения</summary>
        private readonly bool _Result;

        /// <summary>Предыдущее значение свойства</summary>
        private readonly T? _OldValue;

        /// <summary>Новое значение свойства</summary>
        private readonly T? _NewValue;

        /// <summary>Модель-представления, для которой производится установка значения</summary>
        private readonly ViewModel _Model;

        /// <summary>Инициализация нового экземпляра <see cref="SetValueResult{T}"/></summary>
        /// <param name="Result">Было ли значение установлено для свойства</param>
        /// <param name="OldValue">Старое значение</param>
        /// <param name="model">Модель-представления, для свойство которой изменилось</param>
        internal SetValueResult(bool Result, T? OldValue, ViewModel model) : this(Result, OldValue, OldValue, model) { }

        /// <summary>Инициализация нового экземпляра <see cref="SetValueResult{T}"/></summary>
        /// <param name="Result">Было ли значение установлено для свойства</param>
        /// <param name="OldValue">Старое значение</param>
        /// <param name="NewValue">Новое (установленное) значение</param>
        /// <param name="model">Модель-представления, для свойство которой изменилось</param>
        internal SetValueResult(bool Result, T? OldValue, T? NewValue, ViewModel model)
        {
            _Result   = Result;
            _OldValue = OldValue;
            _NewValue = NewValue;
            _Model    = model;
        }

        /// <summary>В случае если значение было установлено, выполнить указанное действие</summary>
        /// <param name="execute">Действие, которое требуется выполнить в случае если значение свойства было установлено</param>
        /// <returns>Истина, если значение свойства было установлено</returns>
        public bool Then(Action execute)
        {
            if (_Result) execute();
            return _Result;
        }

        /// <summary>В случае если значение было установлено, выполнить указанное действие над новым значением</summary>
        /// <param name="execute">Действие над значением, которое требуется выполнить в случае если значение свойства было установлено</param>
        /// <returns>Истина, если значение свойства было установлено</returns>
        public bool Then(Action<T?> execute)
        {
            if (_Result) execute(_NewValue);
            return _Result;
        }

        /// <summary>В случае если значение было установлено, выполнить указанное действие над старым и новым значением</summary>
        /// <param name="execute">Действие над старым и новым значением, которое требуется выполнить в случае если значение свойства было установлено</param>
        /// <returns>Истина, если значение свойства было установлено</returns>
        public bool Then(Action<T?, T?> execute)
        {
            if (_Result) execute(_OldValue, _NewValue);
            return _Result;
        }

        /// <summary>Выполнить генерацию события обновления указанного свойства</summary>
        /// <param name="PropertyName">Имя свойства, которое также изменилось</param>
        /// <returns>Текущий объект установки значения</returns>
        public SetValueResult<T> Update(string PropertyName)
        {
            _Model.OnPropertyChanged(PropertyName);
            return this;
        }

        /// <summary>Выполнить генерацию события обновления для указанных свойств</summary>
        /// <param name="PropertyName">Массив имён изменившихся свойств</param>
        /// <returns>Текущий объект установки значения</returns>
        public SetValueResult<T> Update(params string[] PropertyName)
        {
            foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
            return this;
        }

        /// <summary>Выполнить действие вне зависимости от того, изменилось ли значение свойства, или нет</summary>
        /// <param name="execute">Действие, которое требуется выполнить</param>
        /// <returns>Истина, если свойство было изменено</returns>
        public bool AnywayThen(Action execute)
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
        public bool AnywayThen(Action<bool> execute)
        {
            execute(_Result);
            return _Result;
        }

        /// <summary>Выполнить действие с новым значением вне зависимости от того, изменилось оно, или нет</summary>
        /// <param name="execute">Действие, которое требуется выполнить с параметром в виде значения свойства</param>
        /// <returns>Истина, если свойство было изменено</returns>
        public bool AnywayThen(Action<T?> execute)
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
        public bool AnywayThen(Action<T?, bool> execute)
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
        public bool AnywayThen(Action<T?, T?> execute)
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
        public bool AnywayThen(Action<T, T, bool> execute)
        {
            execute(_OldValue, _NewValue, _Result);
            return _Result;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="other">The object to compare with the current instance.</param>
        public bool Equals(SetValueResult<T?> other) =>
            _Result == other._Result
            && EqualityComparer<T>.Default.Equals(_OldValue, other._OldValue)
            && EqualityComparer<T>.Default.Equals(_NewValue, other._NewValue)
            && _Model.Equals(other._Model);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => throw new NotSupportedException("Невозможно выполнить метод Equals()");

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code         = _Result.GetHashCode();
                var equality_comparer = EqualityComparer<T>.Default;
                hash_code = (hash_code * 397) ^ equality_comparer.GetHashCode(_OldValue);
                hash_code = (hash_code * 397) ^ equality_comparer.GetHashCode(_NewValue);
                hash_code = (hash_code * 397) ^ _Model.GetHashCode();
                return hash_code;
            }
        }

        /// <summary>Оператор равенства двух значений</summary>
        public static bool operator ==(SetValueResult<T> left, SetValueResult<T> right) =>
            ReferenceEquals(left._Model, right._Model)
            && left._Result == right._Result
            && Equals(left._OldValue, right._OldValue)
            && Equals(left._NewValue, right._NewValue);

        /// <summary>Оператор неравенства двух значений</summary>
        public static bool operator !=(SetValueResult<T> left, SetValueResult<T> right) => !(left == right);
    }

    /// <summary>Установить значение свойства</summary>
    /// <param name="field">Поле свойства</param>
    /// <param name="value">Устанавливаемое значение</param>
    /// <param name="PropertyName">Имя свойства (если пусто, то будет взято имя метода, откуда выполняется запуск)</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns>Объект, отвечающий за обработку результата установки значения</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual SetValueResult<T> SetValue<T>(
        ref T? field,
        T? value,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value))
            return new SetValueResult<T>(false, field, field, this);

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
    protected virtual SetValueResult<T> SetValue<T>(
        ref T? field,
        T? value,
        Func<T?, bool> Validator,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value) || !Validator(value))
            return new SetValueResult<T>(false, field, value, this);

        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new SetValueResult<T>(true, old_value, value, this);
    }
}