using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public abstract class Work
    {
        #region Запуск

        [NotNull] public static Work Begin([NotNull] Action action) => new WorkAction(action);

        [NotNull] public static Work<T> Begin<T>([NotNull] Func<T> function) => new WorkFunction<T>(function);

        [NotNull] public static Work<T> With<T>(T value) => new WorkValue<T>(value);
        private class WorkValue<T> : Work<T>
        {
            public override bool Success => true;

            public WorkValue(T Value, Work BaseWork = null) : base(BaseWork)
            {
                _Result = Value;
                Executed = true;
            }

            protected override void ExecuteWork() { }
        }

        #endregion

        #region Поля

        [CanBeNull] protected readonly Work _BaseWork;

        #endregion

        #region Свойства

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

        public bool Executed { get; private set; }

        [CanBeNull] public Exception CurrentError { get; private set; }

        [CanBeNull] public Exception Error => CurrentError ?? SubWorks.FirstOrDefault(w => w.CurrentError != null)?.CurrentError;

        public virtual bool Success => CurrentError is null && SubWorks.All(work => work.Success);

        public bool Failure => !Success; 

        #endregion

        protected internal Work(Work BaseWork = null) => _BaseWork = BaseWork;

        protected abstract void ExecuteWork();

        public void Execute()
        {
            CurrentError = null;
            _BaseWork?.Execute();
            try
            {
                ExecuteWork();
            }
            catch (Exception error)
            {
                CurrentError = error;
            }

            Executed = true;
        }

        #region Work -> Work

        [NotNull] public Work Anyway([NotNull] Action action) => new WorkAction(action, this);
        private class WorkAction : Work
        {
            [NotNull] private readonly Action _Action;

            public WorkAction([NotNull] Action action, Work BaseWork = null) : base(BaseWork) => _Action = action ?? throw new ArgumentNullException(nameof(action));

            protected override void ExecuteWork() => _Action();
        }

        [NotNull] public Work IfSuccess([NotNull] Action action) => new WorkIfSuccessAction(action, this);
        private sealed class WorkIfSuccessAction : WorkAction
        {
            public WorkIfSuccessAction([NotNull] Action action, Work BaseWork = null) : base(action, BaseWork) { }
            protected override void ExecuteWork() { if (_BaseWork?.Failure != true) base.ExecuteWork(); }
        }

        [NotNull] public Work IfFailure([NotNull] Action action) => new WorkIfFailureAction(action, this);
        private sealed class WorkIfFailureAction : WorkAction
        {
            public WorkIfFailureAction([NotNull] Action action, Work BaseWork = null) : base(action, BaseWork) { }
            protected override void ExecuteWork() { if (_BaseWork?.Success != true) base.ExecuteWork(); }
        }

        [NotNull] public Work IfFailure([NotNull] Action<Exception> handler) => new WorkFailureHandlerAction(handler, this);

        private class WorkFailureHandlerAction : Work
        {
            private readonly Action<Exception> _ExceptionHandler;

            public WorkFailureHandlerAction(Action<Exception> ExceptionHandler, Work BaseWork) : base(BaseWork) => _ExceptionHandler = ExceptionHandler;

            protected override void ExecuteWork() { if(_BaseWork?.Success != true) _ExceptionHandler(Error); }
        }

        #endregion

        #region Work -> Work<T>

        [NotNull] public Work<T> Anyway<T>([NotNull] Func<T> function) => new WorkFunction<T>(function, this);
        private class WorkFunction<T> : Work<T>
        {
            [NotNull] private readonly Func<T> _Function;

            internal WorkFunction([NotNull] Func<T> function, Work BaseWork = null) : base(BaseWork) => _Function = function ?? throw new ArgumentNullException(nameof(function));

            protected override void ExecuteWork() => _Result = _Function();
        }

        [NotNull] public Work<T> IfSuccess<T>([NotNull] Func<T> function) => new WorkSuccessFunction<T>(function, this);

        private sealed class WorkSuccessFunction<T> : WorkFunction<T>
        {
            internal WorkSuccessFunction([NotNull] Func<T> function, [CanBeNull] Work BaseWork) : base(function, BaseWork) { }

            protected override void ExecuteWork() { if (_BaseWork?.Failure != false) base.ExecuteWork(); }
        }

        [NotNull] public Work<T> IfFailure<T>([NotNull] Func<T> function) => new WorkFailureFunction<T>(function, this);

        private sealed class WorkFailureFunction<T> : WorkFunction<T>
        {
            internal WorkFailureFunction([NotNull] Func<T> function, Work BaseWork) : base(function, BaseWork) { }

            protected override void ExecuteWork() { if (_BaseWork?.Success != true) base.ExecuteWork(); }
        } 

        #endregion
    }

    public abstract class Work<T> : Work
    {
        protected internal T _Result;

        public virtual T Result
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

        protected internal Work([CanBeNull] Work BaseWork) : base(BaseWork) { }

        [NotNull] public Work<TResult> Anyway<TResult>([NotNull] Func<T, TResult> Selector) => new Work<T, TResult>(Selector, this);

        [NotNull] public Work<TResult> IfSuccess<TResult>([NotNull] Func<T, TResult> Selector) => new WorkIfSuccess<TResult>(Selector, this);
        private sealed class WorkIfSuccess<TResult> : Work<T, TResult>
        {
            internal WorkIfSuccess([NotNull] Func<T, TResult> Selector, [NotNull] Work<T> BaseWork) : base(Selector, BaseWork) { }

            protected override void ExecuteWork() { if(_BaseWork.NotNull().Success) base.ExecuteWork(); }
        }

        [NotNull] public Work<T> IfFailure([NotNull] Func<Exception, T> Selector) => new WorkIfFailure(Selector, this);
        private sealed class WorkIfFailure : Work<T>
        {
            private readonly Func<Exception, T> _Selector;

            public override T Result
            {
                get
                {
                    var base_work = (Work<T>)_BaseWork.NotNull();
                    return base_work.Success ? base_work.Result : base.Result;
                }
            }

            internal WorkIfFailure([NotNull] Func<Exception, T> Selector, [NotNull] Work<T> BaseWork) : base(BaseWork) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));

            protected override void ExecuteWork() { if(_BaseWork.NotNull().Failure) _Result = _Selector(Error);  }
        }
    }

    public class Work<T, TResult> : Work<TResult>
    {
        private readonly Func<T, TResult> _Selector;

        protected internal Work([NotNull] Func<T, TResult> Selector, [NotNull] Work<T> BaseWork) : base(BaseWork) => _Selector = Selector ?? throw new ArgumentNullException(nameof(Selector));
        protected override void ExecuteWork() => _Result = _Selector(((Work<T>)_BaseWork).NotNull()._Result);
    }
}