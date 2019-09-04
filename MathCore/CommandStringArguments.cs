using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore
{
    public class CommandStringArguments : IEnumerable<string>
    {
        /* ------------------------------------------------------------------------------------------ */

        private readonly string[] _Arguments;

        /* ------------------------------------------------------------------------------------------ */

        public int Count => _Arguments.Length;

        public string this[int i] => _Arguments[i];

        /* ------------------------------------------------------------------------------------------ */

        public CommandStringArguments(IEnumerable<string> Args) => _Arguments = Args.ToArray();

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