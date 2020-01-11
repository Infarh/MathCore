using System;
using MathCore.Annotations;

namespace MathCore.CommandProcessor
{
    /// <summary>Аргумент события обработки команды</summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>Обрабатываемая команда</summary>
        public Command Command { get; set; }
        /// <summary>Перечень команд сессии</summary>
        public Command[] Commands { get; set; }
        /// <summary>Индекс команды в перечне команд сессии</summary>
        public int Index { get; set; }
        /// <summary>Признак того, что команда обработана</summary>
        public bool Handled { get; set; }

        /// <summary>Строковое представление</summary>
        /// <returns>Строковое представление</returns>
        [NotNull]
        public override string ToString() => string.Format("Command({0}/{3}):> {1}{2}", Index + 1, Command, Handled ? "- processed" : string.Empty, Commands.Length);

        public void SetHandled() => Handled = true;
    }
}