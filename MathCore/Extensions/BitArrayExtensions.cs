#nullable enable
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Collections;

/// <summary>Класс методов-расширений для <see cref="BitArray"/></summary>
public static class BitArrayExtensions
{
    /// <summary>Установить значение бита с указанным индексом</summary>
    /// <param name="A">Массив бит</param>
    /// <param name="Value">Устанавливаемое состояние бита</param>
    public static void Set(this BitArray A, int Value)
    {
        var bit_count = A.Length;
        for(var i = 0; i < bit_count; i++, Value >>= 1)
            A[i] = (Value & 1) == 1;
    }

    /// <summary>Получить 8-битовое слово из массива бит</summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Байт, сформированный из массива бит</returns>
    public static byte GetInt8(this BitArray A)
    {
        byte result    = 0;
        var  bit_count = A.Length;
        for(var i = 0; i < bit_count && i < 16; i++)
        {
            result <<= 1;
            result +=  (byte)(A[bit_count - i - 1] ? 1 : 0);
        }
        return result;
    }

    /// <summary>Получить 16-битовое слово из массива бит</summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Двухбайтовое целое со знаком, сформированный из массива бит</returns>
    public static short GetInt16(this BitArray A)
    {
        short result    = 0;
        var   bit_count = A.Length;
        for(var i = 0; i < bit_count && i < 16; i++)
        {
            result <<= 1;
            result +=  (short)(A[bit_count - i - 1] ? 1 : 0);
        }
        return result;
    }


    /// <summary>Получить 32-битовое слово из массива бит</summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Четырёхбайтное целое со знаком, сформированный из массива бит</returns>
    public static int GetInt32(this BitArray A)
    {
        var result    = 0;
        var bit_count = A.Length;
        for(var i = 0; i < bit_count && i < 32; i++)
        {
            result <<= 1;
            result +=  A[bit_count - i - 1] ? 1 : 0;
        }
        return result;
    }

    /// <summary>Получить 64-битовое слово из массива бит</summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Восьмибайтное целое со знаком, сформированный из массива бит</returns>
    public static long GetInt64(this BitArray A)
    {
        long result    = 0;
        var  bit_count = A.Length;
        for(var i = 0; i < bit_count && i < 64; i++)
        {
            result <<= 1;
            result +=  A[bit_count - i - 1] ? 1 : 0;
        }
        return result;
    }

    /// <summary>Преобразовать массив бит в массив логических значений</summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Массив <see cref="bool"/></returns>
    public static bool[] ToBoolArray(this BitArray A)
    {
        var result = new bool[A.Length];

        var i_length = A.Length;
        for(var i = 0; i < i_length; i++)
            result[i] = A[i];

        return result;
    }

    /// <summary>Инвертировать состояние бит массива</summary>
    /// <param name="A">Массив бит - источник данных</param>
    public static void Inverse(this BitArray A)
    {
        var bits      = A.ToBoolArray();
        var bit_count = A.Length;
        for(var i = 0; i < bit_count; i++)
            A[i] = bits[bit_count - i - 1];
    }

    /// <summary>
    /// Сформировать новый массив бит,
    /// состояние каждого бита нового массива будет инвертированным по отношению к исходному массиву
    /// </summary>
    /// <param name="A">Массив бит - источник данных</param>
    /// <returns>Новый битовый массив, состояние бит которого обратно к состоянию бит исходного массива</returns>
    public static BitArray GetInversed(this BitArray A)
    {
        var b         = new BitArray(A.Length);
        var bit_count = b.Length;
        for(var i = 0; i < bit_count; i++)
            b[i] = A[bit_count - i - 1];
        return b;
    }

    /// <summary>Проверка корректности чётности</summary>
    /// <param name="bits">Битовый массив</param>
    /// <param name="PartyBit">Бит чётности</param>
    /// <returns>Истина, если сумма бит по модулю 2 и бита чётности равна 0</returns>
    [DST]
    public static bool IsPartyCorrect(this BitArray bits, bool PartyBit)
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
    public static bool GetBitSumMod2(this BitArray bits)
    {
        var result = false;

        for(var i = 0; i < bits.Length; i++)
            result = (bits[i] && !result) || (result && !bits[i]);

        return result;
    }
}