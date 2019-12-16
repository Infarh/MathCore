using System;
using MathCore.Annotations;

namespace MathCore.Monades.WorkFlow
{
    public class FunctionWork<T> : Work<T>
    {
        private readonly Func<T> _WorkFunction;

        internal FunctionWork([NotNull] Func<T> WorkFunction, [CanBeNull] Work BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

        protected override IWorkResult Execute(IWorkResult BaseResult)
        {
            try
            {
                var result = _WorkFunction();
                return new WorkResult<T>(result, BaseResult?.Error);
            }
            catch (Exception error)
            {
                return new WorkResult<T>(BaseResult?.Error, error);
            }
        }
    }

    
}