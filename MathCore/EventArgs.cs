using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Diagnostics.Contracts;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Аргумент события с типизированным параметром</summary>
    /// <typeparam name="TArgument">Тип параметра аргумента</typeparam>
    [DST]
    public class EventArgs<TArgument> : EventArgs
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Параметр аргумента</summary>
        public TArgument Argument { get; set; }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Новый аргумент события с типизированным параметром</summary>
        /// <param name="Argument">Параметр аргумента</param>
        public EventArgs(TArgument Argument)
        {
            Contract.Requires(Argument != null);
            this.Argument = Argument;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>
        /// Возвращает объект <see cref="T:System.String"/>, который представляет текущий объект <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>Объект <see cref="T:System.String"/>, представляющий текущий объект <see cref="T:System.Object"/>.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => Argument.ToString();

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Оператор неявного преобразования аргумента события к типу содержащегося в нём значения </summary>
        /// <param name="Args">Аргумент события</param>
        /// <returns>Хранимый объект</returns>
        public static implicit operator TArgument(EventArgs<TArgument> Args) => Args.Argument;

        /// <summary>
        /// Оgератор неявного преобразования типа зранимого значения в обёртку из аргумента события, содержащего это значение
        /// </summary>
        /// <param name="Argument">Объект аргумента события</param>
        /// <returns>Аргумент события</returns>
        public static implicit operator EventArgs<TArgument>(TArgument Argument) => new EventArgs<TArgument>(Argument);

        /* ------------------------------------------------------------------------------------------ */
    }

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    [DST]
    public class EventArgs<TArgument1, TArgument2> : EventArgs
    {
        /// <summary>Первый аргумент</summary>
        public TArgument1 Argument1 { get; set; }

        /// <summary>Второй аргумент</summary>
        public TArgument2 Argument2 { get; set; }

        /// <summary>Новый аргумент события с двумя параметрами</summary>
        protected EventArgs() { }

        /// <summary>Новый аргумент события с двумя параметрами</summary>
        /// <param name="Argument1">Первый аргумент события</param>
        /// <param name="Argument2">Второй аргумент события</param>
        public EventArgs(TArgument1 Argument1, TArgument2 Argument2)
        {
            this.Argument1 = Argument1;
            this.Argument2 = Argument2;
        }

        /// <summary>Оператор неявного преобразования аргумента события к типу содержащегося в нём значения </summary>
        /// <param name="Args">Аргумент события</param>
        /// <returns>Хранимый объект</returns>
        public static implicit operator TArgument1(EventArgs<TArgument1, TArgument2> Args) => Args.Argument1;

        /// <summary>Оператор неявного преобразования аргумента события к типу содержащегося в нём значения </summary>
        /// <param name="Args">Аргумент события</param>
        /// <returns>Хранимый объект</returns>
        public static implicit operator TArgument2(EventArgs<TArgument1, TArgument2> Args) => Args.Argument2;
    }

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    /// <typeparam name="TArgument3">Тип третьего параметра</typeparam>
    [DST]
    public class EventArgs<TArgument1, TArgument2, TArgument3> : EventArgs
    {
        /// <summary>Первый аргумент</summary>
        public TArgument1 Argument1 { get; set; }

        /// <summary>Второй аргумент</summary>
        public TArgument2 Argument2 { get; set; }

        /// <summary>Третий аргумент</summary>
        public TArgument3 Argument3 { get; set; }

        /// <summary>Новый аргумент события с тремя параметрами</summary>
        public EventArgs() { }

        /// <summary>Новый аргумент события с тремя параметрами</summary>
        /// <param name="Argument1">Первый аргумент события</param>
        /// <param name="Argument2">Второй аргумент события</param>
        /// <param name="Argument3">Третий аргумент события</param>
        public EventArgs(TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3)
        {
            this.Argument1 = Argument1;
            this.Argument2 = Argument2;
            this.Argument3 = Argument3;
        }
    }

    /// <summary>Аргумент события с типизированным параметром</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument">Тип параметра аргумента</typeparam>
    [DST]
    public class EventSenderArgs<TSender, TArgument> : EventArgs<TArgument>
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Источник события</summary>
        public TSender Sender { get; set; }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Новый аргумент события с типизированным параметром</summary>
        /// <param name="Sender">Источник события</param>
        /// <param name="Argument">Параметр аргумента</param>
        public EventSenderArgs(TSender Sender, TArgument Argument) : base(Argument) => this.Sender = Sender;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>
        /// Возвращает объект <see cref="T:System.String"/>, который представляет текущий объект <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>Объект <see cref="T:System.String"/>, представляющий текущий объект <see cref="T:System.Object"/>.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"{Sender}->{Argument}";

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Оператор неявного преобразования аргумента события к типу содержащегося в нём значения </summary>
        /// <param name="Args">Аргумент события</param>
        /// <returns>Хранимый объект</returns>
        public static implicit operator TArgument(EventSenderArgs<TSender, TArgument> Args) => Args.Argument;

        /* ------------------------------------------------------------------------------------------ */
    }

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    [DST]
    public class EventSenderArgs<TSender, TArgument1, TArgument2> : EventArgs<TArgument1, TArgument2>
    {
        /// <summary>Источник события</summary>
        public TSender Sender { get; set; }

        /// <summary>Новый аргумент события с двумя параметрами</summary>
        protected EventSenderArgs() { }

        /// <summary>Новый аргумент события с двумя параметрами</summary>
        /// <param name="Sender">Источник события</param>
        /// <param name="Argument1">Первый аргумент события</param>
        /// <param name="Argument2">Второй аргумент события</param>
        public EventSenderArgs(TSender Sender, TArgument1 Argument1, TArgument2 Argument2) :base(Argument1, Argument2) => this.Sender = Sender;
    }

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    /// <typeparam name="TArgument3">Тип третьего параметра</typeparam>
    [DST]
    public class EventSenderArgs<TSender, TArgument1, TArgument2, TArgument3> : EventArgs<TArgument1, TArgument2, TArgument3>
    {
        /// <summary>Источник события</summary>
        public TSender Sender { get; set; }

        /// <summary>Новый аргумент события с тремя параметрами</summary>
        public EventSenderArgs() { }

        /// <summary>Новый аргумент события с тремя параметрами</summary>
        /// <param name="Sender">Источник события</param>
        /// <param name="Argument1">Первый аргумент события</param>
        /// <param name="Argument2">Второй аргумент события</param>
        /// <param name="Argument3">Третий аргумент события</param>
        public EventSenderArgs(TSender Sender, TArgument1 Argument1, TArgument2 Argument2, TArgument3 Argument3)
            : base(Argument1, Argument2, Argument3) => this.Sender = Sender;
    }
}