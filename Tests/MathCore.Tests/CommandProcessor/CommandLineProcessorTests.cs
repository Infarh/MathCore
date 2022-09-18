using System.Diagnostics;
using System.IO;
using System.Linq.Reactive;
using MathCore.Annotations;
using MathCore.CommandProcessor;

using Moq;
// ReSharper disable ArgumentsStyleLiteral

namespace MathCore.Tests.CommandProcessor;

[TestClass]
public class CommandLineProcessorTests
{
    const string __HelpCommandName = "help";
    const string __StartCommandName = "start";
    const string __ActionCommandName = "action";
    const string __UnknownCommandName = "UnknownCommand";
    const string __ExitCommandName = "exit";
    const string __ClearString = "!!Clear!!";

    const string __SetCommandName = "set";
    const string __SetCommandArgumentPromptName = "prompt";
    const string __SetCommandArgumentWorkName = "work";

    private readonly List<(string, TimeSpan)> _Values = new();

    private readonly Stopwatch _Timer = new();

    private readonly TextWriter _Logger;
    private readonly TextReader _ConsoleInput;

    public CommandLineProcessorTests()
    {
        var console_out_mock = new Mock<TextWriter>();
        console_out_mock
           .Setup(w => w.Write(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
           .Callback((char[] Buffer, int _, int _) => _Values.Add((new string(Buffer), _Timer.Elapsed)));
        _Logger = console_out_mock.Object;

        var command_index = 0;
        string[] commands =
        {
            __HelpCommandName,
            __StartCommandName,
            __ActionCommandName,
            __UnknownCommandName,
            __ExitCommandName,
        };

        var console_in_mock = new Mock<TextReader>();
        console_in_mock.Setup(r => r.ReadLine())
           .Returns(() => command_index >= commands.Length ? null : commands[command_index++]);
        _ConsoleInput = console_in_mock.Object;
    }

    [TestInitialize]
    public void TestInitialize() => _Timer.Start();

    [TestCleanup]
    public void TestFinalize() => _Timer.Stop();

    /// <summary>Флаг обработки запросов пользователя</summary>
    private bool Work { get; set; }

    /// <summary>Приглашение командной строки</summary>
    private string Prompt { get; set; }

    [TestMethod]
    public void Test()
    {
        Prompt = "> ";
        Work   = true;

        var processor = new CommandLineProcessor();

        const string start_string = "Started. Wait 5c...";

        var start_events = processor.Commands
           .Where(c => c.Command.Name == __StartCommandName)
           .ForeachAction(c => c.SetHandled())
           .ForeachAction(_ => _Logger.WriteLine(start_string));

        const string stop_string = "Stopped!";
        var stop_events = start_events
           .WhitAsync(TimeSpan.FromSeconds(5))
           .ForeachAction(_ => _Logger.WriteLine(stop_string));

        // ReSharper disable once UnusedVariable
        var action = processor.Commands
           .Where(c => c.Command.Name == __ActionCommandName)
           .Take(start_events, stop_events, InitialState: false)
           .ForeachAction(c => c.SetHandled())
           .ForeachAction(c => _Logger.WriteLine(c.Command.Argument.ToSeparatedStr(", ")));

        processor[__ExitCommandName] += (_, _, _) => Work = false;
        processor[__HelpCommandName] += (_, _, _) => processor.GetRegisteredCommands().Foreach(_Logger.WriteLine);

        processor.CommandProcess   += ExecuteCommand;
        processor.UnhandledCommand += UnknownCommandInformator;

        while (Work)
        {
            _Logger.Write(Prompt);
            processor.Process(_ConsoleInput.ReadLine());
        }
    }

    /// <summary>Обработчик необработанных команд</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="e">Аргумент, содержащий информацию о необработанной команде</param>
    private void UnknownCommandInformator(object Sender, [NotNull] CommandEventArgs e) =>
        _Logger.WriteLine(e.Command.ToFormattedString("Unknown command \"{0}\""));

    /// <summary>Обработчик команды</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргумент, содержащий информацию о команде</param>
    private void ExecuteCommand(object sender, [NotNull] CommandEventArgs e)
    {
        var processor = (CommandLineProcessor)sender;
        switch (e.Command.Name.ToLower())
        {
            case "clear":
                _Logger.WriteLine(__ClearString);
                e.Handled = true;
                break;
            case __SetCommandName:
                e.Command.Argument.Foreach(SetArgument);
                e.Handled = true;
                break;
            case __HelpCommandName:
                _Logger.Write(processor.GetRegisteredCommands().ToSeparatedStr("\r\n"));
                break;
        }
    }

    /// <summary>Метод установки значения команды Set</summary>
    /// <param name="Argument">Аргумент команды Set</param>
    private void SetArgument(Argument Argument)
    {
        switch (Argument.Name.ToLower())
        {
            case __SetCommandArgumentPromptName:
                Prompt = Argument.Value;
                break;
            case __SetCommandArgumentWorkName when Argument.TryGetValueAs<bool>(out var work):
                Work = work;
                break;
            case __SetCommandArgumentWorkName:
                _Logger.Write(Argument.Value.ToFormattedString("Неверный формат аргумента команды set"));
                break;
        }
    }
}