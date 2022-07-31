#nullable enable
using System.IO;
// ReSharper disable CheckNamespace

namespace System.Text;

public static class StringBuilderExtensions
{
    /// <summary>Создать объект чтения</summary>
    /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект чтения строк</returns>
    public static StringReader CreateReader(this StringBuilder str) => new((str ?? throw new ArgumentNullException(nameof(str))).ToString());

    /// <summary>Создать объект записи строк</summary>
    /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект записи строк</returns>
    public static StringWriter CreateWriter(this StringBuilder str) =>  new(str ?? throw new ArgumentNullException(nameof(str)));
}