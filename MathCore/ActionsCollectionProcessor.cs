// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Процессор, выполняющий действия</summary>
/// <remarks>Инициализация нового экземпляра <see cref="ActionsCollectionProcessor"/></remarks>
/// <param name="ActionsCollection">Перечисление действий, которые требуется выполнить</param>
public class ActionsCollectionProcessor(IEnumerable<Action> ActionsCollection) : Processor
{

    /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
    protected override void MainAction() => ActionsCollection.Foreach(ProcessAction);

    /// <summary>Выполнить очередное действие</summary>
    /// <param name="action">Очередное действие из очереди действий, которые должен выполнить процессор</param>
    protected virtual void ProcessAction(Action action) => action();
}