// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Процессор, выполняющий действия</summary>
public class ActionsCollectionProcessor : Processor
{
    private readonly IEnumerable<Action> _ActionsCollection;

    /// <summary>Инициализация нового экземпляра <see cref="ActionsCollectionProcessor"/></summary>
    /// <param name="ActionsCollection">Перечисление действий, которые требуется выполнить</param>
    public ActionsCollectionProcessor(IEnumerable<Action> ActionsCollection) => _ActionsCollection = ActionsCollection;

    /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
    protected override void MainAction() => _ActionsCollection.Foreach(ProcessAction);

    /// <summary>Выполнить очередное действие</summary>
    /// <param name="action">Очередное действие из очереди действий, которые должен выполнить процессор</param>
    protected virtual void ProcessAction(Action action) => action();
}