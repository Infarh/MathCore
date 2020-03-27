using System.IO;
using MathCore.Annotations;
// ReSharper disable CheckNamespace

namespace System.Text
{
    public static class StringBuilderExtensions
    {
        [NotNull] public static TextReader CreateReader([NotNull] this StringBuilder str) => new StringReader((str ?? throw new ArgumentNullException(nameof(str))).ToString());

        [NotNull] public static TextWriter CreateWriter([NotNull] this StringBuilder str) =>  new StringWriter(str ?? throw new ArgumentNullException(nameof(str)));
    }
}
