using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.CommandProcessor
{
    /// <summary>Список обработчиков команды</summary>
    public class CommandArgHandlersList : List<Action<Command, int, Command[], Argument>>
    {
        /// <summary>Оператор добавления команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Добавляемый обработчик команды</param>
        /// <returns>Список с добавленным обработчиком команды</returns>
        [NotNull]
        public static CommandArgHandlersList operator +([NotNull] CommandArgHandlersList list, [NotNull] Action<Command, int, Command[], Argument> handler)
        {
            list.Add(handler);
            return list;
        }

        /// <summary>Оператор удаления команды к списку</summary>
        /// <param name="list">Список обработчиков команды</param>
        /// <param name="handler">Удаляемый обработчик команды</param>
        /// <returns>Список с удалённым обработчиком команды</returns>
        [NotNull]
        public static CommandArgHandlersList operator -([NotNull] CommandArgHandlersList list, [NotNull] Action<Command, int, Command[], Argument> handler)
        {
            list.Remove(handler);
            return list;
        }

        /// <inheritdoc />
        [NotNull] public override string ToString() => $"Args handlers list count = {Count}";
    }
}