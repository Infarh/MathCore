using System.Collections.Generic;
using System.Linq;

namespace System
{
    public class ActionsCollectionProcessor : Processor
    {
        private readonly IEnumerable<Action> _ActionsCollection;

        public ActionsCollectionProcessor(IEnumerable<Action> ActionsCollection) => _ActionsCollection = ActionsCollection;

        /// <summary>Основной метод действия процессора, вызываемое в цикле. Должно быть переопределено в классах-наследниках</summary>
        protected override void MainAction() => _ActionsCollection.Foreach(a => a());
    }
}