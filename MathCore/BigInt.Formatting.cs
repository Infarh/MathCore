// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Returns a string representing the BigInteger in base 10.
    //***********************************************************************

    /// <inheritdoc />
    public override string ToString() => ToString(10);


    //***********************************************************************
    // Returns a string representing the BigInteger in sign-and-magnitude
    // format in the specified radix.
    //
    // Example
    // -------
    // If the value of BigInteger is -255 in base 10, then
    // ToString(16) returns "-FF"
    //
    //***********************************************************************

    public string ToString(int radix)
    {
        if (radix is < 2 or > 36)
            throw new ArgumentException("Radix must be >= 2 and <= 36");

        // ReSharper disable once StringLiteralTypo
        const string char_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = string.Empty;

        var a = this;

        var negative = false;
        if ((a._Data[MaxLength - 1] & 0x80000000) != 0)
        {
            negative = true;
            try { a = -a; }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // ignored
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        var quotient = new BigInt();
        var remainder = new BigInt();
        var x_radix = new BigInt(radix);

        if (a._DataLength == 1 && a._Data[0] == 0)
            result = "0";
        else
        {
            while (a._DataLength > 1 || (a._DataLength == 1 && a._Data[0] != 0))
            {
                SingleByteDivide(a, x_radix, quotient, remainder);

                result = remainder._Data[0] < 10
                    ? remainder._Data[0] + result
                    : char_set[(int)remainder._Data[0] - 10] + result;

                a = quotient;
            }
            if (negative) result = "-" + result;
        }

        return result;
    }


    // ReSharper disable CommentTypo
    //***********************************************************************
    // Returns a hex string showing the contains of the BigInteger
    //
    // Examples
    // -------
    // 1) If the value of BigInteger is 255 in base 10, then
    //    ToHexString() returns "FF"
    //
    // 2) If the value of BigInteger is -255 in base 10, then
    //    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
    //    which is the 2's complement representation of -255.
    //
    //***********************************************************************
    // ReSharper restore CommentTypo

    /// <summary>Представление <see cref="BigInt"/> в шестнадцатеричной системе счисления</summary>
    /// <returns>Строка шестнадцатеричного представления числа <see cref="BigInt"/></returns>
    public string ToHexString()
    {
        var result = _Data[_DataLength - 1].ToString("X");

        for (var i = _DataLength - 2; i >= 0; i--)
            result += _Data[i].ToString("X8");

        return result;
    }
}
