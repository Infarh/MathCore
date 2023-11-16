#nullable enable

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Объект, выполняющий указанное действие при сборке мусора</summary>
/// <remarks>Инициализация нового уничтожаемого объекта с указанием действия при уничтожении</remarks>
/// <param name="DisposableAction">Действие, выполняемое при уничтожении объекта</param>
public class LambdaDisposable(Action? DisposableAction = null) : IDisposable
{
    /// <summary>При освобождении выполнить указанное действие</summary>
    /// <param name="OnDispose">Действие, выполняемое при освобождении</param>
    /// <returns>Объект <see cref="LambdaDisposable"/></returns>
    public static LambdaDisposable OnDisposed(Action OnDispose) => new(OnDispose);

    /// <summary>Действие, выполняемое при разрушении объекта</summary>
    protected readonly Action? _DisposableAction = DisposableAction;

    /// <summary>Метод уничтожения объекта, вызывающий указанное действие</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Метод уничтожения объекта, вызывающий указанное действие</summary>
    /// <param name="Disposing">Если истина, то освободить управляемые ресурсы</param>
    protected virtual void Dispose(bool Disposing)
    {
        if (!Disposing) return;
        _DisposableAction?.Invoke();
    }
}
