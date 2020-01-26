using System.Diagnostics;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Аргумент события с типизированным параметром</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument">Тип параметра аргумента</typeparam>
    [DebuggerStepThrough]
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
        public static implicit operator TArgument([NotNull] EventSenderArgs<TSender, TArgument> Args) => Args.Argument;

        /* ------------------------------------------------------------------------------------------ */
    }

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    /// <typeparam name="TArgument3">Тип третьего параметра</typeparam>
    [DebuggerStepThrough]
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

    /// <summary>Аргумент события с двумя типизированными параметрами</summary>
    /// <typeparam name="TSender">Тип источника события</typeparam>
    /// <typeparam name="TArgument1">Тип первого параметра</typeparam>
    /// <typeparam name="TArgument2">Тип второго параметра</typeparam>
    [DebuggerStepThrough]
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
        public EventSenderArgs(TSender Sender, TArgument1 Argument1, TArgument2 Argument2) : base(Argument1, Argument2) => this.Sender = Sender;
    }
}