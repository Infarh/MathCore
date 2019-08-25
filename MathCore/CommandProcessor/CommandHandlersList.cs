using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.CommandProcessor
{
    using CommandHandler = Action<Command, int, Command[]>;

    /// <summary>Список обработчиков команды</summary>
    public class CommandHandlersList : List<CommandHandler>
    {
        [NotNull] private readonly Dictionary<int, CommandHandler> _Handlers = new Dictionary<int, CommandHandler>();

        /// <summary>Оператор добалвения команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Добавляемый обработчик команды</param>
        /// <returns>Список с добавленным обработчиком команды</returns>
        public static CommandHandlersList operator +([NotNull] CommandHandlersList list, [NotNull] CommandHandler handler)
        {
            list.Add(handler);
            return list;
        }

        public static CommandHandlersList operator +([NotNull] CommandHandlersList list, [NotNull] Action<Command> handler)
        {
            void Handler(Command c, int i, Command[] cc) => handler(c);
            list._Handlers.Add(handler.GetHashCode(), Handler);
            list.Add(Handler);
            return list;
        }

        public static CommandHandlersList operator +([NotNull] CommandHandlersList list, [NotNull] Action<Command, int> handler)
        {
            void Handler(Command c, int i, Command[] cc) => handler(c, i);
            list._Handlers.Add(handler.GetHashCode(), Handler);
            list.Add(Handler);
            return list;
        }

        public static CommandHandlersList operator +([NotNull] CommandHandlersList list, [NotNull] Action handler)
        {
            void Handler(Command c, int i, Command[] cc) => handler();
            list._Handlers.Add(handler.GetHashCode(), Handler);
            list.Add(Handler);
            return list;
        }

        /// <summary>Оператор удаления команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Удаляемый обработчик команды</param>
        /// <returns>Список с удалённым обработчиком команды</returns>
        public static CommandHandlersList operator -([NotNull] CommandHandlersList list, [NotNull] CommandHandler handler)
        {
            list.Remove(handler);
            return list;
        }

        public static CommandHandlersList operator -([NotNull] CommandHandlersList list, [NotNull] Action<Command> Handler)
        {
            if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
                list.Remove(handler);
            return list;
        }

        public static CommandHandlersList operator -([NotNull] CommandHandlersList list, [NotNull] Action<Command, int> Handler)
        {
            if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
                list.Remove(handler);
            return list;
        }

        public static CommandHandlersList operator -([NotNull] CommandHandlersList list, [NotNull] Action Handler)
        {
            if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
                list.Remove(handler);
            return list;
        }

        [NotNull] private readonly Dictionary<string, CommandArgHandlersList> _CommandHandlers = new Dictionary<string, CommandArgHandlersList>();

        public CommandArgHandlersList this[string ArgumentName]
        {
            get
            {
                if(!_CommandHandlers.TryGetValue(ArgumentName, out var result))
                    _CommandHandlers.Add(ArgumentName, result = new CommandArgHandlersList());
                return result;
            }
            set
            {
                var list = this[ArgumentName];
                if(ReferenceEquals(list, value)) return;
                list.AddRange(value);
                //if(IsRegistredCommand(ArgumentName))
                //{
                //    if(value != null)
                //        _CommandHandlers[ArgumentName] = value;
                //    else
                //        ClaerCommandHandlers(ArgumentName);
                //}
                //else if(value != null)
                //    _CommandHandlers.Add(ArgumentName, value);
            }
        }

        //private bool ClaerCommandHandlers(string ArgumentName) => _CommandHandlers.Remove(ArgumentName);

        public bool IsRegistredCommand(string ArgumentName) => Count != 0 || _CommandHandlers.ContainsKey(ArgumentName);

        public override string ToString() => $"Handlers list count = {Count}";
    }

    public class CommandArgHandlersList : List<Action<Command, int, Command[], Argument>>
    {
        /// <summary>Оператор добалвения команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Добавляемый обработчик команды</param>
        /// <returns>Список с добавленным обработчиком команды</returns>
        public static CommandArgHandlersList operator +([NotNull] CommandArgHandlersList list, [NotNull] Action<Command, int, Command[], Argument> handler)
        {
            list.Add(handler);
            return list;
        }

        /// <summary>Оператор удаления команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Удаляемый обработчик команды</param>
        /// <returns>Список с удалённым обработчиком команды</returns>
        public static CommandArgHandlersList operator -([NotNull] CommandArgHandlersList list, [NotNull] Action<Command, int, Command[], Argument> handler)
        {
            list.Remove(handler);
            return list;
        }

        public override string ToString() => $"Args handlers list count = {Count}";
    }
}