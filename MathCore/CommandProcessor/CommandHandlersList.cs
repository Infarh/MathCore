#nullable enable
namespace MathCore.CommandProcessor;

using CommandHandler = Action<ProcessorCommand, int, IReadOnlyList<ProcessorCommand>>;

/// <summary>Список обработчиков команды</summary>
public class CommandHandlersList : List<CommandHandler>
{
    private readonly Dictionary<int, CommandHandler> _Handlers = [];

    /// <summary>Оператор добавления команды к списку</summary>
    /// <param name="list">Список обработчиков команды</param>
    /// <param name="Handler">Добавляемый обработчик команды</param>
    /// <returns>Список с добавленным обработчиком команды</returns>
    public static CommandHandlersList operator +(CommandHandlersList list, CommandHandler Handler)
    {
        list.Add(Handler);
        return list;
    }

    /// <summary>Оператор добавления обработчика команды в список обработчиков</summary>
    /// <param name="list">Список действий обработчиков команды</param>
    /// <param name="Handler">Добавляемый в список обработчик</param>
    /// <returns>Исходный список с добавленным в него новым обработчиком команды</returns>
    public static CommandHandlersList operator +(CommandHandlersList list, Action<ProcessorCommand> Handler)
    {
        void CommandHandler(ProcessorCommand c, int i, IReadOnlyList<ProcessorCommand> cc) => Handler(c);
        list._Handlers.Add(Handler.GetHashCode(), CommandHandler);
        list.Add(CommandHandler);
        return list;
    }

    /// <summary>Оператор добавления позиционного обработчика команды в список обработчиков</summary>
    /// <param name="list">Список действий обработчиков команды</param>
    /// <param name="Handler">Добавляемый в список обработчик, поддерживающий параметр индекса</param>
    /// <returns>Исходный список с добавленным в него новым обработчиком команды</returns>
    public static CommandHandlersList operator +(CommandHandlersList list, Action<ProcessorCommand, int> Handler)
    {
        void CommandHandler(ProcessorCommand c, int i, IReadOnlyList<ProcessorCommand> cc) => Handler(c, i);
        list._Handlers.Add(Handler.GetHashCode(), CommandHandler);
        list.Add(CommandHandler);
        return list;
    }

    /// <summary>Оператор добавления безпараметрического обработчика команды в список обработчиков</summary>
    /// <param name="list">Список действий обработчиков команды</param>
    /// <param name="Handler">Добавляемый в список обработчик без параметров</param>
    /// <returns>Исходный список с добавленным в него новым обработчиком команды</returns>
    public static CommandHandlersList operator +(CommandHandlersList list, Action Handler)
    {
        list._Handlers.Add(Handler.GetHashCode(), CommandHandler);
        list.Add(CommandHandler);
        return list;
        void CommandHandler(ProcessorCommand c, int i, IReadOnlyList<ProcessorCommand> cc) => Handler();
    }

    /// <summary>Оператор удаления команды из списка обработчиков</summary>
    /// <param name="list">Список обработчиков команды</param>
    /// <param name="Handler">Удаляемый обработчик команды</param>
    /// <returns>Список с удалённым обработчиком команды</returns>
    public static CommandHandlersList operator -(CommandHandlersList list, CommandHandler Handler)
    {
        list.Remove(Handler);
        return list;
    }

    /// <summary>Оператор удаления обработчика команды из списка обработчиков</summary>
    /// <param name="list">Список обработчиков команды</param>
    /// <param name="Handler">Удаляемый обработчик команды</param>
    /// <returns>Список с удалённым обработчиком команды</returns>
    public static CommandHandlersList operator -(CommandHandlersList list, Action<ProcessorCommand> Handler)
    {
        if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
            list.Remove(handler);
        return list;
    }

    /// <summary>Оператор удаления обработчика команды с индексатором из списка обработчиков</summary>
    /// <param name="list">Список обработчиков команды</param>
    /// <param name="Handler">Удаляемый обработчик команды с индексатором</param>
    /// <returns>Список с удалённым обработчиком команды</returns>
    public static CommandHandlersList operator -(CommandHandlersList list, Action<ProcessorCommand, int> Handler)
    {
        if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
            list.Remove(handler);
        return list;
    }

    /// <summary>Оператор удаления обработчика команды из списка обработчиков</summary>
    /// <param name="list">Список обработчиков команды</param>
    /// <param name="Handler">Удаляемый обработчик команды</param>
    /// <returns>Список с удалённым обработчиком команды</returns>
    public static CommandHandlersList operator -(CommandHandlersList list, Action Handler)
    {
        if(list._Handlers.TryGetValue(Handler.GetHashCode(), out var handler))
            list.Remove(handler);
        return list;
    }

    /// <summary>Словарь обработчиков команд (ключ - имя команды, значение - список обработчиков)</summary>
    private readonly Dictionary<string, CommandArgHandlersList> _CommandHandlers = [];

    /// <summary>Обращение к списку обработчиков команды по её имени (индексатор)</summary>
    /// <param name="CommandName">Имя команды</param>
    public CommandArgHandlersList this[string CommandName]
    {
        get
        {
            if(!_CommandHandlers.TryGetValue(CommandName, out var result))
                _CommandHandlers.Add(CommandName, result = []);
            return result;
        }
        set
        {
            var list = this[CommandName];
            if(ReferenceEquals(list, value)) return;
            list.AddRange(value);
        }
    }

    //private bool ClearCommandHandlers(string ArgumentName) => _CommandHandlers.Remove(ArgumentName);

    /// <summary>Зарегистрирована ли команда в процессоре?</summary>
    /// <param name="CommandName">Имя команды</param>
    /// <returns>Истина, если в процессоре существует обработчик команды с указанным именем</returns>
    public bool IsRegisteredCommand(string CommandName) => Count != 0 || _CommandHandlers.ContainsKey(CommandName);

    /// <inheritdoc />
    public override string ToString() => $"Handlers list count = {Count}";
}