using System;
using System.Linq;

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
        public Command(string CommandStr, char ParameterSplitter = ':', char ArgSplitter = ' ', char ValueSplitter = '=')
            : this()
        {
            var items = CommandStr.Split(ArgSplitter);
            var nameitems = items[0].Split(ParameterSplitter);
            Name = nameitems[0];
            if(nameitems.Length > 1)
                Parameter = nameitems[1];

            Argument = items.Skip(1).Where(ArgStr => !string.IsNullOrEmpty(ArgStr))
                        .Select(ArgStr => new Argument(ArgStr, ValueSplitter))
                        .Where(arg => !string.IsNullOrEmpty(arg.Name))
                        .ToArray();
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление команды</returns>
        public override string ToString() => 
            $"{Name}{(Parameter is null ? string.Empty : Parameter.ToFormattedString("({0})"))}{(Argument is null || Argument.Length == 0 ? string.Empty : Argument.ToSeparatedStr(" ").ToFormattedString(" {0}"))}";
    }
}