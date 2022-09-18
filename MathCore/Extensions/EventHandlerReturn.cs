// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Тип обработчика события, подразумевающий возможность возврата значения</summary>
/// <param name="Sender">Объект-источник события</param>
/// <param name="args">Аргумент события</param>
/// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
/// <typeparam name="TReturn">Тип возвращаемого значения</typeparam>
[Serializable]
public delegate TReturn EventHandlerReturn<in TEventArgs, out TReturn>(object Sender, TEventArgs args) where TEventArgs : EventArgs;