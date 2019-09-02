using System;
using System.Linq;

namespace MathCore.CommandProcessor
{
    /// <summary>Аргумент команды</summary>
    public struct Argument
    {
        /// <summary>Имя аргумента</summary>
        public string Name { get; set; }
        /// <summary>Массив значений аргумента</summary>
        public string[] Values { get; set; }
        /// <summary>Значение аргумента</summary>
        public string Value => Values?.Length > 0 ? Values[0] : string.Empty;

        /// <summary>Количество значений аргумента</summary>
        public int Count => Values?.Length ?? 0;

        /// <summary>Доступ к значениям аргумента по номеру</summary>
        /// <param name="i">Номер значения</param>
        /// <returns>Значение аргумента с указанным номером</returns>
        public string this[int i] => Values[i];

        /// <summary>Аргумент команды</summary>
        /// <param name="ArgStr">Строковое описание аргумента</param>
        /// <param name="ValueSplitter">Разделитель имени аргумента и значения</param>
        public Argument(string ArgStr, char ValueSplitter = '=')
            : this()
        {
            var ArgItems = ArgStr.Split(ValueSplitter);
            Name = ArgItems[0].ClearSystemSymbolsAtBeginAndEnd();
            Values = ArgItems.Skip(1)
                        .Select(value => value.ClearSystemSymbolsAtBeginAndEnd())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToArray();
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление аргумента</returns>
        public override string ToString() => $"{Name}{(Values is null || Values.Length == 0 ? "" : Values.ToSeparatedStr(", ").ToFormattedString("={0}"))}";
    }
}