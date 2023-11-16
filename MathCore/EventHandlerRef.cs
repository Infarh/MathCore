namespace System;

public delegate void EventHandlerRef<TEventArgs>(object Sender, in TEventArgs e = default) where TEventArgs : struct;

public delegate void EventHandlerRef<T1, T2>(object Sender, in EventArgsRef<T1, T2> e = default);

public readonly ref struct EventArgsRef<T1, T2>(T1 Arg1, T2 Arg2)
{
    public T1 Arg1 { get; init; } = Arg1;

    public T2 Arg2 { get; init; } = Arg2;

    public void Deconstruct(out T1 arg1, out T2 arg2)
    {
        arg1 = Arg1;
        arg2 = Arg2;
    }
}

public delegate void EventHandlerRef<T1, T2, T3>(object Sender, in EventArgsRef<T1, T2, T3> e = default);

public readonly ref struct EventArgsRef<T1, T2, T3>(T1 Arg1, T2 Arg2, T3 Arg3)
{
    public T1 Arg1 { get; init; } = Arg1;
    public T2 Arg2 { get; init; } = Arg2;
    public T3 Arg3 { get; init; } = Arg3;

    public void Deconstruct(out T1 arg1, out T2 arg2, out T3 arg3)
    {
        arg1 = Arg1;
        arg2 = Arg2;
        arg3 = Arg3;
    }
}

public delegate void EventHandlerRef<T1, T2, T3, T4>(object Sender, in EventArgsRef<T1, T2, T3, T4> e = default);

public readonly ref struct EventArgsRef<T1, T2, T3, T4>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4)
{
    public T1 Arg1 { get; init; } = Arg1;
    public T2 Arg2 { get; init; } = Arg2;
    public T3 Arg3 { get; init; } = Arg3;
    public T4 Arg4 { get; init; } = Arg4;

    public void Deconstruct(out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
    {
        arg1 = Arg1;
        arg2 = Arg2;
        arg3 = Arg3;
        arg4 = Arg4;
    }
}
