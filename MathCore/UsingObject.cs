#nullable enable
using System.Diagnostics.CodeAnalysis;

// ReSharper disable MemberCanBeProtected.Global

namespace MathCore;

/// <summary>Оболочка, обеспечивающая освобождение ресурсов указанным методом для указанного объекта</summary>
/// <typeparam name="T">Тип объекта, с которым работает оболочка</typeparam>
/// <remarks>Упаковка объекта в оболочку с указанием метода освобождения ресурсов, занимаемых указанным объектом</remarks>
/// <param name="obj">Уничтожаемый объект</param>
/// <param name="Disposer">Метод освобождения ресурсов</param>
[method: DST]
/// <summary>Оболочка, обеспечивающая освобождение ресурсов указанным методом для указанного объекта</summary>
/// <typeparam name="T">Тип объекта, с которым работает оболочка</typeparam>
public class UsingObject<T>([DisallowNull] T obj, Action<T> Disposer) : IDisposable
{
    /* ------------------------------------------------------------------------------------------ */

    private readonly T _Obj = obj ?? throw new ArgumentNullException(nameof(obj));

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Используемый объект</summary>
    public T Object => obj;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Разрушение обёртки, влекущее разрушение используемого объекта</summary>
    [DST]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Объект разрушен</summary>
    private bool _Disposed;

    /// <summary>Разрушение обёртки, влекущее разрушение используемого объекта</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _Disposed) return;
        _Disposed = true;
        Disposer(_Obj);
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного приведения типов</summary>
    /// <param name="obj">Объект-оболочка</param>
    /// <returns>Внутренний объект</returns>
    [DST]
    public static implicit operator T(UsingObject<T> obj) => obj._Obj;

    /* ------------------------------------------------------------------------------------------ */
}