// ReSharper disable once CheckNamespace
namespace System
{
    public class LambdaProcessor : Processor
    {
        private readonly Action _Action;

        public LambdaProcessor(Action action) => _Action = action;

        /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
        protected override void MainAction() => _Action();
    }
}