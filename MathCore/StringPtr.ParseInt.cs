namespace MathCore;

public readonly ref partial struct StringPtr
{
    public int? TryParseInt32() => TryParseInt32(out var x) ? x : null;

    /// <summary>Попытка преобразования подстроки в <see cref="int"/></summary>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseInt32(out int value)
    {
#if NET8_0_OR_GREATER
        return int.TryParse(Span, out value);
#else
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;

        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
        {
            value = default;
            return false;
        }

        var sign = 1;

        switch (str[start + index])
        {
            case '-':
                sign = -1;
                index++;
                break;
            case '+':
                index++;
                break;
        }

        while (str[start + index] == '0' && index < length) index++;

        if (index >= length || !char.IsDigit(str, start + index))
        {
            value = default;
            return false;
        }

        var result = 0;
        var digits = 0;
        while (index < length)
        {
            var digit = str[start + index] - '0';

            if (!char.IsDigit(str, start + index))
            {
                if (digits > 0 && char.IsWhiteSpace(str, start + index))
                {
                    while (++index < length)
                        if (!char.IsWhiteSpace(str, start + index))
                        {
                            value = default;
                            return false;
                        }

                    value = result;
                    return true;
                }

                value = default;
                return false;
            }

            if (digits >= 8 || digit > int.MaxValue - result)
            {
                value = default;
                return false;
            }

            result = result * 10 + digit;

            index++;
            digits++;
        }

        value = sign * result;
        return true;
#endif
    }

    /// <summary>Преобразование подстроки в <see cref="int"/></summary>
    /// <returns>Преобразованное значение</returns>
    /// <exception cref="FormatException">В случае если строка не является представлением <see cref="int"/></exception>
    /// <exception cref="OverflowException">Если длина строковой записи числа превышает <see cref="int"/>.<see cref="int.MaxValue"/></exception>
    public int ParseInt32()
    {
#if NET8_0_OR_GREATER
        return int.Parse(Span);
#else
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;

        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
            throw index > 0
                ? new("Строка содержит лишь символы пробелов")
                : new FormatException("Пустая строка");

        var sign = 1;

        switch (str[start + index])
        {
            case '-':
                sign = -1;
                index++;
                break;
            case '+':
                index++;
                break;
        }

        var index_before_skip_zeros = index;
        while (index < length && str[start + index] == '0') index++;

        if (index >= length || !char.IsDigit(str, start + index))
        {
            if (index > index_before_skip_zeros)
                return 0;
            throw new FormatException("Строка имела неверный формат");
        }

        var result = 0;
        var digits = 0;
        while (index < length)
        {
            if (!char.IsDigit(str, start + index))
            {
                if (digits == 0 || !char.IsWhiteSpace(str, start + index))
                    throw new FormatException("Строка имела неверный формат");

                while (++index < length)
                    if (!char.IsWhiteSpace(str, start + index))
                        throw new FormatException("Некорректные символы в конце строки");

                return sign * result;
            }

            var digit = str[start + index] - '0';
            if (digits >= 8 || digit > int.MaxValue - result)
                throw new OverflowException("Размер числа превышает максимально допустимое значение int");

            result = result * 10 + digit;

            index++;
            digits++;
        }

        return sign * result;
#endif
    }
}
