using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore
{
    public class LambdaEnumerable<T> : Factory<IEnumerator<T>>, IEnumerable<T>
    {  
        /* ------------------------------------------------------------------------------------------ */

        public LambdaEnumerable([CanBeNull] Func<IEnumerable<T>> Generator) : base(() => (Generator?.Invoke() ?? Enumerable.Empty<T>()).GetEnumerator()) { }

        /* ------------------------------------------------------------------------------------------ */

        #region Implementation of IEnumerable

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции.</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator() => Create();

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        /* ------------------------------------------------------------------------------------------ */
    }
}