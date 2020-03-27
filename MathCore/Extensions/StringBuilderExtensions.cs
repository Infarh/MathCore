using System.IO;
using MathCore.Annotations;
// ReSharper disable CheckNamespace

namespace System.Text
{
    public static class StringBuilderExtensions
    {
        /// <summary>Создать объект чтения</summary>
        /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
        /// <returns>Объект чтения строк</returns>
        [NotNull] public static TextReader CreateReader([NotNull] this StringBuilder str) => new StringReader((str ?? throw new ArgumentNullException(nameof(str))).ToString());

        /// <summary>Создать объект записи строк</summary>
        /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
        /// <returns>Объект записи строк</returns>
        [NotNull] public static TextWriter CreateWriter([NotNull] this StringBuilder str) =>  new StringWriter(str ?? throw new ArgumentNullException(nameof(str)));
    }
}
