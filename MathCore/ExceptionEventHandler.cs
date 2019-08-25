using System.ComponentModel;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Diagnostics.Contracts;

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
        public static void Start<TException>(this ExceptionEventHandler<TException> Handler, object Sender,
            ExceptionEventHandlerArgs<TException> e) where TException : Exception
        {
            var lv_Handler = Handler;
            if(lv_Handler == null) return;
            var invocations = lv_Handler.GetInvocationList();
            for(var i = 0; i < invocations.Length; i++)
            {
                var lv_T = invocations[i];
                if(lv_T.Target is ISynchronizeInvoke && ((ISynchronizeInvoke)lv_T.Target).InvokeRequired)
                    ((ISynchronizeInvoke)lv_T.Target).Invoke(lv_T, new[] { Sender, e });
                else
                    lv_T.DynamicInvoke(Sender, e);
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
        public static void StartAsync<TException>(this ExceptionEventHandler<TException> Handler, object Sender,
            ExceptionEventHandlerArgs<TException> e, AsyncCallback CallBack, object @State) where TException : Exception
        {
            var lv_Handler = Handler;
            if(lv_Handler != null)
                lv_Handler.BeginInvoke(Sender, e, CallBack, State);
        }

        /// <summary>Быстрый запуск события без учёта многопоточных компонентов</summary>
        /// <param name="Handler">Обработчики события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        /// <typeparam name="TException">Тип события</typeparam>
        [DST]
        public static void FastStart<TException>(this ExceptionEventHandler<TException> Handler, object Sender,
            ExceptionEventHandlerArgs<TException> e)
            where TException : Exception
        {
            if(Handler != null)
                Handler.Invoke(Sender, e);
        }


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
        public static void ThrowIfUnhandled<TException>(this ExceptionEventHandler<TException> Handler,
            object Sender, ExceptionEventHandlerArgs<TException> e, bool? IsHandledDefault = null) where TException : Exception
        {
            Contract.Requires(e != null);
            if(Handler != null || IsHandledDefault.GetValueOrDefault(false)) e.Handled();
            Handler.Start(Sender, e);
            if(e.IsHandled) return;
            throw e.Argument;
        }
    }
}
