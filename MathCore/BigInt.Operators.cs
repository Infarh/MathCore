// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Overloading of addition operator
    //***********************************************************************

    public static BigInt operator +(BigInt x, BigInt y)
    {
        var result = new BigInt
        {
            _DataLength = x._DataLength > y._DataLength
                ? x._DataLength
                : y._DataLength
        };

        long carry = 0;
        for (var i = 0; i < result._DataLength; i++)
        {
            var sum = x._Data[i] + (long)y._Data[i] + carry;
            carry = sum >> 32;
            result._Data[i] = (uint)(sum & 0xFFFFFFFF);
        }

        if (carry != 0 && result._DataLength < MaxLength)
        {
            result._Data[result._DataLength] = (uint)carry;
            result._DataLength++;
        }

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;


        // overflow check
        const int last_pos = MaxLength - 1;
        if ((x._Data[last_pos] & 0x80000000) == (y._Data[last_pos] & 0x80000000) &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException();

        return result;
    }


    //***********************************************************************
    // Overloading of the unary ++ operator
    //***********************************************************************

    public static BigInt operator ++(BigInt x)
    {
        var result = new BigInt(x);

        long carry = 1;
        var index = 0;

        while (carry != 0 && index < MaxLength)
        {
            var val = (long)result._Data[index];
            val++;

            result._Data[index] = (uint)(val & 0xFFFFFFFF);
            carry = val >> 32;

            index++;
        }

        if (index > result._DataLength) result._DataLength = index;
        else while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // overflow check
        const int last_pos = MaxLength - 1;

        // overflow if initial value was +ve but ++ caused a sign
        // change to negative.

        if ((x._Data[last_pos] & 0x80000000) == 0 &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException("Overflow in ++.");
        return result;
    }


    //***********************************************************************
    // Overloading of subtraction operator
    //***********************************************************************

    public static BigInt operator -(BigInt x, BigInt y)
    {
        var result = new BigInt
        {
            _DataLength = x._DataLength > y._DataLength
                ? x._DataLength
                : y._DataLength
        };

        long carry_in = 0;
        for (var i = 0; i < result._DataLength; i++)
        {
            var diff = x._Data[i] - (long)y._Data[i] - carry_in;
            result._Data[i] = (uint)(diff & 0xFFFFFFFF);

            carry_in = diff < 0 ? 1 : 0;
        }

        // roll over to negative
        if (carry_in != 0)
        {
            for (var i = result._DataLength; i < MaxLength; i++)
                result._Data[i] = 0xFFFFFFFF;
            result._DataLength = MaxLength;
        }

        // fixed in v1.03 to give correct data length for a - (-b)
        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // overflow check

        const int last_pos = MaxLength - 1;
        if ((x._Data[last_pos] & 0x80000000) != (y._Data[last_pos] & 0x80000000) &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException();

        return result;
    }


    //***********************************************************************
    // Overloading of the unary -- operator
    //***********************************************************************

    public static BigInt operator --(BigInt x)
    {
        var result = new BigInt(x);

        var carry_in = true;
        var index = 0;

        while (carry_in && index < MaxLength)
        {
            var val = (long)result._Data[index];
            val--;

            result._Data[index] = (uint)(val & 0xFFFFFFFF);

            if (val >= 0) carry_in = false;

            index++;
        }

        if (index > result._DataLength) result._DataLength = index;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // overflow check
        const int last_pos = MaxLength - 1;

        // overflow if initial value was -ve but -- caused a sign
        // change to positive.

        if ((x._Data[last_pos] & 0x80000000) != 0 &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException("Underflow in --.");

        return result;
    }

    /// <summary>Overloading of multiplication operator</summary>
    /// <exception cref="ArithmeticException">Multiplication overflow</exception>
    public static BigInt operator *(BigInt x, BigInt y)
    {
        const int last_pos = MaxLength - 1;
        var x_neg = false;
        var y_neg = false;

        // take the absolute value of the inputs
        try
        {
            if ((x._Data[last_pos] & 0x80000000) != 0) // x negative
            {
                x_neg = true;
                x = -x;
            }
            if ((y._Data[last_pos] & 0x80000000) != 0) // y negative
            {
                y_neg = true;
                y = -y;
            }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
        {
            // ignored
        }
#pragma warning restore CA1031 // Do not catch general exception types

        var result = new BigInt();

        // multiply the absolute values
        try
        {
            for (var i = 0; i < x._DataLength; i++)
            {
                if (x._Data[i] == 0) continue;

                ulong mc_array = 0;
                for (int j = 0, k = i; j < y._DataLength; j++, k++)
                {
                    // k = i + j
                    var val = x._Data[i] * (ulong)y._Data[j] + result._Data[k] + mc_array;

                    result._Data[k] = (uint)(val & 0xFFFFFFFF);
                    mc_array = val >> 32;
                }

                if (mc_array != 0) result._Data[i + y._DataLength] = (uint)mc_array;
            }
        }
        catch (Exception)
        {
            throw new ArithmeticException("Multiplication overflow.");
        }


        result._DataLength = x._DataLength + y._DataLength;
        if (result._DataLength > MaxLength) result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // overflow check (result is -ve)
        if ((result._Data[last_pos] & 0x80000000) == 0)
            // if input has different signs, then result is -ve
            return x_neg != y_neg ? -result : result;

        if (x_neg == y_neg || result._Data[last_pos] != 0x80000000)
            throw new ArithmeticException("Multiplication overflow.");
        // handle the special case where multiplication produces
        // a max negative number in 2's complement.

        if (result._DataLength == 1) return result;
        var is_max_neg = true;
        for (var i = 0; i < result._DataLength - 1 && is_max_neg; i++)
            if (result._Data[i] != 0)
                is_max_neg = false;

        if (is_max_neg) return result;

        throw new ArithmeticException("Multiplication overflow.");
    }



    //***********************************************************************
    // Overloading of unary << operators
    //***********************************************************************

    public static BigInt operator <<(BigInt x, int ShiftVal)
    {
        var result = new BigInt(x);
        result._DataLength = ShiftLeft(result._Data, ShiftVal);
        return result;
    }


    // least significant bits at lower part of buffer

    private static int ShiftLeft(uint[] buffer, int ShiftVal)
    {
        var shift_amount = 32;
        var buf_len = buffer.Length;

        while (buf_len > 1 && buffer[buf_len - 1] == 0) buf_len--;

        for (var count = ShiftVal; count > 0;)
        {
            if (count < shift_amount) shift_amount = count;

            ulong carry = 0;
            for (var i = 0; i < buf_len; i++)
            {
                var val = (ulong)buffer[i] << shift_amount;
                val |= carry;

                buffer[i] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;
            }

            if (carry != 0 && buf_len + 1 <= buffer.Length) buffer[buf_len++] = (uint)carry;
            count -= shift_amount;
        }
        return buf_len;
    }


    //***********************************************************************
    // Overloading of unary >> operators
    //***********************************************************************

    public static BigInt operator >>(BigInt x, int ShiftVal)
    {
        var result = new BigInt(x);
        result._DataLength = ShiftRight(result._Data, ShiftVal);

        if ((x._Data[MaxLength - 1] & 0x80000000) == 0) return result;
        for (var i = MaxLength - 1; i >= result._DataLength; i--)
            result._Data[i] = 0xFFFFFFFF;

        var mask = 0x80000000;
        for (var i = 0; i < 32; i++)
        {
            if ((result._Data[result._DataLength - 1] & mask) != 0) break;

            result._Data[result._DataLength - 1] |= mask;
            mask >>= 1;
        }
        result._DataLength = MaxLength;

        return result;
    }


    private static int ShiftRight(uint[] buffer, int ShiftVal)
    {
        var shift_amount = 32;
        var inv_shift = 0;
        var buf_len = buffer.Length;

        while (buf_len > 1 && buffer[buf_len - 1] == 0) buf_len--;

        for (var count = ShiftVal; count > 0;)
        {
            if (count < shift_amount)
            {
                shift_amount = count;
                inv_shift = 32 - shift_amount;
            }

            ulong carry = 0;
            for (var i = buf_len - 1; i >= 0; i--)
            {
                var val = (ulong)buffer[i] >> shift_amount;
                val |= carry;

                carry = (ulong)buffer[i] << inv_shift;
                buffer[i] = (uint)val;
            }

            count -= shift_amount;
        }

        while (buf_len > 1 && buffer[buf_len - 1] == 0) buf_len--;

        return buf_len;
    }


    //***********************************************************************
    // Overloading of the NOT operator (1's complement)
    //***********************************************************************

    public static BigInt operator ~(BigInt x)
    {
        var result = new BigInt(x);

        for (var i = 0; i < MaxLength; i++)
            result._Data[i] = ~x._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Overloading of the NEGATE operator (2's complement)
    //***********************************************************************

    public static BigInt operator -(BigInt x)
    {
        // handle neg of zero separately since it'll cause an overflow
        // if we proceed.

        if (x._DataLength == 1 && x._Data[0] == 0)
            return new();

        var result = new BigInt(x);

        // 1's complement
        for (var i = 0; i < MaxLength; i++)
            result._Data[i] = ~x._Data[i];

        // add one to result of 1's complement
        long carry = 1;
        var index = 0;

        while (carry != 0 && index < MaxLength)
        {
            var val = (long)result._Data[index];
            val++;

            result._Data[index] = (uint)(val & 0xFFFFFFFF);
            carry = val >> 32;

            index++;
        }

        if ((x._Data[MaxLength - 1] & 0x80000000) == (result._Data[MaxLength - 1] & 0x80000000))
            throw new ArithmeticException("Overflow in negation.\n");

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;
        return result;
    }


    //***********************************************************************
    // Overloading of equality operator
    //***********************************************************************

    public static bool operator ==(BigInt? x, BigInt? y) => (Equals(x, null) && Equals(y, null)) || (!Equals(x, null) && x.Equals(y));


    public static bool operator !=(BigInt? x, BigInt? y) => !(x == y);


    /// <inheritdoc />
    public override bool Equals(object? o) => Equals(o as BigInt);

    public bool Equals(BigInt? x)
    {
        if (_DataLength != x?._DataLength) return false;

        for (var i = 0; i < _DataLength; i++)
            if (_Data[i] != x._Data[i])
                return false;
        return true;
    }


    /// <inheritdoc />
    public override int GetHashCode() => _Data.GetComplexHashCode(); //        return ToString().GetHashCode();


    //***********************************************************************
    // Overloading of inequality operator
    //***********************************************************************
    public static bool operator >(BigInt x, BigInt y)
    {
        var pos = MaxLength - 1;

        // x is negative, y is positive
        if ((x._Data[pos] & 0x80000000) != 0 && (y._Data[pos] & 0x80000000) == 0)
            return false;

        // x is positive, y is negative
        if ((x._Data[pos] & 0x80000000) == 0 && (y._Data[pos] & 0x80000000) != 0)
            return true;

        // same sign
        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;
        for (pos = len - 1; pos >= 0 && x._Data[pos] == y._Data[pos]; pos--) { }

        return pos >= 0 && x._Data[pos] > y._Data[pos];
    }


    public static bool operator <(BigInt x, BigInt y)
    {
        var pos = MaxLength - 1;

        // x is negative, y is positive
        if ((x._Data[pos] & 0x80000000) != 0 && (y._Data[pos] & 0x80000000) == 0)
            return true;

        // x is positive, y is negative
        if ((x._Data[pos] & 0x80000000) == 0 && (y._Data[pos] & 0x80000000) != 0)
            return false;

        // same sign
        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;
        for (pos = len - 1; pos >= 0 && x._Data[pos] == y._Data[pos]; pos--) { }

        return pos >= 0 && x._Data[pos] < y._Data[pos];
    }


    public static bool operator >=(BigInt x, BigInt y) => x == y || x > y;


    public static bool operator <=(BigInt x, BigInt y) => x == y || x < y;


    //***********************************************************************
    // Private function that supports the division of two numbers with
    // a divisor that has more than 1 digit.
    //
    // Algorithm taken from [1]
    //***********************************************************************

    private static void MultiByteDivide(
        BigInt X,
        BigInt Y,
        BigInt OutQuotient,
        BigInt OutRemainder)
    {
        var result = new uint[MaxLength];

        var remainder_len = X._DataLength + 1;
        var remainder = new uint[remainder_len];

        var mask = 0x80000000;
        var val = Y._Data[Y._DataLength - 1];
        var shift = 0;
        var result_pos = 0;

        while (mask != 0 && (val & mask) == 0)
        {
            shift++;
            mask >>= 1;
        }

        for (var i = 0; i < X._DataLength; i++) remainder[i] = X._Data[i];
        ShiftLeft(remainder, shift);
        Y <<= shift;

        var j = remainder_len - Y._DataLength;
        var pos = remainder_len - 1;

        ulong first_divisor_byte = Y._Data[Y._DataLength - 1];
        ulong second_divisor_byte = Y._Data[Y._DataLength - 2];

        var divisor_len = Y._DataLength + 1;
        var dividend_part = new uint[divisor_len];

        while (j > 0)
        {
            var dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];

            var q_hat = dividend / first_divisor_byte;
            var r_hat = dividend % first_divisor_byte;

            var done = false;
            while (!done)
            {
                done = true;

                if (q_hat != 0x100000000 && q_hat * second_divisor_byte <= (r_hat << 32) + remainder[pos - 2])
                    continue;
                q_hat--;
                r_hat += first_divisor_byte;

                if (r_hat < 0x100000000) done = false;
            }

            for (var h = 0; h < divisor_len; h++)
                dividend_part[h] = remainder[pos - h];

            var kk = new BigInt(dividend_part);
            var ss = Y * (long)q_hat;

            while (ss > kk)
            {
                q_hat--;
                ss -= Y;
            }
            var yy = kk - ss;

            for (var h = 0; h < divisor_len; h++)
                remainder[pos - h] = yy._Data[Y._DataLength - h];

            result[result_pos++] = (uint)q_hat;

            pos--;
            j--;
        }

        OutQuotient._DataLength = result_pos;
        var y = 0;
        for (var x = OutQuotient._DataLength - 1; x >= 0; x--, y++)
            OutQuotient._Data[y] = result[x];
        for (; y < MaxLength; y++) OutQuotient._Data[y] = 0;

        while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
            OutQuotient._DataLength--;

        if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

        OutRemainder._DataLength = ShiftRight(remainder, shift);

        for (y = 0; y < OutRemainder._DataLength; y++) OutRemainder._Data[y] = remainder[y];
        for (; y < MaxLength; y++) OutRemainder._Data[y] = 0;
    }


    //***********************************************************************
    // Private function that supports the division of two numbers with
    // a divisor that has only 1 digit.
    //***********************************************************************

    private static void SingleByteDivide(
        BigInt x,
        BigInt y,
        BigInt OutQuotient,
        BigInt OutRemainder)
    {
        var result = new uint[MaxLength];
        var result_pos = 0;

        // copy dividend to reminder
        for (var i = 0; i < MaxLength; i++)
            OutRemainder._Data[i] = x._Data[i];
        OutRemainder._DataLength = x._DataLength;

        while (OutRemainder._DataLength > 1 && OutRemainder._Data[OutRemainder._DataLength - 1] == 0)
            OutRemainder._DataLength--;

        var divisor = (ulong)y._Data[0];
        var pos = OutRemainder._DataLength - 1;
        var dividend = (ulong)OutRemainder._Data[pos];

        if (dividend >= divisor)
        {
            var quotient = dividend / divisor;
            result[result_pos++] = (uint)quotient;

            OutRemainder._Data[pos] = (uint)(dividend % divisor);
        }
        pos--;

        while (pos >= 0)
        {
            dividend = ((ulong)OutRemainder._Data[pos + 1] << 32) + OutRemainder._Data[pos];
            var quotient = dividend / divisor;
            result[result_pos++] = (uint)quotient;

            OutRemainder._Data[pos + 1] = 0;
            OutRemainder._Data[pos--] = (uint)(dividend % divisor);
        }

        OutQuotient._DataLength = result_pos;
        var j = 0;
        for (var i = OutQuotient._DataLength - 1; i >= 0; i--, j++)
            OutQuotient._Data[j] = result[i];
        for (; j < MaxLength; j++) OutQuotient._Data[j] = 0;

        while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
            OutQuotient._DataLength--;

        if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

        while (OutRemainder._DataLength > 1 && OutRemainder._Data[OutRemainder._DataLength - 1] == 0)
            OutRemainder._DataLength--;
    }


    //***********************************************************************
    // Overloading of division operator
    //***********************************************************************

    public static BigInt operator /(BigInt x, BigInt y)
    {
        var quotient = new BigInt();
        var remainder = new BigInt();

        const int last_pos = MaxLength - 1;
        bool divisor_neg = false, dividend_neg = false;

        if ((x._Data[last_pos] & 0x80000000) != 0) // x negative
        {
            x = -x;
            dividend_neg = true;
        }
        if ((y._Data[last_pos] & 0x80000000) != 0) // y negative
        {
            y = -y;
            divisor_neg = true;
        }

        if (x < y) return quotient;
        if (y._DataLength == 1)
            SingleByteDivide(x, y, quotient, remainder);
        else
            MultiByteDivide(x, y, quotient, remainder);

        return dividend_neg != divisor_neg ? -quotient : quotient;
    }


    //***********************************************************************
    // Overloading of modulus operator
    //***********************************************************************

    public static BigInt operator %(BigInt x, BigInt y)
    {
        var quotient = new BigInt();
        var remainder = new BigInt(x);

        const int last_pos = MaxLength - 1;
        var dividend_neg = false;

        if ((x._Data[last_pos] & 0x80000000) != 0) // x negative
        {
            x = -x;
            dividend_neg = true;
        }
        if ((y._Data[last_pos] & 0x80000000) != 0) // y negative
            y = -y;

        if (x < y) return remainder;
        if (y._DataLength == 1)
            SingleByteDivide(x, y, quotient, remainder);
        else
            MultiByteDivide(x, y, quotient, remainder);

        return dividend_neg ? -remainder : remainder;
    }


    //***********************************************************************
    // Overloading of bitwise AND operator
    //***********************************************************************

    public static BigInt operator &(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
            result._Data[i] = x._Data[i] & y._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Overloading of bitwise OR operator
    //***********************************************************************

    public static BigInt operator |(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
            result._Data[i] = x._Data[i] | y._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Overloading of bitwise XOR operator
    //***********************************************************************

    public static BigInt operator ^(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
        {
            var sum = x._Data[i] ^ y._Data[i];
            result._Data[i] = sum;
        }

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }
}
