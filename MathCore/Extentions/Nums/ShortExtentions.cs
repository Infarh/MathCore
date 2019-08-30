using System.Diagnostics.Contracts;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using MathCore;

namespace System
{
    public static class ShortExtentions
    {
        [DST]
        public static bool IsPrime(this short x)
        {
            if(x % 2 == 0) return x == 2;

            var max = (short)Math.Sqrt(x);

            for(var i = 3; i <= max; i += 2)
                if(x % i == 0)
                    return false;
            return true;
        }

        /// <summary>Является ли число степенью двойки?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
        [DST]
        public static bool IsPowerOf2(this short x) => (x & (x - 1)) == 0 || x == 1;

        /// <summary>Число бит числа</summary>
        /// <param name="x">Значащее число</param>
        /// <returns>Число бит числа</returns>
        [DST]
        public static int BitCount(this short x) => (int)Math.Round(Math.Log(x, 2));

        /// <summary>Реверсирование бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <param name="N">Число реверсируемых бит</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static short BitReversing(this short x, int N)
        {
            short Result = 0;
            for(var i = 0; i < N; i++)
            {
                Result <<= 1;
                Result += (short)(x & 1);
                x >>= 1;
            }
            return Result;
        }

        /// <summary>Реверсирование всех 16 бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static short BitReversing(this short x) => x.BitReversing(16);

        [DST]
        public static bool IsDevidedTo(this short x, short y) => x % y == 0;

        [DST]
        public static short GetAbsMod(this short x, short mod) => (short)(x % mod + (x < 0 ? mod : 0));

        /// <summary>Является ли число нечётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число нечётное</returns>
        [DST]
        public static bool IsOdd(this short x) => !x.IsEven();

        /// <summary>Является ли число чётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число чётное</returns>
        [DST]
        public static bool IsEven(this short x) => x.IsDevidedTo(2);

        [DST]
        public static short Power(this short x, int n)
        {
            if(n > 1000 || n < -1000) return (short)Math.Pow(x, n);
            if(n < 0) return (short)(1 / (double)x).Power(-n);
            var result = 1.0;
            for(var i = 0; i < n; i++) result *= x;
            return (short)result;
        }

        [DST]
        public static double Power(this short x, short y) => Math.Pow(x, y);

        [DST]
        public static Complex Power(this short x, Complex z) => x ^ z;

        [Pure, DST]
        public static short GetFlags(this short Value, short Mask) => (short)(Value & Mask);

        [Pure, DST]
        public static short SetFlag(this short Value, short Flag, short Mask) => (short)((Value & ~Mask) | (Flag & Mask));
    }
}
