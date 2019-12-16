using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using INN = MathCore.Annotations.ItemNotNullAttribute;
using ICN = MathCore.Annotations.ItemCanBeNullAttribute;

namespace MathCore.Monades.WorkFlow
{
    public class Work<TParameter, TResult> : Work<TResult>
    {
        protected class ExceptionHandlerAlternateResult : Work<TParameter, TResult>
        {
            internal ExceptionHandlerAlternateResult([NN] Func<TParameter, TResult> function, [NN] Work<TParameter, TResult> BaseWork) : base(function, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = (WorkResult<TParameter, TResult>)BaseResult.NotNull();
                if (!base_result.Failure) return base_result;
                var parameter = base_result.Parameter;
                try
                {
                    return new WorkResult<TParameter, TResult>(parameter, _WorkFunction(parameter));
                }
                catch (Exception error)
                {
                    return new WorkResult<TParameter, TResult>(parameter, base_result.Error, error);
                }
            }
        }

        private class ExceptionHandlerAlternateResultWithException : Work<TParameter, TResult>
        {
            [NotNull] private readonly Func<TParameter, Exception, TResult> _ExceptionHandler;
            internal ExceptionHandlerAlternateResultWithException([NN] Func<TParameter, Exception, TResult> function, [NN] Work<TParameter, TResult> BaseWork) : base(null, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) => _ExceptionHandler = function ?? throw new ArgumentNullException(nameof(function));

            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = (WorkResult<TParameter, TResult>)BaseResult.NotNull();
                if (!base_result.Failure) return base_result;
                var parameter = base_result.Parameter;
                try
                {
                    return new WorkResult<TParameter, TResult>(parameter, _ExceptionHandler(parameter, base_result.Error));
                }
                catch (Exception error)
                {
                    return new WorkResult<TParameter, TResult>(parameter, base_result.Error, error);
                }
            }
        }

        private readonly Func<TParameter, TResult> _WorkFunction;

        private Work([CanBeNull] Func<TParameter, TResult> WorkFunction, [NN] Work BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));
        internal Work([NotNull] Func<TParameter, TResult> WorkFunction, [NN] Work<TParameter> BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

        protected override IWorkResult Execute(IWorkResult BaseResult)
        {
            var base_result = (IWorkResult<TParameter>)BaseResult ?? throw new InvalidOperationException("Отсутствует базовая задача", new ArgumentNullException(nameof(BaseResult)));
            var parameter = base_result.Result;
            try
            {
                return new WorkResult<TParameter, TResult>(parameter, _WorkFunction(parameter), BaseResult.Error);
            }
            catch (Exception error)
            {
                return new WorkResult<TParameter, TResult>(parameter, BaseResult.Error, error);
            }
        }

        [NN] public new IWorkResult<TParameter, TResult> Execute() => (IWorkResult<TParameter, TResult>)base.Execute();

        [NotNull] public Work<TParameter, TResult> IfFailure([NotNull] Func<TParameter, TResult> funciton) => new ExceptionHandlerAlternateResult(funciton, this);
        [NotNull] public Work<TParameter, TResult> IfFailure([NotNull] Func<TParameter, Exception, TResult> funciton) => new ExceptionHandlerAlternateResultWithException(funciton, this);
    }

    public abstract class Work<T> : Work
    {
        #region Задачи с условием

        private class FunctionWorkIfSuccess<TParameter, TResult> : Work<TParameter, TResult>
        {
            internal FunctionWorkIfSuccess([NN] Func<TParameter, TResult> WorkFunction, [NN] Work<TParameter> BaseWork) : base(WorkFunction, BaseWork) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult.NotNull().Success
                ? base.Execute(BaseResult)
                : new WorkResult<TResult>(default(TResult));
        } 

        #endregion

        protected Work([CN] Work BaseWork) : base(BaseWork) { }

        [NN] public new IWorkResult<T> Execute() => (IWorkResult<T>)base.Execute();

        [NN] public Work<T, TResult> Do<TResult>([NN] Func<T, TResult> function) => new Work<T, TResult>(function, this);

        [NN] public Work<T, TResult> IsSuccess<TResult>([NN] Func<T, TResult> function) => new FunctionWorkIfSuccess<T, TResult>(function, this);
    }

    public abstract class Work
    {
        #region Задачи с условием

        private class ActionWorkIfSuccess : ActionWork
        {
            internal ActionWorkIfSuccess([NN] Action WorkAction, [NN] Work BaseWork) : base(WorkAction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Success ?? true ? base.Execute(BaseResult) : BaseResult;
        }

        private class ActionWorkIfFailure : ActionWork
        {
            internal ActionWorkIfFailure([NN] Action WorkAction, [NN] Work BaseWork) : base(WorkAction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Failure ?? false ? base.Execute(BaseResult) : BaseResult ?? new WorkResult();
        }

        private class FunctionWorkIfSuccess<T> : FunctionWork<T>
        {
            internal FunctionWorkIfSuccess([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult.NotNull().Success 
                ? base.Execute(BaseResult) 
                : new WorkResult<T>(default(T));
        }

        private class FunctionWorkIfFailure<T> : FunctionWork<T>
        {
            internal FunctionWorkIfFailure([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult.NotNull().Failure 
                ? base.Execute(BaseResult) 
                : new WorkResult<T>(default(T));
        }

        protected class ExceptionHandler<T> : Work<T>
        {
            [NotNull] private readonly Func<Exception, T> _ErrorHandler;

            internal ExceptionHandler([NN] Func<Exception, T> ErrorHandler, [NN] Work BaseWork) : base(BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) => _ErrorHandler = ErrorHandler ?? throw new ArgumentNullException(nameof(ErrorHandler));

            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                if (!base_result.Failure) return base_result;
                try
                {
                    return new WorkResult<T>(_ErrorHandler(base_result.Error));
                }
                catch (Exception error)
                {
                    return new WorkResult<T>(base_result.Error, error);
                }
            }
        }

        #endregion

        #region Begin actions

        [NN] public static ActionWork Begin([NN] Action WorkAction) => new ActionWork(WorkAction);

        #endregion

        [CN] protected readonly Work _BaseWork;

        protected Work([CN] Work BaseWork) => _BaseWork = BaseWork;

        #region Execute

        [NN] protected abstract IWorkResult Execute([CN] IWorkResult BaseResult);

        [NN] public IWorkResult Execute() => Execute(_BaseWork?.Execute());

        #endregion

        [NN] public Work Do([NN] Action action) => new ActionWork(action, this);

        [NN] public Work IfSuccess([NN] Action action) => new ActionWorkIfSuccess(action, this);

        [NN] public Work IfFailure([NN] Action action) => new ActionWorkIfFailure(action, this);

        [NN] public Work<T> Do<T>([NN] Func<T> function) => new FunctionWork<T>(function, this);

        [NN] public Work<T> IfSuccess<T>([NN] Func<T> function) => new FunctionWorkIfSuccess<T>(function, this);

        [NN] public Work<T> IfFailure<T>([NN] Func<T> function) => new FunctionWorkIfFailure<T>(function, this);

        [NN] public Work<T> IfFailure<T>([NN] Func<Exception, T> ErrorHandler) => new ExceptionHandler<T>(ErrorHandler, this);
    }
}