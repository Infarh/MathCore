//************************************************************************************
// BigInteger Class Version 1.03
//
// Copyright (c) 2002 Chew Keong TAN
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, provided that the above
// copyright notice(s) and this permission notice appear in all copies of
// the Software and that both the above copyright notice(s) and this
// permission notice appear in supporting documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
//
// Disclaimer
// ----------
// Although reasonable care has been taken to ensure the correctness of this
// implementation, this code should never be used in any application without
// proper verification and testing.  I disclaim all liability and responsibility
// to any person or entity with respect to any loss or damage caused, or alleged
// to be caused, directly or indirectly, by the use of this BigInteger class.
//
// Comments, bugs and suggestions to
// (http://www.codeproject.com/csharp/biginteger.asp)
//
//
// Overloaded Operators +, -, *, /, %, >>, <<, ==, !=, >, <, >=, <=, &, |, ^, ++, --, ~
//
// Features
// --------
// 1) Arithmetic operations involving large signed integers (2's complement).
// 2) Primality test using Fermat little theorem, Rabin Miller's method,
//    Solovay Strassen's method and Lucas strong pseudoprime.
// 3) Modulo exponential with Barrett's reduction.
// 4) Inverse modulo.
// 5) Pseudo prime generation.
// 6) Co-prime generation.
//
//
// Known Problem
// -------------
// This pseudoprime passes my implementation of
// primality test but failed in SDK IsProbablePrime test.
//
//       byte[] pseudoPrime1 = { (byte)0x00,
//             (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
//             (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
//             (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
//             (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
//             (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
//             (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
//             (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
//             (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
//             (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
//             (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
//             (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
//       };
//
//
// Change Log
// ----------
// 1) September 23, 2002 (Version 1.03)
//    - Fixed operator- to give correct data length.
//    - Added Lucas sequence generation.
//    - Added Strong Lucas Primality test.
//    - Added integer square root method.
//    - Added setBit/unsetBit methods.
//    - New IsProbablePrime() method which do not require the
//      confident parameter.
//
// 2) August 29, 2002 (Version 1.02)
//    - Fixed bug in the exponentiation of negative numbers.
//    - Faster modular exponentiation using Barrett reduction.
//    - Added getBytes() method.
//    - Fixed bug in ToHexString method.
//    - Added overloading of ^ operator.
//    - Faster computation of Jacobi symbol.
//
// 3) August 19, 2002 (Version 1.01)
//    - Big integer is stored and manipulated as unsigned integers (4 bytes) instead of
//      individual bytes this gives significant performance improvement.
//    - Updated Fermat's Little Theorem test to use a^(p-1) mod p = 1
//    - Added IsProbablePrime method.
//    - Updated documentation.
//
// 4) August 9, 2002 (Version 1.0)
//    - Initial Release.
//
//
// ReSharper disable CommentTypo
// References
// [1] D. E. Knuth, "Seminumerical Algorithms", The Art of Computer Programming Vol. 2,
//     3rd Edition, Addison-Wesley, 1998.
//
// [2] K. H. Rosen, "Elementary Number Theory and Its Applications", 3rd Ed,
//     Addison-Wesley, 1993.
//
// [3] B. Schneier, "Applied Cryptography", 2nd Ed, John Wiley & Sons, 1996.
//
// [4] A. Menezes, P. van Oorschot, and S. Vanstone, "Handbook of Applied Cryptography",
//     CRC Press, 1996, www.cacr.math.uwaterloo.ca/hac
//
// [5] A. Bosselaers, R. Govaerts, and J. Vandewalle, "Comparison of Three Modular
//     Reduction Functions," Proc. CRYPTO'93, pp.175-186.
//
// [6] R. Baillie and S. S. Wagstaff Jr, "Lucas Pseudoprimes", Mathematics of Computation,
//     Vol. 35, No. 152, Oct 1980, pp. 1391-1417.
//
// [7] H. C. Williams, "�douard Lucas and Primality Testing", Canadian Mathematical
//     Society Series of Monographs and Advance Texts, vol. 22, John Wiley & Sons, New York,
//     NY, 1998.
//
// [8] P. Ribenboim, "The new book of prime number records", 3rd edition, Springer-Verlag,
//     New York, NY, 1995.
//
// [9] M. Joye and J.-J. Quisquater, "Efficient computation of full Lucas sequences",
//     Electronics Letters, 32(6), 1996, pp 537-538.
// ReSharper restore CommentTypo
//
//************************************************************************************

using System;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore
{
    /// <summary> Целочисленная арифметика с большими числами  </summary>
    public class BigInteger
    {
        // maximum length of the BigInteger in uint (4 bytes)
        // change this to suit the required level of precision.
        /// <summary>Максимальная длина числа в байтах</summary>
        private const int __MaxLength = 70;

        // primes smaller than 2000 to test the generated prime number
        /// <summary>Простые числа до 2000</summary>
        public static readonly int[] PrimesBelow2000 =
        {
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
        };


        private readonly uint[] _Data;             // stores bytes from the Big Integer
        private int _DataLength;                 // number of actual chars used

        public int DataLength => _DataLength;

        //***********************************************************************
        // Constructor (Default value for BigInteger is 0
        //***********************************************************************

        public BigInteger()
        {
            _Data = new uint[__MaxLength];
            _DataLength = 1;
        }


        //***********************************************************************
        // Constructor (Default value provided by long)
        //***********************************************************************

        public BigInteger(long Value)
        {
            _Data = new uint[__MaxLength];
            var temp_val = Value;

            // copy bytes from long to BigInteger without any assumption of
            // the length of the long datatype

            _DataLength = 0;
            while (Value != 0 && _DataLength < __MaxLength)
            {
                _Data[_DataLength] = (uint)(Value & 0xFFFFFFFF);
                Value >>= 32;
                _DataLength++;
            }

            if (temp_val > 0)         // overflow check for +ve value
            {
                if (Value != 0 || (_Data[__MaxLength - 1] & 0x80000000) != 0)
                    throw new ArithmeticException("Positive overflow in constructor.");
            }
            else if (temp_val < 0) // underflow check for -ve value
                if (Value != -1 || (_Data[_DataLength - 1] & 0x80000000) == 0)
                    throw new ArithmeticException("Negative underflow in constructor.");

            if (_DataLength == 0) _DataLength = 1;
        }


        //***********************************************************************
        // Constructor (Default value provided by ulong)
        //***********************************************************************

        public BigInteger(ulong Value)
        {
            _Data = new uint[__MaxLength];

            // copy bytes from ulong to BigInteger without any assumption of
            // the length of the ulong datatype

            _DataLength = 0;
            while (Value != 0 && _DataLength < __MaxLength)
            {
                _Data[_DataLength] = (uint)(Value & 0xFFFFFFFF);
                Value >>= 32;
                _DataLength++;
            }

            if (Value != 0 || (_Data[__MaxLength - 1] & 0x80000000) != 0)
                throw new ArithmeticException("Positive overflow in constructor.");

            if (_DataLength == 0) _DataLength = 1;
        }



        //***********************************************************************
        // Constructor (Default value provided by BigInteger)
        //***********************************************************************

        public BigInteger([NotNull] BigInteger Value)
        {
            _Data = new uint[__MaxLength];

            _DataLength = Value._DataLength;

            for (var i = 0; i < _DataLength; i++) _Data[i] = Value._Data[i];
        }


        //***********************************************************************
        // Constructor (Default value provided by a string of digits of the
        //              specified base)
        //
        // Example (base 10)
        // -----------------
        // To initialize "a" with the default value of 1234 in base 10
        //      BigInteger a = new BigInteger("1234", 10)
        //
        // To initialize "a" with the default value of -1234
        //      BigInteger a = new BigInteger("-1234", 10)
        //
        // Example (base 16)
        // -----------------
        // To initialize "a" with the default value of 0x1D4F in base 16
        //      BigInteger a = new BigInteger("1D4F", 16)
        //
        // To initialize "a" with the default value of -0x1D4F
        //      BigInteger a = new BigInteger("-1D4F", 16)
        //
        // Note that string values are specified in the <sign><magnitude>
        // format.
        //
        //***********************************************************************

        public BigInteger(string value, int radix)
        {
            var multiplier = new BigInteger(1);
            var result = new BigInteger();
            value = value.ToUpper().Trim();
            var limit = 0;

            if (value[0] == '-') limit = 1;

            for (var i = value.Length - 1; i >= limit; i--)
            {
                var pos_val = (int)value[i];

                if (pos_val >= '0' && pos_val <= '9') pos_val -= '0';
                else pos_val = pos_val >= 'A' && pos_val <= 'Z' ? pos_val - 'A' + 10 : 9999999;       // arbitrary large


                if (pos_val >= radix)
                    throw new ArithmeticException("Invalid string in constructor.");
                if (value[0] == '-') pos_val = -pos_val;

                result += multiplier * pos_val;

                if (i - 1 >= limit) multiplier *= radix;
            }

            if (value[0] == '-')     // negative values
            {
                if ((result._Data[__MaxLength - 1] & 0x80000000) == 0)
                    throw new ArithmeticException("Negative underflow in constructor.");
            }
            else    // positive values
            {
                if ((result._Data[__MaxLength - 1] & 0x80000000) != 0)
                    throw new ArithmeticException("Positive overflow in constructor.");
            }

            _Data = new uint[__MaxLength];
            for (var i = 0; i < result._DataLength; i++)
                _Data[i] = result._Data[i];

            _DataLength = result._DataLength;
        }


        //***********************************************************************
        // Constructor (Default value provided by an array of bytes)
        //
        // The lowest index of the input byte array (i.e [0]) should contain the
        // most significant byte of the number, and the highest index should
        // contain the least significant byte.
        //
        // E.g.
        // To initialize "a" with the default value of 0x1D4F in base 16
        //      byte[] temp = { 0x1D, 0x4F };
        //      BigInteger a = new BigInteger(temp)
        //
        // Note that this method of initialization does not allow the
        // sign to be specified.
        //
        //***********************************************************************

        public BigInteger([NotNull] byte[] Data)
        {
            _DataLength = Data.Length >> 2;

            var left_over = Data.Length & 0x3;
            if (left_over != 0)         // length not multiples of 4
                _DataLength++;


            if (_DataLength > __MaxLength)
                throw new ArithmeticException("Byte overflow in constructor.");

            _Data = new uint[__MaxLength];

            for (int i = Data.Length - 1, j = 0; i >= 3; i -= 4, j++)
                _Data[j] = (uint)((Data[i - 3] << 24) + (Data[i - 2] << 16) +
                                 (Data[i - 1] << 8) + Data[i]);

            _Data[_DataLength - 1] = left_over switch
            {
                1 => Data[0],
                2 => (uint)((Data[0] << 8) + Data[1]),
                3 => (uint)((Data[0] << 16) + (Data[1] << 8) + Data[2]),
                _ => _Data[_DataLength - 1]
            };

            while (_DataLength > 1 && _Data[_DataLength - 1] == 0) _DataLength--;
        }


        //***********************************************************************
        // Constructor (Default value provided by an array of bytes of the
        // specified length.)
        //***********************************************************************

        public BigInteger([NotNull] byte[] Data, int inLen)
        {
            _DataLength = inLen >> 2;

            var left_over = inLen & 0x3;
            if (left_over != 0) _DataLength++;        // length not multiples of 4


            if (_DataLength > __MaxLength || inLen > Data.Length)
                throw new ArithmeticException("Byte overflow in constructor.");


            _Data = new uint[__MaxLength];

            for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
                _Data[j] = (uint)((Data[i - 3] << 24) + (Data[i - 2] << 16) +
                                 (Data[i - 1] << 8) + Data[i]);

            _Data[_DataLength - 1] = left_over switch
            {
                1 => Data[0],
                2 => (uint)((Data[0] << 8) + Data[1]),
                3 => (uint)((Data[0] << 16) + (Data[1] << 8) + Data[2]),
                _ => _Data[_DataLength - 1]
            };

            if (_DataLength == 0)
                _DataLength = 1;

            while (_DataLength > 1 && _Data[_DataLength - 1] == 0)
                _DataLength--;

            //Console.WriteLine("Len = " + _DataLength);
        }


        //***********************************************************************
        // Constructor (Default value provided by an array of unsigned integers)
        //*********************************************************************

        public BigInteger([NotNull] uint[] inData)
        {
            _DataLength = inData.Length;

            if (_DataLength > __MaxLength)
                throw new ArithmeticException("Byte overflow in constructor.");

            _Data = new uint[__MaxLength];

            for (int i = _DataLength - 1, j = 0; i >= 0; i--, j++)
                _Data[j] = inData[i];

            while (_DataLength > 1 && _Data[_DataLength - 1] == 0)
                _DataLength--;

            //Console.WriteLine("Len = " + _DataLength);
        }


        //***********************************************************************
        // Overloading of the typecast operator.
        // For BigInteger Value = 10;
        //***********************************************************************

        [NotNull]
        public static implicit operator BigInteger(long value) => new BigInteger(value);

        [NotNull]
        public static implicit operator BigInteger(ulong value) => new BigInteger(value);

        [NotNull]
        public static implicit operator BigInteger(int value) => new BigInteger(value);

        [NotNull]
        public static implicit operator BigInteger(uint value) => new BigInteger((ulong)value);


        //***********************************************************************
        // Overloading of addition operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator +([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var result = new BigInteger
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

            if (carry != 0 && result._DataLength < __MaxLength)
            {
                result._Data[result._DataLength] = (uint)carry;
                result._DataLength++;
            }

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;


            // overflow check
            const int lc_LastPos = __MaxLength - 1;
            if ((x._Data[lc_LastPos] & 0x80000000) == (y._Data[lc_LastPos] & 0x80000000) &&
               (result._Data[lc_LastPos] & 0x80000000) != (x._Data[lc_LastPos] & 0x80000000))
                throw new ArithmeticException();

            return result;
        }


        //***********************************************************************
        // Overloading of the unary ++ operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator ++([NotNull] BigInteger x)
        {
            var result = new BigInteger(x);

            long carry = 1;
            var index = 0;

            while (carry != 0 && index < __MaxLength)
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
            const int lc_LastPos = __MaxLength - 1;

            // overflow if initial value was +ve but ++ caused a sign
            // change to negative.

            if ((x._Data[lc_LastPos] & 0x80000000) == 0 &&
               (result._Data[lc_LastPos] & 0x80000000) != (x._Data[lc_LastPos] & 0x80000000))
                throw new ArithmeticException("Overflow in ++.");
            return result;
        }


        //***********************************************************************
        // Overloading of subtraction operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator -([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var result = new BigInteger
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
                for (var i = result._DataLength; i < __MaxLength; i++)
                    result._Data[i] = 0xFFFFFFFF;
                result._DataLength = __MaxLength;
            }

            // fixed in v1.03 to give correct datalength for a - (-b)
            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            // overflow check

            const int lc_LastPos = __MaxLength - 1;
            if ((x._Data[lc_LastPos] & 0x80000000) != (y._Data[lc_LastPos] & 0x80000000) &&
               (result._Data[lc_LastPos] & 0x80000000) != (x._Data[lc_LastPos] & 0x80000000))
                throw new ArithmeticException();

            return result;
        }


        //***********************************************************************
        // Overloading of the unary -- operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator --([NotNull] BigInteger x)
        {
            var result = new BigInteger(x);

            var carry_in = true;
            var index = 0;

            while (carry_in && index < __MaxLength)
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
            const int last_pos = __MaxLength - 1;

            // overflow if initial value was -ve but -- caused a sign
            // change to positive.

            if ((x._Data[last_pos] & 0x80000000) != 0 &&
               (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
                throw new ArithmeticException("Underflow in --.");

            return result;
        }


        //***********************************************************************
        // Overloading of multiplication operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator *(BigInteger x, BigInteger y)
        {
            const int last_pos = __MaxLength - 1;
            var x_neg = false;
            var y_neg = false;

            // take the absolute value of the inputs
            try
            {
                if ((x._Data[last_pos] & 0x80000000) != 0)     // x negative
                {
                    x_neg = true;
                    x = -x;
                }
                if ((y._Data[last_pos] & 0x80000000) != 0)     // y negative
                {
                    y_neg = true;
                    y = -y;
                }
            }
            catch
            {
                // ignored
            }

            var result = new BigInteger();

            // multiply the absolute values
            try
            {
                for (var i = 0; i < x._DataLength; i++)
                {
                    if (x._Data[i] == 0) continue;

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < y._DataLength; j++, k++)
                    {
                        // k = i + j
                        var val = x._Data[i] * (ulong)y._Data[j] + result._Data[k] + mcarry;

                        result._Data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = val >> 32;
                    }

                    if (mcarry != 0) result._Data[i + y._DataLength] = (uint)mcarry;
                }
            }
            catch (Exception)
            {
                throw new ArithmeticException("Multipycation overflow.");
            }


            result._DataLength = x._DataLength + y._DataLength;
            if (result._DataLength > __MaxLength) result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            // overflow check (result is -ve)
            if ((result._Data[last_pos] & 0x80000000) == 0)
                // if input has different signs, then result is -ve
                return x_neg != y_neg ? -result : result;

            if (x_neg == y_neg || result._Data[last_pos] != 0x80000000)
                throw new ArithmeticException("Multipycation overflow.");
            // handle the special case where multiplication produces
            // a max negative number in 2's complement.

            if (result._DataLength == 1) return result;
            var is_max_neg = true;
            for (var i = 0; i < result._DataLength - 1 && is_max_neg; i++)
                if (result._Data[i] != 0)
                    is_max_neg = false;

            if (is_max_neg) return result;

            throw new ArithmeticException("Multipycation overflow.");
        }



        //***********************************************************************
        // Overloading of unary << operators
        //***********************************************************************

        [NotNull]
        public static BigInteger operator <<([NotNull] BigInteger x, int ShiftVal)
        {
            var result = new BigInteger(x);
            result._DataLength = ShiftLeft(result._Data, ShiftVal);
            return result;
        }


        // least significant bits at lower part of buffer

        private static int ShiftLeft([NotNull] uint[] buffer, int ShiftVal)
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

        [NotNull]
        public static BigInteger operator >>([NotNull] BigInteger x, int ShiftVal)
        {
            var result = new BigInteger(x);
            result._DataLength = ShiftRight(result._Data, ShiftVal);

            if ((x._Data[__MaxLength - 1] & 0x80000000) == 0) return result;
            for (var i = __MaxLength - 1; i >= result._DataLength; i--)
                result._Data[i] = 0xFFFFFFFF;

            var mask = 0x80000000;
            for (var i = 0; i < 32; i++)
            {
                if ((result._Data[result._DataLength - 1] & mask) != 0) break;

                result._Data[result._DataLength - 1] |= mask;
                mask >>= 1;
            }
            result._DataLength = __MaxLength;

            return result;
        }


        private static int ShiftRight([NotNull] uint[] buffer, int ShiftVal)
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

        [NotNull]
        public static BigInteger operator ~([NotNull] BigInteger x)
        {
            var result = new BigInteger(x);

            for (var i = 0; i < __MaxLength; i++)
                result._Data[i] = ~x._Data[i];

            result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of the NEGATE operator (2's complement)
        //***********************************************************************

        [NotNull]
        public static BigInteger operator -([NotNull] BigInteger x)
        {
            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.

            if (x._DataLength == 1 && x._Data[0] == 0)
                return new BigInteger();

            var result = new BigInteger(x);

            // 1's complement
            for (var i = 0; i < __MaxLength; i++)
                result._Data[i] = ~x._Data[i];

            // add one to result of 1's complement
            long carry = 1;
            var index = 0;

            while (carry != 0 && index < __MaxLength)
            {
                var val = (long)result._Data[index];
                val++;

                result._Data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((x._Data[__MaxLength - 1] & 0x80000000) == (result._Data[__MaxLength - 1] & 0x80000000))
                throw new ArithmeticException("Overflow in negation.\n");

            result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;
            return result;
        }


        //***********************************************************************
        // Overloading of equality operator
        //***********************************************************************

        public static bool operator ==([CanBeNull] BigInteger x, [CanBeNull] BigInteger y) => Equals(x, null) && Equals(y, null) || !Equals(x, null) && x.Equals(y);


        public static bool operator !=([CanBeNull] BigInteger x, [CanBeNull] BigInteger y) => !(x == y);


        public override bool Equals(object o) => Equals(o as BigInteger);

        public bool Equals([CanBeNull] BigInteger x)
        {
            if (_DataLength != x?._DataLength) return false;

            for (var i = 0; i < _DataLength; i++)
                if (_Data[i] != x._Data[i])
                    return false;
            return true;
        }


        public override int GetHashCode() => _Data.GetComplexHashCode();//        return ToString().GetHashCode();


        //***********************************************************************
        // Overloading of inequality operator
        //***********************************************************************
        public static bool operator >([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var pos = __MaxLength - 1;

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


        public static bool operator <([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var pos = __MaxLength - 1;

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


        public static bool operator >=(BigInteger x, BigInteger y) => x == y || x > y;


        public static bool operator <=(BigInteger x, BigInteger y) => x == y || x < y;


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has more than 1 digit.
        //
        // Algorithm taken from [1]
        //***********************************************************************

        private static void MultiByteDivide(
            [NotNull] BigInteger X,
            [NotNull] BigInteger Y,
            [NotNull] BigInteger OutQuotient,
            [NotNull] BigInteger OutRemainder)
        {
            var result = new uint[__MaxLength];

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

                var kk = new BigInteger(dividend_part);
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
            for (; y < __MaxLength; y++) OutQuotient._Data[y] = 0;

            while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
                OutQuotient._DataLength--;

            if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

            OutRemainder._DataLength = ShiftRight(remainder, shift);

            for (y = 0; y < OutRemainder._DataLength; y++) OutRemainder._Data[y] = remainder[y];
            for (; y < __MaxLength; y++) OutRemainder._Data[y] = 0;
        }


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has only 1 digit.
        //***********************************************************************

        private static void SingleByteDivide(
            [NotNull] BigInteger x,
            [NotNull] BigInteger y,
            [NotNull] BigInteger OutQuotient,
            [NotNull] BigInteger OutRemainder)
        {
            var result = new uint[__MaxLength];
            var result_pos = 0;

            // copy dividend to reminder
            for (var i = 0; i < __MaxLength; i++)
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
            for (; j < __MaxLength; j++)
                OutQuotient._Data[j] = 0;

            while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
                OutQuotient._DataLength--;

            if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

            while (OutRemainder._DataLength > 1 && OutRemainder._Data[OutRemainder._DataLength - 1] == 0)
                OutRemainder._DataLength--;
        }


        //***********************************************************************
        // Overloading of division operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator /(BigInteger x, BigInteger y)
        {
            var quotient = new BigInteger();
            var remainder = new BigInteger();

            const int lc_LastPos = __MaxLength - 1;
            bool divisor_neg = false, dividend_neg = false;

            if ((x._Data[lc_LastPos] & 0x80000000) != 0)     // x negative
            {
                x = -x;
                dividend_neg = true;
            }
            if ((y._Data[lc_LastPos] & 0x80000000) != 0)     // y negative
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

        [NotNull]
        public static BigInteger operator %(BigInteger x, BigInteger y)
        {
            var quotient = new BigInteger();
            var remainder = new BigInteger(x);

            const int last_pos = __MaxLength - 1;
            var dividend_neg = false;

            if ((x._Data[last_pos] & 0x80000000) != 0)     // x negative
            {
                x = -x;
                dividend_neg = true;
            }
            if ((y._Data[last_pos] & 0x80000000) != 0)     // y negative
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

        [NotNull]
        public static BigInteger operator &([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var result = new BigInteger();

            var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

            for (var i = 0; i < len; i++)
                result._Data[i] = x._Data[i] & y._Data[i];

            result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of bitwise OR operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator |([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var result = new BigInteger();

            var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

            for (var i = 0; i < len; i++)
                result._Data[i] = x._Data[i] | y._Data[i];

            result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of bitwise XOR operator
        //***********************************************************************

        [NotNull]
        public static BigInteger operator ^([NotNull] BigInteger x, [NotNull] BigInteger y)
        {
            var result = new BigInteger();

            var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

            for (var i = 0; i < len; i++)
            {
                var sum = x._Data[i] ^ y._Data[i];
                result._Data[i] = sum;
            }

            result._DataLength = __MaxLength;

            while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
                result._DataLength--;

            return result;
        }


        //***********************************************************************
        // Returns max(this, Value)
        //***********************************************************************

        [NotNull]
        public BigInteger Max(BigInteger x) => this > x ? new BigInteger(this) : new BigInteger(x);


        //***********************************************************************
        // Returns min(this, Value)
        //***********************************************************************

        [NotNull]
        public BigInteger Min(BigInteger x) => this < x ? new BigInteger(this) : new BigInteger(x);


        //***********************************************************************
        // Returns the absolute value
        //***********************************************************************

        [NotNull]
        public BigInteger Abs() => (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : new BigInteger(this);


        //***********************************************************************
        // Returns a string representing the BigInteger in base 10.
        //***********************************************************************

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

        [NotNull]
        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw new ArgumentException("Radix must be >= 2 and <= 36");

            // ReSharper disable once StringLiteralTypo
            const string char_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = string.Empty;

            var a = this;

            var negative = false;
            if ((a._Data[__MaxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try { a = -a; }
                catch
                {
                    // ignored
                }
            }

            var quotient = new BigInteger();
            var remainder = new BigInteger();
            var x_radix = new BigInteger(radix);

            if (a._DataLength == 1 && a._Data[0] == 0)
                result = "0";
            else
            {
                while (a._DataLength > 1 || a._DataLength == 1 && a._Data[0] != 0)
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

        public string ToHexString()
        {
            var result = _Data[_DataLength - 1].ToString("X");

            for (var i = _DataLength - 2; i >= 0; i--)
                result += _Data[i].ToString("X8");

            return result;
        }



        //***********************************************************************
        // Modulo Exponentiation
        //***********************************************************************

        public BigInteger ModPow([NotNull] BigInteger exp, BigInteger n)
        {
            if ((exp._Data[__MaxLength - 1] & 0x80000000) != 0)
                throw new ArithmeticException("Positive exponents only.");

            BigInteger result_num = 1;
            BigInteger temp_num;
            var this_negative = false;

            if ((_Data[__MaxLength - 1] & 0x80000000) != 0)   // negative this
            {
                temp_num = -this % n;
                this_negative = true;
            }
            else
                temp_num = this % n;  // ensures (tempNum * tempNum) < b^(2k)

            if ((n._Data[__MaxLength - 1] & 0x80000000) != 0)   // negative n
                n = -n;

            // calculate constant = b^(2k) / m
            var constant = new BigInteger();

            var i = n._DataLength << 1;
            constant._Data[i] = 0x00000001;
            constant._DataLength = i + 1;

            constant /= n;
            var total_bits = exp.BitCount();
            var count = 0;

            // perform squaring and multiply exponentiation
            for (var pos = 0; pos < exp._DataLength; pos++)
            {
                uint mask = 0x01;
                //Console.WriteLine("pos = " + pos);

                for (var index = 0; index < 32; index++)
                {
                    if ((exp._Data[pos] & mask) != 0)
                        result_num = BarrettReduction(result_num * temp_num, n, constant);

                    mask <<= 1;

                    temp_num = BarrettReduction(temp_num * temp_num, n, constant);


                    if (temp_num._DataLength == 1 && temp_num._Data[0] == 1)
                        return this_negative && (exp._Data[0] & 0x1) != 0 ? -result_num : result_num;
                    count++;
                    if (count == total_bits)
                        break;
                }
            }

            return this_negative && (exp._Data[0] & 0x1) != 0 ? -result_num : result_num;
        }



        //***********************************************************************
        // Fast calculation of modular reduction using Barrett's reduction.
        // Requires x < b^(2k), where b is the base.  In this case, base is
        // 2^32 (uint).
        //
        // Reference [4]
        //***********************************************************************

        [NotNull]
        private BigInteger BarrettReduction([NotNull] BigInteger x, [NotNull] BigInteger n, BigInteger constant)
        {
            var k = n._DataLength;
            var k_plus_one = k + 1;
            var k_minus_one = k - 1;

            var q1 = new BigInteger();

            // q1 = x / b^(k-1)
            for (int i = k_minus_one, j = 0; i < x._DataLength; i++, j++)
                q1._Data[j] = x._Data[i];
            q1._DataLength = x._DataLength - k_minus_one;
            if (q1._DataLength <= 0)
                q1._DataLength = 1;


            var q2 = q1 * constant;
            var q3 = new BigInteger();

            // q3 = q2 / b^(k+1)
            for (int i = k_plus_one, j = 0; i < q2._DataLength; i++, j++)
                q3._Data[j] = q2._Data[i];
            q3._DataLength = q2._DataLength - k_plus_one;
            if (q3._DataLength <= 0)
                q3._DataLength = 1;


            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            var r1 = new BigInteger();
            var length_to_copy = x._DataLength > k_plus_one ? k_plus_one : x._DataLength;
            for (var i = 0; i < length_to_copy; i++)
                r1._Data[i] = x._Data[i];
            r1._DataLength = length_to_copy;


            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n

            var r2 = new BigInteger();
            for (var i = 0; i < q3._DataLength; i++)
            {
                if (q3._Data[i] == 0) continue;

                ulong mcarry = 0;
                var t = i;
                for (var j = 0; j < n._DataLength && t < k_plus_one; j++, t++)
                {
                    // t = i + j
                    var val = q3._Data[i] * (ulong)n._Data[j] + r2._Data[t] + mcarry;

                    r2._Data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = val >> 32;
                }

                if (t < k_plus_one)
                    r2._Data[t] = (uint)mcarry;
            }
            r2._DataLength = k_plus_one;
            while (r2._DataLength > 1 && r2._Data[r2._DataLength - 1] == 0)
                r2._DataLength--;

            r1 -= r2;
            if ((r1._Data[__MaxLength - 1] & 0x80000000) != 0)        // negative
            {
                var val = new BigInteger();
                val._Data[k_plus_one] = 0x00000001;
                val._DataLength = k_plus_one + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }


        //***********************************************************************
        // Returns gcd(this, Value)
        //***********************************************************************

        [NotNull]
        public BigInteger Gcd([NotNull] BigInteger X)
        {
            var x = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

            var y = (X._Data[__MaxLength - 1] & 0x80000000) != 0 ? -X : X;

            var g = y;

            while (x._DataLength > 1 || x._DataLength == 1 && x._Data[0] != 0)
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }


        //***********************************************************************
        // Populates "this" with the specified amount of random bits
        //***********************************************************************

        public void GenRandomBits(int bits, Random rand)
        {
            var dwords = bits >> 5;
            var rem_bits = bits & 0x1F;

            if (rem_bits != 0)
                dwords++;

            if (dwords > __MaxLength)
                throw new ArithmeticException("Number of required bits > maxLength.");

            for (var i = 0; i < dwords; i++)
                _Data[i] = (uint)(rand.NextDouble() * 0x100000000);

            for (var i = dwords; i < __MaxLength; i++)
                _Data[i] = 0;

            if (rem_bits != 0)
            {
                var mask = (uint)(0x01 << (rem_bits - 1));
                _Data[dwords - 1] |= mask;

                mask = 0xFFFFFFFF >> (32 - rem_bits);
                _Data[dwords - 1] &= mask;
            }
            else
                _Data[dwords - 1] |= 0x80000000;

            _DataLength = dwords;

            if (_DataLength == 0)
                _DataLength = 1;
        }


        //***********************************************************************
        // Returns the position of the most significant bit in the BigInteger.
        //
        // Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
        //      The result is 1, if the value of BigInteger is 0...0000 0001
        //      The result is 2, if the value of BigInteger is 0...0000 0010
        //      The result is 2, if the value of BigInteger is 0...0000 0011
        //
        //***********************************************************************

        public int BitCount()
        {
            while (_DataLength > 1 && _Data[_DataLength - 1] == 0)
                _DataLength--;

            var value = _Data[_DataLength - 1];
            var mask = 0x80000000;
            var bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }
            bits += (_DataLength - 1) << 5;

            return bits;
        }


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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

            if (this_val._DataLength == 1)
            {
                // test small numbers
                if (this_val._Data[0] == 0 || this_val._Data[0] == 1)
                    return false;
                if (this_val._Data[0] == 2 || this_val._Data[0] == 3)
                    return true;
            }

            if ((this_val._Data[0] & 0x1) == 0)     // even numbers
                return false;

            var bits = this_val.BitCount();
            var a = new BigInteger();
            var p_sub1 = this_val - new BigInteger(1);
            var rand = new Random();

            for (var round = 0; round < confidence; round++)
            {
                var done = false;

                while (!done)        // generate a < n
                {
                    var test_bits = 0;

                    // make sure "a" has at least 2 bits
                    while (test_bits < 2)
                        test_bits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(test_bits, rand);

                    var byte_len = a._DataLength;

                    // make sure "a" is not 0
                    if (byte_len > 1 || byte_len == 1 && a._Data[0] != 1)
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

                if (result_len > 1 || result_len == 1 && exp_result._Data[0] != 1)
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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

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

            if ((this_val._Data[0] & 0x1) == 0)     // even numbers
                return false;


            // calculate values of s and t
            var p_sub1 = this_val - new BigInteger(1);
            var s = 0;

            for (var index = 0; index < p_sub1._DataLength; index++)
            {
                uint mask = 0x01;

                for (var i = 0; i < 32; i++, mask <<= 1, s++)
                    if ((p_sub1._Data[index] & mask) != 0)
                    {
                        index = p_sub1._DataLength;      // to break the outer loop
                        break;
                    }
            }

            var t = p_sub1 >> s;

            var bits = this_val.BitCount();
            var a = new BigInteger();
            var rand = new Random();

            for (var round = 0; round < confidence; round++)
            {
                var done = false;

                while (!done)        // generate a < n
                {
                    var test_bits = 0;

                    // make sure "a" has at least 2 bits
                    while (test_bits < 2)
                        test_bits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(test_bits, rand);

                    var byte_len = a._DataLength;

                    // make sure "a" is not 0
                    if (byte_len > 1 || byte_len == 1 && a._Data[0] != 1)
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                var gcd_test = a.Gcd(this_val);
                if (gcd_test._DataLength == 1 && gcd_test._Data[0] != 1)
                    return false;

                var b = a.ModPow(t, this_val);

                var result = b._DataLength == 1 && b._Data[0] == 1; // a^t mod p = 1

                for (var j = 0; !result && j < s; j++, b *= b % this_val)
                    if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

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

            if ((this_val._Data[0] & 0x1) == 0)     // even numbers
                return false;


            var bits = this_val.BitCount();
            var a = new BigInteger();
            var p_sub1 = this_val - 1;
            var p_sub1_shift = p_sub1 >> 1;

            var rand = new Random();

            for (var round = 0; round < confidence; round++)
            {
                var done = false;

                while (!done)        // generate a < n
                {
                    var test_bits = 0;

                    // make sure "a" has at least 2 bits
                    while (test_bits < 2)
                        test_bits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(test_bits, rand);

                    var byte_len = a._DataLength;

                    // make sure "a" is not 0
                    if (byte_len > 1 || byte_len == 1 && a._Data[0] != 1)
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
                BigInteger jacob = Jacobi(a, this_val);

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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

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


        private static bool LucasStrongTestHelper([NotNull] BigInteger thisVal)
        {
            // Do the test (selects D based on Selfridge)
            // Let D be the first element of the sequence
            // 5, -7, 9, -11, 13, ... for which J(D,n) = -1
            // Let P = 1, Q = (1-D) / 4

            var D = 5;
            var sign = -1;
            var d_count = 0;
            var done = false;

            while (!done)
            {
                var j_result = Jacobi(D, thisVal);

                if (j_result == -1)
                    done = true;    // J(D, this) = 1
                else
                {
                    if (j_result == 0 && Math.Abs(D) < thisVal)       // divisor found
                        return false;

                    if (d_count == 20)
                    {
                        // check for square
                        var root = thisVal.Sqrt();
                        if (root * root == thisVal)
                            return false;
                    }

                    D = (Math.Abs(D) + 2) * sign;
                    sign = -sign;
                }
                d_count++;
            }

            var Q = (1 - D) >> 2;


            var p_add1 = thisVal + 1;
            var s = 0;

            for (var index = 0; index < p_add1._DataLength; index++)
            {
                uint mask = 0x01;

                for (var i = 0; i < 32; i++)
                {
                    if ((p_add1._Data[index] & mask) != 0)
                    {
                        index = p_add1._DataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            var t = p_add1 >> s;

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            var constant = new BigInteger();

            var n_len = thisVal._DataLength << 1;
            constant._Data[n_len] = 0x00000001;
            constant._DataLength = n_len + 1;

            constant /= thisVal;

            var lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
            var is_prime = lucas[0]._DataLength == 1 && lucas[0]._Data[0] == 0 ||
                                lucas[1]._DataLength == 1 && lucas[1]._Data[0] == 0;

            for (var i = 1; i < s; i++)
            {
                if (!is_prime)
                {
                    // doubling of index
                    lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
                    lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

                    if (lucas[1]._DataLength == 1 && lucas[1]._Data[0] == 0)
                        is_prime = true;
                }

                lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
            }


            if (!is_prime) return false;
            // If n is prime and gcd(n, Q) == 1, then
            // Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

            var g = thisVal.Gcd(Q);
            if (g._DataLength != 1 || g._Data[0] != 1) return true;
            if ((lucas[2]._Data[__MaxLength - 1] & 0x80000000) != 0)
                lucas[2] += thisVal;

            var temp = Q * Jacobi(Q, thisVal) % thisVal;
            if ((temp._Data[__MaxLength - 1] & 0x80000000) != 0)
                temp += thisVal;

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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;


            // test for divisibility by primes < 2000
            return PrimesBelow2000
                    .Select(i => new BigInteger(i))
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
            var this_val = (_Data[__MaxLength - 1] & 0x80000000) != 0 ? -this : this;

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

            if ((this_val._Data[0] & 0x1) == 0)     // even numbers
                return false;


            // test for divisibility by primes < 2000
            if (PrimesBelow2000
               .Select(i => new BigInteger(i))
               .TakeWhile(divisor => divisor < this_val)
               .Select(divisor => this_val % divisor)
               .Any(ResultNum => ResultNum.IntValue() == 0))
                return false;

            // Perform BASE 2 Rabin-Miller Test

            // calculate values of s and t
            var p_sub1 = this_val - new BigInteger(1);
            var s = 0;

            for (var index = 0; index < p_sub1._DataLength; index++)
            {
                uint mask = 0x01;

                for (var i = 0; i < 32; i++, mask <<= 1, s++)
                    if ((p_sub1._Data[index] & mask) != 0)
                    {
                        index = p_sub1._DataLength;      // to break the outer loop
                        break;
                    }
            }

            var t = p_sub1 >> s;

            //        thisVal.bitCount();
            BigInteger a = 2;

            // b = a^t mod p
            var b = a.ModPow(t, this_val);
            var result = b._DataLength == 1 && b._Data[0] == 1;  // a^t mod p = 1

            for (var j = 0; !result && j < s; j++, b *= b % this_val)
                if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                {
                    result = true;
                    break;
                }

            // if number is strong pseudoprime to base 2, then do a strong lucas test
            return result && LucasStrongTestHelper(this_val);
        }



        /// <summary>Последние 4 байта значения числа <see cref="BigInteger"/></summary>
        public int IntValue() => (int)_Data[0];


        /// <summary>Последние 8 байт значения числа <see cref="BigInteger"/></summary>
        public long LongValue()
        {
            long val = _Data[0];
            try
            {       // exception if maxLength = 1
                val |= (long)_Data[1] << 32;
            }
            catch (Exception) //todo: избавиться от исключения
            {
                if ((_Data[0] & 0x80000000) != 0) // negative
                    val = (int)_Data[0];
            }

            return val;
        }


        //***********************************************************************
        // Computes the Jacobi Symbol for a and b.
        // Algorithm adapted from [3] and [4] with some optimizations
        //***********************************************************************

        public static int Jacobi(BigInteger a, [NotNull] BigInteger b)
        {
            // Jacobi defined only for odd integers
            if ((b._Data[0] & 0x1) == 0)
                throw new ArgumentException("Jacobi defined only for odd integers.");

            if (a >= b) a %= b;
            switch (a._DataLength)
            {
                case 1 when a._Data[0] == 0: return 0;  // a == 0
                case 1 when a._Data[0] == 1: return 1;  // a == 1
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
                        index = a._DataLength;      // to break the outer loop
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

        [NotNull]
        public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
        {
            var result = new BigInteger();
            var done = false;

            while (!done)
            {
                result.GenRandomBits(bits, rand);
                result._Data[0] |= 0x01;     // make it odd

                // prime test
                done = result.IsProbablePrime(confidence);
            }
            return result;
        }


        //***********************************************************************
        // Generates a random number with the specified number of bits such
        // that gcd(number, this) = 1
        //***********************************************************************

        [NotNull]
        public BigInteger GenCoPrime(int bits, Random rand)
        {
            var done = false;
            var result = new BigInteger();

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

        [NotNull]
        public BigInteger ModInverse(BigInteger modulus)
        {
            BigInteger[] p = { 0, 1 };
            var q = new BigInteger[2];    // quotients
            BigInteger[] r = { 0, 0 };    // remainders

            var step = 0;

            var a = modulus;
            var b = this;

            while (b._DataLength > 1 || b._DataLength == 1 && b._Data[0] != 0)
            {
                var quotient = new BigInteger();
                var remainder = new BigInteger();

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

            if (r[0]._DataLength > 1 || r[0]._DataLength == 1 && r[0]._Data[0] != 1)
                throw new ArithmeticException("No inverse!");

            var result = (p[0] - p[1] * q[0]) % modulus;

            if ((result._Data[__MaxLength - 1] & 0x80000000) != 0)
                result += modulus;  // get the least positive modulus

            return result;
        }


        //***********************************************************************
        // Returns the value of the BigInteger as a byte array.  The lowest
        // index contains the MSB.
        //***********************************************************************

        [NotNull]
        public byte[] GetBytes()
        {
            var num_bits = BitCount();

            var num_bytes = num_bits >> 3;
            if ((num_bits & 0x7) != 0)
                num_bytes++;

            var result = new byte[num_bytes];

            var pos = 0;
            uint temp_val;
            var val = _Data[_DataLength - 1];

            if ((temp_val = val >> 24 & 0xFF) != 0)
                result[pos++] = (byte)temp_val;
            if ((temp_val = val >> 16 & 0xFF) != 0)
                result[pos++] = (byte)temp_val;
            if ((temp_val = val >> 8 & 0xFF) != 0)
                result[pos++] = (byte)temp_val;
            if ((temp_val = val & 0xFF) != 0)
                result[pos++] = (byte)temp_val;

            for (var i = _DataLength - 2; i >= 0; i--, pos += 4)
            {
                val = _Data[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }


        //***********************************************************************
        // Sets the value of the specified bit to 1
        // The Least Significant Bit position is 0.
        //***********************************************************************

        public void SetBit(uint BitNum)
        {
            var byte_pos = BitNum >> 5;             // divide by 32
            var bit_pos = (byte)(BitNum & 0x1F);    // get the lowest 5 bits

            var mask = (uint)1 << bit_pos;
            _Data[byte_pos] |= mask;

            if (byte_pos >= _DataLength)
                _DataLength = (int)byte_pos + 1;
        }


        //***********************************************************************
        // Sets the value of the specified bit to 0
        // The Least Significant Bit position is 0.
        //***********************************************************************

        public void UnsetBit(uint BitNum)
        {
            var byte_pos = BitNum >> 5;

            if (byte_pos >= _DataLength) return;
            var bit_pos = (byte)(BitNum & 0x1F);

            var mask = (uint)1 << bit_pos;
            var mask2 = 0xFFFFFFFF ^ mask;

            _Data[byte_pos] &= mask2;

            if (_DataLength > 1 && _Data[_DataLength - 1] == 0)
                _DataLength--;
        }


        //***********************************************************************
        // Returns a value that is equivalent to the integer square root
        // of the BigInteger.
        //
        // The integer square root of "this" is defined as the largest integer n
        // such that (n * n) <= this
        //
        //***********************************************************************

        [NotNull]
        public BigInteger Sqrt()
        {
            var num_bits = (uint)BitCount();

            if ((num_bits & 0x1) != 0)        // odd number of bits
                num_bits = (num_bits >> 1) + 1;
            else
                num_bits >>= 1;

            var byte_pos = num_bits >> 5;
            var bit_pos = (byte)(num_bits & 0x1F);

            uint mask;

            var result = new BigInteger();
            if (bit_pos == 0)
                mask = 0x80000000;
            else
            {
                mask = (uint)1 << bit_pos;
                byte_pos++;
            }
            result._DataLength = (int)byte_pos;

            for (var i = (int)byte_pos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // guess
                    result._Data[i] ^= mask;

                    // undo the guess if its square is larger than this
                    if (result * result > this)
                        result._Data[i] ^= mask;

                    mask >>= 1;
                }
                mask = 0x80000000;
            }
            return result;
        }


        //***********************************************************************
        // Returns the k_th number in the Lucas Sequence reduced modulo n.
        //
        // Uses index doubling to speed up the process.  For example, to calculate V(k),
        // we maintain two numbers in the sequence V(n) and V(n+1).
        //
        // To obtain V(2n), we use the identity
        //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
        // To obtain V(2n+1), we first write it as
        //      V(2n+1) = V((n+1) + n)
        // and use the identity
        //      V(m+n) = V(m) * V(n) - Q * V(m-n)
        // Hence,
        //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
        //                   = V(n+1) * V(n) - Q^n * V(1)
        //                   = V(n+1) * V(n) - Q^n * P
        //
        // We use k in its binary expansion and perform index doubling for each
        // bit position.  For each bit position that is set, we perform an
        // index doubling followed by an index addition.  This means that for V(n),
        // we need to update it to V(2n+1).  For V(n+1), we need to update it to
        // V((2n+1)+1) = V(2*(n+1))
        //
        // This function returns
        // [0] = U(k)
        // [1] = V(k)
        // [2] = Q^n
        //
        // Where U(0) = 0 % n, U(1) = 1 % n
        //       V(0) = 2 % n, V(1) = P % n
        //***********************************************************************

        [NotNull]
        public static BigInteger[] LucasSequence(
            [NotNull] BigInteger P,
            [NotNull] BigInteger Q,
            [NotNull] BigInteger k,
            BigInteger n)
        {
            if (k._DataLength == 1 && k._Data[0] == 0)
            {
                var result = new BigInteger[3];

                result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
                return result;
            }

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            var constant = new BigInteger();

            var n_len = n._DataLength << 1;
            constant._Data[n_len] = 0x00000001;
            constant._DataLength = n_len + 1;

            constant /= n;

            // calculate values of s and t
            var s = 0;

            for (var index = 0; index < k._DataLength; index++)
            {
                uint mask = 0x01;

                for (var i = 0; i < 32; i++, mask <<= 1, s++)
                    if ((k._Data[index] & mask) != 0)
                    {
                        index = k._DataLength;      // to break the outer loop
                        break;
                    }
            }

            return LucasSequenceHelper(P, Q, k >> s, n, constant, s);
        }


        //***********************************************************************
        // Performs the calculation of the kth term in the Lucas Sequence.
        // For details of the algorithm, see reference [9].
        //
        // k must be odd.  i.e LSB == 1
        //***********************************************************************

        [NotNull]
        private static BigInteger[] LucasSequenceHelper(
            [NotNull] BigInteger P,
            [NotNull] BigInteger Q,
            [NotNull] BigInteger k,
            [NotNull] BigInteger n,
            [NotNull] BigInteger constant,
            int s)
        {
            var result = new BigInteger[3];

            if ((k._Data[0] & 0x00000001) == 0)
                throw new ArgumentException("Argument k must be odd.");

            var num_of_bits = k.BitCount();
            var mask = (uint)0x1 << ((num_of_bits & 0x1F) - 1);

            var v = 2 % n;
            var q_k = 1 % n;
            var v1 = P % n;
            var u1 = q_k;
            var flag = true;

            for (var i = k._DataLength - 1; i >= 0; i--)     // iterate on the binary expansion of k
            {
                while (mask != 0)
                {
                    if (i == 0 && mask == 0x00000001)        // last bit
                        break;

                    if ((k._Data[i] & mask) != 0)             // bit is set
                    {
                        // index doubling with addition

                        u1 = u1 * v1 % n;

                        v = (v * v1 - P * q_k) % n;
                        v1 = n.BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((q_k * Q) << 1)) % n;

                        if (flag)
                            flag = false;
                        else
                            q_k = n.BarrettReduction(q_k * q_k, n, constant);

                        q_k = q_k * Q % n;
                    }
                    else
                    {
                        // index doubling
                        u1 = (u1 * v - q_k) % n;

                        v1 = (v * v1 - P * q_k) % n;
                        v = n.BarrettReduction(v * v, n, constant);
                        v = (v - (q_k << 1)) % n;

                        if (flag)
                        {
                            q_k = Q % n;
                            flag = false;
                        }
                        else
                            q_k = n.BarrettReduction(q_k * q_k, n, constant);
                    }

                    mask >>= 1;
                }
                mask = 0x80000000;
            }

            // at this point u1 = u(n+1) and v = v(n)
            // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

            u1 = (u1 * v - q_k) % n;
            v = (v * v1 - P * q_k) % n;
            if (!flag)
                q_k = n.BarrettReduction(q_k * q_k, n, constant);
            //        else
            //            flag = false;

            q_k = q_k * Q % n;


            for (var i = 0; i < s; i++)
            {
                // index doubling
                u1 = u1 * v % n;
                v = (v * v - (q_k << 1)) % n;

                q_k = n.BarrettReduction(q_k * q_k, n, constant);
            }

            result[0] = u1;
            result[1] = v;
            result[2] = q_k;

            return result;
        }


        //***********************************************************************
        // Tests the correct implementation of the /, %, * and + operators
        //***********************************************************************

        //todo: Перенести в модульные тесты
        public static void MulDivTest(int rounds) 
        {
            var rand = new Random();
            var val = new byte[64];
            var val2 = new byte[64];

            for (var count = 0; count < rounds; count++)
            {
                // generate 2 numbers of random length
                var t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                var t2 = 0;
                while (t2 == 0)
                    t2 = (int)(rand.NextDouble() * 65);

                var done = false;
                while (!done)
                    for (var i = 0; i < 64; i++)
                    {
                        if (i < t1)
                            val[i] = (byte)(rand.NextDouble() * 256);
                        else
                            val[i] = 0;

                        if (val[i] != 0)
                            done = true;
                    }

                done = false;
                while (!done)
                    for (var i = 0; i < 64; i++)
                    {
                        val2[i] = (byte)(i < t2 ? (byte)(rand.NextDouble() * 256) : 0);

                        if (val2[i] != 0)
                            done = true;
                    }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);
                while (val2[0] == 0)
                    val2[0] = (byte)(rand.NextDouble() * 256);

                //            Console.WriteLine(count);
                var bn1 = new BigInteger(val, t1);
                var bn2 = new BigInteger(val2, t2);


                // Determine the quotient and remainder by dividing
                // the first number by the second.

                var bn3 = bn1 / bn2;
                var bn4 = bn1 % bn2;

                // Recalculate the number
                var bn5 = bn3 * bn2 + bn4;

                // Make sure they're the same
                if (bn5 == bn1) continue;
                //            Console.WriteLine("Error at " + count);
                //            Console.WriteLine(bn1 + "\n");
                //            Console.WriteLine(bn2 + "\n");
                //            Console.WriteLine(bn3 + "\n");
                //            Console.WriteLine(bn4 + "\n");
                //            Console.WriteLine(bn5 + "\n");
                return;
            }
        }


        //***********************************************************************
        // Tests the correct implementation of the modulo exponential function
        // using RSA encryption and decryption (using pre-computed encryption and
        // decryption keys).
        //***********************************************************************

        //todo: Перенести в модульные тесты
        public static void RSATest(int rounds)
        {
            var rand = new Random(1);
            var val = new byte[64];

            // private and public key
            var bi_e = new BigInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880c" +
                                      "c1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb4" +
                                      "51917663d47232488f23a117fc97720f1e7", 16);
            var bi_d = new BigInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e85" +
                                      "9a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60" +
                                      "bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864" +
                                      "dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc924" +
                                      "70a747b8792d6a83b0092d2e5ebaf852c85cacf34278ef" +
                                      "a99160f2f8aa7ee7214de07b7", 16);
            var bi_n = new BigInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479" +
                                      "cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a" +
                                      "73610d94ec36f17f3f46ad75e17bc1adfec99839589f45" +
                                      "f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f014" +
                                      "7a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e7" +
                                      "1e921b9bd9017c16a5231af7f", 16);

            Console.WriteLine("e =\n" + bi_e.ToString(10));
            Console.WriteLine("\nd =\n" + bi_d.ToString(10));
            Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

            for (var count = 0; count < rounds; count++)
            {
                // generate data of random length
                var t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                var done = false;
                while (!done)
                    for (var i = 0; i < 64; i++)
                    {
                        val[i] = i < t1 ? (byte)(rand.NextDouble() * 256) : (byte)0;

                        if (val[i] != 0)
                            done = true;
                    }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);

                Console.Write("Round = " + count);

                // encrypt and decrypt data
                var bi_data = new BigInteger(val, t1);
                var bi_encrypted = bi_data.ModPow(bi_e, bi_n);
                var bi_decrypted = bi_encrypted.ModPow(bi_d, bi_n);

                // compare
                if (bi_decrypted != bi_data)
                {
                    Console.WriteLine("\nError at round " + count);
                    Console.WriteLine(bi_data + "\n");
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }

        }


        //***********************************************************************
        // Tests the correct implementation of the modulo exponential and
        // inverse modulo functions using RSA encryption and decryption.  The two
        // pseudoprimes p and q are fixed, but the two RSA keys are generated
        // for each round of testing.
        //***********************************************************************

        //todo: Перенести в модульные тесты
        public static void RSATest2(int rounds)
        {
            var rand = new Random();
            var val = new byte[64];

            byte[] lv_PseudoPrime1 =
            {
                0x85, 0x84, 0x64, 0xFD, 0x70, 0x6A,
                0x9F, 0xF0, 0x94, 0x0C, 0x3E, 0x2C,
                0x74, 0x34, 0x05, 0xC9, 0x55, 0xB3,
                0x85, 0x32, 0x98, 0x71, 0xF9, 0x41,
                0x21, 0x5F, 0x02, 0x9E, 0xEA, 0x56,
                0x8D, 0x8C, 0x44, 0xCC, 0xEE, 0xEE,
                0x3D, 0x2C, 0x9D, 0x2C, 0x12, 0x41,
                0x1E, 0xF1, 0xC5, 0x32, 0xC3, 0xAA,
                0x31, 0x4A, 0x52, 0xD8, 0xE8, 0xAF,
                0x42, 0xF4, 0x72, 0xA1, 0x2A, 0x0D,
                0x97, 0xB1, 0x31, 0xB3
            };

            byte[] lv_PseudoPrime2 =
            {
                0x99, 0x98, 0xCA, 0xB8, 0x5E, 0xD7,
                0xE5, 0xDC, 0x28, 0x5C, 0x6F, 0x0E,
                0x15, 0x09, 0x59, 0x6E, 0x84, 0xF3,
                0x81, 0xCD, 0xDE, 0x42, 0xDC, 0x93,
                0xC2, 0x7A, 0x62, 0xAC, 0x6C, 0xAF,
                0xDE, 0x74, 0xE3, 0xCB, 0x60, 0x20,
                0x38, 0x9C, 0x21, 0xC3, 0xDC, 0xC8,
                0xA2, 0x4D, 0xC6, 0x2A, 0x35, 0x7F,
                0xF3, 0xA9, 0xE8, 0x1D, 0x7B, 0x2C,
                0x78, 0xFA, 0xB8, 0x02, 0x55, 0x80,
                0x9B, 0xC2, 0xA5, 0xCB
            };


            var bi_p = new BigInteger(lv_PseudoPrime1);
            var bi_q = new BigInteger(lv_PseudoPrime2);
            var bi_pq = (bi_p - 1) * (bi_q - 1);
            var bi_n = bi_p * bi_q;

            for (var count = 0; count < rounds; count++)
            {
                // generate private and public key
                var bi_e = bi_pq.GenCoPrime(512, rand);
                var bi_d = bi_e.ModInverse(bi_pq);

                Console.WriteLine("\ne =\n" + bi_e.ToString(10));
                Console.WriteLine("\nd =\n" + bi_d.ToString(10));
                Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

                // generate data of random length
                var t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                var done = false;
                while (!done)
                    for (var i = 0; i < 64; i++)
                    {
                        val[i] = (byte)(i < t1 ? (byte)(rand.NextDouble() * 256) : 0);

                        if (val[i] != 0)
                            done = true;
                    }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);

                Console.Write("Round = " + count);

                // encrypt and decrypt data
                var bi_data = new BigInteger(val, t1);
                var bi_encrypted = bi_data.ModPow(bi_e, bi_n);
                var bi_decrypted = bi_encrypted.ModPow(bi_d, bi_n);

                // compare
                if (bi_decrypted != bi_data)
                {
                    Console.WriteLine("\nError at round {0}", count);
                    Console.WriteLine("{0}\n", bi_data);
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }

        }


        //***********************************************************************
        // Tests the correct implementation of sqrt() method.
        //***********************************************************************

        //todo: Перенести в модульные тесты
        public static void SqrtTest(int rounds)
        {
            var rand = new Random();
            for (var count = 0; count < rounds; count++)
            {
                // generate data of random length
                var t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 1024);

                Console.Write("Round = " + count);

                var a = new BigInteger();
                a.GenRandomBits(t1, rand);

                var b = a.Sqrt();
                var c = (b + 1) * (b + 1);

                // check that b is the largest integer such that b*b <= a
                if (c <= a)
                {
                    Console.WriteLine("\nError at round " + count);
                    Console.WriteLine(a + "\n");
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }
        }



        //todo: Перенести в модульные тесты
        public static void Main(string[] args)
        {
            // Known problem -> these two pseudoprimes passes my implementation of
            // primality test but failed in JDK's IsProbablePrime test.

            byte[] pseudo_prime1 =
        {
            0x00, 0x85, 0x84, 0x64, 0xFD, 0x70,
            0x6A, 0x9F, 0xF0, 0x94, 0x0C, 0x3E,
            0x2C, 0x74, 0x34, 0x05, 0xC9, 0x55,
            0xB3, 0x85, 0x32, 0x98, 0x71, 0xF9,
            0x41, 0x21, 0x5F, 0x02, 0x9E, 0xEA,
            0x56, 0x8D, 0x8C, 0x44, 0xCC, 0xEE,
            0xEE, 0x3D, 0x2C, 0x9D, 0x2C, 0x12,
            0x41, 0x1E, 0xF1, 0xC5, 0x32, 0xC3,
            0xAA, 0x31, 0x4A, 0x52, 0xD8, 0xE8,
            0xAF, 0x42, 0xF4, 0x72, 0xA1, 0x2A,
            0x0D, 0x97, 0xB1, 0x31, 0xB3
        };

            //        byte[] pseudoPrime2 = { (byte)0x00,
            //                        (byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
            //                        (byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
            //                        (byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
            //                        (byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
            //                        (byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
            //                        (byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
            //                        (byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
            //                        (byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
            //                        (byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
            //                        (byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
            //                        (byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
            //                };

            Console.WriteLine("List of primes < 2000\n---------------------");
            int limit = 100, count = 0;
            for (var i = 0; i < 2000; i++)
            {
                if (i >= limit)
                {
                    Console.WriteLine();
                    limit += 100;
                }

                var p = new BigInteger(-i);

                if (!p.IsProbablePrime()) continue;
                Console.Write(i + ", ");
                count++;
            }
            Console.WriteLine("\nCount = " + count);


            var x = new BigInteger(pseudo_prime1);
            Console.WriteLine("\n\nPrimality testing for\n{0}\n", x);
            Console.WriteLine("SolovayStrassenTest(5) = {0}", x.SolovayStrassenTest(5));
            Console.WriteLine("RabinMillerTest(5) = {0}", x.RabinMillerTest(5));
            Console.WriteLine("FermatLittleTest(5) = {0}", x.FermatLittleTest(5));
            Console.WriteLine("IsProbablePrime() = {0}", x.IsProbablePrime());

            Console.Write("\nGenerating 512-bits random pseudoprime. . .");
            var rand = new Random();
            var prime = genPseudoPrime(512, 5, rand);
            Console.WriteLine("\n" + prime);

            //int dwStart = System.Environment.TickCount;
            //BigInteger.MulDivTest(100000);
            //BigInteger.RSATest(10);
            //BigInteger.RSATest2(10);
            //Console.WriteLine(System.Environment.TickCount - dwStart);
        }
    }
}