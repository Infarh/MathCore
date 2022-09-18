#nullable enable
using System.ComponentModel;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Класс методов расширений для обработчиков событий</summary>
public static class EventHandlerTyped3Extension
{
    /// <summary>Потоко-безопасная генерация события</summary>
    /// <param name="Handler">Обработчик события</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="e">Аргумент события</param>
    [DST]
    public static void Start<TSender, TEventArgs1, TEventArgs2, TEventArgs3>(
        this EventHandler<TSender, TEventArgs1, TEventArgs2, TEventArgs3>? Handler,
        TSender Sender,
        EventArgs<TEventArgs1, TEventArgs2, TEventArgs3> e)
    {
        if (Handler is null) return;
        var invocations = Handler.GetInvocationList();
        foreach (var invocation in invocations)
            if (invocation.Target is ISynchronizeInvoke { InvokeRequired: true } invoke)
                invoke.Invoke(invocation, new object?[] { Sender, e });
            else
                invocation.DynamicInvoke(Sender, e);
    }

    /// <summary>Потоко-безопасная асинхронная генерация события</summary>
    /// <param name="Handler">Обработчик события</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="e">Аргумент события</param>
    /// <param name="CallBack">Метод завершения генерации события</param>
    /// <param name="State">Объект-состояние, передаваемый в метод завершения генерации события</param>
    [DST]
    public static void StartAsync<TS, TEventArgs1, TEventArgs2, TEventArgs3>(
        this EventHandler<TS, TEventArgs1, TEventArgs2, TEventArgs3>? Handler,
        TS Sender, EventArgs<TEventArgs1, TEventArgs2, TEventArgs3> e,
        AsyncCallback? CallBack = null,
        object? State = null)
        => Handler?.BeginInvoke(Sender, e, CallBack, State);

    /// <summary>Быстрая генерация события</summary>
    /// <param name="Handler">Обработчик события</param>
    /// <param name="Sender">Источник события</param>
    [DST]
    public static void FastStart<TSender, TEventArgs1, TEventArgs2, TEventArgs3>(
        this EventHandler<TSender, TEventArgs1, TEventArgs2, TEventArgs3>? Handler,
        TSender Sender)
        => Handler?.Invoke(Sender, default);

    /// <summary>Быстрая генерация события</summary>
    /// <param name="Handler">Обработчик события</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    [DST]
    public static void FastStart<TSender, TEventArgs1, TEventArgs2, TEventArgs3>(
        this EventHandler<TSender, TEventArgs1, TEventArgs2, TEventArgs3>? Handler,
        TSender Sender,
        EventArgs<TEventArgs1, TEventArgs2, TEventArgs3> e)
        => Handler?.Invoke(Sender, e);

    ///// <summary>Быстрая генерация события</summary>
    ///// <param name="Handler">Обработчик события</param>
    ///// <param name="Sender">Источник события</param>
    ///// <param name="e">Аргументы события</param>
    //[DST]
    //public static void FastStart<TEventArgs>(this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e)
    //    where TEventArgs : EventArgs
    //{
    //    var lv_Handler = Handler;
    //    if(lv_Handler != null)
    //        lv_Handler.Invoke(Sender, e);
    //}

    ///// <summary>Потоко-безопасная генерация события</summary>
    ///// <param name="Handler">Обработчик события</param>
    ///// <param name="Sender">Источник события</param>
    ///// <param name="e">Аргументы события</param>
    //[DST]
    //public static void Start<TEventArgs>(this EventHandler<TEventArgs> Handler, object Sender, TEventArgs e)
    //    where TEventArgs : EventArgs
    //{
    //    var lv_Handler = Handler;
    //    if(lv_Handler is null) return;
    //    var invocations = lv_Handler.GetInvocationList();
    //    for(var i = 0; i < invocations.Length; i++)
    //    {
    //        var I = invocations[i];
    //        if(I.Target is ISynchronizeInvoke && ((ISynchronizeInvoke)I.Target).InvokeRequired)
    //            ((ISynchronizeInvoke)I.Target).Invoke(I, new[] { Sender, e });
    //        else
    //            I.DynamicInvoke(Sender, e);
    //    }
    //}

    ///// <summary>Потоко-безопасная асинхронная генерация события</summary>
    ///// <param name="Handler">Обработчик события</param>
    ///// <param name="Sender">Источник события</param>
    ///// <param name="e">Аргументы события</param>
    ///// <param name="CallBack">Метод завершения генерации события</param>
    ///// <param name="State">Объект-состояние, передаваемый в метод завершения генерации события</param>
    //[DST]
    //public static void StartAsync<TEventArgs>(this EventHandler<TEventArgs> Handler,
    //    object Sender, TEventArgs e, AsyncCallback CallBack = null, object @State = null)
    //    where TEventArgs : EventArgs
    //{
    //    var lv_Handler = Handler;
    //    if(lv_Handler != null)
    //        lv_Handler.BeginInvoke(Sender, e, CallBack, State);
    //}

    ///// <summary>Потоко-безопасная генерация события</summary>
    ///// <param name="Handler">Обработчик события</param>
    ///// <param name="Sender">Источник события</param>
    ///// <param name="Args">Аргументы события</param>
    ///// <returns>Массив результатов обработки события</returns>
    //[DST]
    //public static TResult[] Start<TResult, TSender, TArgs>(this EventHandler<TResult, TSender, TArgs> Handler,
    //                                                       TSender Sender, TArgs Args)
    //{
    //    var lv_Handler = Handler;
    //    if(lv_Handler is null) return new TResult[0];

    //    return lv_Handler
    //                .GetInvocationList()
    //                .Select(I => (TResult)(I.Target is ISynchronizeInvoke && ((ISynchronizeInvoke)I.Target).InvokeRequired
    //                                                   ? ((ISynchronizeInvoke)I.Target)
    //                                                                 .Invoke(I, new object[] { Sender, Args })
    //                                                   : I.DynamicInvoke(Sender, Args))).ToArray();
    //}
}