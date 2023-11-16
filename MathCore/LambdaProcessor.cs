#nullable enable
// ReSharper disable once CheckNamespace
namespace System;

public class LambdaProcessor(Action action) : Processor
{

    /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
    protected override void MainAction() => action();
}