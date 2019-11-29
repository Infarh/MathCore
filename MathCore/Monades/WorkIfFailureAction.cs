using System;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public sealed class WorkIfFailureAction : WorkAction
    {
        public WorkIfFailureAction([NotNull] Action action, Work BaseWork = null) : base(action, BaseWork) { }

        protected override void ExecuteWork()
        {
            if (Failure) base.ExecuteWork();
        }
    }
}