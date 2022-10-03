#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore;

/// <summary>Настраиваемое перечисление</summary>
/// <typeparam name="T">Тип элемента перечисления</typeparam>
public class LambdaEnumerable<T> : Factory<IEnumerator<T>>, IEnumerable<T>
{
    /* ------------------------------------------------------------------------------------------ */

    public LambdaEnumerable(Func<IEnumerable<T>>? Generator) : base(() => (Generator?.Invoke() ?? Enumerable.Empty<T>()).GetEnumerator()) { }

    /* ------------------------------------------------------------------------------------------ */

    #region Implementation of IEnumerable

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Create() ?? throw new InvalidOperationException();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    /* ------------------------------------------------------------------------------------------ */
}