using System;
using System.Text;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public sealed class WorkFunction<T> : WorkWithResult<T>
    {
        [NotNull] private readonly Func<T> _Function;

        public WorkFunction([NotNull] Func<T> function, Work BaseWork = null) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

        protected override void ExecuteWork() => _Result = _Function();
    }
}
