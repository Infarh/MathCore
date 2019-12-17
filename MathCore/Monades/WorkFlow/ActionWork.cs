using System;

using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using INN = MathCore.Annotations.ItemNotNullAttribute;
using ICN = MathCore.Annotations.ItemCanBeNullAttribute;

namespace MathCore.Monades.WorkFlow
{
    public class ActionWork : Work
    {
        private readonly Action _WorkAction;

        internal ActionWork([NN] Action WorkAction, Work BaseWork = null) : base(BaseWork) => _WorkAction = WorkAction;

        protected override IWorkResult Execute(IWorkResult BaseResult)
        {
            try
            {
                _WorkAction();
                return new WorkResult(BaseResult?.Error);
            }
            catch (Exception error)
            {
                return new WorkResult(error, BaseResult?.Error);
            }
        }
    }
}