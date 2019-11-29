using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public abstract class Work
    {
        [NotNull] public static WorkAction Begin([NotNull] Action action) => new WorkAction(action);

        [NotNull] public static WorkFunction<T> Begin<T>([NotNull] Func<T> function) => new WorkFunction<T>(function);

        [CanBeNull] protected readonly Work _BaseWork;

        [CanBeNull] private Exception _CurrentError;

        private bool _Executed;

        [NotNull, ItemNotNull]
        public IEnumerable<Work> SubWorks
        {
            get
            {
                var work = _BaseWork;
                while (work != null)
                {
                    yield return work;
                    work = work._BaseWork;
                }
            }
        }

        public bool Executed => _Executed;

        [CanBeNull] public Exception CurrentError => _CurrentError;

        [CanBeNull] public Exception Error => _CurrentError ?? SubWorks.FirstOrDefault(w => w.CurrentError != null)?.CurrentError;

        public virtual bool Success => _CurrentError is null && SubWorks.All(work => work.Success);

        public virtual bool Failure => !Success;

        protected Work(Work BaseWork = null) => _BaseWork = BaseWork;

        protected abstract void ExecuteWork();

        public void Execute()
        {
            _BaseWork?.Execute();
            try
            {
                ExecuteWork();
            }
            catch (Exception error)
            {
                _CurrentError = error;
            }

            _Executed = true;
        }

        public virtual void Reset()
        {
            _Executed = false;
            _CurrentError = null;
            _BaseWork?.Reset();
        }

        [NotNull] public WorkAction Anyway([NotNull] Action action) => new WorkAction(action, this);

        [NotNull] public WorkAction IfSuccess([NotNull] Action action) => new WorkIfSuccessAction(action, this);

         [NotNull] public WorkAction IfFailure([NotNull] Action action) => new WorkIfFailureAction(action, this);

        [NotNull] public WorkWithResult<T> Anyway<T>([NotNull] Func<T> function) => new WorkFunction<T>(function, this);
    }
}