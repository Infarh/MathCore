using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection;

/// <summary>Класс методов-расширения для <see cref="object"/>, осуществляющих доступ к методам</summary>
public static class MethodExtensions
{
    /// <summary>Получить объект контроля метода</summary>
    /// <param name="o">Объект, Метод которого требуется контролировать</param>
    /// <param name="Name">Имя метода, контроль над которым требуется получить</param>
    /// <param name="Private">Метод является непубличным?</param>
    /// <typeparam name="TObject">Тип объекта</typeparam>
    /// <typeparam name="TValue">Тип значения метода</typeparam>
    /// <returns>Объект контроля метода</returns>
    [NotNull] public static Method<TObject, TValue> GetMethod<TObject, TValue>(this TObject o, string Name, bool Private = false) => new(o, Name, Private);
}