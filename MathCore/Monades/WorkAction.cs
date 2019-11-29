using System;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public class WorkAction : Work
    {
        [NotNull] private readonly Action _Action;

        public WorkAction([NotNull] Action action, Work BaseWork = null) : base(BaseWork) => _Action = action ?? throw new ArgumentNullException(nameof(action));

        protected override void ExecuteWork() => _Action();
    }
}