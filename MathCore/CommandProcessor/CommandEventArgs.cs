using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.CommandProcessor;

/// <summary>Аргумент события обработки команды</summary>
public class CommandEventArgs : EventArgs
{
    /// <summary>Обрабатываемая команда</summary>
    public ProcessorCommand Command { get; }

    /// <summary>Перечень команд сессии</summary>
    public IReadOnlyList<ProcessorCommand> Commands { get; }

    /// <summary>Индекс команды в перечне команд сессии</summary>
    public int Index { get; }

    /// <summary>Признак того, что команда обработана</summary>
    public bool Handled { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="CommandEventArgs"/></summary>
    /// <param name="Command">Обрабатываемая команда</param>
    /// <param name="Index">Индекс команды в перечне команд сессии</param>
    /// <param name="Commands">Перечень команд сессии</param>
    public CommandEventArgs(ProcessorCommand Command, int Index, IReadOnlyList<ProcessorCommand> Commands)
    {
        this.Command  = Command;
        this.Index    = Index;
        this.Commands = Commands;
    }

    /// <summary>Строковое представление</summary>
    /// <returns>Строковое представление</returns>
    [NotNull]
    public override string ToString() => 
        $"Command({Index + 1}/{Commands.Count}):> {Command}{(Handled ? "- processed" : string.Empty)}";

    /// <summary>Установка признака того, что команда была обработана</summary>
    public void SetHandled() => Handled = true;
}