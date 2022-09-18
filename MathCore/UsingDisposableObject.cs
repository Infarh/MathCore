using System;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Класс-обёртка для inline-доступа к свойствам созданного объекта, наследующего интерфейс IDisposable</summary>
/// <typeparam name="T">Тип используемого объекта, наследующего интерфейс IDisposable</typeparam>
public class UsingDisposableObject<T> : UsingObject<T> where T : IDisposable
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Новая обёртка для используемого объекта</summary>
    /// <param name="Obj">Используемый объект</param>
    [DST]
    public UsingDisposableObject(T Obj) : base(Obj, o => o.Dispose()) { }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Получить значение от объекта</summary>
    /// <typeparam name="TValue">Тип значения, получаемого от объекта</typeparam>
    /// <param name="f">Метод получения значения</param>
    /// <returns>Значение, полученное от объекта указанным методом</returns>
    [DST]
    public TValue GetValue<TValue>([NotNull] Func<T, TValue> f) => f(Object);

    /* ------------------------------------------------------------------------------------------ */
}