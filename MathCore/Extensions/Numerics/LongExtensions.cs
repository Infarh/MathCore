using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    public static class LongExtensions
    {
        [DST]
        public static bool IsPrime(this long x)
        {
            if(x % 2 == 0) return x == 2;

            var max = (long)Math.Sqrt(x);

            for(var i = 3; i <= max; i += 2)
                if(x % i == 0)
                    return false;
            return true;
        }

        /// <summary>Является ли число степенью двойки?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
        [DST]
        public static bool IsPowerOf2(this long x) => (x & (x - 1)) == 0 || x == 1;

        /// <summary>Число бит числа</summary>
        /// <param name="x">Значащее число</param>
        /// <returns>Число бит числа</returns>
        [DST]
        public static int BitCount(this long x) => (int)Math.Round(Math.Log(x, 2));

        /// <summary>Реверсирование бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <param name="N">Число реверсируемых бит</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static long BitReversing(this long x, int N)
        {
            long Result = 0;
            for(var i = 0; i < N; i++)
            {
                Result <<= 1;
                Result += x & 1;
                x >>= 1;
            }
            return Result;
        }

        /// <summary>Реверсирование всех 64 бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static long BitReversing(this long x) => x.BitReversing(16);

        [DST]
        public static bool IsDeviatedTo(this long x, long y) => x % y == 0;

        [DST]
        public static long GetAbsMod(this long x, long mod) => x % mod + (x < 0 ? mod : 0);

        /// <summary>Является ли число нечётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число нечётное</returns>
        [DST]
        public static bool IsOdd(this long x) => !x.IsEven();

        /// <summary>Является ли число чётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число чётное</returns>
        [DST]
        public static bool IsEven(this long x) => x.IsDeviatedTo(2);

        [DST]
        public static long Factorial(this long x)
        {
            if(x < 0) throw new ArgumentOutOfRangeException(nameof(x), "x должен быть >= 0");
            if(x == 0) return 1;
            var result = x;
            while(x > 1) result *= --x;
            return result;
        }

        [DST]
        public static long GetFlags(this long Value, long Mask) => Value & Mask;

        [DST]
        public static long SetFlag(this long Value, long Flag, long Mask) => (Value & ~Mask) | (Flag & Mask);
    }
}