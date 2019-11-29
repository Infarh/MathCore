using System;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public sealed class WorkIfSuccessAction : WorkAction
    {
        public WorkIfSuccessAction([NotNull] Action action, Work BaseWork = null) : base(action, BaseWork) { }

        protected override void ExecuteWork()
        {
            if (Success) base.ExecuteWork();
        }
    }
}