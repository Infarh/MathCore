using System;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public abstract class WorkWithResult<T> : Work
    {
        protected T _Result;

        public T Result
        {
            get
            {
                if (!Executed) Execute();
                var error = CurrentError;
                if (error != null)
                    throw new InvalidOperationException("Ошибка при вычислении значения", error);
                return _Result;
            }
        }

        protected WorkWithResult(Work BaseWork = null) : base(BaseWork) { }

        [NotNull] public WorkWithResult<TResult> IsSuccess<TResult>([NotNull] Func<T, TResult> function) => new WorkWithResultIfSuccess<T, TResult>(function, this);
    }

    public class WorkWithResultIfSuccess<T> : WorkWithResult<T>
    {
        [NotNull] private readonly Func<T> _Function;
        public WorkWithResultIfSuccess([NotNull] Func<T> function, [NotNull] Work BaseWork) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

        protected override void ExecuteWork()
        {
            if (Success) _Result = _Function();
        }
    }

    public class WorkWithResultIfSuccess<T, TResult> : WorkWithResult<TResult>
    {
        [NotNull] private readonly Func<T, TResult> _Function;
        public WorkWithResultIfSuccess([NotNull] Func<T, TResult> function, [NotNull] WorkWithResult<T> BaseWork) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

        protected override void ExecuteWork()
        {
            if (Failure) return;
            var base_work = (WorkWithResult<T>)_BaseWork ?? throw new InvalidOperationException("Предыдущая работа не определена");
            _Result = _Function(base_work.Result);
        }
    }

    public class WorkWithResultIfFailure<T> : WorkWithResult<T>
    {
        [NotNull] private readonly Func<T> _Function;
        public WorkWithResultIfFailure([NotNull] Func<T> function, [NotNull] Work BaseWork) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

        protected override void ExecuteWork()
        {
            if (Failure) _Result = _Function();
        }
    }

    public class WorkWithResultIfFailure<TException, TResult> : WorkWithResult<TResult> where TException : Exception
    {
        [NotNull] private readonly Func<TException, TResult> _Function;
        public WorkWithResultIfFailure([NotNull] Func<TException, TResult> function, [NotNull] WorkWithResult<TException> BaseWork) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

        protected override void ExecuteWork()
        {
            if (Success) return;
            var base_work = (WorkWithResult<TException>)_BaseWork ?? throw new InvalidOperationException("Предыдущая работа не определена");
            _Result = _Function(base_work.Result);
        }
    }
}