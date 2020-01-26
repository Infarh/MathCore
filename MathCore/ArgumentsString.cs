using System;
using System.Linq;
using System.Text;
using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore
{
    /// <summary>Аргументы командной строки</summary>
    public class ArgumentsString
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Строки значений аргументов</summary>
        private readonly string[] _Arguments;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Число аргументов</summary>
        public int Count => _Arguments?.Length ?? 0;

        /// <summary>Получение аргумента по индексу</summary>
        /// <param name="index">Индекс аргумента</param>
        /// <returns>Значение аргумента по указанному индексу</returns>
        public ref readonly string this[int index] => ref _Arguments[index];

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового экземпляра <see cref="ArgumentsString"/></summary>
        /// <param name="Arguments">Массив значений аргументов</param>
        public ArgumentsString(string[] Arguments) => _Arguments = Arguments;

        /* ------------------------------------------------------------------------------------------ */

        /// <inheritdoc />
        [NotNull]
        public override string ToString()
        {
            if (_Arguments is null) return string.Empty;
            var last_index = _Arguments.Length - 1;
            return _Arguments
               .Aggregate(new StringBuilder(), (S, s, i) => S.AppendFormat(i != last_index ? "{0} " : "{0}", s))
               .ToString();
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Оператор неявного приведения типа массива строк к <see cref="ArgumentsString"/></summary>
        /// <param name="Arguments">Массив строковых значений аргументов</param>
        /// <returns>Экземпляр <see cref="ArgumentsString"/></returns>
        [NotNull]
        public static implicit operator ArgumentsString(string[] Arguments) => new ArgumentsString(Arguments);

        /// <summary>Оператор неявного приведения <see cref="ArgumentsString"/> к типу массива строк</summary>
        /// <param name="Arguments">Экземпляр <see cref="ArgumentsString"/></param>
        /// <returns>Массив строковых значений аргументов</returns>
        public static implicit operator string[]([NotNull] ArgumentsString Arguments) => Arguments._Arguments;

        /// <summary>Оператор неявного приведения <see cref="ArgumentsString"/> к строке</summary>
        /// <param name="Argument">Экземпляр <see cref="ArgumentsString"/></param>
        /// <returns>Строковое представление <see cref="ArgumentsString"/></returns>
        [NotNull]
        public static explicit operator string([NotNull] ArgumentsString Argument) => Argument.ToString();

        /* ------------------------------------------------------------------------------------------ */
    }
}