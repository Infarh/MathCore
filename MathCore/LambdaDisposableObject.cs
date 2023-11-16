#nullable enable

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System;

public class LambdaDisposableObject<T>(
    T obj,
    Action<T, object>? ObjectDisposableAction = null,
    object? parameter = null,
    Action? BaseDisposableAction = null) : LambdaDisposable(BaseDisposableAction)
{
    public T Object => obj;

    public object? Parameter { get; set; } = parameter;

    /// <inheritdoc />
    protected override void Dispose(bool Disposing)
    {
        if (!Disposing) return;
        ObjectDisposableAction?.Invoke(obj, Parameter);
        (obj as IDisposable)?.Dispose();
        base.Dispose(Disposing);
    }
}