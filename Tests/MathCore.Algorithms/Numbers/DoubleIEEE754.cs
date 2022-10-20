#nullable enable
using System.Globalization;

namespace MathCore.Algorithms.Numbers;

public static class DoubleIEEE754
{
    private const double log10_2 = 0.3010299956639811952137388947244930267681898814621085413104274611271081892744245094869272521181861720406844771914309953790947678;
    private const int __ExpMask = (1 << 11) - 1;
    private const long __MantissaMask = ((long)1 << 52) - 1;

    public static double Exp10(this double x, int exp)
    {


        return x;
    }

    public static (long Mantissa, short Exp2, bool Sign) Parse(double value)
    {
        var bits = BitConverter.DoubleToInt64Bits(value);

        var exp = (short)(((bits >> 52) & __ExpMask) - 1023);
        var mantissa = bits & __MantissaMask;
        var sign = (bits >> (52 + 11)) != 0;

        return (mantissa, exp, sign);
    }

    /// <summary>Формирование вещественного числа</summary>
    /// <param name="Mantissa">Битовое представление мантиссы</param>
    /// <param name="Exp2">Показатель экспоненты с основанием 2 - 2^Exp2</param>
    /// <param name="Sign">Знак</param>
    /// <returns>Вещественное число</returns>
    public static double Create(long Mantissa, short Exp2, bool Sign)
    {
        var exp = ((Exp2 + 1023L) & __ExpMask) << 52;
        var mantissa = Mantissa & __MantissaMask;

        var bits = Sign
            ? exp | mantissa | (1L << (11 + 52))
            : exp | mantissa;

        var result = BitConverter.Int64BitsToDouble(bits);

        return result;
    }

    public static (long Mantissa, short Exp2, bool Sign) Decode(double x)
    {
        var sign = Math.Sign(x) < 0;
        var abs_x = Math.Abs(x);
        var exp2 = (short)Math.Floor(Math.Log2(abs_x));


        var mantissa = x * Math.Pow(2, -exp2) - 1;
        var q = 1d;

        var mantissa2 = 0L;
        for (var n = 0; n < 52; n++)
        {
            q /= 2;
            var m = mantissa % q;
            mantissa2 <<= 1;
            if (m != mantissa)
                mantissa2 |= 1;
            mantissa = m;
        }

        return (mantissa2, exp2, sign);
    }

    public static (short exp2, double k) GetPower2(int n)
    {
        if (n == 0) return (0, 1);

        short m = 0;
        var k = n > 0 ? 10 : 0.1;
        if (n > 0)
            while (k > 2)
            {
                m++;
                if ((k /= 2) < 2 && n > 1)
                    (k, n) = (k * 10, n - 1);
            }
        else
            while (k < 1)
            {
                m--;
                if ((k *= 2) > 1 && n < -1)
                    (k, n) = (k / 10, n + 1);
            }

        return (m, k);
    }

    public static (short exp2, double k) GetPower21(int n)
    {
        if (n == 0) return (0, 1);

        var m = Math.Floor(Consts.Log2_10 * n);
        var k = Math.Pow(10, n) * Math.Pow(2, -m);
        return ((short)m, k);
    }

    /* ---------------------------------------------------------------------------------- */

    public static bool FastTryParseDouble(string input, out double result)
    {
        // https://codereview.stackexchange.com/questions/200627/custom-double-parser-optimized-for-performance/200830#200830

        var length = input.Length;
        if (length <= 0)
        {
            result = double.NaN;
            return false;
        }

        var format_info = NumberFormatInfo.InvariantInfo;
        var char_negative = format_info.NegativeSign[0];
        var char_decimal_separator = format_info.NumberDecimalSeparator[0];
        var char_positive = format_info.PositiveSign[0];

        var sign = 1d;
        var current_index = 0;
        var next_char = input[0];

        /**************** Sign (+/-) and Special Case String Representations *****************/
        // Handle all cases where the string does not start with a numeric character
        if (next_char is < '0' or > '9')
        {
            // Non-numeric 1-character strings must match one of the supported special cases.
            if (length == 1)
                return CheckForSpecialCaseDoubleStrings(input, out result);

            // For anything more than one character, this should be a sign character.
            if (next_char == char_negative)
                sign = -1d;

            // The very next character may also be the decimal separator.
            else if (next_char == char_decimal_separator)
            {
                // In this case, we treat the integer part as 0 and skip to the fractional part.
                result = 0d;
                goto SkipIntegerPart;
            }
            // Finally, unless it was a '+' sign, input must match one of a set of special cases.
            else if (next_char != char_positive)
                return CheckForSpecialCaseDoubleStrings(input, out result);

            // Once the sign is consumed, advance to the next character for further parsing
            next_char = input[unchecked(++current_index)];
            // We must once more check whether the character is numeric before proceeding.
            if (next_char is < '0' or > '9')
            {
                // If not numeric, at this point, the character can only be a decimal separator
                // (as in "-.123" or "+.123"), or else it must be part of a special case string
                // (as in "-∞"). So check for those.
                if (next_char != char_decimal_separator)
                    return CheckForSpecialCaseDoubleStrings(input, out result);

                result = 0d;
                goto SkipIntegerPart;
            }
        }

        /********************************** "Integer Part" ***********************************/
        // Treat all subsequent numeric characters as the "integer part" of the result.
        // Since we've already checked that the next character is numeric,
        // We can save 2 ops by initializing the result directly.
        unchecked
        {
            result = next_char - '0';
            while (++current_index < length)
            {
                next_char = input[current_index];
                if (next_char < '0' || next_char > '9') break;
                result = result * 10d + (next_char - '0');
            }
        }

    // This label and corresponding goto statements is a performance optimization to
    // allow us to efficiently skip "integer part" parsing in cases like ".123456"
    // Please don't be mad.
    SkipIntegerPart:

        // The expected case is that the next character is a decimal separator, however
        // this section might be skipped in normal use cases (e.g. as in "1e18")
        // TODO: If we broke out of the while loop above due to reaching the end of the
        //       string, this operation is superfluous. Is there a way to skip it?
        if (next_char == char_decimal_separator)
        {
            /******************************* "Fractional Part" *******************************/
            // Track the index at the start of the fraction part.
            unchecked
            {
                var fraction_pos = ++current_index;

                // Continue shifting and adding to the result as before
                do
                {
                    next_char = input[current_index];

                    // Note that we flip the OR here, because it's now more likely that
                    // nextChar > '9' ('e' or 'E'), leading to an early exit condition.
                    if (next_char is > '9' or < '0')
                        break;

                    result = result * 10d + (next_char - '0');
                }
                while (++current_index < length);

                // Update this to store the number of digits in the "fraction part".
                // We will use this to adjust down the magnitude of the double.
                fraction_pos = current_index - fraction_pos;

                // Use our tiny array of negative powers of 10 if possible, but fallback to
                // our larger array (still fast), whose higher indices store negative powers.
                // Finally, while practically unlikely, ridiculous strings (>300 characters)
                // can still be supported with a final fallback to native Math.Pow
                // TODO: Is it possible to combine this magnitude adjustment with any
                //       applicable adjustment due to scientific notation?
                switch (fraction_pos)
                {

                    case < __NegPow10Length:
                        result *= __NegPow10[fraction_pos];
                        break;
                    case < __MaxDoubleExponent:
                        result *= __Pow10[__MaxDoubleExponent + fraction_pos];
                        break;
                    default:
                        result *= Math.Pow(10, -fraction_pos);
                        break;
                }
            }
        }

        // Apply the sign now that we've added all digits that belong to the significand
        result *= sign;

        // If we have consumed every character in the string, return now.
        if (current_index >= length)
            return true;

        // The next character encountered must be an exponent character
        if (next_char != 'e' && next_char != 'E')
            return false;

        /**************************** "Scientific Notation Part" *****************************/
        unchecked
        {
            // If we're at the end of the string (last character was 'e' or 'E'), that's an error
            if (++current_index >= length)
                return false;

            // Otherwise, advance the current character and begin parsing the exponent
            next_char = input[current_index];
            var exponent_is_negative = false;

            // The next character can only be a +/- sign, or a numeric character
            if (next_char is < '0' or > '9')
            {
                if (next_char == char_negative)
                    exponent_is_negative = true;
                else if (next_char != char_positive)
                    return false;

                // Again, require there to be at least one more character in the string after the sign
                if (++current_index >= length)
                    return false;

                next_char = input[current_index];

                // And verify that this next character is numeric
                if (next_char is < '0' or > '9')
                    return false;
            }

            // Since we know the next character is a digit, we can initialize the exponent int
            // directly and avoid 2 wasted ops (multiplying by and adding to zero).
            var exponent = next_char - '0';

            // Shift and add any additional digit characters
            while (++current_index < length)
            {
                next_char = input[current_index];

                // If we encounter any non-numeric characters now, it's definitely an error
                if (next_char is < '0' or > '9')
                    return false;

                exponent = exponent * 10 + (next_char - '0');
            }
            // Apply the exponent. If negative, our index jump is a little different.
            if (exponent_is_negative)
                result *= exponent < __Pow10Length - __MaxDoubleExponent
                    ? __Pow10[exponent + __MaxDoubleExponent] // Fallback to Math.Pow if the lookup array doesn't cover it.
                    : Math.Pow(10, -exponent);
            // If positive, our array covers all possible positive exponents - ensure its valid.
            else if (exponent > __MaxDoubleExponent)
                return false;
            else
                result *= __Pow10[exponent];
        }

        // doubles that underwent scientific notation parsing should be checked for overflow
        // (Otherwise, this isn't really a risk we don't expect strings of >308 characters)
        return !double.IsInfinity(result);
    }

    /// <summary>Checks if the string matches one of a few supported special case
    /// double strings. If so, assigns the result and returns true.</summary>
    public static bool CheckForSpecialCaseDoubleStrings(string input, out double result)
    {
        var current_info = NumberFormatInfo.InvariantInfo;

        if (input == current_info.PositiveInfinitySymbol)
            result = double.PositiveInfinity;
        else if (input == current_info.NegativeInfinitySymbol)
            result = double.NegativeInfinity;
        else if (input == current_info.NaNSymbol)
            result = double.NaN;

        // Special Case: Excel has been known to format zero as "-".
        // We intentionally support it by returning zero now (most parsers would not)
        else if (input == current_info.NegativeSign)
            result = 0d;

        // Special Case: Our organization treats the term "Unlimited" as referring
        // to double.MaxValue (most parsers would not)
        else if (input.Equals("unlimited", StringComparison.OrdinalIgnoreCase))
            result = double.MaxValue;

        // Anything else is not a valid input
        else
        {
            result = double.NaN;
            return false;
        }
        return true;
    }

    /// <summary>The largest exponent (or smallest when negative) that can be given to a double.</summary>
    private const int __MaxDoubleExponent = 308;

    /// <summary>The number of elements that will be generated in the Pow10 array.</summary>
    private const int __Pow10Length = __MaxDoubleExponent * 2 + 1;

    /// <summary>A cache of all possible positive powers of 10 that might be required to
    /// apply an exponent to a double (Indices 0-308), as well as the first 308 negative
    /// exponents. (Indices 309-616)</summary>
    private static readonly double[] __Pow10 = Enumerable
       .Range(0, __MaxDoubleExponent + 1)
       .Select(i => Math.Pow(10, i))
       .Concat(Enumerable.Range(1, __MaxDoubleExponent).Select(i => Math.Pow(10, -i)))
       .ToArray();

    /// <summary>The number of negative powers to pre-compute and store in a small array.</summary>
    private const int __NegPow10Length = 16;

    /// <summary>A cache of the first 15 negative powers of 10 for quick
    /// magnitude adjustment of common parsed fractional parts of doubles.</summary>
    /// <remarks>Even though this overlaps with the Pow10 array, it is kept separate so that
    /// users that don't use scientific notation or extremely long fractional parts
    /// might get a speedup by being able to reference the smaller array, which has a better
    /// chance of being served out of L1/L2 cache.</remarks>
    private static readonly double[] __NegPow10 = Enumerable
       .Range(0, __NegPow10Length)
       .ToArray(i => Math.Pow(10, -i));

    /* -------------------------------------------------------------------------------------------------------- */

    public static void TestParsingStrings()
    {
        // Numbers without a fractional part
        Test("0");
        Test("1");
        Test("-1");
        Test("12345678901234");
        Test("-12345678901234");
        // Numbers with a fractional part
        Test("123.45678");
        Test("-123.45678");
        // Numbers without an integer part
        Test(".12345678901234");
        Test("-.12345678901234");
        // Various high-precision numbers
        Test("0.12345678901234");
        Test("-0.12345678901234");
        Test("0.00000987654321");
        Test("-0.00000987654321");
        Test("1234567890123.0123456789");
        Test("-1234567890123.0123456789");
        // Numbers with very long fractional parts (more than 16 characters)
        Test("0.00826499999979784");
        Test("-0.00826499999979784");
        Test("1.0123456789012345678901234567890");
        Test("-1.0123456789012345678901234567890");
        // Numbers with a leading positive sign
        Test("+1");
        Test("+12345678901234");
        Test("+.12345678901234");
        Test("+0.00826499999979784");
        // Very large numbers without scientific notation
        Test("123456789000000000000000");
        Test("-123456789000000000000000");
        // Very small numbers without scientific notation
        Test("0.00000000000000000123456789");
        Test("-0.00000000000000000123456789");
        // Scientific notation without a sign
        Test("1.2345678e5");
        Test("1.2345678e5");
        Test("-1.2345678e5");
        // Scientific notation with a sign
        Test("1.2345678e+25");
        Test("-1.2345678e+25");
        Test("1.2345678e-255");
        Test("-1.2345678e-255");
        // Epsilon, and other tiny doubles
        // TODO: Known "failure" scenarios. Our parsing logic results in a return value of 0
        // for these, but the native parser returns Double.Epsilon (smallest number greater
        // than zero). I think we can live with this shortcoming.
        //TestSuccess("4.94065645841247e-324", 4.94065645841247e-324);
        //TestSuccess("-4.94065645841247e-324", -4.94065645841247e-324);
        Test("3.33E-333");
        Test("-3.33E-333");
        Test("1E-1022");
        Test("-1E-1022");
        // Boundary cases
        Test("1e0");
        Test("1e1");
        Test("1e-1");
        Test("1e-308");
        Test("1e308");
        // Min and Max Double
        Test("1.7976931348623157E+308");
        Test("-1.7976931348623157E+308");
        // Large Negative Exponents (Near-epsilon) doubles.
        Test("1.23E-999");
        Test("-1.23E-999");
        // Special keywords
        Test("∞");
        Test("-∞");
        Test("NaN");
        // Special case: "Unlimited" is used in our organization to refer to Double.MaxValue
        Test("Unlimited");
        // Special case: "-" character only means zero in accounting formats.
        Test("-");
    }

    private static void Test(string str)
    {
        if (str == "123.45678")
            Console.WriteLine();

        var expected = double.Parse(str, CultureInfo.InvariantCulture);

        if (!FastTryParseDouble(str, out var actual))
            throw new FormatException($"Строка {str} мела неверный формат");

        if (actual != expected)
            throw new InvalidOperationException(
                $"При разборе строки {str} полученное значение {actual} не совпадает с ожидаемым {expected} на {expected - actual}({(expected - actual) / expected})");
    }
}