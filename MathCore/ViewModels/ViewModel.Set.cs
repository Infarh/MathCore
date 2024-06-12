using System.Runtime.CompilerServices;

using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AssignNullToNotNullAttribute

namespace MathCore.ViewModels;

public partial class ViewModel
{
    protected virtual bool Set<T>(
        [CanBeNull] T value, 
        [CanBeNull] T OldValue,
        Action<T> Setter, 
        [CanBeNull] Func<T, bool> ValueChecker = null, 
        [NotNull, CallerMemberName] string PropertyName = null!)
    {
        if (Equals(value, OldValue)) return false;
        if (ValueChecker is { } checker && !checker(value)) return false;
        Setter(value);
        OnPropertyChanged(PropertyName!);
        return true;
    }

    /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
    /// <typeparam name="T">Тип значения поля</typeparam>
    /// <param name="field">Ссылка на поле модели</param>
    /// <param name="value">Значение, устанавливаемое для поля</param>
    /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
    /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
    protected virtual bool Set<T>([CanBeNull/*, NotNullIfNotNull(nameof(field))*/] ref T field, [CanBeNull] T value, [NotNull, CallerMemberName] in string PropertyName = null)
    {
        if (Equals(field, value) || OnPropertyChanging(field, ref value, PropertyName)) return false;
        field = value;
        if (!string.IsNullOrWhiteSpace(PropertyName))
            OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
    /// <typeparam name="T">Тип значения поля</typeparam>
    /// <param name="field">Ссылка на поле модели</param>
    /// <param name="value">Значение, устанавливаемое для поля</param>
    /// <param name="ValueChecker">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
    /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
    /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
    protected virtual bool Set<T>(
        [CanBeNull] ref T field, 
        [CanBeNull] in T value,
        [NotNull] in Func<T, bool> ValueChecker, 
        [NotNull, CallerMemberName] in string PropertyName = null)
        => ValueChecker(value) && Set(ref field, value, PropertyName);

    /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
    /// <typeparam name="T">Тип значения поля</typeparam>
    /// <param name="field">Ссылка на поле модели</param>
    /// <param name="value">Значение, устанавливаемое для поля</param>
    /// <param name="ErrorMessage">Сообщение, записываемое в генерируемое исключение <see cref="ArgumentOutOfRangeException"/> в случае если проверка <paramref name="Validator"/> не пройдена</param>
    /// <param name="Validator">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
    /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
    /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
    protected virtual bool Set<T>(
        [CanBeNull] ref T field, 
        [CanBeNull] in T value,
        [NotNull] in string ErrorMessage,
        [NotNull] in Func<T, bool> Validator,
        [NotNull, CallerMemberName] in string PropertyName = null) =>
        Validator(value) 
            ? Set(ref field, value, PropertyName) 
            : throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage);

    /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
    /// <typeparam name="T">Тип значения поля</typeparam>
    /// <param name="value">Значение, устанавливаемое для поля</param>
    /// <param name="OldValue">Старое значение свойства</param>
    /// <param name="Setter">Метод установки значения свойства</param>
    /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
    /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
    protected virtual bool Set<T>(
        T value,
        in T OldValue,
        Action<T> Setter,
        [CallerMemberName] string PropertyName = null)
    {
        if (Equals(value, OldValue)) return false;
        Setter(value);
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OnPropertyChanged">Действие, выполняемое для извещения об изменении свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        [CanBeNull] ref T field,
        [CanBeNull] in T value, 
        [CanBeNull] in Action<string> OnPropertyChanged,
        [NotNull] [CallerMemberName] in string PropertyName = null)
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
    /// <param name="OnPropertyChanged">Действие, выполняемое для извещения об изменении свойства</param>
    /// <param name="Validator">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        [CanBeNull] ref T field, 
        [CanBeNull] in T value,
        [CanBeNull] in Action<string> OnPropertyChanged, 
        [NotNull] in Func<T, bool> Validator,
        [NotNull, CallerMemberName] in string PropertyName = null)
        => Validator(value) && Set(ref field, value, OnPropertyChanged, PropertyName);
}