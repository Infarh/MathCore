#nullable enable

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedTypeParameter
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Аргумент события с типизированным параметром</summary>
[DST]
public class EventArgs<TArgument> : EventArgs
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Параметр аргумента</summary>
    public TArgument Argument { get; set;}

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgumen}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgumen}"/></summary>
    /// <param name="Argument">Параметр аргумента</param>
    public EventArgs(TArgument Argument) => this.Argument = Argument;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Строковое представление аргумента события</summary>
    public override string ToString() => Argument.ToString();

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования аргумента события к типу содержащегося в нём значения</summary>
    /// <param name="Args">Аргумент события</param>
    /// <returns>Хранимый объект</returns>
    public static implicit operator TArgument(EventArgs<TArgument> Args) => Args.Argument;

    /// <summary>Оператор неявного преобразования типа хранимого значения в обёртку из аргумента события, содержащего это значение</summary>
    /// <param name="Argument">Объект аргумента события</param>
    /// <returns>Аргумент события</returns>
    public static implicit operator EventArgs<TArgument>(TArgument Argument) => new(Argument);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 2 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2> 
    : EventArgs
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>1 параметр аргумента</summary>
    public TArgument1 Argument1 { get; set; }

    /// <summary>2 параметр аргумента</summary>
    public TArgument2 Argument2 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2) 
    {
        this.Argument1 = Argument1;
        this.Argument2 = Argument2;
    }
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2}"/></param>
    /// <returns>Кортеж из 2 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2)
        (EventArgs<TArgument1, TArgument2> Args)
        => (Args.Argument1, Args.Argument2);

    /// <summary>Оператор неявного преобразования кортежа из 2 параметров к типу <see cref="EventArgs{TArgument1, TArgument2}"/></summary>
    /// <param name="Args">Кортеж из 2 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2>
        ((TArgument1 Arg1, TArgument2 Arg2) Args)
        => new(Args.Arg1, Args.Arg2);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 3 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3> 
    : EventArgs<TArgument1, TArgument2>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>3 параметр аргумента</summary>
    public TArgument3 Argument3 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3) 
        : base(Argument1, Argument2) 
        => this.Argument3 = Argument3;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></param>
    /// <returns>Кортеж из 3 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3)
        (EventArgs<TArgument1, TArgument2, TArgument3> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3);

    /// <summary>Оператор неявного преобразования кортежа из 3 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></summary>
    /// <param name="Args">Кортеж из 3 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 4 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4> 
    : EventArgs<TArgument1, TArgument2, TArgument3>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>4 параметр аргумента</summary>
    public TArgument4 Argument4 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4) 
        : base(Argument1, Argument2, Argument3) 
        => this.Argument4 = Argument4;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></param>
    /// <returns>Кортеж из 4 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4);

    /// <summary>Оператор неявного преобразования кортежа из 4 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></summary>
    /// <param name="Args">Кортеж из 4 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 5 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>5 параметр аргумента</summary>
    public TArgument5 Argument5 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5) 
        : base(Argument1, Argument2, Argument3, Argument4) 
        => this.Argument5 = Argument5;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
        Arg5 = Argument5;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></param>
    /// <returns>Кортеж из 5 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5);

    /// <summary>Оператор неявного преобразования кортежа из 5 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></summary>
    /// <param name="Args">Кортеж из 5 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 6 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
/// <typeparam name="TArgument6">Тип аргумента 6</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>6 параметр аргумента</summary>
    public TArgument6 Argument6 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    /// <param name="Argument6">6 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5, TArgument6 Argument6) 
        : base(Argument1, Argument2, Argument3, Argument4, Argument5) 
        => this.Argument6 = Argument6;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    /// <param name="Arg6">6 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5, out TArgument6 Arg6)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
        Arg5 = Argument5;
        Arg6 = Argument6;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></param>
    /// <returns>Кортеж из 6 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5, Args.Argument6);

    /// <summary>Оператор неявного преобразования кортежа из 6 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></summary>
    /// <param name="Args">Кортеж из 6 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5, Args.Arg6);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 7 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
/// <typeparam name="TArgument6">Тип аргумента 6</typeparam>
/// <typeparam name="TArgument7">Тип аргумента 7</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>7 параметр аргумента</summary>
    public TArgument7 Argument7 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    /// <param name="Argument6">6 параметр аргумента</param>
    /// <param name="Argument7">7 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5, TArgument6 Argument6, TArgument7 Argument7) 
        : base(Argument1, Argument2, Argument3, Argument4, Argument5, Argument6) 
        => this.Argument7 = Argument7;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    /// <param name="Arg6">6 параметр аргумента</param>
    /// <param name="Arg7">7 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5, out TArgument6 Arg6, out TArgument7 Arg7)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
        Arg5 = Argument5;
        Arg6 = Argument6;
        Arg7 = Argument7;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></param>
    /// <returns>Кортеж из 7 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5, Args.Argument6, Args.Argument7);

    /// <summary>Оператор неявного преобразования кортежа из 7 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></summary>
    /// <param name="Args">Кортеж из 7 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5, Args.Arg6, Args.Arg7);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 8 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
/// <typeparam name="TArgument6">Тип аргумента 6</typeparam>
/// <typeparam name="TArgument7">Тип аргумента 7</typeparam>
/// <typeparam name="TArgument8">Тип аргумента 8</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>8 параметр аргумента</summary>
    public TArgument8 Argument8 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    /// <param name="Argument6">6 параметр аргумента</param>
    /// <param name="Argument7">7 параметр аргумента</param>
    /// <param name="Argument8">8 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5, TArgument6 Argument6, TArgument7 Argument7, TArgument8 Argument8) 
        : base(Argument1, Argument2, Argument3, Argument4, Argument5, Argument6, Argument7) 
        => this.Argument8 = Argument8;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    /// <param name="Arg6">6 параметр аргумента</param>
    /// <param name="Arg7">7 параметр аргумента</param>
    /// <param name="Arg8">8 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5, out TArgument6 Arg6, out TArgument7 Arg7, out TArgument8 Arg8)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
        Arg5 = Argument5;
        Arg6 = Argument6;
        Arg7 = Argument7;
        Arg8 = Argument8;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></param>
    /// <returns>Кортеж из 8 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5, Args.Argument6, Args.Argument7, Args.Argument8);

    /// <summary>Оператор неявного преобразования кортежа из 8 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></summary>
    /// <param name="Args">Кортеж из 8 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5, Args.Arg6, Args.Arg7, Args.Arg8);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 9 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
/// <typeparam name="TArgument6">Тип аргумента 6</typeparam>
/// <typeparam name="TArgument7">Тип аргумента 7</typeparam>
/// <typeparam name="TArgument8">Тип аргумента 8</typeparam>
/// <typeparam name="TArgument9">Тип аргумента 9</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>9 параметр аргумента</summary>
    public TArgument9 Argument9 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    /// <param name="Argument6">6 параметр аргумента</param>
    /// <param name="Argument7">7 параметр аргумента</param>
    /// <param name="Argument8">8 параметр аргумента</param>
    /// <param name="Argument9">9 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5, TArgument6 Argument6, TArgument7 Argument7, TArgument8 Argument8, TArgument9 Argument9) 
        : base(Argument1, Argument2, Argument3, Argument4, Argument5, Argument6, Argument7, Argument8) 
        => this.Argument9 = Argument9;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    /// <param name="Arg6">6 параметр аргумента</param>
    /// <param name="Arg7">7 параметр аргумента</param>
    /// <param name="Arg8">8 параметр аргумента</param>
    /// <param name="Arg9">9 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5, out TArgument6 Arg6, out TArgument7 Arg7, out TArgument8 Arg8, out TArgument9 Arg9)
    {
        Arg1 = Argument1;
        Arg2 = Argument2;
        Arg3 = Argument3;
        Arg4 = Argument4;
        Arg5 = Argument5;
        Arg6 = Argument6;
        Arg7 = Argument7;
        Arg8 = Argument8;
        Arg9 = Argument9;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></param>
    /// <returns>Кортеж из 9 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8, TArgument9 Arg9)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5, Args.Argument6, Args.Argument7, Args.Argument8, Args.Argument9);

    /// <summary>Оператор неявного преобразования кортежа из 9 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></summary>
    /// <param name="Args">Кортеж из 9 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8, TArgument9 Arg9) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5, Args.Arg6, Args.Arg7, Args.Arg8, Args.Arg9);

    /* ------------------------------------------------------------------------------------------ */
}
 
/// <summary>Аргумент события с 10 типизированными параметрами</summary>
/// <typeparam name="TArgument1">Тип аргумента 1</typeparam>
/// <typeparam name="TArgument2">Тип аргумента 2</typeparam>
/// <typeparam name="TArgument3">Тип аргумента 3</typeparam>
/// <typeparam name="TArgument4">Тип аргумента 4</typeparam>
/// <typeparam name="TArgument5">Тип аргумента 5</typeparam>
/// <typeparam name="TArgument6">Тип аргумента 6</typeparam>
/// <typeparam name="TArgument7">Тип аргумента 7</typeparam>
/// <typeparam name="TArgument8">Тип аргумента 8</typeparam>
/// <typeparam name="TArgument9">Тип аргумента 9</typeparam>
/// <typeparam name="TArgument10">Тип аргумента 10</typeparam>
[DST]
public class EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10> 
    : EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>10 параметр аргумента</summary>
    public TArgument10 Argument10 { get; set; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></summary>
    public EventArgs() { }

    /// <summary>Инициализация нового экземпляра <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></summary>
    /// <param name="Argument1">1 параметр аргумента</param>
    /// <param name="Argument2">2 параметр аргумента</param>
    /// <param name="Argument3">3 параметр аргумента</param>
    /// <param name="Argument4">4 параметр аргумента</param>
    /// <param name="Argument5">5 параметр аргумента</param>
    /// <param name="Argument6">6 параметр аргумента</param>
    /// <param name="Argument7">7 параметр аргумента</param>
    /// <param name="Argument8">8 параметр аргумента</param>
    /// <param name="Argument9">9 параметр аргумента</param>
    /// <param name="Argument10">10 параметр аргумента</param>
    public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3, TArgument4 Argument4, TArgument5 Argument5, TArgument6 Argument6, TArgument7 Argument7, TArgument8 Argument8, TArgument9 Argument9, TArgument10 Argument10) 
        : base(Argument1, Argument2, Argument3, Argument4, Argument5, Argument6, Argument7, Argument8, Argument9) 
        => this.Argument10 = Argument10;
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Деконструктор <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></summary>
    /// <param name="Arg1">1 параметр аргумента</param>
    /// <param name="Arg2">2 параметр аргумента</param>
    /// <param name="Arg3">3 параметр аргумента</param>
    /// <param name="Arg4">4 параметр аргумента</param>
    /// <param name="Arg5">5 параметр аргумента</param>
    /// <param name="Arg6">6 параметр аргумента</param>
    /// <param name="Arg7">7 параметр аргумента</param>
    /// <param name="Arg8">8 параметр аргумента</param>
    /// <param name="Arg9">9 параметр аргумента</param>
    /// <param name="Arg10">10 параметр аргумента</param>
    public void Deconstruct(out TArgument1 Arg1, out TArgument2 Arg2, out TArgument3 Arg3, out TArgument4 Arg4, out TArgument5 Arg5, out TArgument6 Arg6, out TArgument7 Arg7, out TArgument8 Arg8, out TArgument9 Arg9, out TArgument10 Arg10)
    {
        Arg1  = Argument1;
        Arg2  = Argument2;
        Arg3  = Argument3;
        Arg4  = Argument4;
        Arg5  = Argument5;
        Arg6  = Argument6;
        Arg7  = Argument7;
        Arg8  = Argument8;
        Arg9  = Argument9;
        Arg10 = Argument10;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного преобразования типа <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/> к кортежу</summary>
    /// <param name="Args">Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></param>
    /// <returns>Кортеж из 10 параметров</returns>
    public static implicit operator
        (TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8, TArgument9 Arg9, TArgument10 Arg10)
        (EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10> Args)
        => (Args.Argument1, Args.Argument2, Args.Argument3, Args.Argument4, Args.Argument5, Args.Argument6, Args.Argument7, Args.Argument8, Args.Argument9, Args.Argument10);

    /// <summary>Оператор неявного преобразования кортежа из 10 параметров к типу <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></summary>
    /// <param name="Args">Кортеж из 10 параметров</param>
    /// <returns>Аргумент события <see cref="EventArgs{TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10}"/></returns>
    public static implicit operator EventArgs<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TArgument7, TArgument8, TArgument9, TArgument10>
        ((TArgument1 Arg1, TArgument2 Arg2, TArgument3 Arg3, TArgument4 Arg4, TArgument5 Arg5, TArgument6 Arg6, TArgument7 Arg7, TArgument8 Arg8, TArgument9 Arg9, TArgument10 Arg10) Args)
        => new(Args.Arg1, Args.Arg2, Args.Arg3, Args.Arg4, Args.Arg5, Args.Arg6, Args.Arg7, Args.Arg8, Args.Arg9, Args.Arg10);

    /* ------------------------------------------------------------------------------------------ */
}