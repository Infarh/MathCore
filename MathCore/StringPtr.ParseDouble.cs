using System.Globalization;

namespace MathCore;

public readonly ref partial struct StringPtr
{
    public double? TryParseDouble() => TryParseDouble(out var d) ? d : null;

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseDouble(out double value)
    {
#if NET8_0_OR_GREATER
        return double.TryParse(Span, CultureInfo.InvariantCulture, out value);
#else
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;

        if (length == 0 || str[start + length - 1] is '.' or ',')
        {
            value = double.NaN;
            return false;
        }

        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
        {
            value = double.NaN;
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

        var fraction_index = 0d;
        var result = 0d;
        while (index < length)
        {
            if (fraction_index == 0)
            {
                if (char.IsDigit(str, start + index))
                {
                    result = result * 10 + (str[start + index] - '0');
                    index++;
                    continue;
                }

                if (str[start + index] is '.' or ',')
                    fraction_index = 0.1;
                else
                {
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                    {
                        value = result;
                        return true;
                    }

                    value = double.NaN;
                    return false;
                }
            }
            else
            {
                if (!char.IsDigit(str, start + index))
                {
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                    {
                        value = result;
                        return true;
                    }

                    value = double.NaN;
                    return false;
                }

                result += (str[start + index] - '0') * fraction_index;
                fraction_index /= 10;
            }

            index++;
        }

        value = sign * result;
        return true;
#endif
    }

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="Provider">Информация о формате</param>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseDouble(IFormatProvider Provider, out double value)
    {
#if NET8_0_OR_GREATER
        return double.TryParse(Span, Provider, out value);
#else
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;
        var format = NumberFormatInfo.GetInstance(Provider);
        var decimal_separator_str = format.NumberDecimalSeparator;

        if (length == 0/* || EndWith(decimal_separator_str)*/)
        {
            // Если строка имеет длину 0, или заканчивается на разделитель, то выход с ошибкой
            value = double.NaN;
            return false;
        }

        // Пропускаем ведущие пробелы в строке если они были
        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
        {
            // Если строка кончилась, то выход с ошибкой
            value = double.NaN;
            return false;
        }

        var sign = 1; // Пусть знак будет '+'

        // Проверяем знаковый первый разряд
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

        var state = 0;
        var whole = 0L;
        var fraction = 0L;
        var fraction_base = 1L;
        var exp = 0;
        var exp_sign = 1;

        while (index < length) // Побежали по символам строки
            switch (state)
            {
                case 0:
                    if (char.IsDigit(str, start + index))
                    {
                        whole = whole * 10 + (str[start + index] - '0'); // Накапливаем целую часть числа
                        index++;
                        // Если символ был цифрой, то идём дальше
                        break;
                    }

                    //Очередной символ был не цифрой
                    if (Substring(index).StartWith(decimal_separator_str))
                    {
                        // Если наткнулись на разделитель дробной части числа
                        // то переходим к анализу дробной части
                        state = 1;
                        index += decimal_separator_str.Length;
                        break;
                    }

                    if (str[start + index] is 'e' or 'E')
                    {
                        state = 2;
                        index++;
                        if (index < length)
                            switch (str[start + index])
                            {
                                case '-':
                                    exp_sign = -1;
                                    index++;
                                    break;
                                case '+':
                                    index++;
                                    break;
                            }
                        break;
                    }

                    //Потенциально число закончилось. Пытаемся пропустить конечные пробелы
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                    {
                        // Если строка закончилась (пробелы пропущены), то число сформировано
                        // победа!
                        value = sign * whole;
                        return true;
                    }

                    // Строка имеет неверный формат. Выходим с ошибкой.
                    value = double.NaN;
                    return false;

                case 1:
                    // Если анализируем символы дробной части
                    if (char.IsDigit(str, start + index))
                    {
                        fraction = fraction * 10 + (str[start + index] - '0');
                        fraction_base *= 10;

                        index++;
                        break;
                    }

                    if (str[start + index] is 'e' or 'E')
                    {
                        state = 2;
                        index++;
                        if (index < length)
                            switch (str[start + index])
                            {
                                case '-':
                                    exp_sign = -1;
                                    index++;
                                    break;
                                case '+':
                                    index++;
                                    break;
                            }

                        break;
                    }

                    // Если очередной символ это не цифра, то потенциально конец строки

                    // Пытаемся пропустить пробелы в конце строки
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                        break;

                    // Очередной символ некорректен. Выходим с ошибкой.
                    value = double.NaN;
                    return false;

                case 2:
                    if (char.IsDigit(str, start + index))
                    {
                        exp = exp * 10 + (str[start + index] - '0');
                        index++;
                        break;
                    }

                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                        break;

                    value = double.NaN;
                    return false;
            }

        value = whole + fraction / (double)fraction_base;

        if (exp != 0)
        {
            if (value > 1)
                while (exp > 0 && value > 10)
                {
                    exp--;
                    value /= 10;
                }
            else
                while (exp > 0 && value < 1)
                {
                    exp--;
                    value *= 10;
                }

            if (exp != 0)
                value *= Math.Pow(10, exp_sign * exp);
        }

        if (sign < 0)
            value = -value;

        return true;
#endif
    }

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <returns>Преобразованное вещественное число</returns>
    public double ParseDouble() => ParseDouble(CultureInfo.InvariantCulture);

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="Provider">Информация о формате</param>
    /// <returns>Преобразованное вещественное число</returns>
    public double ParseDouble(IFormatProvider Provider)
    {
#if NET8_0_OR_GREATER
        return double.Parse(Span, Provider);
#else
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;
        var format = NumberFormatInfo.GetInstance(Provider);
        var decimal_separator_str = format.NumberDecimalSeparator;

        if (length == 0 /* || EndWith(decimal_separator_str)*/)
            throw new FormatException("Пустая строка");

        // Пропускаем ведущие пробелы в строке если они были
        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
            throw new FormatException("Строка состоит из одних пробелов");

        var sign = 1; // Пусть знак будет '+'

        // Проверяем знаковый первый разряд
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

        var state = 0;
        var whole = 0L;
        var fraction = 0L;
        var fraction_base = 1L;
        var exp = 0;
        var exp_sign = 1;

        while (index < length) // Побежали по символам строки
            switch (state)
            {
                case 0:
                    if (char.IsDigit(str, start + index))
                    {
                        whole = whole * 10 + (str[start + index] - '0'); // Накапливаем целую часть числа
                        index++;
                        // Если символ был цифрой, то идём дальше
                        break;
                    }

                    //Очередной символ был не цифрой
                    if (Substring(index).StartWith(decimal_separator_str))
                    {
                        // Если наткнулись на разделитель дробной части числа
                        // то переходим к анализу дробной части
                        state = 1;
                        index += decimal_separator_str.Length;
                        break;
                    }

                    if (str[start + index] is 'e' or 'E')
                    {
                        state = 2;
                        index++;
                        if (index < length)
                            switch (str[start + index])
                            {
                                case '-':
                                    exp_sign = -1;
                                    index++;
                                    break;
                                case '+':
                                    index++;
                                    break;
                            }
                        break;
                    }

                    //Потенциально число закончилось. Пытаемся пропустить конечные пробелы
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                        // Если строка закончилась (пробелы пропущены), то число сформировано
                        // победа!
                        return sign * whole;

                    // Строка имеет неверный формат. Выходим с ошибкой.
                    throw new FormatException("Строка имела неверный формат");

                case 1:
                    // Если анализируем символы дробной части
                    if (char.IsDigit(str, start + index))
                    {
                        fraction = fraction * 10 + (str[start + index] - '0');
                        fraction_base *= 10;

                        index++;
                        break;
                    }

                    if (str[start + index] is 'e' or 'E')
                    {
                        state = 2;
                        index++;
                        if (index < length)
                            switch (str[start + index])
                            {
                                case '-':
                                    exp_sign = -1;
                                    index++;
                                    break;
                                case '+':
                                    index++;
                                    break;
                            }

                        break;
                    }

                    // Если очередной символ это не цифра, то потенциально конец строки

                    // Пытаемся пропустить пробелы в конце строки
                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                        break;

                    // Очередной символ некорректен. Выходим с ошибкой.
                    throw new FormatException("Строка имела неверный формат");

                case 2:
                    if (char.IsDigit(str, start + index))
                    {
                        exp = exp * 10 + (str[start + index] - '0');
                        index++;
                        break;
                    }

                    while (char.IsWhiteSpace(str, start + index) && index < length)
                        index++;

                    if (index == length)
                        break;

                    throw new FormatException("Строка имела неверный формат");
            }

        var value = sign * (whole + fraction / (double)fraction_base);

        if (exp == 0)
            return value;


        return value * Math.Pow(10, exp_sign * exp);
#endif
    }
}
