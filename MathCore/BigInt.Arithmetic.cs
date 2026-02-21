// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Возвращает max(this, Value)
    //***********************************************************************
    /// <summary>Максимум из двух чисел</summary>
    /// <param name="x">Сравниваемое значение</param>
    /// <returns>Максимальное значение</returns>
    public BigInt Max(BigInt x) => this > x ? new(this) : new BigInt(x);


    //***********************************************************************
    // Возвращает min(this, Value)
    //***********************************************************************
    /// <summary>Минимум из двух чисел</summary>
    /// <param name="x">Сравниваемое значение</param>
    /// <returns>Минимальное значение</returns>
    public BigInt Min(BigInt x) => this < x ? new(this) : new BigInt(x);


    //***********************************************************************
    // Возвращает абсолютное значение
    //***********************************************************************
    /// <summary>Абсолютное значение числа</summary>
    /// <returns>Модуль числа</returns>
    public BigInt Abs() => (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : new(this);


    //***********************************************************************
    // Возведение в степень по модулю
    //***********************************************************************
    /// <summary>Возведение в степень по модулю</summary>
    /// <param name="exp">Показатель степени</param>
    /// <param name="n">Модуль</param>
    /// <returns>Результат возведения в степень по модулю</returns>
    public BigInt ModPow(BigInt exp, BigInt n)
    {
        if ((exp._Data[MaxLength - 1] & 0x80000000) != 0)
            throw new ArithmeticException("Positive exponents only.");

        BigInt result_num = 1;
        BigInt temp_num;
        var this_negative = false;

        if ((_Data[MaxLength - 1] & 0x80000000) != 0) // this отрицательное
        {
            temp_num = -this % n;
            this_negative = true;
        }
        else
            temp_num = this % n; // гарантирует (tempNum * tempNum) < b^(2k)

        if ((n._Data[MaxLength - 1] & 0x80000000) != 0) // n отрицательное
            n = -n;

        // Вычисление constant = b^(2k) / m
        var constant = new BigInt();

        var i = n._DataLength << 1;
        constant._Data[i] = 0x00000001;
        constant._DataLength = i + 1;

        constant /= n;
        var total_bits = exp.BitCount;
        var count = 0;

        // Выполнение возведения в степень методом «квадрат и умножение»
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

    /// <summary>Быстрое вычисление сокращения числа по модулю с использованием редукции Барретта</summary>
    /// <returns></returns>
    /// <remarks>
    /// Требуется <paramref name="x"/> &lt; b^(2k), где b база.
    /// В этом случае база соответствует 2^32 (uint).
    /// </remarks>
    private static BigInt BarrettReduction(BigInt x, BigInt n, BigInt constant)
    {
        var k = n._DataLength;
        var k_plus_one = k + 1;
        var k_minus_one = k - 1;

        var q1 = new BigInt();

        // q1 = x / b^(k-1)
        for (int i = k_minus_one, j = 0; i < x._DataLength; i++, j++)
            q1._Data[j] = x._Data[i];
        q1._DataLength = x._DataLength - k_minus_one;
        if (q1._DataLength <= 0)
            q1._DataLength = 1;


        var q2 = q1 * constant;
        var q3 = new BigInt();

        // q3 = q2 / b^(k+1)
        for (int i = k_plus_one, j = 0; i < q2._DataLength; i++, j++)
            q3._Data[j] = q2._Data[i];
        q3._DataLength = q2._DataLength - k_plus_one;
        if (q3._DataLength <= 0)
            q3._DataLength = 1;


        // r1 = x mod b^(k+1)
        // т.е. сохраняем младшие (k+1) слов
        var r1 = new BigInt();
        var length_to_copy = x._DataLength > k_plus_one ? k_plus_one : x._DataLength;
        for (var i = 0; i < length_to_copy; i++)
            r1._Data[i] = x._Data[i];
        r1._DataLength = length_to_copy;


        // r2 = (q3 * n) mod b^(k+1)
        // частичное умножение q3 и n

        var r2 = new BigInt();
        for (var i = 0; i < q3._DataLength; i++)
        {
            if (q3._Data[i] == 0) continue;

            ulong mc_array = 0;
            var t = i;
            for (var j = 0; j < n._DataLength && t < k_plus_one; j++, t++)
            {
                // t = i + j
                var val = q3._Data[i] * (ulong)n._Data[j] + r2._Data[t] + mc_array;

                r2._Data[t] = (uint)(val & 0xFFFFFFFF);
                mc_array = val >> 32;
            }

            if (t < k_plus_one)
                r2._Data[t] = (uint)mc_array;
        }
        r2._DataLength = k_plus_one;
        while (r2._DataLength > 1 && r2._Data[r2._DataLength - 1] == 0)
            r2._DataLength--;

        r1 -= r2;
        if ((r1._Data[MaxLength - 1] & 0x80000000) != 0) // отрицательное
        {
            var val = new BigInt
            {
                _Data =
                {
                    [k_plus_one] = 0x00000001
                },
                _DataLength = k_plus_one + 1
            };
            r1 += val;
        }

        while (r1 >= n)
            r1 -= n;

        return r1;
    }
}
