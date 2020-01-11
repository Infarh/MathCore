using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Reactive;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MathCore.CommandProcessor
{
    using CommandHandler = Action<Command, int, Command[]>;

    /// <summary>Командный процессор</summary>
    public class CommandLineProcessor : IObservable<CommandEventArgs>
    {
        public static IEnumerable<Command> ParseConsole(string Prompt = ">", [CanBeNull] Action<CommandLineProcessor> Initializer = null)
        {
            var work = true;
            var processor = new CommandLineProcessor();
            processor["exit"] += (command, index, commands) => work = false;
            processor["help"] += (command, index, commands) => processor.GetRegisteredCommands().Foreach(Console.WriteLine);
            var set = processor["set"];
            set["prompt"] += (command, index, commands, arg) => Prompt = arg.Values is null || arg.Values.Length == 0 ? string.Empty : arg.Values[0];
            set["work"] += (command, index, commands, arg) => {
                if(bool.TryParse(arg.Value, out var can_work)) work = can_work; };

            Initializer?.Invoke(processor);
            while(work)
            {
                Console.Write(Prompt);
                foreach(var command in processor.Process(Console.ReadLine()))
                    yield return command;
            }
        }

        /// <summary>Событие обработки команды</summary>
        public event EventHandler<CommandEventArgs> CommandProcess;

        /// <summary>Обработка команды</summary>
        /// <param name="Arg">Аргумент, содержащий сведения о команде</param>
        protected virtual void OnCommandProcess([NotNull] CommandEventArgs Arg)
        {
            if(IsRegisteredCommand(Arg.Command.Name))
            {
                this[Arg.Command.Name].Foreach(Arg, (action, i, arg) => action.Invoke(arg.Command, arg.Index, arg.Commands));
                Arg.Handled = true;
            }

            var handler = CommandProcess;
            if(handler is null) return;
            var invocations = handler.GetInvocationList();
            for(var i = 0; !Arg.Handled && i < invocations.Length; i++)
            {
                var I = invocations[i];
                if(I.Target is ISynchronizeInvoke invoke && invoke.InvokeRequired)
                    invoke.Invoke(I, new object[] { this, Arg });
                else
                    I.DynamicInvoke(this, Arg);
            }
            _ObservableObject.OnNext(Arg);
        }

        /// <summary>Обработка команды</summary>
        /// <param name="command">Обрабатываемая команда</param>
        /// <param name="index">Индекс команды в массиве команд сессии</param>
        /// <param name="commands">Массив команд сессии</param>
        private void OnCommandProcess(Command command, int index, Command[] commands)
        {
            var Arg = new CommandEventArgs { Command = command, Index = index, Commands = commands };
            OnCommandProcess(Arg);
            if(!Arg.Handled) OnUnhandledCommand(Arg);
        }

        /// <summary>Событие появления необработанной команды</summary>
        public event EventHandler<CommandEventArgs> UnhandledCommand;

        /// <summary>Генерация события обнаружения необработанной команды</summary>
        /// <param name="Arg">Аргумент события, содержащий сведения о команде</param>
        protected virtual void OnUnhandledCommand(CommandEventArgs Arg)
        {
            var handlers = UnhandledCommand;
            if(handlers is null) return;
            var invocations = handlers.GetInvocationList();
            for(var i = 0; i < invocations.Length; i++)
            {
                var I = invocations[i];
                if(I.Target is ISynchronizeInvoke invoke && invoke.InvokeRequired)
                    invoke.Invoke(I, new object[] { this, Arg });
                else
                    I.DynamicInvoke(this, Arg);
            }
        }

        /// <summary>Словарь списков обработчиков команд</summary>
        private readonly Dictionary<string, CommandHandlersList> _CommandHandlers =
                    new Dictionary<string, CommandHandlersList>();

        private readonly SimpleObservableEx<CommandEventArgs> _ObservableObject = new SimpleObservableEx<CommandEventArgs>();

        /// <summary>Разделитель команд в строке</summary>
        public char CommandSplitter { get; set; }
        /// <summary>Разделитель имени команды и её параметра</summary>
        public char CommandParameterSplitter { get; set; }
        /// <summary>Разделитель аргументов команды</summary>
        public char ArgSplitter { get; set; }
        /// <summary>Разделитель имени аргумента и его значения</summary>
        public char ValueSplitter { get; set; }

        /// <summary>Доступ к списку обработчиков команды по её имени</summary>
        /// <param name="CommandName">Имя команды</param>
        /// <returns>Список обработчиков команды</returns>
        public CommandHandlersList this[[NotNull] string CommandName]
        {
            get
            {
                if(!_CommandHandlers.TryGetValue(CommandName, out var result))
                    _CommandHandlers.Add(CommandName, result = new CommandHandlersList());
                return result;
            }
            set
            {
                if(IsRegisteredCommand(CommandName))
                {
                    if(value != null)
                        _CommandHandlers[CommandName] = value;
                    else
                        ClearCommandHandlers(CommandName);
                }
                else if(value != null)
                    _CommandHandlers.Add(CommandName, value);
            }
        }

        /// <summary>Командный процессор</summary>
        /// <param name="CommandSplitter">Разделитель команд в строке</param>
        /// <param name="CommandParameterSplitter">Разделитель имени команды и её параметра</param>
        /// <param name="ArgSplitter">Разделитель аргументов</param>
        /// <param name="ValueSplitter">Разделитель имени аргумента и его значения</param>
        public CommandLineProcessor(char CommandSplitter = ';', char CommandParameterSplitter = ':',
            char ArgSplitter = ' ', char ValueSplitter = '=')
        {
            this.CommandSplitter = CommandSplitter;
            this.CommandParameterSplitter = CommandParameterSplitter;
            this.ArgSplitter = ArgSplitter;
            this.ValueSplitter = ValueSplitter;
        }

        /// <summary>Обработать команду</summary>
        /// <param name="CommandLine">Командная строка</param>
        [NotNull]
        public IEnumerable<Command> Process([NotNull] params string[] CommandLine)
        {
            var commands = CommandLine.SelectMany(str => str.Split(CommandSplitter))
                .Select(s => s.ClearSystemSymbolsAtBeginAndEnd())
                .Where(s => s.Length > 0)
                .Select(CommandStr => new Command(CommandStr, CommandParameterSplitter, ArgSplitter, ValueSplitter))
                .ToArray();

            // ReSharper disable once IdentifierTypo
            commands.Foreach(this, commands, (command, i, processor, cmds) => processor.OnCommandProcess(command, i, cmds));
            return commands;
        }

        /// <summary>Добавить обработчик команды</summary>
        /// <param name="CommandName">Имя команды</param>
        /// <param name="CommandHandler">Добавляемый обработчик команды</param>
        public void AddCommandHandler([NotNull] string CommandName, CommandHandler CommandHandler) => this[CommandName].Add(CommandHandler);

        /// <summary>Удалить обработчик команды</summary>
        /// <param name="CommandName">Имя команды</param>
        /// <param name="CommandHandler">Удаляемый обработчик команды</param>
        /// <returns>Истина, если удалось обработчик команды удалить</returns>
        public bool RemoveCommandHandler([NotNull] string CommandName, CommandHandler CommandHandler) => this[CommandName].Remove(CommandHandler);

        /// <summary>Очистить список обработчиков команды</summary>
        /// <param name="CommandName">Имя команды</param>
        public void ClearCommandHandlers([NotNull] string CommandName) => this[CommandName].Clear();

        /// <summary>Очистить все обработчики команд</summary>
        public void ClearCommandHandlers() => _CommandHandlers.Clear();

        /// <summary>Получить перечисление имён команд с зарегистрированными обработчиками</summary>
        /// <returns>Перечисление имён команд, имеющих свои обработчики</returns>
        [NotNull]
        public IEnumerable<string> GetRegisteredCommands() => _CommandHandlers.Keys.ToArray();

        /// <summary>Проверка - имеет ли команда обработчики</summary>
        /// <param name="CommandName">Проверяемая команда</param>
        /// <returns>Истина, если указаны обработчики команды</returns>
        public bool IsRegisteredCommand([NotNull] string CommandName) => 
            _CommandHandlers.ContainsKey(CommandName) && this[CommandName].IsRegistredCommand(CommandName);

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<CommandEventArgs> observer) => _ObservableObject.Subscribe(observer);
    }
}