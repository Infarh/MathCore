#if NET8_0_OR_GREATER

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

using MathCore.Extensions;

namespace MathCore;

public partial struct Complex : IParsable<Complex>, ISpanParsable<Complex>, IUtf8SpanParsable<Complex>,
                                IFormattable, ISpanFormattable, IUtf8SpanFormattable,
                                IUnaryPlusOperators<Complex, Complex>,
                                IUnaryNegationOperators<Complex, Complex>,
                                IAdditionOperators<Complex, Complex, Complex>,
                                ISubtractionOperators<Complex, Complex, Complex>,
                                IMultiplyOperators<Complex, Complex, Complex>,
                                IEqualityOperators<Complex, Complex, bool>,
                                IMultiplicativeIdentity<Complex, Complex>,
                                IAdditiveIdentity<Complex, Complex>,
                                IDivisionOperators<Complex, Complex, Complex>
{
    static Complex IAdditiveIdentity<Complex, Complex>.AdditiveIdentity => new(0, 0);

    static Complex IMultiplicativeIdentity<Complex, Complex>.MultiplicativeIdentity => new(1, 0);


    #region ISpanParsable<Complex>

    /// <summary>Метод убирает все парные символы скобок в начале и конце строки</summary>
    /// <param name="str">Очищаемая строка</param>
    private static ReadOnlySpan<char> ClearStringPtr(ReadOnlySpan<char> str)
    {
        var start_len = str.Length;
        while (start_len > 0)
        {
            while (str is ['{', .., '}']) str = str[1..^1];
            while (str is ['[', .., ']']) str = str[1..^1];
            while (str is ['(', .., ')']) str = str[1..^1];
            while (str is ['"', .., '"']) str = str[1..^1];
            while (str is ['\'', .., '\'']) str = str[1..^1];

            var len = str.Length;
            start_len = len == start_len ? 0 : len;
        }

        return str;
    }

    public static Complex Parse(ReadOnlySpan<char> str, IFormatProvider provider) => throw new NotImplementedException();

    private readonly ref struct SpanSplitEnumerable(ReadOnlySpan<char> str, ReadOnlySpan<char> separators)
    {
        private readonly ReadOnlySpan<char> _Str = str;
        private readonly ReadOnlySpan<char> _Separators = separators;

        public SpanCharEnumerator GetEnumerator() => new(_Str, _Separators);
        public ref struct SpanCharEnumerator(ReadOnlySpan<char> str, ReadOnlySpan<char> separators)
        {
            private readonly ReadOnlySpan<char> _Str = str;
            private readonly ReadOnlySpan<char> _Separators = separators;
            private int _Index;

            public ReadOnlySpan<char> Current { get; private set; } = default;

            public bool MoveNext()
            {
                if (_Index < 0) return false;

                var next_index = _Str[_Index..].IndexOfAny(_Separators);

                if (next_index == -1)
                {
                    Current = _Str[(_Index + 1)..];
                    _Index = -1;
                    return true;
                }

                Current = _Str[(_Index == 0 ? 0 : _Index + 1)..next_index];
                _Index = next_index;

                return false;
            }
        }
    }

    public static bool TryParse(ReadOnlySpan<char> str, IFormatProvider provider, [MaybeNullWhen(false)] out Complex z)
    {
        var str_ptr = ClearStringPtr(str);

        var format = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));

        var minus_char = format.NegativeSign[0];

        var Re = 0d; // Аккумулятор действительной части
        var Im = 0d; // Аккумулятор мнимой части

        foreach (var v in new SpanSplitEnumerable(str, stackalloc[] { format.PositiveSign[0], minus_char }))
        {
            if (v.Length == 0) continue;

            var sign = v[0] == minus_char;
            double val;

            var is_im = false;
            if (v is ['i' or 'j', ..])
            {
                is_im = true;
                if (!double.TryParse(v[1..], provider, out val))
                {
                    z = default;
                    return false;
                }
            }
            else if (v is [.., 'i' or 'j'])
            {
                is_im = true;
                if (!double.TryParse(v[..^1], provider, out val))
                {
                    z = default;
                    return false;
                }
            }
            else if (!double.TryParse(v, provider, out val))
            {
                z = default;
                return false;
            }

            if (is_im)
                Im += sign ? -val : val;
            else
                Re += sign ? -val : val;
        }

        z = new(Re, Im);

        return true;
    }

    #endregion

    #region IUtf8SpanParsable<Complex>

    /// <summary>Разобрать строку в комплексное число</summary>
    /// <param name="str">Разбираемая строка</param>
    /// <returns>Комплексное число, получаемое в результате разбора строки</returns>
    /// <exception cref="ArgumentNullException">В случае если передана пустая ссылка на строку</exception>
    /// <exception cref="FormatException">В случае ошибочной строки</exception>
    public static Complex Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider provider) =>
        TryParse(utf8Text, provider, out var result)
            ? result
            : throw new FormatException("Строка имела неверный формат");
    public static bool TryParse(ReadOnlySpan<byte> str, IFormatProvider provider, out Complex z) => TryParse(str.Cast<char>(), provider, out z);

    #endregion

    public bool TryFormat(Span<char> destination, out int CharsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0)
            return 0.TryFormat(destination, out CharsWritten, format, provider);

        CharsWritten = 0;
        if (re != 0)
        {
            var re_was_written = re.TryFormat(destination, out var re_chars_written, format, provider);
            CharsWritten += re_chars_written;
            if(!re_was_written) return false;
        }

        if (im == 0) return true;

        if(im > 0 && CharsWritten > 0)
        {
            if (destination.Length < CharsWritten + 1)
                return false;
            destination[CharsWritten] = '+';
            CharsWritten++;
        }

        if(im == -1)
        {
            if (destination.Length < CharsWritten + 1)
                return false;
            destination[CharsWritten] = '-';
            CharsWritten++;
        }
        else if(im != 1)
        {
            var im_was_written = im.TryFormat(destination[CharsWritten..], out var im_chars_written, format, provider);
            CharsWritten += im_chars_written;
            if (!im_was_written) return false;
        }

        if (destination.Length < CharsWritten + 1)
            return false;
        destination[CharsWritten] = 'i';
        CharsWritten++;

        return true;
    }

    public bool TryFormat(Span<byte> destination, out int BytesWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0)
            return 0.TryFormat(destination, out BytesWritten, format, provider);

        BytesWritten = 0;
        if (re != 0)
        {
            var re_was_written = re.TryFormat(destination, out var re_bytes_written, format, provider);
            BytesWritten += re_bytes_written;
            if (!re_was_written) return false;
        }

        if (im == 0) return true;

        if (im > 0 && BytesWritten > 0)
        {
            if (destination.Length < BytesWritten + 1)
                return false;
            destination[BytesWritten] = (byte)'+';
            BytesWritten++;
        }

        if (im == -1)
        {
            if (destination.Length < BytesWritten + 1)
                return false;
            destination[BytesWritten] = (byte)'-';
            BytesWritten++;
        }
        else if (im != 1)
        {
            var im_was_written = im.TryFormat(destination[BytesWritten..], out var im_bytes_written, format, provider);
            BytesWritten += im_bytes_written;
            if (!im_was_written) return false;
        }

        if (destination.Length < BytesWritten + 1)
            return false;
        destination[BytesWritten] = (byte)'i';
        BytesWritten++;

        return true;
    }
}

#endif