#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable MemberCanBeProtected.Global

namespace MathCore;

/// <summary>Оболочка, обеспечивающая освобождение ресурсов указанным методом для указанного объекта</summary>
/// <typeparam name="T">Тип объекта, с которым работает оболочка</typeparam>
public class UsingObject<T> : IDisposable
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Используемый объект</summary>
    private readonly T _Obj;

    /// <summary>Метод освобождения ресурсов</summary>
    private readonly Action<T> _Disposer;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Используемый объект</summary>
    public T Object => _Obj;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Упаковка объекта в оболочку с указанием метода освобождения ресурсов, занимаемых указанным объектом</summary>
    /// <param name="obj">Уничтожаемый объект</param>
    /// <param name="Disposer">Метод освобождения ресурсов</param>
    [DST]
    public UsingObject([DisallowNull] T obj, Action<T> Disposer)
    {
        if(obj is null) throw new ArgumentNullException(nameof(obj));
        _Obj      = obj;
        _Disposer = Disposer ?? throw new ArgumentNullException(nameof(Disposer));
    }

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
        _Disposer(_Obj);
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного приведения типов</summary>
    /// <param name="obj">Объект-оболочка</param>
    /// <returns>Внутренний объект</returns>
    [DST]
    public static implicit operator T(UsingObject<T> obj) => obj._Obj;

    /* ------------------------------------------------------------------------------------------ */
}