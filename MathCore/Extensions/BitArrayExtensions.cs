using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Collections
{
    /// <summary>Класс методов-расширений для <see cref="BitArray"/></summary>
    public static class BitArrayExtensions
    {
        /// <summary>Установить значение бита с указанным индексом</summary>
        /// <param name="A">Массив бит</param>
        /// <param name="Value">Устанавливаемое состояние бита</param>
        public static void Set([NotNull] this BitArray A, int Value)
        {
            var bit_count = A.Length;
            for(var i = 0; i < bit_count; i++, Value >>= 1)
                A[i] = (Value & 1) == 1;
        }

        /// <summary>Получить 8-битовое слово из массива бит</summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Байт, сформированный из массива бит</returns>
        public static byte GetInt8([NotNull] this BitArray A)
        {
            byte Result = 0;
            var bit_count = A.Length;
            for(var i = 0; i < bit_count && i < 16; i++)
            {
                Result <<= 1;
                Result += (byte)(A[bit_count - i - 1] ? 1 : 0);
            }
            return Result;
        }

        /// <summary>Получить 16-битовое слово из массива бит</summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Двухбайтовое целое со знаком, сформированный из массива бит</returns>
        public static short GetInt16([NotNull] this BitArray A)
        {
            short Result = 0;
            var bit_count = A.Length;
            for(var i = 0; i < bit_count && i < 16; i++)
            {
                Result <<= 1;
                Result += (short)(A[bit_count - i - 1] ? 1 : 0);
            }
            return Result;
        }


        /// <summary>Получить 32-битовое слово из массива бит</summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Четырёхбайтное целое со знаком, сформированный из массива бит</returns>
        public static int GetInt32([NotNull] this BitArray A)
        {
            var Result = 0;
            var bit_count = A.Length;
            for(var i = 0; i < bit_count && i < 32; i++)
            {
                Result <<= 1;
                Result += A[bit_count - i - 1] ? 1 : 0;
            }
            return Result;
        }

        /// <summary>Получить 64-битовое слово из массива бит</summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Восьмибайтное целое со знаком, сформированный из массива бит</returns>
        public static long GetInt64([NotNull] this BitArray A)
        {
            long Result = 0;
            var bit_count = A.Length;
            for(var i = 0; i < bit_count && i < 64; i++)
            {
                Result <<= 1;
                Result += A[bit_count - i - 1] ? 1 : 0;
            }
            return Result;
        }

        /// <summary>Преобразовать массив бит в массив логических значений</summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Массив <see cref="bool"/></returns>
        [NotNull]
        public static bool[] ToBoolArray([NotNull] this BitArray A)
        {
            var Result = new bool[A.Length];

            var i_length = A.Length;
            for(var i = 0; i < i_length; i++)
                Result[i] = A[i];

            return Result;
        }

        /// <summary>Инвертировать состояние бит массива</summary>
        /// <param name="A">Массив бит - источник данных</param>
        public static void Inverse([NotNull] this BitArray A)
        {
            var Bits = A.ToBoolArray();
            var bit_count = A.Length;
            for(var i = 0; i < bit_count; i++)
                A[i] = Bits[bit_count - i - 1];
        }

        /// <summary>
        /// Сформировать новый массив бит,
        /// состояние каждого бита нового массива будет инвертированным по отношению к исходному массиву
        /// </summary>
        /// <param name="A">Массив бит - источник данных</param>
        /// <returns>Новый битовый массив, состояние бит которого обратно к состоянию бит исходного массива</returns>
        [NotNull]
        public static BitArray GetInversed([NotNull] this BitArray A)
        {
            var B = new BitArray(A.Length);
            var bit_count = B.Length;
            for(var i = 0; i < bit_count; i++)
                B[i] = A[bit_count - i - 1];
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