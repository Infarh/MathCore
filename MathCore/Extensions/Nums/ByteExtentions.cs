using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace System
{
    public static class ByteExtentions
    {
        /// <summary>Является ли число степенью двойки?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
        [DebuggerStepThrough]
        public static bool IsPowerOf2(this byte x) => (x & (x - 1)) == 0 || x == 1;

        /// <summary>Число бит числа</summary>
        /// <param name="x">Значащее число</param>
        /// <returns>Число бит числа</returns>
        [DebuggerStepThrough]
        public static int BitCount(this byte x) => (int)Math.Round(Math.Log(x, 2));

        /// <summary>Реверсирование бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <param name="N">Число реверсируемых бит [ = 16 ]</param>
        /// <returns>Реверсированное число</returns>
        [DebuggerStepThrough]
        public static byte BitReversing(this byte x, int N = 16)
        {
            byte lv_Result = 0;
            for(var i = 0; i < N; i++)
            {
                lv_Result <<= 1;
                lv_Result += (byte)(x & 1);
                x >>= 1;
            }
            return lv_Result;
        }

        [DebuggerStepThrough]
        public static bool IsDevidedTo(this byte x, byte y) => (x % y) == 0;

        [DebuggerStepThrough]
        public static byte GetAbsMod(this byte x, byte mod) => (byte)((x % mod) + (x < 0 ? mod : 0));

        /// <summary>Является ли число нечётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число нечётное</returns>
        [DebuggerStepThrough]
        public static bool IsOdd(this byte x) => !x.IsEven();

        /// <summary>Является ли число чётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число чётное</returns>
        [DebuggerStepThrough]
        public static bool IsEven(this byte x) => x.IsDevidedTo(2);

        [DebuggerStepThrough]
        public static bool IsSetBit(this byte x, int BitN)
        {
            var num = (byte)(1 << BitN);
            return ((x & num) == num);
        }

        [DebuggerStepThrough]
        public static byte SetBit(this byte x, int BitN, bool Set)
        {
            var num = (byte)(1 << BitN);
            return (Set ? ((byte)(x | num)) : ((byte)(x & ~num)));
        }

        [DebuggerStepThrough]
        public static int ToOctBase(this byte x)
        {
            var num = 0;
            for(var i = 1; x != 0; i *= 10)
            {
                num += (x % 8) * i;
                x >>= 3;
            }
            return num;
        }

        [Pure, DebuggerStepThrough]
        public static byte GetFlags(this byte Value, byte Mask) => (byte)(Value & Mask);

        [Pure, DebuggerStepThrough]
        public static byte SetFlag(this byte Value, byte Flag, byte Mask) => (byte)((Value & ~Mask) | (Flag & Mask));
    }
}
