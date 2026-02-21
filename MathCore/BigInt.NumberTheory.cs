// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Returns gcd(this, Value)
    //***********************************************************************

    /// <summary>Наибольший общий делитель</summary>
    public BigInt Gcd(BigInt X)
    {
        var x = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        var y = (X._Data[MaxLength - 1] & 0x80000000) != 0 ? -X : X;

        var g = y;

        while (x._DataLength > 1 || (x._DataLength == 1 && x._Data[0] != 0))
        {
            g = x;
            x = y % x;
            y = g;
        }

        return g;
    }


    //***********************************************************************
    // Computes the Jacobi Symbol for a and b.
    // Algorithm adapted from [3] and [4] with some optimizations
    //***********************************************************************

    public static int Jacobi(BigInt a, BigInt b)
    {
        // Jacobi defined only for odd integers
        if ((b._Data[0] & 0x1) == 0)
            throw new ArgumentException("Jacobi defined only for odd integers.");

        if (a >= b) a %= b;
        switch (a._DataLength)
        {
            case 1 when a._Data[0] == 0: return 0; // a == 0
            case 1 when a._Data[0] == 1: return 1; // a == 1
        }

        if (a < 0)
            return ((b - 1)._Data[0] & 0x2) == 0
                ? Jacobi(-a, b)
                : -Jacobi(-a, b);

        var e = 0;
        for (var index = 0; index < a._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, e++)
                if ((a._Data[index] & mask) != 0)
                {
                    index = a._DataLength; // to break the outer loop
                    break;
                }
        }

        var a1 = a >> e;

        var s = 1;
        if ((e & 0x1) != 0 && ((b._Data[0] & 0x7) == 3 || (b._Data[0] & 0x7) == 5))
            s = -1;

        if ((b._Data[0] & 0x3) == 3 && (a1._Data[0] & 0x3) == 3)
            s = -s;

        return a1._DataLength == 1 && a1._Data[0] == 1 ? s : s * Jacobi(b % a1, a1);
    }


    //***********************************************************************
    // Generates a positive BigInteger that is probably prime.
    //***********************************************************************

    public static BigInt GetPseudoPrime(int bits, int confidence, Random rand)
    {
        var result = new BigInt();
        var done = false;

        while (!done)
        {
            result.GenRandomBits(bits, rand);
            result._Data[0] |= 0x01; // make it odd

            // prime test
            done = result.IsProbablePrime(confidence);
        }
        return result;
    }


    //***********************************************************************
    // Generates a random number with the specified number of bits such
    // that gcd(number, this) = 1
    //***********************************************************************

    public BigInt GenCoPrime(int bits, Random rand)
    {
        var done = false;
        var result = new BigInt();

        while (!done)
        {
            result.GenRandomBits(bits, rand);

            // gcd test
            var g = result.Gcd(this);
            if (g._DataLength == 1 && g._Data[0] == 1)
                done = true;
        }

        return result;
    }


    //***********************************************************************
    // Returns the modulo inverse of this.  Throws ArithmeticException if
    // the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
    //***********************************************************************

    public BigInt ModInverse(BigInt modulus)
    {
        BigInt[] p = [0, 1];
        var q = new BigInt[2]; // quotients
        BigInt[] r = [0, 0];      // remainders

        var step = 0;

        var a = modulus;
        var b = this;

        while (b._DataLength > 1 || (b._DataLength == 1 && b._Data[0] != 0))
        {
            var quotient = new BigInt();
            var remainder = new BigInt();

            if (step > 1)
            {
                var p_val = (p[0] - p[1] * q[0]) % modulus;
                p[0] = p[1];
                p[1] = p_val;
            }

            if (b._DataLength == 1)
                SingleByteDivide(a, b, quotient, remainder);
            else
                MultiByteDivide(a, b, quotient, remainder);

            q[0] = q[1];
            r[0] = r[1];
            q[1] = quotient; r[1] = remainder;

            a = b;
            b = remainder;

            step++;
        }

        if (r[0]._DataLength > 1 || (r[0]._DataLength == 1 && r[0]._Data[0] != 1))
            throw new ArithmeticException("No inverse!");

        var result = (p[0] - p[1] * q[0]) % modulus;

        if ((result._Data[MaxLength - 1] & 0x80000000) != 0)
            result += modulus; // get the least positive modulus

        return result;
    }
}
