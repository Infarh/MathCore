#nullable enable
// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Класс-обёртка для inline-доступа к свойствам созданного объекта, наследующего интерфейс IDisposable</summary>
/// <typeparam name="T">Тип используемого объекта, наследующего интерфейс IDisposable</typeparam>
/// <remarks>Новая обёртка для используемого объекта</remarks>
/// <param name="Obj">Используемый объект</param>
[method: DST]
/// <summary>Класс-обёртка для inline-доступа к свойствам созданного объекта, наследующего интерфейс IDisposable</summary>
/// <typeparam name="T">Тип используемого объекта, наследующего интерфейс IDisposable</typeparam>
public class UsingDisposableObject<T>(T Obj) : UsingObject<T>(Obj, o => o.Dispose()) where T : IDisposable
{

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Получить значение от объекта</summary>
    /// <typeparam name="TValue">Тип значения, получаемого от объекта</typeparam>
    /// <param name="f">Метод получения значения</param>
    /// <returns>Значение, полученное от объекта указанным методом</returns>
    [DST]
    public TValue GetValue<TValue>(Func<T, TValue> f) => f(Object);

    /* ------------------------------------------------------------------------------------------ */
}