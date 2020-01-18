using System;
using System.Linq;
using MathCore.Annotations;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.CommandProcessor
{
    /// <summary>Команда</summary>
    public struct Command
    {
        /// <summary>Имя команды</summary>
        public string Name { get; set; }

        /// <summary>Параметр команды</summary>
        public string Parameter { get; set; }

        /// <summary>Массив аргументов команды</summary>
        public Argument[] Argument { get; set; }

        /// <summary>Команда</summary>
        /// <param name="CommandStr">Строковое представление команды</param>
        /// <param name="ParameterSplitter">Разделитель имени и параметра команды</param>
        /// <param name="ArgSplitter">Разделитель аргументов команды</param>
        /// <param name="ValueSplitter">Разделитель имени аргумента и его значения</param>
        public Command([NotNull] string CommandStr, char ParameterSplitter = ':', char ArgSplitter = ' ', char ValueSplitter = '=')
        {
            var items = CommandStr.Split(ArgSplitter);
            var name_items = items[0].Split(ParameterSplitter);
            Name = name_items[0];
            Parameter = name_items.Length > 1 ? name_items[1] : null;

            Argument = items.Skip(1).Where(ArgStr => !string.IsNullOrEmpty(ArgStr))
                        .Select(ArgStr => new Argument(ArgStr, ValueSplitter))
                        .Where(arg => !string.IsNullOrEmpty(arg.Name))
                        .ToArray();
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление команды</returns>
        [NotNull]
        public override string ToString() => 
            $"{Name}{(Parameter is null ? string.Empty : Parameter.ToFormattedString("({0})"))}{(Argument is null || Argument.Length == 0 ? string.Empty : Argument.ToSeparatedStr(" ").ToFormattedString(" {0}"))}";
    }
}