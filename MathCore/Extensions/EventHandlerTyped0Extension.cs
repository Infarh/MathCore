using System.ComponentModel;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов расширений для обработчиков событий</summary>
    public static class EventHandlerTyped1Extension
    {
        /// <summary>Потоко-безопасная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        //[DST]
        public static void Start<TS, TE>(this EventHandler<TS, TE> Handler, TS Sender, EventArgs<TE> e)
        {
            var handler = Handler;
            if(handler is null) return;
            var invocations = handler.GetInvocationList();
            foreach (var invocation in invocations)
            {
                if(invocation.Target is ISynchronizeInvoke invoke && invoke.InvokeRequired)
                    invoke.Invoke(invocation, new object[] { Sender, e });
                else
                    invocation.DynamicInvoke(Sender, e);
            }
        }

        /// <summary>Потоко-безопасная асинхронная генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        /// <param name="CallBack">Метод завершения генерации события</param>
        /// <param name="State">Объект-состояние, Передаваемый в метод завершения генерации события</param>
        [DST]
        public static void StartAsync<TS, TE>(this EventHandler<TS, TE> Handler, TS Sender, EventArgs<TE> e, AsyncCallback CallBack = null, object State = null) => Handler?.BeginInvoke(Sender, e, CallBack, State);

        /// <summary>Быстрая генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        [DST]
        public static void FastStart<TSender, TEventArgs>(this EventHandler<TSender, TEventArgs> Handler, TSender Sender) => Handler?.Invoke(Sender, default);

        /// <summary>Быстрая генерация события</summary>
        /// <param name="Handler">Обработчик события</param>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        [DST]
        public static void FastStart<TSender, TEventArgs>(this EventHandler<TSender, TEventArgs> Handler, TSender Sender, EventArgs<TEventArgs> e) => Handler?.Invoke(Sender, e);

        ///// <summary>Быстрая генерация события</summary>
        ///// <param name="Handler">Обработчик события</param>
        ///// <param name="Sender">Источник события</param>
        ///// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        ///// <param name="e">Аргументы события</param>
        //[DST]
        //public static void FastStart<TEventArgs>(this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e)
        //    where TEventArgs : EventArgs =>
        //    Handler?.Invoke(Sender, e);

        ///// <summary>Потоко-безопасная генерация события</summary>
        ///// <param name="Handler">Обработчик события</param>
        ///// <param name="Sender">Источник события</param>
        ///// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        ///// <param name="e">Аргументы события</param>
        //[DST]
        //public static void Start<TEventArgs>(this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e)
        //    where TEventArgs : EventArgs
        //{
        //    var handler = Handler;
        //    if(handler is null) return;
        //    var invocations = handler.GetInvocationList();
        //    for(var i = 0; i < invocations.Length; i++)
        //    {
        //        var I = invocations[i];
        //        if(I.Target is ISynchronizeInvoke && ((ISynchronizeInvoke)I.Target).InvokeRequired)
        //            ((ISynchronizeInvoke)I.Target).Invoke(I, new[] { Sender, e });
        //        else
        //            I.DynamicInvoke(Sender, e);
        //    }
        //}

        ///// <summary>Потоко-безопасная асинхроная генерация события</summary>
        ///// <param name="Handler">Обработчик события</param>
        ///// <param name="Sender">Источник события</param>
        ///// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
        ///// <param name="e">Аргументы события</param>
        ///// <param name="CallBack">Метод завершения генерации события</param>
        ///// <param name="State">Объект-состояние, Передаваемый в метод завершения генерации события</param>
        //[DST]
        //public static void StartAsync<TEventArgs>(this EventHandler<TEventArgs> Handler,
        //    object Sender, TEventArgs e, AsyncCallback CallBack = null, object @State = null)
        //    where TEventArgs : EventArgs => Handler?.BeginInvoke(Sender, e, CallBack, State); 

        ///// <summary>Потоко-безопасная генерация события</summary>
        ///// <param name="Handler">Обработчик события</param>
        ///// <param name="Sender">Источник события</param>
        ///// <typeparam name="TArgs">Тип аргумента события</typeparam>
        ///// <param name="Args">Аргументы события</param>
        ///// <typeparam name="TResult">Тип результата обработки события</typeparam>
        ///// <typeparam name="TSender">Тип источника события</typeparam>
        ///// <returns>Массив результатов обработки события</returns>
        //[DST]
        //public static TResult[] Start<TResult, TSender, TArgs>(this EventHandler<TResult, TSender, TArgs> Handler,
        //                                                       TSender Sender, TArgs Args)
        //{
        //    var handler = Handler;
        //    if(handler is null) return new TResult[0];

        //    return handler
        //                .GetInvocationList()
        //                .Select(I => (TResult)(I.Target is ISynchronizeInvoke && ((ISynchronizeInvoke)I.Target).InvokeRequired
        //                                                   ? ((ISynchronizeInvoke)I.Target)
        //                                                                 .Invoke(I, new object[] { Sender, Args })
        //                                                   : I.DynamicInvoke(Sender, Args))).ToArray();
        //}
    }
}