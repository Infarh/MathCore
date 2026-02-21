namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Перегрузка оператора сложения
    //***********************************************************************

    /// <summary>Сложение двух больших чисел</summary>
    /// <param name="x">Первое слагаемое</param>
    /// <param name="y">Второе слагаемое</param>
    /// <returns>Сумма чисел</returns>
    public static BigInt operator +(BigInt x, BigInt y)
    {
        var result = new BigInt
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

        if (carry != 0 && result._DataLength < MaxLength)
        {
            result._Data[result._DataLength] = (uint)carry;
            result._DataLength++;
        }

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;


        // Проверка переполнения
        const int last_pos = MaxLength - 1;
        if ((x._Data[last_pos] & 0x80000000) == (y._Data[last_pos] & 0x80000000) &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException();

        return result;
    }


    //***********************************************************************
    // Перегрузка унарного оператора ++
    //***********************************************************************

    /// <summary>Инкремент значения большого числа</summary>
    /// <param name="x">Исходное значение</param>
    /// <returns>Результат инкремента</returns>
    public static BigInt operator ++(BigInt x)
    {
        var result = new BigInt(x);

        long carry = 1;
        var index = 0;

        while (carry != 0 && index < MaxLength)
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

        // Проверка переполнения
        const int last_pos = MaxLength - 1;

        // Переполнение, если исходное значение было положительным, а ++ сменил знак на отрицательный

        if ((x._Data[last_pos] & 0x80000000) == 0 &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException("Overflow in ++.");
        return result;
    }


    //***********************************************************************
    // Перегрузка оператора вычитания
    //***********************************************************************
    /// <summary>Вычитание двух больших чисел</summary>
    /// <param name="x">Уменьшаемое</param>
    /// <param name="y">Вычитаемое</param>
    /// <returns>Разность чисел</returns>
    public static BigInt operator -(BigInt x, BigInt y)
    {
        var result = new BigInt
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

        // Перенос в отрицательную область
        if (carry_in != 0)
        {
            for (var i = result._DataLength; i < MaxLength; i++)
                result._Data[i] = 0xFFFFFFFF;
            result._DataLength = MaxLength;
        }

        // Исправление v1.03 для корректной длины данных при a - (-b)
        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // Проверка переполнения

        const int last_pos = MaxLength - 1;
        if ((x._Data[last_pos] & 0x80000000) != (y._Data[last_pos] & 0x80000000) &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException();

        return result;
    }


    //***********************************************************************
    // Перегрузка унарного оператора --
    //***********************************************************************

    /// <summary>Декремент значения большого числа</summary>
    /// <param name="x">Исходное значение</param>
    /// <returns>Результат декремента</returns>
    public static BigInt operator --(BigInt x)
    {
        var result = new BigInt(x);

        var carry_in = true;
        var index = 0;

        while (carry_in && index < MaxLength)
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

        // Проверка переполнения
        const int last_pos = MaxLength - 1;

        // Переполнение, если исходное значение было отрицательным, а -- сменил знак на положительный

        if ((x._Data[last_pos] & 0x80000000) != 0 &&
            (result._Data[last_pos] & 0x80000000) != (x._Data[last_pos] & 0x80000000))
            throw new ArithmeticException("Underflow in --.");

        return result;
    }

    /// <summary>Перегрузка оператора умножения</summary>
    /// <param name="x">Первый множитель</param>
    /// <param name="y">Второй множитель</param>
    /// <returns>Произведение чисел</returns>
    /// <exception cref="ArithmeticException">При переполнении умножения</exception>
    public static BigInt operator *(BigInt x, BigInt y)
    {
        const int last_pos = MaxLength - 1;
        var x_neg = false;
        var y_neg = false;

        // Получаем абсолютные значения входных чисел
        try
        {
            if ((x._Data[last_pos] & 0x80000000) != 0) // x отрицательное
            {
                x_neg = true;
                x = -x;
            }
            if ((y._Data[last_pos] & 0x80000000) != 0) // y отрицательное
            {
                y_neg = true;
                y = -y;
            }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
        {
            // игнорируется
        }
#pragma warning restore CA1031 // Do not catch general exception types

        var result = new BigInt();

        // Умножение абсолютных значений
        try
        {
            for (var i = 0; i < x._DataLength; i++)
            {
                if (x._Data[i] == 0) continue;

                ulong mc_array = 0;
                for (int j = 0, k = i; j < y._DataLength; j++, k++)
                {
                    // k = i + j
                    var val = x._Data[i] * (ulong)y._Data[j] + result._Data[k] + mc_array;

                    result._Data[k] = (uint)(val & 0xFFFFFFFF);
                    mc_array = val >> 32;
                }

                if (mc_array != 0) result._Data[i + y._DataLength] = (uint)mc_array;
            }
        }
        catch (Exception)
        {
            throw new ArithmeticException("Multiplication overflow.");
        }


        result._DataLength = x._DataLength + y._DataLength;
        if (result._DataLength > MaxLength) result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        // Проверка переполнения (результат отрицательный)
        if ((result._Data[last_pos] & 0x80000000) == 0)
            // Если входные числа разного знака, то результат отрицательный
            return x_neg != y_neg ? -result : result;

        if (x_neg == y_neg || result._Data[last_pos] != 0x80000000)
            throw new ArithmeticException("Multiplication overflow.");
        // Обработка особого случая, когда умножение даёт максимальное отрицательное число в дополнительном коде

        if (result._DataLength == 1) return result;
        var is_max_neg = true;
        for (var i = 0; i < result._DataLength - 1 && is_max_neg; i++)
            if (result._Data[i] != 0)
                is_max_neg = false;

        if (is_max_neg) return result;

        throw new ArithmeticException("Multiplication overflow.");
    }



    //***********************************************************************
    // Перегрузка унарного оператора <<
    //***********************************************************************

    /// <summary>Сдвиг числа влево</summary>
    /// <param name="x">Исходное значение</param>
    /// <param name="ShiftVal">Количество бит сдвига</param>
    /// <returns>Результат сдвига</returns>
    public static BigInt operator <<(BigInt x, int ShiftVal)
    {
        var result = new BigInt(x);
        result._DataLength = ShiftLeft(result._Data, ShiftVal);
        return result;
    }


    // Младшие биты расположены в младшей части буфера

    private static int ShiftLeft(uint[] buffer, int ShiftVal)
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
    // Перегрузка унарного оператора >>
    //***********************************************************************

    /// <summary>Сдвиг числа вправо</summary>
    /// <param name="x">Исходное значение</param>
    /// <param name="ShiftVal">Количество бит сдвига</param>
    /// <returns>Результат сдвига</returns>
    public static BigInt operator >>(BigInt x, int ShiftVal)
    {
        var result = new BigInt(x);
        result._DataLength = ShiftRight(result._Data, ShiftVal);

        if ((x._Data[MaxLength - 1] & 0x80000000) == 0) return result;
        for (var i = MaxLength - 1; i >= result._DataLength; i--)
            result._Data[i] = 0xFFFFFFFF;

        var mask = 0x80000000;
        for (var i = 0; i < 32; i++)
        {
            if ((result._Data[result._DataLength - 1] & mask) != 0) break;

            result._Data[result._DataLength - 1] |= mask;
            mask >>= 1;
        }
        result._DataLength = MaxLength;

        return result;
    }


    private static int ShiftRight(uint[] buffer, int ShiftVal)
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
    // Перегрузка оператора НЕ (дополнение до 1)
    //***********************************************************************

    /// <summary>Побитовое отрицание</summary>
    /// <param name="x">Исходное значение</param>
    /// <returns>Результат операции</returns>
    public static BigInt operator ~(BigInt x)
    {
        var result = new BigInt(x);

        for (var i = 0; i < MaxLength; i++)
            result._Data[i] = ~x._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Перегрузка унарного оператора отрицания (дополнение до 2)
    //***********************************************************************

    /// <summary>Унарное отрицание большого числа</summary>
    /// <param name="x">Исходное значение</param>
    /// <returns>Результат отрицания</returns>
    public static BigInt operator -(BigInt x)
    {
        // Обработка отрицания нуля отдельно, чтобы избежать переполнения

        if (x._DataLength == 1 && x._Data[0] == 0)
            return new();

        var result = new BigInt(x);

        // Дополнение до 1
        for (var i = 0; i < MaxLength; i++)
            result._Data[i] = ~x._Data[i];

        // Добавление единицы к результату дополнения до 1
        long carry = 1;
        var index = 0;

        while (carry != 0 && index < MaxLength)
        {
            var val = (long)result._Data[index];
            val++;

            result._Data[index] = (uint)(val & 0xFFFFFFFF);
            carry = val >> 32;

            index++;
        }

        if ((x._Data[MaxLength - 1] & 0x80000000) == (result._Data[MaxLength - 1] & 0x80000000))
            throw new ArithmeticException("Overflow in negation.\n");

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;
        return result;
    }


    //***********************************************************************
    // Перегрузка оператора равенства
    //***********************************************************************

    /// <summary>Проверка равенства двух чисел</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если значения равны</returns>
    public static bool operator ==(BigInt? x, BigInt? y) => (Equals(x, null) && Equals(y, null)) || (!Equals(x, null) && x.Equals(y));


    /// <summary>Проверка неравенства двух чисел</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если значения не равны</returns>
    public static bool operator !=(BigInt? x, BigInt? y) => !(x == y);


    /// <summary>Проверка равенства с другим объектом</summary>
    /// <param name="o">Сравниваемый объект</param>
    /// <returns>True, если значения равны</returns>
    public override bool Equals(object? o) => Equals(o as BigInt);

    /// <summary>Проверка равенства с другим <see cref="BigInt"/></summary>
    /// <param name="x">Сравниваемое значение</param>
    /// <returns>True, если значения равны</returns>
    public bool Equals(BigInt? x)
    {
        if (_DataLength != x?._DataLength) return false;

        for (var i = 0; i < _DataLength; i++)
            if (_Data[i] != x._Data[i])
                return false;
        return true;
    }


    /// <summary>Получение хэш-кода</summary>
    /// <returns>Хэш-код экземпляра</returns>
    public override int GetHashCode() => _Data.GetComplexHashCode(); //        return ToString().GetHashCode();


    //***********************************************************************
    // Перегрузка оператора сравнения
    //***********************************************************************
    /// <summary>Проверка, что первое число больше второго</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если первое число больше второго</returns>
    public static bool operator >(BigInt x, BigInt y)
    {
        var pos = MaxLength - 1;

        // x отрицательное, y положительное
        if ((x._Data[pos] & 0x80000000) != 0 && (y._Data[pos] & 0x80000000) == 0)
            return false;

        // x положительное, y отрицательное
        if ((x._Data[pos] & 0x80000000) == 0 && (y._Data[pos] & 0x80000000) != 0)
            return true;

        // одинаковый знак
        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;
        for (pos = len - 1; pos >= 0 && x._Data[pos] == y._Data[pos]; pos--) { }

        return pos >= 0 && x._Data[pos] > y._Data[pos];
    }


    /// <summary>Проверка, что первое число меньше второго</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если первое число меньше второго</returns>
    public static bool operator <(BigInt x, BigInt y)
    {
        var pos = MaxLength - 1;

        // x отрицательное, y положительное
        if ((x._Data[pos] & 0x80000000) != 0 && (y._Data[pos] & 0x80000000) == 0)
            return true;

        // x положительное, y отрицательное
        if ((x._Data[pos] & 0x80000000) == 0 && (y._Data[pos] & 0x80000000) != 0)
            return false;

        // одинаковый знак
        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;
        for (pos = len - 1; pos >= 0 && x._Data[pos] == y._Data[pos]; pos--) { }

        return pos >= 0 && x._Data[pos] < y._Data[pos];
    }


    /// <summary>Проверка, что первое число больше либо равно второму</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если первое число больше либо равно второму</returns>
    public static bool operator >=(BigInt x, BigInt y) => x == y || x > y;


    /// <summary>Проверка, что первое число меньше либо равно второму</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>True, если первое число меньше либо равно второму</returns>
    public static bool operator <=(BigInt x, BigInt y) => x == y || x < y;


    //***********************************************************************
    // Закрытая функция, поддерживающая деление двух чисел, когда делитель имеет более 1 разряда
    //
    // Алгоритм взят из [1]
    //***********************************************************************

    private static void MultiByteDivide(
        BigInt X,
        BigInt Y,
        BigInt OutQuotient,
        BigInt OutRemainder)
    {
        var result = new uint[MaxLength];

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

            var kk = new BigInt(dividend_part);
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
        for (; y < MaxLength; y++) OutQuotient._Data[y] = 0;

        while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
            OutQuotient._DataLength--;

        if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

        OutRemainder._DataLength = ShiftRight(remainder, shift);

        for (y = 0; y < OutRemainder._DataLength; y++) OutRemainder._Data[y] = remainder[y];
        for (; y < MaxLength; y++) OutRemainder._Data[y] = 0;
    }


    //***********************************************************************
    // Закрытая функция, поддерживающая деление двух чисел, когда делитель имеет 1 разряд
    //***********************************************************************

    private static void SingleByteDivide(
        BigInt x,
        BigInt y,
        BigInt OutQuotient,
        BigInt OutRemainder)
    {
        var result = new uint[MaxLength];
        var result_pos = 0;

        // Копирование делимого в остаток
        for (var i = 0; i < MaxLength; i++)
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
        for (; j < MaxLength; j++) OutQuotient._Data[j] = 0;

        while (OutQuotient._DataLength > 1 && OutQuotient._Data[OutQuotient._DataLength - 1] == 0)
            OutQuotient._DataLength--;

        if (OutQuotient._DataLength == 0) OutQuotient._DataLength = 1;

        while (OutRemainder._DataLength > 1 && OutRemainder._Data[OutRemainder._DataLength - 1] == 0)
            OutRemainder._DataLength--;
    }


    //***********************************************************************
    // Перегрузка оператора деления
    //***********************************************************************

    /// <summary>Деление двух больших чисел</summary>
    /// <param name="x">Делимое</param>
    /// <param name="y">Делитель</param>
    /// <returns>Частное от деления</returns>
    public static BigInt operator /(BigInt x, BigInt y)
    {
        var quotient = new BigInt();
        var remainder = new BigInt();

        const int last_pos = MaxLength - 1;
        bool divisor_neg = false, dividend_neg = false;

        if ((x._Data[last_pos] & 0x80000000) != 0) // x отрицательное
        {
            x = -x;
            dividend_neg = true;
        }
        if ((y._Data[last_pos] & 0x80000000) != 0) // y отрицательное
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
    // Перегрузка оператора остатка от деления
    //***********************************************************************

    /// <summary>Вычисление остатка от деления</summary>
    /// <param name="x">Делимое</param>
    /// <param name="y">Делитель</param>
    /// <returns>Остаток от деления</returns>
    public static BigInt operator %(BigInt x, BigInt y)
    {
        var quotient = new BigInt();
        var remainder = new BigInt(x);

        const int last_pos = MaxLength - 1;
        var dividend_neg = false;

        if ((x._Data[last_pos] & 0x80000000) != 0) // x отрицательное
        {
            x = -x;
            dividend_neg = true;
        }
        if ((y._Data[last_pos] & 0x80000000) != 0) // y отрицательное
            y = -y;

        if (x < y) return remainder;
        if (y._DataLength == 1)
            SingleByteDivide(x, y, quotient, remainder);
        else
            MultiByteDivide(x, y, quotient, remainder);

        return dividend_neg ? -remainder : remainder;
    }


    //***********************************************************************
    // Перегрузка побитового оператора И
    //***********************************************************************
    /// <summary>Побитовое И двух чисел</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>Результат операции</returns>
    public static BigInt operator &(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
            result._Data[i] = x._Data[i] & y._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Перегрузка побитового оператора ИЛИ
    //***********************************************************************
    /// <summary>Побитовое ИЛИ двух чисел</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>Результат операции</returns>
    public static BigInt operator |(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
            result._Data[i] = x._Data[i] | y._Data[i];

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }


    //***********************************************************************
    // Перегрузка побитового оператора исключающего ИЛИ
    //***********************************************************************
    /// <summary>Побитовое исключающее ИЛИ двух чисел</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>Результат операции</returns>
    public static BigInt operator ^(BigInt x, BigInt y)
    {
        var result = new BigInt();

        var len = x._DataLength > y._DataLength ? x._DataLength : y._DataLength;

        for (var i = 0; i < len; i++)
        {
            var sum = x._Data[i] ^ y._Data[i];
            result._Data[i] = sum;
        }

        result._DataLength = MaxLength;

        while (result._DataLength > 1 && result._Data[result._DataLength - 1] == 0)
            result._DataLength--;

        return result;
    }
}
