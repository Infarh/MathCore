using System;
using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using INN = MathCore.Annotations.ItemNotNullAttribute;
using ICN = MathCore.Annotations.ItemCanBeNullAttribute;

namespace MathCore.Monads.WorkFlow
{
    /// <summary>Работа, выполняющая преобразование данных указанным методом</summary>
    /// <typeparam name="TParameter">Тип сходных данных для преобразования</typeparam>
    /// <typeparam name="TResult">Тип результата преобразования</typeparam>
    public class Work<TParameter, TResult> : Work<TResult>
    {
        /// <summary>Функция преобразования значения, выполняемая в рамках работы</summary>
        private readonly Func<TParameter, TResult> _WorkFunction;

        /// <summary>Внутренняя инициализация новой работы по преобразованию значения на основе указанной функции</summary>
        /// <param name="WorkFunction">Функция, преобразующая значение</param>
        /// <param name="BaseWork">Базовая работа, являющаяся источником аргумента функции</param>
        private Work([CN] Func<TParameter, TResult> WorkFunction, [NN] Work BaseWork) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

        /// <summary>Инициализация новой работы по преобразованию значения на основе указанной функции</summary>
        /// <param name="WorkFunction">Функция, преобразующая значение</param>
        /// <param name="BaseWork">Базовая работа, являющаяся источником аргумента функции</param>
        internal Work([NN] Func<TParameter, TResult> WorkFunction, [NN] Work<TParameter> BaseWork) : this(WorkFunction, (Work)BaseWork) { }

        /// <inheritdoc />
        protected override IWorkResult Execute(IWorkResult BaseResult)
        {
            var base_result = (IWorkResult<TParameter>)BaseResult ?? throw new InvalidOperationException("Отсутствует базовая задача", new ArgumentNullException(nameof(BaseResult)));
            var parameter = base_result.Result;
            try
            {
                return new WorkResult<TParameter, TResult>(parameter, _WorkFunction(parameter), BaseResult.Error);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception error)
            {
                return new WorkResult<TParameter, TResult>(parameter, BaseResult.Error, error);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>Выполнение работы</summary>
        /// <returns>Результат выполнения работы, содержавший исходные данные и полученное значение</returns>
        [NN] public new IWorkResult<TParameter, TResult> Execute() => (IWorkResult<TParameter, TResult>)base.Execute();
    }

    /// <summary>Работа, возвращающая значение</summary>
    /// <typeparam name="T">Тип результата работы</typeparam>
    public abstract class Work<T> : Work
    {
        #region Задачи с условием

        /// <summary>Работа по преобразованию значения, выполняемая в случае, если предыдущая работа была выполнена успешно</summary>
        /// <typeparam name="TParameter">Тип исходных данных преобразования</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        private class FunctionWorkIfSuccess<TParameter, TResult> : Work<TParameter, TResult>
        {
            /// <summary>Инициализация новой работы по преобразованию значения, выполняемой в случае если предыдущая работа была выполнена успешно</summary>
            /// <param name="WorkFunction">Метод преобразования значения</param>
            /// <param name="BaseWork">Предыдущая работа, формирующая исходные данные для текущей выполняемой работы</param>
            internal FunctionWorkIfSuccess([NN] Func<TParameter, TResult> WorkFunction, [NN] Work<TParameter> BaseWork) : base(WorkFunction, BaseWork) { }

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult) =>
                (BaseResult ?? throw new InvalidOperationException("Отсутствует результат вычисления предыдущей работы")).Success
                    ? base.Execute(BaseResult)
                    : new WorkResult<TParameter, TResult>(BaseResult.Error);
        }

        /// <summary>Работа, выполняемая над результатом предыдущей работы</summary>
        /// <typeparam name="TParameter">Тип результата предыдущей работы</typeparam>
        private class ActionWork<TParameter> : Work
        {
            /// <summary>Действие, выполняемое над результатом предыдущей работы</summary>
            [NN] private readonly Action<TParameter> _WorkAction;

            /// <summary>Инициализация новой работы, выполняющей действие над результатом выполнения предыдущей работы</summary>
            /// <param name="WorkAction">Действие, выполняемое над результатом предыдущей работы</param>
            /// <param name="BaseWork">Предыдущая работа</param>
            public ActionWork([NN] Action<TParameter> WorkAction, [CN] Work<TParameter> BaseWork) : base(BaseWork) => _WorkAction = WorkAction ?? throw new ArgumentNullException(nameof(WorkAction));

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = (IWorkResult<TParameter>)BaseResult ?? throw new InvalidOperationException("Отсутствует результат выполнения предыдущей работы");
                try
                {
                    _WorkAction(base_result.Result);
                    return new WorkResult(base_result.Error);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception error)
                {
                    return new WorkResult(base_result.Error, error);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        #endregion

        /// <summary>Инициализация новой работы</summary><param name="BaseWork">объект базовой работы</param>
        protected Work([CN] Work BaseWork) : base(BaseWork) { }

        /// <summary>Выполнение работы</summary><returns>Результат работы</returns>
        [NN] public new IWorkResult<T> Execute()
        {
            var work_result = base.Execute();
            return (IWorkResult<T>)work_result;
        }

        /// <summary>Действие, выполняемое в любом случае для результата предыдущей работы</summary>
        /// <param name="action">Выполняемое действие</param>
        /// <returns>Работа, выполняющая действие для результата выполнения предыдущей работы</returns>
        [NN] public Work Invoke([NN] Action<T> action) => new ActionWork<T>(action, this);

        /// <summary>Работа, в результате которой формируется результат</summary>
        /// <typeparam name="TResult">Тип результата работы</typeparam>
        /// <param name="function">Функция, выполняемая в рамках работы над параметром, получаемым от предыдущей работы</param>
        /// <returns>Работа по получению результата</returns>
        [NN] public Work<T, TResult> Get<TResult>([NN] Func<T, TResult> function) => new Work<T, TResult>(function, this);

        /// <summary>Выполнение преобразования в случае если предыдущая работа выполнена успешно</summary>
        /// <typeparam name="TResult">Результат преобразования</typeparam>
        /// <param name="function">Метод преобразования значения</param>
        /// <returns>Работа по преобразованию значения, выполняемая в случае, если предыдущая работа выполнена успешно</returns>
        [NN] public Work<T, TResult> GetIfSuccess<TResult>([NN] Func<T, TResult> function) => new FunctionWorkIfSuccess<T, TResult>(function, this);

        /// <summary>Добавление обработчика исключительных ситуаций</summary>
        /// <param name="ErrorHandler">Функция, получающая в качестве параметра исключение и на его основе формирующая значение функции</param>
        /// <returns>Работа по обработке исключений, возникающих на предыдущих этапах выполнения работы</returns>
        [NN] public Work<T> GetIfFailure([NN] Func<Exception, T> ErrorHandler) => new ExceptionFunctionHandler<T>(ErrorHandler, this);

        /// <summary>Оператор неявного преобразования типа <see cref="Work{T}"/> к типу <typeparamref name="T"/></summary>
        /// <param name="work">Работа с типизированным результатом <see cref="Work{T}"/></param>
        /// <returns>Результат выполнения работы типа <typeparamref name="T"/></returns>
        public static implicit operator T([NN] Work<T> work) => work.Execute().Result;
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

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Success ?? true ? base.Execute(BaseResult) : BaseResult;
        }

        /// <summary>Работа на основе действия, выполняемая лишь в том случае, если базовая работа выполнилась с ошибкой</summary>
        private class ActionWorkIfFailure : ActionWork
        {
            /// <summary>Инициализация новой работы, выполняемой в случае ошибочного выполнения предыдущей работы</summary>
            /// <param name="WorkAction">Действие, выполняемое в рамках работы</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal ActionWorkIfFailure([NN] Action WorkAction, [NN] Work BaseWork) : base(WorkAction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult) => BaseResult?.Failure ?? false ? base.Execute(BaseResult) : BaseResult ?? new WorkResult();
        }

        /// <summary>Работа по обработке ошибок</summary>
        protected class ExceptionActionHandler : Work
        {
            /// <summary>Действие-обработчик исключения</summary>
            [NN] private readonly Action<Exception> _ErrorHandler;

            /// <summary>Инициализация новой работы по обработке ошибок</summary>
            /// <param name="ErrorHandler">Действие-обработчик ошибок</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal ExceptionActionHandler([NN] Action<Exception> ErrorHandler, [NN] Work BaseWork) : base(BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) => _ErrorHandler = ErrorHandler ?? throw new ArgumentNullException(nameof(ErrorHandler));

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                if (base_result.Success)
                    return base_result;
                try
                {
                    _ErrorHandler(base_result.Error);
                    return new WorkResult(base_result.Error);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception error)
                {
                    return new WorkResult(base_result.Error, error);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>Работа, возвращающая результат выполнения функции в случае если предыдущая работа завершилась успешно</summary>
        /// <typeparam name="T">Тип значения функции</typeparam>
        private class FunctionWorkIfSuccess<T> : FunctionWork<T>
        {
            /// <summary>Инициализация новой работы, возвращающей значение функции в случае если предыдущая работа завершилась успешно</summary>
            /// <param name="WorkFunction">Функция, выполняемая в рамках работы</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal FunctionWorkIfSuccess([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                return base_result.Success
                    ? base.Execute(base_result)
                    : new WorkResult<T>(default(T), base_result.Error);
            }
        }

        /// <summary>Работа по преобразованию значения, выполняемая в случае, если предыдущая работа завершилась исключением</summary>
        /// <typeparam name="T">Тип значения</typeparam>
        private class FunctionWorkIfFailure<T> : FunctionWork<T>
        {
            /// <summary>Инициализация новой работы, выполняемой в случае неудачи предыдущей работы</summary>
            /// <param name="WorkFunction">Функция - генератор значения</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal FunctionWorkIfFailure([NN] Func<T> WorkFunction, [NN] Work BaseWork) : base(WorkFunction, BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) { }

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult) =>
                (BaseResult ?? throw new InvalidOperationException("Отсутствует результат выполнения базовой задачи")).Success
                    ? new WorkResult<T>(default(T), BaseResult.Error)
                    : base.Execute(BaseResult);
        }

        /// <summary>Работа по обработке ошибок</summary><typeparam name="T">Тип исключительной ситуации</typeparam>
        protected class ExceptionFunctionHandler<T> : Work<T>
        {
            /// <summary>Функция-обработчик исключения</summary>
            [NN] private readonly Func<Exception, T> _ErrorHandler;

            /// <summary>Инициализация новой работы по обработке ошибок</summary>
            /// <param name="ErrorHandler">Функция-обработчик ошибок</param>
            /// <param name="BaseWork">Базовая работа</param>
            internal ExceptionFunctionHandler([NN] Func<Exception, T> ErrorHandler, [NN] Work<T> BaseWork) : base(BaseWork ?? throw new ArgumentNullException(nameof(BaseWork))) => _ErrorHandler = ErrorHandler ?? throw new ArgumentNullException(nameof(ErrorHandler));

            /// <inheritdoc />
            protected override IWorkResult Execute(IWorkResult BaseResult)
            {
                var base_result = BaseResult.NotNull();
                if (base_result.Success)
                    return base_result;
                try
                {
                    return new WorkResult<T>(_ErrorHandler(base_result.Error));
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception error)
                {
                    return new WorkResult<T>(base_result.Error, error);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        #endregion

        #region Begin actions

        /// <summary>Начало работы на основе делегата действия</summary>
        /// <param name="WorkAction">Делегат, на основе которого формируется работа</param>
        /// <returns>Работа, выполняющая указанный делегат</returns>
        [NN] public static ActionWork BeginInvoke([NN] Action WorkAction) => new ActionWork(WorkAction);

        /// <summary>Начало работы с указанной функцией</summary>
        /// <typeparam name="T">Тип значения, возвращаемого функцией</typeparam>
        /// <param name="WorkFunction">Функция, выполняемая работой</param>
        /// <returns>Работа, выполняющая указанную функцию, возвращающую значение</returns>
        [NN] public static FunctionWork<T> BeginGet<T>([NN] Func<T> WorkFunction) => new FunctionWork<T>(WorkFunction);

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
        [NN]
        public IWorkResult Execute() => Execute(_BaseWork?.Execute());

        #endregion

        /// <summary>Действие, выполняемое в любом случае</summary>
        /// <param name="action">Выполняемое действие</param>
        /// <returns>Сформированная работа, выполняемая в любом случае</returns>
        [NN] public Work Invoke([NN] Action action) => new ActionWork(action, this);

        /// <summary>Действие, выполняемое в случае успеха предыдущего действия</summary>
        /// <param name="action">Выполняемое действие</param>
        /// <returns>Сформированная работа, выполняемая в случае успеха предыдущего действия</returns>
        [NN] public Work InvokeIfSuccess([NN] Action action) => new ActionWorkIfSuccess(action, this);

        /// <summary>Работа, которую надо выполнить в случае, если предыдущая работа завершилась с ошибкой</summary>
        /// <param name="action">Действие, выполняемое в случае неудачи предыдущего действия</param>
        /// <returns>Сформированная работа, выполняемая в случае неудачи предыдущего действия</returns>
        [NN] public Work InvokeIfFailure([NN] Action action) => new ActionWorkIfFailure(action, this);

        /// <summary>Действие, выполняемое в случае неудачи предыдущего действия</summary>
        /// <param name="ErrorHandler">Обработчик ошибки</param>
        /// <returns>Сформированная работа, выполняемая в случае неудачи предыдущего действия</returns>
        [NN] public Work InvokeIfFailure([NN] Action<Exception> ErrorHandler) => new ExceptionActionHandler(ErrorHandler, this);

        /// <summary>Выполнение функции в любом случае</summary>
        /// <typeparam name="T">Тип значения функции</typeparam>
        /// <param name="function">Функция, выполняемая в рамках работы</param>
        /// <returns>Работа, выполняющая функцию, возвращающую значение</returns>
        [NN] public Work<T> Get<T>([NN] Func<T> function) => new FunctionWork<T>(function, this);

        /// <summary>Выполнение функции в случае если предыдущая работа завершилась успешно</summary>
        /// <typeparam name="T">Тип результата функции</typeparam>
        /// <param name="function">Выполняемая функция</param>
        /// <returns>Работа, выполняющая функцию в случае если предыдущая работа завершилась успешно</returns>
        [NN] public Work<T> GetIfSuccess<T>([NN] Func<T> function) => new FunctionWorkIfSuccess<T>(function, this);

        /// <summary>Работа, выполняемая в случае если функция завершилась ошибкой</summary>
        /// <typeparam name="T">Тип значения функции</typeparam>
        /// <param name="function">Функция, выполняемая в случае неудачи</param>
        /// <returns>Работа, выполняющая функцию в случае неудачи предыдущей работы</returns>
        [NN] public Work<T> GetIfFailure<T>([NN] Func<T> function) => new FunctionWorkIfFailure<T>(function, this);
    }
}