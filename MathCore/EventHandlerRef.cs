namespace System;

/// <summary>Делегат обработчика события с аргументами по ссылке</summary>
/// <typeparam name="TEventArgs">Тип аргументов события</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="e">Аргументы события</param>
public delegate void EventHandlerRef<TEventArgs>(object Sender, in TEventArgs e = default) where TEventArgs : struct;

/// <summary>Делегат обработчика события с двумя аргументами по ссылке</summary>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="e">Аргументы события</param>
public delegate void EventHandlerRef<T1, T2>(object Sender, in EventArgsRef<T1, T2> e = default);

/// <summary>Аргументы события с двумя значениями по ссылке</summary>
/// <param name="Arg1">Первый аргумент</param>
/// <param name="Arg2">Второй аргумент</param>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
public readonly ref struct EventArgsRef<T1, T2>(T1 Arg1, T2 Arg2)
{
    /// <summary>Первый аргумент</summary>
    public T1 Arg1 { get; init; } = Arg1;

    /// <summary>Второй аргумент</summary>
    public T2 Arg2 { get; init; } = Arg2;

    /// <summary>Деконструкция аргументов</summary>
    /// <param name="arg1">Первый аргумент</param>
    /// <param name="arg2">Второй аргумент</param>
    public void Deconstruct(out T1 arg1, out T2 arg2)
    {
        arg1 = Arg1;
        arg2 = Arg2;
    }
}

/// <summary>Делегат обработчика события с тремя аргументами по ссылке</summary>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
/// <typeparam name="T3">Тип третьего аргумента</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="e">Аргументы события</param>
public delegate void EventHandlerRef<T1, T2, T3>(object Sender, in EventArgsRef<T1, T2, T3> e = default);

/// <summary>Аргументы события с тремя значениями по ссылке</summary>
/// <param name="Arg1">Первый аргумент</param>
/// <param name="Arg2">Второй аргумент</param>
/// <param name="Arg3">Третий аргумент</param>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
/// <typeparam name="T3">Тип третьего аргумента</typeparam>
public readonly ref struct EventArgsRef<T1, T2, T3>(T1 Arg1, T2 Arg2, T3 Arg3)
{
    /// <summary>Первый аргумент</summary>
    public T1 Arg1 { get; init; } = Arg1;
    /// <summary>Второй аргумент</summary>
    public T2 Arg2 { get; init; } = Arg2;
    /// <summary>Третий аргумент</summary>
    public T3 Arg3 { get; init; } = Arg3;

    /// <summary>Деконструкция аргументов</summary>
    /// <param name="arg1">Первый аргумент</param>
    /// <param name="arg2">Второй аргумент</param>
    /// <param name="arg3">Третий аргумент</param>
    public void Deconstruct(out T1 arg1, out T2 arg2, out T3 arg3)
    {
        arg1 = Arg1;
        arg2 = Arg2;
        arg3 = Arg3;
    }
}

/// <summary>Делегат обработчика события с четырьмя аргументами по ссылке</summary>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
/// <typeparam name="T3">Тип третьего аргумента</typeparam>
/// <typeparam name="T4">Тип четвёртого аргумента</typeparam>
/// <param name="Sender">Источник события</param>
/// <param name="e">Аргументы события</param>
public delegate void EventHandlerRef<T1, T2, T3, T4>(object Sender, in EventArgsRef<T1, T2, T3, T4> e = default);

/// <summary>Аргументы события с четырьмя значениями по ссылке</summary>
/// <param name="Arg1">Первый аргумент</param>
/// <param name="Arg2">Второй аргумент</param>
/// <param name="Arg3">Третий аргумент</param>
/// <param name="Arg4">Четвёртый аргумент</param>
/// <typeparam name="T1">Тип первого аргумента</typeparam>
/// <typeparam name="T2">Тип второго аргумента</typeparam>
/// <typeparam name="T3">Тип третьего аргумента</typeparam>
/// <typeparam name="T4">Тип четвёртого аргумента</typeparam>
public readonly ref struct EventArgsRef<T1, T2, T3, T4>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4)
{
    /// <summary>Первый аргумент</summary>
    public T1 Arg1 { get; init; } = Arg1;
    /// <summary>Второй аргумент</summary>
    public T2 Arg2 { get; init; } = Arg2;
    /// <summary>Третий аргумент</summary>
    public T3 Arg3 { get; init; } = Arg3;
    /// <summary>Четвёртый аргумент</summary>
    public T4 Arg4 { get; init; } = Arg4;

    /// <summary>Деконструкция аргументов</summary>
    /// <param name="arg1">Первый аргумент</param>
    /// <param name="arg2">Второй аргумент</param>
    /// <param name="arg3">Третий аргумент</param>
    /// <param name="arg4">Четвёртый аргумент</param>
    public void Deconstruct(out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
    {
        arg1 = Arg1;
        arg2 = Arg2;
        arg3 = Arg3;
        arg4 = Arg4;
    }
}
