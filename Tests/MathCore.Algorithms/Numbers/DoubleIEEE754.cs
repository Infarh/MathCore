namespace MathCore.Algorithms.Numbers;

public static class DoubleIEEE754
{
    const int __ExpMask = (1 << 11) - 1;
    const long __MantissaMask = ((long)1 << 52) - 1;

    public static (long Mantissa, short Exp2, bool Sign) Parse(double value)
    {
        var bits = BitConverter.DoubleToInt64Bits(value);

        var exp      = (short)(((bits >> 52) & __ExpMask) - 1023);
        var mantissa = bits & __MantissaMask;
        var sign     = (bits >> (52 + 11)) != 0;

        return (mantissa, exp, sign);
    }

    public static double Create(long Mantissa, short Exp2, bool Sign)
    {
        var exp      = ((Exp2 + 1023L) & __ExpMask) << 52;
        var mantissa = Mantissa & __MantissaMask;

        var bits = Sign
            ? exp | mantissa | (1L << (11 + 52))
            : exp | mantissa;

        var result = BitConverter.Int64BitsToDouble(bits);

        return result;
    }
}
