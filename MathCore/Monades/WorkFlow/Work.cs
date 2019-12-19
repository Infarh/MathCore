using System;
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
            [NN] private readonly Func<TParameter, Exception, TResult> _ExceptionHandler;
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

        private Work([CN] Func<TParameter, TResult> WorkFunction, [NN] Work BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

        internal Work([NN] Func<TParameter, TResult> WorkFunction, [NN] Work<TParameter> BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

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

        [NN] public Work<TParameter, TResult> IfFailure([NN] Func<TParameter, TResult> funciton) => new ExceptionHandlerAlternateResult(funciton, this);

        [NN] public Work<TParameter, TResult> IfFailure([NN] Func<TParameter, Exception, TResult> funciton) => new ExceptionHandlerAlternateResultWithException(funciton, this);
    }

    /// <summary>Работа, возвращающая значение</summary>
    /// <typeparam name="T">Тип результата работы</typeparam>
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

        /// <summary>Инициализация новой работы</summary><param name="BaseWork">объект базовой работы</param>
        protected Work([CN] Work BaseWork) : base(BaseWork) { }

        /// <summary>Выполнение работы</summary><returns>Результат работы</returns>
        [NN] public new IWorkResult<T> Execute() => (IWorkResult<T>)base.Execute();

        [NN] public Work<T, TResult> Do<TResult>([NN] Func<T, TResult> function) => new Work<T, TResult>(function, this);

        [NN] public Work<T, TResult> IsSuccess<TResult>([NN] Func<T, TResult> function) => new FunctionWorkIfSuccess<T, TResult>(function, this);
    }

    /// <summary>Класс-оболочка для выполняемой работы</summary>
    public abstract class Work
    {
        #region Задачи с условием

        /// <summary>Работа на основе действия, выполняемая лишь в том случае, если базовая работа выполнилась успешно</summary>
        private class ActionWorkIfSuccess : ActionWork
        {
            /// <summary>Инициализация новой работы, выполняемой в случае успешного выполнения предыдущей работы</summary>
            /// <param name="WorkAction">Действие, выполняемое в рамках работы</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal ActionWorkIfSuccess([NN] Action WorkAction, [NN] Work BaseWork) : base(WorkAction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Success ?? true ? base.Execute(BaseResult) : BaseResult;
        }

        /// <summary>Работа на основе действия, выполняемая лишь в том случае, если базовая работа выполнилась с ошибкой</summary>
        private class ActionWorkIfFailure : ActionWork
        {
            /// <summary>Инициализация новой работы, выполняемой в случае ошибочного выполнения предыдущей работы</summary>
            /// <param name="WorkAction">Действие, выполняемое в рамках работы</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal ActionWorkIfFailure([NN] Action WorkAction, [NN] Work BaseWork) : base(WorkAction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Failure ?? false ? base.Execute(BaseResult) : BaseResult ?? new WorkResult();
        }

        private class FunctionWorkIfSuccess<T> : FunctionWork<T>
        {
            internal FunctionWorkIfSuccess([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                return base_result.Success
                    ? base.Execute(BaseResult)
                    : new WorkResult<T>(default(T), base_result.Error);
            }
        }

        private class FunctionWorkIfFailure<T> : FunctionWork<T>
        {
            internal FunctionWorkIfFailure([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                return base_result.Failure
                    ? base.Execute(BaseResult)
                    : new WorkResult<T>(default(T), base_result.Error);
            }
        }

        protected class ExceptionHandler<T> : Work<T>
        {
            [NN] private readonly Func<Exception, T> _ErrorHandler;

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

        /// <summary>Начало работы на основе делегата действия</summary>
        /// <param name="WorkAction">Делегат, на основе которого формируется работа</param>
        /// <returns>Работа, выполняющая указанный делегат</returns>
        [NN] public static ActionWork Begin([NN] Action WorkAction) => new ActionWork(WorkAction);

        /// <summary>Начало работы с указанной функцией</summary>
        /// <typeparam name="T">Тип значения, возвращаемого функцией</typeparam>
        /// <param name="WorkFunction">Функция, выполняемая работой</param>
        /// <returns>Работа, выполняющая указанную функцию, возвращающую значение</returns>
        [NN] public static FunctionWork<T> Begin<T>([NN] Func<T> WorkFunction) => new FunctionWork<T>(WorkFunction);

        /// <summary>Фиксированное исходное значение для начала работы</summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="value">Исходное, используемое в дальнейшем, значение</param>
        /// <returns>Работа, результатом которой является указанное значение</returns>
        [NN] public static Work<T> With<T>(T value) => new ConstValueWork<T>(value);

        #endregion

        /// <summary>Базовая работа</summary>
        [CN] private readonly Work _BaseWork;

        /// <summary>Инициализация нового работы</summary><param name="BaseWork">Базовая работа</param>
        protected Work([CN] Work BaseWork) => _BaseWork = BaseWork;

        #region Execute

        /// <summary>Выполнить действие текущей работы</summary>
        /// <param name="BaseResult">Результат выполнения предыдущей работы</param>
        /// <returns>Результат выполнения действия</returns>
        [NN] protected abstract IWorkResult Execute([CN] IWorkResult BaseResult);

        /// <summary>Выполнить работу</summary>
        /// <returns>Результат выполнения работы</returns>
        [NN] public IWorkResult Execute() => Execute(_BaseWork?.Execute());

        #endregion

        /// <summary>Действие, выполняемое в любом случае</summary>
        /// <param name="action">Выполняемое действие</param>
        /// <returns>СФормированная работа, выполняемая в любом случае</returns>
        [NN] public Work Do([NN] Action action) => new ActionWork(action, this);

        /// <summary>Действие, выполняемое в случае успеха предыдущего действия</summary>
        /// <param name="action">Выполняемое действие</param>
        /// <returns>Сформированная работа, выполняемая в случае успеха предыдущего действия</returns>
        [NN] public Work IfSuccess([NN] Action action) => new ActionWorkIfSuccess(action, this);

        /// <summary>Действие, выполняемое в случае неудачи предыдущего действия</summary>
        /// <param name="action"></param>
        /// <returns>Сформированная работа, выполняемая в случае неудачи предыдущего действия</returns>
        [NN] public Work IfFailure([NN] Action action) => new ActionWorkIfFailure(action, this);

        /// <summary>Выполнение функции в любом случае</summary>
        /// <typeparam name="T">Тип значения функции</typeparam>
        /// <param name="function">Функция, выполняемая в рамках работы</param>
        /// <returns>Работа, выполняющая функцию, возвращающую значение</returns>
        [NN] public Work<T> Do<T>([NN] Func<T> function) => new FunctionWork<T>(function, this);

        /// <summary>Выполнение функции в случае если предыдущая работа завершилась успешно</summary>
        /// <typeparam name="T">Тип результата функции</typeparam>
        /// <param name="function">Выполняемая функция</param>
        /// <returns>Работа, выполняющая функцию в случае если предыдущая работа завершилась успешно</returns>
        [NN] public Work<T> IfSuccess<T>([NN] Func<T> function) => new FunctionWorkIfSuccess<T>(function, this);

        /// <summary>Работа, выполняемая в случае если функция завершилась ошибкой</summary>
        /// <typeparam name="T">Тип значения функции</typeparam>
        /// <param name="function">Функция, выполняемая в случае неудачи</param>
        /// <returns>Работа, выполняющая функцию в случае неудачи предыдущей работы</returns>
        [NN] public Work<T> IfFailure<T>([NN] Func<T> function) => new FunctionWorkIfFailure<T>(function, this);

        [NN] public Work<T> IfFailure<T>([NN] Func<Exception, T> ErrorHandler) => new ExceptionHandler<T>(ErrorHandler, this);
    }
}