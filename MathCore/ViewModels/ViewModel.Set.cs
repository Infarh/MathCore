using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using MathCore.Annotations;

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || OnPropertyChanging(field, ref value, PropertyName)) return false;
            field = value;
            if (!string.IsNullOrWhiteSpace(PropertyName)) OnPropertyChanged(PropertyName);
            return true;
        }

        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="value_check">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_check, [NotNull] [CallerMemberName] string PropertyName = null)
            => value_check(value) && Set(ref field, value, PropertyName);

        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="value_check">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
        /// <param name="ErrorMessage">Сообщение, записываемое в генерируемое исключение <see cref="ArgumentOutOfRangeException"/> в случае если проверка <paramref name="value_check"/> не пройдена</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_check, [NotNull] string ErrorMessage, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (!value_check(value)) throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage);
            return Set(ref field, value, PropertyName);
        }

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CanBeNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            if (!string.IsNullOrWhiteSpace(PropertyName))
                OnPropertyChanged?.Invoke(PropertyName);
            return true;
        }

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CanBeNull] Action<string> OnPropertyChanged, [NotNull] Func<T, bool> value_check, [NotNull] [CallerMemberName] string PropertyName = null)
            => value_check(value) && Set(ref field, value, OnPropertyChanged, PropertyName);
    }
}
