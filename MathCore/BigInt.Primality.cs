// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    /// <summary>Простые числа до 2000</summary>
    public static readonly IReadOnlyList<int> PrimesBelow2000 =
    [
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
        101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193,
        197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307,
        311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421,
        431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547,
        557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
        661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
        809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929,
        937, 941, 947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049,
        1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163,
        1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283,
        1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399, 1409, 1423,
        1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
        1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607, 1609, 1613, 1619,
        1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747,
        1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877,
        1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999
    ];

    //***********************************************************************
    // Probabilistic prime test based on Fermat's little theorem
    //
    // for any a < p (p does not divide a) if
    //      a^(p-1) mod p != 1 then p is not prime.
    //
    // Otherwise, p is probably prime (pseudoprime to the chosen base).
    //
    // Returns
    // -------
    // True if "this" is a pseudoprime to randomly chosen
    // bases.  The number of chosen bases is given by the "confidence"
    // parameter.
    //
    // False if "this" is definitely NOT prime.
    //
    // Note - this method is fast but fails for Carmichael numbers except
    // when the randomly chosen base is a factor of the number.
    //
    //***********************************************************************

    public bool FermatLittleTest(int confidence)
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        if (this_val._DataLength == 1)
        {
            // test small numbers
            if (this_val._Data[0] is 0 or 1)
                return false;
            if (this_val._Data[0] is 2 or 3)
                return true;
        }

        if ((this_val._Data[0] & 0x1) == 0) // even numbers
            return false;

        var bits = this_val.BitCount;
        var a = new BigInt();
        var p_sub1 = this_val - new BigInt(1);
        var rand = new Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // generate a < n
            {
                var test_bits = 0;

                // make sure "a" has at least 2 bits
                while (test_bits < 2)
                    test_bits = (int)(rand.NextDouble() * bits);

                a.GenRandomBits(test_bits, rand);

                var byte_len = a._DataLength;

                // make sure "a" is not 0
                if (byte_len > 1 || (byte_len == 1 && a._Data[0] != 1))
                    done = true;
            }

            // check whether a factor exists (fix for version 1.03)
            var gcd_test = a.Gcd(this_val);
            if (gcd_test._DataLength == 1 && gcd_test._Data[0] != 1)
                return false;

            // calculate a^(p-1) mod p
            var exp_result = a.ModPow(p_sub1, this_val);

            var result_len = exp_result._DataLength;

            // is NOT prime is a^(p-1) mod p != 1

            if (result_len > 1 || (result_len == 1 && exp_result._Data[0] != 1))
                return false;
        }

        return true;
    }


    //***********************************************************************
    // Probabilistic prime test based on Rabin-Miller's
    //
    // for any p > 0 with p - 1 = 2^s * t
    //
    // p is probably prime (strong pseudoprime) if for any a < p,
    // 1) a^t mod p = 1 or
    // 2) a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
    //
    // Otherwise, p is composite.
    //
    // Returns
    // -------
    // True if "this" is a strong pseudoprime to randomly chosen
    // bases.  The number of chosen bases is given by the "confidence"
    // parameter.
    //
    // False if "this" is definitely NOT prime.
    //
    //***********************************************************************

    public bool RabinMillerTest(int confidence)
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        if (this_val._DataLength == 1)
            switch (this_val._Data[0])
            {
                // test small numbers
                case 0:
                case 1:
                    return false;
                case 2:
                case 3:
                    return true;
            }

        if ((this_val._Data[0] & 0x1) == 0) // even numbers
            return false;


        // calculate values of s and t
        var p_sub1 = this_val - new BigInt(1);
        var s = 0;

        for (var index = 0; index < p_sub1._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, s++)
                if ((p_sub1._Data[index] & mask) != 0)
                {
                    index = p_sub1._DataLength; // to break the outer loop
                    break;
                }
        }

        var t = p_sub1 >> s;

        var bits = this_val.BitCount;
        var a = new BigInt();
        var rand = new Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // generate a < n
            {
                var test_bits = 0;

                // make sure "a" has at least 2 bits
                while (test_bits < 2)
                    test_bits = (int)(rand.NextDouble() * bits);

                a.GenRandomBits(test_bits, rand);

                var byte_len = a._DataLength;

                // make sure "a" is not 0
                if (byte_len > 1 || (byte_len == 1 && a._Data[0] != 1))
                    done = true;
            }

            // check whether a factor exists (fix for version 1.03)
            var gcd_test = a.Gcd(this_val);
            if (gcd_test._DataLength == 1 && gcd_test._Data[0] != 1)
                return false;

            var b = a.ModPow(t, this_val);

            var result = b._DataLength == 1 && b._Data[0] == 1; // a^t mod p = 1

            for (var j = 0; !result && j < s; j++, b *= b % this_val)
                if (b == p_sub1) // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                {
                    result = true;
                    break;
                }

            if (!result) return false;
        }
        return true;
    }


    //***********************************************************************
    // Probabilistic prime test based on Solovay-Strassen (Euler Criterion)
    //
    // p is probably prime if for any a < p (a is not multiple of p),
    // a^((p-1)/2) mod p = J(a, p)
    //
    // where J is the Jacobi symbol.
    //
    // Otherwise, p is composite.
    //
    // Returns
    // -------
    // True if "this" is a Euler pseudoprime to randomly chosen
    // bases.  The number of chosen bases is given by the "confidence"
    // parameter.
    //
    // False if "this" is definitely NOT prime.
    //
    //***********************************************************************

    public bool SolovayStrassenTest(int confidence)
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        if (this_val._DataLength == 1)
            switch (this_val._Data[0])
            {
                // test small numbers
                case 0:
                case 1:
                    return false;
                case 2:
                case 3:
                    return true;
            }

        if ((this_val._Data[0] & 0x1) == 0) // even numbers
            return false;


        var bits = this_val.BitCount;
        var a = new BigInt();
        var p_sub1 = this_val - 1;
        var p_sub1_shift = p_sub1 >> 1;

        var rand = new Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // generate a < n
            {
                var test_bits = 0;

                // make sure "a" has at least 2 bits
                while (test_bits < 2)
                    test_bits = (int)(rand.NextDouble() * bits);

                a.GenRandomBits(test_bits, rand);

                var byte_len = a._DataLength;

                // make sure "a" is not 0
                if (byte_len > 1 || (byte_len == 1 && a._Data[0] != 1))
                    done = true;
            }

            // check whether a factor exists (fix for version 1.03)
            var gcd_test = a.Gcd(this_val);
            if (gcd_test._DataLength == 1 && gcd_test._Data[0] != 1)
                return false;

            // calculate a^((p-1)/2) mod p

            var exp_result = a.ModPow(p_sub1_shift, this_val);
            if (exp_result == p_sub1)
                exp_result = -1;

            // calculate Jacobi symbol
            BigInt jacob = Jacobi(a, this_val);

            //Console.WriteLine("a = " + a.ToString(10) + " b = " + thisVal.ToString(10));
            //Console.WriteLine("expResult = " + expResult.ToString(10) + " Jacob = " + jacob.ToString(10));

            // if they are different then it is not prime
            if (exp_result != jacob)
                return false;
        }

        return true;
    }


    // ReSharper disable CommentTypo
    //***********************************************************************
    // Implementation of the Lucas Strong Pseudo Prime test.
    //
    // Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
    // with d odd and s >= 0.
    //
    // If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 <= r < s, then n
    // is a strong Lucas pseudoprime with parameters (P, Q).  We select
    // P and Q based on Selfridge.
    //
    // Returns True if number is a strong Lucus pseudo prime.
    // Otherwise, returns False indicating that number is composite.
    //***********************************************************************
    // ReSharper restore CommentTypo

    public bool LucasStrongTest()
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        if (this_val._DataLength == 1)
            switch (this_val._Data[0]) // test small numbers
            {
                case 0:
                case 1:
                    return false;
                case 2:
                case 3:
                    return true;
            }

        return (this_val._Data[0] & 0x1) != 0 && LucasStrongTestHelper(this_val);
    }


    private static bool LucasStrongTestHelper(BigInt ThisVal)
    {
        // Do the test (selects D based on Self ridge)
        // Let D be the first element of the sequence
        // 5, -7, 9, -11, 13, ... for which J(D,n) = -1
        // Let P = 1, Q = (1-D) / 4

        var d = 5;
        var sign = -1;
        var d_count = 0;
        var done = false;

        while (!done)
        {
            var j_result = Jacobi(d, ThisVal);

            if (j_result == -1)
                done = true; // J(D, this) = 1
            else
            {
                if (j_result == 0 && Math.Abs(d) < ThisVal) // divisor found
                    return false;

                if (d_count == 20)
                {
                    // check for square
                    var root = ThisVal.Sqrt();
                    if (root * root == ThisVal)
                        return false;
                }

                d = (Math.Abs(d) + 2) * sign;
                sign = -sign;
            }
            d_count++;
        }

        var q = (1 - d) >> 2;


        var p_add1 = ThisVal + 1;
        var s = 0;

        for (var index = 0; index < p_add1._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((p_add1._Data[index] & mask) != 0)
                {
                    index = p_add1._DataLength; // to break the outer loop
                    break;
                }
                mask <<= 1;
                s++;
            }
        }

        var t = p_add1 >> s;

        // calculate constant = b^(2k) / m
        // for Barrett Reduction
        var constant = new BigInt();

        var n_len = ThisVal._DataLength << 1;
        constant._Data[n_len] = 0x00000001;
        constant._DataLength = n_len + 1;

        constant /= ThisVal;

        var lucas = LucasSequenceHelper(1, q, t, ThisVal, constant, 0);
        var is_prime = (lucas[0]._DataLength == 1 && lucas[0]._Data[0] == 0) ||
            (lucas[1]._DataLength == 1 && lucas[1]._Data[0] == 0);

        for (var i = 1; i < s; i++)
        {
            if (!is_prime)
            {
                // doubling of index
                lucas[1] = BarrettReduction(lucas[1] * lucas[1], ThisVal, constant);
                lucas[1] = (lucas[1] - (lucas[2] << 1)) % ThisVal;

                if (lucas[1]._DataLength == 1 && lucas[1]._Data[0] == 0)
                    is_prime = true;
            }

            lucas[2] = BarrettReduction(lucas[2] * lucas[2], ThisVal, constant); //Q^k
        }


        if (!is_prime) return false;
        // If n is prime and gcd(n, Q) == 1, then
        // Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

        var g = ThisVal.Gcd(q);
        if (g._DataLength != 1 || g._Data[0] != 1) return true;
        if ((lucas[2]._Data[MaxLength - 1] & 0x80000000) != 0)
            lucas[2] += ThisVal;

        var temp = q * Jacobi(q, ThisVal) % ThisVal;
        if ((temp._Data[MaxLength - 1] & 0x80000000) != 0)
            temp += ThisVal;

        if (lucas[2] != temp)
            is_prime = false;

        return is_prime;
    }


    //***********************************************************************
    // Determines whether a number is probably prime, using the Rabin-Miller's
    // test.  Before applying the test, the number is tested for divisibility
    // by primes < 2000
    //
    // Returns true if number is probably prime.
    //***********************************************************************

    public bool IsProbablePrime(int confidence)
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;


        // test for divisibility by primes < 2000
        return PrimesBelow2000
               .Select(i => new BigInt(i))
               .TakeWhile(divisor => divisor < this_val)
               .All(divisor => (this_val % divisor).IntValue() != 0)
            && this_val.RabinMillerTest(confidence);
    }


    //***********************************************************************
    // Determines whether this BigInteger is probably prime using a
    // combination of base 2 strong pseudoprime test and Lucas strong
    // pseudoprime test.
    //
    // The sequence of the primality test is as follows,
    //
    // 1) Trial divisions are carried out using prime numbers below 2000.
    //    if any of the primes divides this BigInteger, then it is not prime.
    //
    // 2) Perform base 2 strong pseudoprime test.  If this BigInteger is a
    //    base 2 strong pseudoprime, proceed on to the next step.
    //
    // 3) Perform strong Lucas pseudoprime test.
    //
    // Returns True if this BigInteger is both a base 2 strong pseudoprime
    // and a strong Lucas pseudoprime.
    //
    // For a detailed discussion of this primality test, see [6].
    //
    //***********************************************************************

    public bool IsProbablePrime()
    {
        var this_val = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        if (this_val._DataLength == 1)
            switch (this_val._Data[0])
            {
                // test small numbers
                case 0:
                case 1:
                    return false;
                case 2:
                case 3:
                    return true;
            }

        if ((this_val._Data[0] & 0x1) == 0) // even numbers
            return false;


        // test for divisibility by primes < 2000
        if (PrimesBelow2000
           .Select(i => new BigInt(i))
           .TakeWhile(divisor => divisor < this_val)
           .Select(divisor => this_val % divisor)
           .Any(ResultNum => ResultNum.IntValue() == 0))
            return false;

        // Perform BASE 2 Rabin-Miller Test

        // calculate values of s and t
        var p_sub1 = this_val - new BigInt(1);
        var s = 0;

        for (var index = 0; index < p_sub1._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, s++)
                if ((p_sub1._Data[index] & mask) != 0)
                {
                    index = p_sub1._DataLength; // to break the outer loop
                    break;
                }
        }

        var t = p_sub1 >> s;

        //        thisVal.bitCount();
        BigInt a = 2;

        // b = a^t mod p
        var b = a.ModPow(t, this_val);
        var result = b._DataLength == 1 && b._Data[0] == 1; // a^t mod p = 1

        for (var j = 0; !result && j < s; j++, b *= b % this_val)
            if (b == p_sub1) // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
            {
                result = true;
                break;
            }

        // if number is strong pseudoprime to base 2, then do a strong lucas test
        return result && LucasStrongTestHelper(this_val);
    }
}
