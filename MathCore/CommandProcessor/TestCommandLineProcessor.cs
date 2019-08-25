using System;
using System.Linq;
using System.Linq.Reactive;
using System.Threading;

namespace MathCore.CommandProcessor
{
    /// <summary>Пример использования класса команданого процессора</summary>
    public static class TestCommandLineProcessor
    {
        /// <summary>Флаг обработки запросов пользователя</summary>
        public static bool Work { get; set; }

        /// <summary>Приглашение командной строки</summary>
        public static string Prompt { get; set; }

        /// <summary>Точка входа в пример</summary>
        public static void Test()
        {
            Prompt = "> ";
            Work = true;
            var processor = new CommandLineProcessor();

            var start = processor
                        .Where(c => c.Command.Name == "start")
                        .ForeachAction(c => c.SetHandled())
                        .ForeachAction(c => Console.Title = "Whait 5c...")
                        .ForeachAction(c => Console.WriteLine("Started..."));

            var stop = start.WhaitAsync(TimeSpan.FromSeconds(5))
                        .ForeachAction(c => Console.Title = "Stoped.")
                        .ForeachAction(c => Console.WriteLine("Stoped!"));


            var action = processor
                        .Where(c => c.Command.Name == "action")
                        .Take(start, stop, false)
                        .ForeachAction(c => c.SetHandled())
                        .ForeachAction(c => c.Command.Argument.ToSeparatedStr(", ").ToConsoleLN());

            var t = processor.FromEvent<CommandEventArgs>("CommandProcess");

            processor["exit"] += (c, i, cc) => Work = false;
            processor["help"] += (c, i, cc) => processor.GetRegistredCommands().Foreach(Console.WriteLine);

            processor.CommandProcess += ExecuteCommand;
            processor.UnhandledCommand += UnknownCommandInformator;

            while(Work)
            {
                Console.Write(Prompt);
                processor.Process(Console.ReadLine());
            }

            Console.WriteLine("Programm complited.");
            Thread.Sleep(1500);
        }

        public static void Test1()
        {
            var commands = CommandLineProcessor.ParseConsole().ToArray();
            Console.WriteLine("Programm complited.");
            Thread.Sleep(1500);
        }

        /// <summary>Обработчик необработанных команд</summary>
        /// <param name="Sender">Источник события</param>
        /// <param name="e">Аргумент, содержащий информацию о необработанной команде</param>
        private static void UnknownCommandInformator(object Sender, CommandEventArgs e) => Console.WriteLine(e.Command.ToFormattedString("Unknown command \"{0}\""));

        /// <summary>Обработчик команды</summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргумент, содержащий информацию о команде</param>
        private static void ExecuteCommand(object sender, CommandEventArgs e)
        {
            var processor = (CommandLineProcessor)sender;
            switch(e.Command.Name.ToLower())
            {
                case "clear":
                    Console.Clear();
                    e.Handled = true;
                    break;
                case "set":
                    e.Command.Argument.Foreach(SetArgument);
                    e.Handled = true;
                    break;
                case "help":
                    processor.GetRegistredCommands().ToSeparatedStr("\r\n").ToConsole();
                    break;
            }
        }

        /// <summary>Метод установки значения команды Set</summary>
        /// <param name="SetArg">Аргумент команды Set</param>
        private static void SetArgument(Argument SetArg)
        {
            switch(SetArg.Name.ToLower())
            {
                case "prompt":
                    Prompt = SetArg.Values == null || SetArg.Values.Length == 0 ? "" : SetArg.Values[0];
                    break;
                case "work":
                    if(bool.TryParse(SetArg.Value, out var work))
                        Work = work;
                    else
                        SetArg.Value.ToFormattedString("Unknown set value \"{0}\" of command \"Set Work=\"").ToConsole();
                    break;
            }
        }
    }
}