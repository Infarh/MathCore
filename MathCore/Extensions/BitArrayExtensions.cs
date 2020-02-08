using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System.Collections
{
    public static class BitArrayExtensions
    {
        public static void Set([NotNull] this BitArray A, int Value)
        {
            var i_Length = A.Length;
            for(var i = 0; i < i_Length; i++, Value >>= 1)
                A[i] = (Value & 1) == 1;
        }

        public static byte GetInt8([NotNull] this BitArray A)
        {
            byte Result = 0;
            var i_Length = A.Length;
            for(var i = 0; i < i_Length && i < 16; i++)
            {
                Result <<= 1;
                Result += (byte)(A[i_Length - i - 1] ? 1 : 0);
            }
            return Result;
        }

        public static short GetInt16([NotNull] this BitArray A)
        {
            short Result = 0;
            var i_Length = A.Length;
            for(var i = 0; i < i_Length && i < 16; i++)
            {
                Result <<= 1;
                Result += (short)(A[i_Length - i - 1] ? 1 : 0);
            }
            return Result;
        }


        public static int GetInt32([NotNull] this BitArray A)
        {
            var Result = 0;
            var i_Length = A.Length;
            for(var i = 0; i < i_Length && i < 32; i++)
            {
                Result <<= 1;
                Result += A[i_Length - i - 1] ? 1 : 0;
            }
            return Result;
        }

        public static long GetInt64([NotNull] this BitArray A)
        {
            long Result = 0;
            var i_Length = A.Length;
            for(var i = 0; i < i_Length && i < 64; i++)
            {
                Result <<= 1;
                Result += A[i_Length - i - 1] ? 1 : 0;
            }
            return Result;
        }

        [NotNull]
        public static bool[] ToBoolArray([NotNull] this BitArray A)
        {
            var Result = new bool[A.Length];

            var i_length = A.Length;
            for(var i = 0; i < i_length; i++)
                Result[i] = A[i];

            return Result;
        }

        public static void Inverse([NotNull] this BitArray A)
        {
            var Bits = A.ToBoolArray();
            var i_Length = A.Length;
            for(var i = 0; i < i_Length; i++)
                A[i] = Bits[i_Length - i - 1];
        }

        [NotNull]
        public static BitArray GetInversed([NotNull] this BitArray A)
        {
            var B = new BitArray(A.Length);
            var i_Length = B.Length;
            for(var i = 0; i < i_Length; i++)
                B[i] = A[i_Length - i - 1];
            return B;
        }

        /// <summary>Проверка корректности чётности</summary>
        /// <param name="bits">Битовый массив</param>
        /// <param name="PartyBit">Бит чётности</param>
        /// <returns>Истина, если сумма бит по модулю 2 и бита чётности равна 0</returns>
        [DST]
        public static bool IsPartyCorrect([NotNull] this BitArray bits, bool PartyBit)
        {
            var result = PartyBit;

            for(var i = 0; i < bits.Length; i++)
                result = (bits[i] && !result) || (result && !bits[i]);

            return !result;
        }

        /// <summary>Сумма бит по модулю 2</summary>
        /// <param name="bits">Битовый массив</param>
        /// <returns>Результат сложения бит массива по модулю 2</returns>
        [DST]
        public static bool GetBitSumMod2([NotNull] this BitArray bits)
        {
            var result = false;

            for(var i = 0; i < bits.Length; i++)
                result = (bits[i] && !result) || (result && !bits[i]);

            return result;
        }
    }
}