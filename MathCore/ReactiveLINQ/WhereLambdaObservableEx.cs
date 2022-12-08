#nullable enable

// ReSharper disable NotAccessedField.Local

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class WhereLambdaObservableEx<T> : SimpleObservableEx<T>
{
    [Diagnostics.CodeAnalysis.SuppressMessage("Качество кода", "IDE0052:Удалить непрочитанные закрытые члены", Justification = "<Ожидание>")]
    private readonly IObserver<T> _Observer;

    public WhereLambdaObservableEx(IObservable<T> observable, Func<T, bool> WhereSelector) => _Observer = new WhereLambdaObserverEx<T>(observable, this, WhereSelector);
}