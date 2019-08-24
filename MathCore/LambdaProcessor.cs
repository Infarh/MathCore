using System.Diagnostics.Contracts;

namespace System
{
    public class LambdaProcessor : Processor
    {
        private readonly Action _Action;

        public LambdaProcessor(Action action)
        {
            Contract.Requires(action != null, "Указана нуливая ссылка на выполняемое действие");

            _Action = action;
        }

        /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
        protected override void MainAction() => _Action();
    }
}