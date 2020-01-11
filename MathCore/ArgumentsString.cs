using System.Linq;
using System.Text;
using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>Аргументы командной строки</summary>
    public class ArgumentsString
    {
        /* ------------------------------------------------------------------------------------------ */

        private readonly string[] _Arguments;

        /* ------------------------------------------------------------------------------------------ */

        public int Count => _Arguments?.Length ?? 0;

        public string this[int index] => _Arguments[index];

        /* ------------------------------------------------------------------------------------------ */

        public ArgumentsString(string[] Arguments) => _Arguments = Arguments;

        /* ------------------------------------------------------------------------------------------ */

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

        [NotNull]
        public static implicit operator ArgumentsString(string[] Arguments) => new ArgumentsString(Arguments);

        public static implicit operator string[]([NotNull] ArgumentsString Arguments) => Arguments._Arguments;

        [NotNull]
        public static explicit operator string([NotNull] ArgumentsString Argument) => Argument.ToString();

        /* ------------------------------------------------------------------------------------------ */
    }
}