using System.ComponentModel;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Обработчик событий генерации исключения</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргументы события</param>
    /// <typeparam name="TException">Тип исключения</typeparam>
    public delegate void ExceptionEventHandler<TException>(object Sender, ExceptionEventHandlerArgs<TException> Args)
        where TException : Exception;

    /// <summary>Класс методов расширений для обработчика событий генерации исключений</summary>
    public static class ExceptionEventHandlerExtentions
    {
        /// <summary>Генерация события обработки исключения</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        /// <typeparam name="TException">Тип исключения</typeparam>
        [DST]
        public static void Start<TException>(
            this ExceptionEventHandler<TException> Handler, 
            object Sender,
            ExceptionEventHandlerArgs<TException> e) where TException : Exception
        {
            var handler = Handler;
            if(handler is null) return;
            var invocations = handler.GetInvocationList();
            for(var i = 0; i < invocations.Length; i++)
            {
                var invocation = invocations[i];
                if(invocation.Target is ISynchronizeInvoke invocation_target && invocation_target.InvokeRequired)
                    invocation_target.Invoke(invocation, new[] { Sender, e });
                else
                    invocation.DynamicInvoke(Sender, e);
            }
        }

        /// <summary>Асинхронная генерация события обработки исключения</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        /// <param name="CallBack">Делегат заврешения вызова события</param>
        /// <param name="State">ОБъект состояния, передаваемый в обработчик завершающего метода</param>
        /// <typeparam name="TException">Тип исключения</typeparam>
        [DST]
        public static void StartAsync<TException>(
            [NotNull] this ExceptionEventHandler<TException> Handler, 
            object Sender,
            ExceptionEventHandlerArgs<TException> e, 
            AsyncCallback CallBack,
            object @State) 
            where TException : Exception =>
            Handler?.BeginInvoke(Sender, e, CallBack, State);

        /// <summary>Быстрый запуск события без учёта многопоточных компонентов</summary>
        /// <param name="Handler">Обработчики события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        /// <typeparam name="TException">Тип события</typeparam>
        [DST]
        public static void FastStart<TException>(this ExceptionEventHandler<TException> Handler, object Sender,
            ExceptionEventHandlerArgs<TException> e)
            where TException : Exception =>
            Handler?.Invoke(Sender, e);


        /// <summary>
        /// Вызвать <typeparamref name="TException">исключение</typeparamref>, 
        /// если обработчики его не обработали, либо если кто-либо из обработчиков принял решение вызвать исключение
        /// </summary>
        /// <param name="Handler">Обработчики событий</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        /// <param name="IsHandledDefault">
        /// Если истина, то исключение считается обработанным до тех пор, пока обработчик обытия не укажет обратного
        /// Если ложь, то обработчики должны явно указать, что исключение обработано.
        /// По умолчанию значение не определено (= null) - при наличии обработчиков у события исключение считается обработанным. Иначе оно генерируется. 
        /// </param>
        /// <typeparam name="TException">Тип исключения</typeparam>
        /// <exception cref="Exception"><typeparamref name="TException">Исключение</typeparamref> генерируется при отсутствии обработки его обработчиками события</exception>
        [DST]
        public static void ThrowIfUnhandled<TException>(
            this ExceptionEventHandler<TException> Handler,
            object Sender, 
            [NotNull] ExceptionEventHandlerArgs<TException> e, 
            bool? IsHandledDefault = null) 
            where TException : Exception
        {
            if(Handler != null || IsHandledDefault.GetValueOrDefault(false)) e.Handled();
            Handler.Start(Sender, e);
            if(e.IsHandled) return;
            throw e.Argument;
        }
    }
}
