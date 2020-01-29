using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore
{
    /// <summary>Аргументы командной строки</summary>
    public class CommandStringArguments : IEnumerable<string>
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Массив аргументов командной строки</summary>
        private readonly string[] _Arguments;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Количество аргументов командной строки</summary>
        public int Count => _Arguments.Length;

        /// <summary>Аргумент командной строки с указанным индексом</summary>
        /// <param name="i">Индекс аргумента</param>
        /// <returns>Значение аргумента с указанным индексом</returns>
        public ref readonly string this[int i] => ref _Arguments[i];

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового экземпляра <see cref="CommandStringArguments"/></summary>
        /// <param name="Args">Перечисление значений аргументов командной строки</param>
        public CommandStringArguments([NotNull] IEnumerable<string> Args) => _Arguments = Args.ToArray();

        /* ------------------------------------------------------------------------------------------ */

        #region Implementation of IEnumerable

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции аргументов командной строки</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, 
        /// который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator() => _Arguments.Cast<string>().GetEnumerator();

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, 
        /// который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}