// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedTypeParameter
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable once CheckNamespace
namespace System;

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
/// <typeparam name="TParameter">Тип параметра аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter>(TSender Sender, EventArgs<TParameter> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <typeparam name="TParameter6">Тип параметра 6 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <typeparam name="TParameter6">Тип параметра 6 аргумента события</typeparam>
/// <typeparam name="TParameter7">Тип параметра 7 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <typeparam name="TParameter6">Тип параметра 6 аргумента события</typeparam>
/// <typeparam name="TParameter7">Тип параметра 7 аргумента события</typeparam>
/// <typeparam name="TParameter8">Тип параметра 8 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <typeparam name="TParameter6">Тип параметра 6 аргумента события</typeparam>
/// <typeparam name="TParameter7">Тип параметра 7 аргумента события</typeparam>
/// <typeparam name="TParameter8">Тип параметра 8 аргумента события</typeparam>
/// <typeparam name="TParameter9">Тип параметра 9 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9> Args);

/// <summary>Делегат обработчика события</summary>
/// <typeparam name="TSender">Тип источника события</typeparam>
/// <typeparam name="TParameter1">Тип параметра 1 аргумента события</typeparam>
/// <typeparam name="TParameter2">Тип параметра 2 аргумента события</typeparam>
/// <typeparam name="TParameter3">Тип параметра 3 аргумента события</typeparam>
/// <typeparam name="TParameter4">Тип параметра 4 аргумента события</typeparam>
/// <typeparam name="TParameter5">Тип параметра 5 аргумента события</typeparam>
/// <typeparam name="TParameter6">Тип параметра 6 аргумента события</typeparam>
/// <typeparam name="TParameter7">Тип параметра 7 аргумента события</typeparam>
/// <typeparam name="TParameter8">Тип параметра 8 аргумента события</typeparam>
/// <typeparam name="TParameter9">Тип параметра 9 аргумента события</typeparam>
/// <typeparam name="TParameter10">Тип параметра 10 аргумента события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="Args">Аргумент события</param>
[Serializable]
public delegate void EventHandler<in TSender, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10>(
    TSender Sender,
    EventArgs<TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10> Args);