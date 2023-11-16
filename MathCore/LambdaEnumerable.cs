#nullable enable
using System.Collections;

namespace MathCore;

/// <summary>Настраиваемое перечисление</summary>
/// <typeparam name="T">Тип элемента перечисления</typeparam>
public class LambdaEnumerable<T>(Func<IEnumerable<T>>? Generator) : Factory<IEnumerator<T>>(() => (Generator?.Invoke() ?? Enumerable.Empty<T>()).GetEnumerator()), IEnumerable<T>
{

    /* ------------------------------------------------------------------------------------------ */

    #region Implementation of IEnumerable

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Create() ?? throw new InvalidOperationException();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    /* ------------------------------------------------------------------------------------------ */
}