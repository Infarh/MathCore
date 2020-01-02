using System;
using System.Globalization;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public class LambdaFormatter<T> : IFormatProvider, ICustomFormatter
    {
        private readonly Func<string, object, IFormatProvider, string> _Formatter;
        public LambdaFormatter(Func<string, object, IFormatProvider, string> Formatter) => _Formatter = Formatter;

        public object GetFormat(Type formatType) => formatType == typeof(ICustomFormatter) ? this : null;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg.GetType() == typeof(T)) return _Formatter(format, arg, formatProvider);
            try { return HandleOtherFormats(format, arg); } catch(FormatException e)
            {
                throw new FormatException($"The format of '{format}' is invalid.", e);
            }
        }

        private string HandleOtherFormats(string format, object arg)
        {
            var formattable = arg as IFormattable;
            return formattable?.ToString(format, CultureInfo.CurrentCulture) ?? (arg?.ToString() ?? string.Empty);
        }

    }
}