// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System
{

    /// <summary>Делегат обработчика события</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgs">Тип аргумента события</typeparam>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргумент события</param>
    [Serializable]
    public delegate void EventHandlerArgs<in TSender, in TArgs>(TSender Sender, TArgs Args) where TArgs : EventArgs;

    /// <summary>Делегат обработчика события</summary>
    /// <typeparam name="TResult">Тип результата события</typeparam>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgs">Тип аргумента события</typeparam>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргумент события</param>
    /// <returns>Результат события</returns>
    [Serializable]
    public delegate TResult EventHandlerArgs<out TResult, in TSender, in TArgs>(TSender Sender, TArgs Args) where TArgs : EventArgs;

    /// <summary>Делегат обработчика события</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TEventParameter">Тип параметра аргумента события</typeparam>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргумент события</param>
    [Serializable]
    public delegate void EventHandler<in TSender, TEventParameter>(TSender Sender, EventArgs<TEventParameter> Args);

    /// <summary>Делегат обработчика события</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TEventParameter1">Тип параметра 1 аргумента события</typeparam>
    /// <typeparam name="TEventParameter2">Тип параметра 2 аргумента события</typeparam>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргумент события</param>
    [Serializable]
    public delegate void EventHandler<in TSender, TEventParameter1, TEventParameter2>(TSender Sender,
        EventArgs<TEventParameter1, TEventParameter2> Args);

    /// <summary>Делегат обработчика события</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TEventParameter1">Тип параметра 1 аргумента события</typeparam>
    /// <typeparam name="TEventParameter2">Тип параметра 2 аргумента события</typeparam>
    /// <typeparam name="TEventParameter3">Тип параметра 3 аргумента события</typeparam>
    /// <param name="Sender">Источник события</param>
    /// <param name="Args">Аргумент события</param>
    [Serializable]
    public delegate void EventHandler<in TSender, TEventParameter1, TEventParameter2, TEventParameter3>(TSender Sender,
        EventArgs<TEventParameter1, TEventParameter2, TEventParameter3> Args);
}