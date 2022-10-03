namespace MathCore.Algorithms.Numbers;

public static class DoubleIEEE754
{
    private const double log10_2 = 0.3010299956639811952137388947244930267681898814621085413104274611271081892744245094869272521181861720406844771914309953790947678;
    private const int __ExpMask = (1 << 11) - 1;
    private const long __MantissaMask = ((long)1 << 52) - 1;

    public static (long Mantissa, short Exp2, bool Sign) Parse(double value)
    {
        var bits = BitConverter.DoubleToInt64Bits(value);

        var exp = (short)(((bits >> 52) & __ExpMask) - 1023);
        var mantissa = bits & __MantissaMask;
        var sign = (bits >> (52 + 11)) != 0;

        return (mantissa, exp, sign);
    }

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
}