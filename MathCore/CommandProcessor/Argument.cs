using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.CommandProcessor
{
    /// <summary>Аргумент команды</summary>
    public struct Argument
    {
        /// <summary>Имя аргумента</summary>
        public string Name { get; set; }

        /// <summary>Значения аргумента</summary>
        private string[] _Values;

        /// <summary>Значения аргумента</summary>
        public IReadOnlyList<string> Values => _Values;

        /// <summary>Значение аргумента</summary>
        public string Value => _Values?.Length > 0 ? _Values[0] : string.Empty;

        /// <summary>Количество значений аргумента</summary>
        public int Count => _Values?.Length ?? 0;

        /// <summary>Доступ к значениям аргумента по номеру</summary>
        /// <param name="i">Номер значения</param>
        /// <returns>Значение аргумента с указанным номером</returns>
        public string this[int i] => _Values[i];

        /// <summary>Аргумент команды</summary>
        /// <param name="ArgStr">Строковое описание аргумента</param>
        /// <param name="ValueSplitter">Разделитель имени аргумента и значения</param>
        public Argument([NotNull] string ArgStr, char ValueSplitter = '=')
            : this()
        {
            var ArgItems = ArgStr.Split(ValueSplitter);
            Name = ArgItems[0].ClearSystemSymbolsAtBeginAndEnd();
            _Values = ArgItems.Skip(1)
                        .Select(value => value.ClearSystemSymbolsAtBeginAndEnd())
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToArray();
        }

        /// <summary>Представление значения в указанном типе</summary>
        /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
        /// <returns>Значение аргумента указанного типа</returns>
        public T ValueAs<T>() => (T)Convert.ChangeType(Value, typeof(T));

        /// <summary>Попытаться получить значение аргумента команды в указанном типе <typeparamref name="T"/></summary>
        /// <param name="value">Приведённое к типу <typeparamref name="T"/> значение аргумента</param>
        /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
        /// <returns>Исключение, возникшее в процессе преобразования строки значения аргумента к типу <typeparamref name="T"/></returns>
        public bool TryGetValueAs<T>(out T value)
        {
            try
            {
                value = ValueAs<T>();
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>Попытаться получить значение аргумента команды в указанном типе <typeparamref name="T"/></summary>
        /// <param name="value">Приведённое к типу <typeparamref name="T"/> значение аргумента</param>
        /// <param name="Error">Исключение, возникшее в процессе преобразования строки значения аргумента к типу <typeparamref name="T"/></param>
        /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
        /// <returns>Истина, если преобразование выполнено успешно</returns>
        public bool TryGetValueAs<T>(out T value, [CanBeNull] out Exception Error)
        {
            try
            {
                value = ValueAs<T>();
                Error = null;
                return true;
            }
            catch (Exception error)
            {
                value = default;
                Error = error;
                return false;
            }
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление аргумента</returns>
        [NotNull]
        public override string ToString() => $"{Name}{(_Values is null || _Values.Length == 0 ? string.Empty : Values.ToSeparatedStr(", ").ToFormattedString("={0}"))}";
    }
}