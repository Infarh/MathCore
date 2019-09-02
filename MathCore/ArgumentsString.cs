using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MathCore.Annotations;

namespace MathCore
{
    public class ArgumentsString
    {
        /* ------------------------------------------------------------------------------------------ */

        public static event EventHandler<ClassInitializeEventArgs> ClassInitialize;

        public class ClassInitializeEventArgs : EventArgs
        {
            public char TermsSeparator { get; set; } = ' ';
            public char ArgumentsSeparator { get; set; } = ':';
            public List<char> KeyCharsList { get; } = new List<char> { '/', '-' };
        }

        /* ------------------------------------------------------------------------------------------ */

        private static char __TermsSeparator;
        private static char __ArgumentsSeparator;
        private static readonly List<char> __KeyCharsList;

        /* ------------------------------------------------------------------------------------------ */

        private readonly string[] _Arguments;

        /* ------------------------------------------------------------------------------------------ */

        public int Count => _Arguments?.Length ?? 0;

        public string this[int index] => _Arguments[index];

        /* ------------------------------------------------------------------------------------------ */

        static ArgumentsString()
        {
            var event_args = new ClassInitializeEventArgs();
            ClassInitialize.FastStart(typeof(ArgumentsString), event_args);

            __TermsSeparator = event_args.TermsSeparator;
            __ArgumentsSeparator = event_args.ArgumentsSeparator;
            __KeyCharsList = event_args.KeyCharsList;
        }

        public ArgumentsString(string[] Arguments) => _Arguments = Arguments;

        /* ------------------------------------------------------------------------------------------ */

        public override string ToString()
        {
            if (_Arguments is null) return "";
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