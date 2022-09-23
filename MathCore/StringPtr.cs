#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using MathCore.CSV;

// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global

namespace MathCore;

/// <summary>Указатель на позицию в строке</summary>
public readonly ref struct StringPtr
{
    /* --------------------------------------------------------------------------------------- */

    /// <summary>Исходная строка</summary>
    public string Source { get; }

    /// <summary>Положение начала в строке</summary>
    public int Pos { get; }

    /// <summary>Длина подстроки</summary>
    public int Length { get; }

    /// <summary>Подстрока является пустой</summary>
    public bool IsEmpty => Source is null || Length == 0;

    /// <summary>Индекс символа в подстроке</summary>
    /// <param name="index">Индекс в подстроке</param>
    /// <returns>Символ по указанному положению</returns>
    public char this[int index] => Source[Pos + index];

    /// <summary>Новая подстрока</summary>
    /// <param name="index">Индекс начала</param>
    /// <param name="length">Длина</param>
    /// <returns>Указатель на новое положение</returns>
    public StringPtr this[int index, int length] => new(Source, Pos + index, length);

    //public StringPtr this[Range range] => this[range.Start, range.End - range.End];

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового указателя на положение в строке</summary>
    /// <param name="Source">Исходная строка</param>
    public StringPtr(string Source)
    {
        this.Source = Source;
        Pos = 0;
        Length = Source.Length;
    }

    /// <summary>Инициализация нового указателя на положение в строке</summary>
    /// <param name="Source">Исходная строка</param>
    /// <param name="Pos">Положение в исходной строке</param>
    /// <param name="Length">Длина подстроки</param>
    public StringPtr(string Source, int Pos, int Length)
    {
        this.Source = Source;
        this.Pos = Pos;
        this.Length = Math.Max(Math.Min(Length, Source.Length - Pos), 0);
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Подстрока</summary>
    /// <param name="Offset">Смещение в текущей подстроке</param>
    /// <returns>Новый указатель на подстроку, смещённую на указанное значение символов относительно текущей подстроки</returns>
    public StringPtr Substring(int Offset) => new(Source, Pos + Offset, Length - Offset);

    /// <summary>Подстрока</summary>
    /// <param name="Offset">Смещение в текущей подстроке</param>
    /// <param name="Count">Число символов в новой подстроке</param>
    /// <returns>Указатель на подстроку, смещённую на указанное значение символов относительно текущей подстроки</returns>
    public StringPtr Substring(int Offset, int Count) => new(Source, Pos + Offset, Count);

    /// <summary>Начинается ли подстрока с указанного символа</summary>
    /// <param name="c">Символ, с которого должна начинаться текущая подстрока</param>
    /// <returns>Истина, если текущая подстрока начинается с указанного символа</returns>
    public bool StartWith(char c) => Length >= 1 && Source[Pos] == c;

    /// <summary>Начинается ли подстрока с указанной строки</summary>
    /// <param name="Str">Строка, с которой должна начинаться текущая подстрока</param>
    /// <returns>Истина, если текущая подстрока начинается с указанной строки</returns>
    public bool StartWith(string Str) => Length >= Str.Length && string.Compare(Source, Pos, Str, 0, Str.Length) == 0;

    /// <summary>Начинается ли подстрока с указанной строки</summary>
    /// <param name="Str">Строка, с которой должна начинаться текущая подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если текущая подстрока начинается с указанной строки</returns>
    public bool StartWith(string Str, StringComparison Comparison) => Length >= Str.Length && string.Compare(Source, Pos, Str, 0, Str.Length, Comparison) == 0;

    /// <summary>Начинается ли текущая подстрока с указанной подстроки</summary>
    /// <param name="Str">Подстрока, с которой должна начинаться текущая подстрока</param>
    /// <returns>Истина, если в начале текущей подстроки содержится указанная подстрока</returns>
    public bool StartWith(StringPtr Str) => Length >= Str.Length && string.Compare(Source, Pos, Str.Source, Str.Pos, Str.Length) == 0;

    /// <summary>Начинается ли текущая подстрока с указанной подстроки</summary>
    /// <param name="Str">Подстрока, с которой должна начинаться текущая подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если в начале текущей подстроки содержится указанная подстрока</returns>
    public bool StartWith(StringPtr Str, StringComparison Comparison) => Length >= Str.Length && string.Compare(Source, Pos, Str.Source, Str.Pos, Str.Length, Comparison) == 0;

    /// <summary>Заканчивается ли подстрока с указанным символом</summary>
    /// <param name="c">Символ, на который должна заканчиваться текущая подстрока</param>
    /// <returns>Истина, если текущая подстрока заканчивается на указанный символ</returns>
    public bool EndWith(char c) => Length >= 1 && Source[Pos + Length - 1] == c;

    /// <summary>Заканчивается ли подстрока указанной строкой</summary>
    /// <param name="Str">Строка, на которую должна заканчиваться текущая подстрока</param>
    /// <returns>Истина, если текущая подстрока заканчивается на указанную строку</returns>
    public bool EndWith(string Str) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str, 0, Str.Length) == 0;

    /// <summary>Заканчивается ли подстрока указанной строкой</summary>
    /// <param name="Str">Строка, на которую должна заканчиваться текущая подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если текущая подстрока заканчивается на указанную строку</returns>
    public bool EndWith(string Str, StringComparison Comparison) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str, 0, Str.Length, Comparison) == 0;

    /// <summary>Начинается ли текущая подстрока указанной подстрокой</summary>
    /// <param name="Str">Подстрока, с которой должна начинаться текущая подстрока</param>
    /// <returns>Истина, если в начале текущей подстроки содержится указанная подстрока</returns>
    public bool EndWith(StringPtr Str) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str.Source, Str.Pos, Str.Length) == 0;

    /// <summary>Начинается ли текущая подстрока указанной подстрокой</summary>
    /// <param name="Str">Подстрока, с которой должна начинаться текущая подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если в начале текущей подстроки содержится указанная подстрока</returns>
    public bool EndWith(StringPtr Str, StringComparison Comparison) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str.Source, Str.Pos, Str.Length, Comparison) == 0;

    /// <summary>Содержит ли подстрока указанный символ</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>Истина, если в текущей подстроке есть указанный символ</returns>
    public bool Contains(char c) => IndexOf(c) >= 0;

    /// <summary>Содержит ли подстрока указанную строку</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <returns>Истина, если в текущей подстроке есть указанная строка</returns>
    public bool Contains(string str) => IndexOf(str) >= 0;

    /// <summary>Содержит ли подстрока указанную строку</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если в текущей подстроке есть указанная строка</returns>
    public bool Contains(string str, StringComparison Comparison) => IndexOf(str, Comparison) >= 0;

    /// <summary>Индекс первого вхождения символа в подстроку</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>Индекс символа в подстроке, либо -1 в случае его отсутствия</returns>
    public int IndexOf(char c) =>
        Source.IndexOf(c, Pos, Length) is >= 0 and var index
            ? index - Pos
            : -1;

    /// <summary>Индекс последнего вхождения символа в подстроку</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>Индекс символа в подстроке с конца, либо -1 в случае его отсутствия</returns>
    public int LastIndexOf(char c) =>
        Source.LastIndexOf(c, Pos, Length) is >= 0 and var index
            ? index - Pos
            : -1;

    /// <summary>Индекс первого вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <returns>Индекс первого вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int IndexOf(string str) => IndexOf(str, StringComparison.Ordinal);

    /// <summary>Индекс последнего вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <returns>Индекс последнего вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int LastIndexOf(string str) => LastIndexOf(str, StringComparison.Ordinal);

    /// <summary>Индекс первого вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Индекс первого вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int IndexOf(string str, StringComparison Comparison) =>
        str.Length <= Length && str.IndexOf(str, Pos, Length, Comparison) is >= 0 and var index
            ? index - Pos
            : -1;

    /// <summary>Индекс последнего вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Индекс последнего вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int LastIndexOf(string str, StringComparison Comparison) =>
        str.Length <= Length && str.LastIndexOf(str, Pos, Length, Comparison) is >= 0 and var index
            ? index - Pos
            : -1;

    /// <summary>Индекс последнего вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Индекс последнего вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int LastIndexOf(StringPtr str, StringComparison Comparison)
    {
        if (str.Length > Length) return -1;
        var index = Length - str.Length;

        while (true)
        {
            index = Substring(0, index).LastIndexOf(str[0]);
            if (index < 0)
                return -1;

            if (Substring(index, str.Length).Equals(str, Comparison))
                return index;
        }
    }

    /// <summary>Проверка соответствия текущей подстроки с указанной строкой</summary>
    /// <param name="str">Проверяемая на равенство строка</param>
    /// <returns>Истина, если текущая подстрока эквивалентна указанной строке</returns>
    public bool Equals(string? str) =>
        str is not null
        && str.Length == Length && string.Compare(Source, Pos, str, 0, Length) == 0;

    /// <summary>Проверка соответствия текущей подстроки с указанной строкой</summary>
    /// <param name="str">Проверяемая на равенство строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если текущая подстрока эквивалентна указанной строке</returns>
    public bool Equals(string? str, StringComparison Comparison) =>
        str is not null &&
        str.Length == Length &&
        string.Compare(Source, Pos, str, 0, Length, Comparison) == 0;

    /// <summary>Проверка соответствия текущей подстроки с указанной подстрокой</summary>
    /// <param name="str">Проверяемая на равенство подстрока</param>
    /// <returns>Истина, если текущая подстрока эквивалентна указанной подстроке</returns>
    public bool Equals(StringPtr str) =>
        Length == str.Length &&
        string.Compare(Source, Pos, str.Source, str.Pos, Length) == 0;

    /// <summary>Проверка соответствия текущей подстроки с указанной подстрокой</summary>
    /// <param name="str">Проверяемая на равенство подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если текущая подстрока эквивалентна указанной подстроке</returns>
    public bool Equals(StringPtr str, StringComparison Comparison) =>
        Length == str.Length &&
        string.Compare(Source, Pos, str.Source, str.Pos, Length, Comparison) == 0;

    /// <summary>Оператор проверки на равенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно равен указанной строке</returns>
    public static bool operator ==(StringPtr ptr, string str) => ptr.Equals(str);

    /// <summary>Оператор проверки на неравенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно неравен указанной строке</returns>
    public static bool operator !=(StringPtr ptr, string str) => !(ptr == str);

    /// <summary>Оператор проверки на равенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно равен указанной строке</returns>
    public static bool operator ==(string str, StringPtr ptr) => ptr.Equals(str);

    /// <summary>Оператор проверки на неравенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно неравен указанной строке</returns>
    public static bool operator !=(string str, StringPtr ptr) => !(ptr == str);

    /// <summary>Оператор порядка "больше", сравнивающий фрагмент строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки больше, чем указанная строка</returns>
    public static bool operator >(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) > 0;

    /// <summary>Оператор порядка "меньше", сравнивающий фрагмент строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки меньше, чем указанная строка</returns>
    public static bool operator <(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) < 0;

    /// <summary>Оператор порядка "больше", сравнивающий строку с фрагментом строки</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если строка больше, чем указанный фрагмент строки</returns>
    public static bool operator >(string str, StringPtr ptr) => ptr < str;

    /// <summary>Оператор порядка "меньше", сравнивающий строку с фрагментом строки</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если строка меньше, чем указанный фрагмент строки</returns>
    public static bool operator <(string str, StringPtr ptr) => ptr > str;

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Попытка преобразования подстроки в <see cref="int"/></summary>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseAsInt32(out int value)
    {
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
    }

    /// <summary>Преобразование подстроки в <see cref="int"/></summary>
    /// <returns>Преобразованное значение</returns>
    /// <exception cref="FormatException">В случае если строка не является представлением <see cref="int"/></exception>
    /// <exception cref="OverflowException">Если длина строковой записи числа превышает <see cref="int"/>.<see cref="int.MaxValue"/></exception>
    public int ParseAsInt32()
    {
        var start = Pos;
        var index = 0;
        var length = Length;
        var str = Source;

        while (index < length && char.IsWhiteSpace(str, start + index))
            index++;

        if (index >= length)
            throw index > 0
                ? new FormatException("Строка содержит лишь символы пробелов")
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

        while (str[start + index] == '0' && index < length) index++;

        if (index >= length || !char.IsDigit(str, start + index))
            throw new FormatException("Строка имела неверный формат");

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
    }

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseDouble(out double value)
    {
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
    }

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="Provider">Информация о формате</param>
    /// <param name="value">Преобразованное значение</param>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryParseDouble(IFormatProvider Provider, out double value)
    {
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
            // Если строка коныилась, то выход с ошибкой
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
                    if (Substring(start + index).StartWith(decimal_separator_str))
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

                    //Потенциально число закончилось.Пытаемся пропустить конечные пробелы
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

        value = sign * (whole + fraction / (double)fraction_base);

        if (exp != 0)
        {
            var e = Math.Pow(10, exp_sign * exp);
            value *= e;
        }

        return true;
    }

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <returns>Преобразованное вещественное число</returns>
    public double ParseDouble() => ParseDouble(CultureInfo.InvariantCulture);

    /// <summary>Попытка преобразования подстроки в <see cref="double"/></summary>
    /// <param name="Provider">Информация о формате</param>
    /// <returns>Преобразованное вещественное число</returns>
    public double ParseDouble(IFormatProvider Provider)
    {
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
                    if (Substring(start + index).StartWith(decimal_separator_str))
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

                    //Потенциально число закончилось.Пытаемся пропустить конечные пробелы
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

        return exp == 0 
            ? sign * (whole + fraction / (double)fraction_base) 
            : sign * (whole + fraction / (double)fraction_base) * Math.Pow(10, exp_sign * exp);
    }

    /// <summary>Получить имя (ключ) из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая имя (ключ) из пары ключ-значение</returns>
    public StringPtr GetName(char Separator = '=')
    {
        var index = IndexOf(Separator);
        var pos = Pos;
        var str = Source;
        if (index <= 0) return new(str, pos, 0);
        return Substring(0, index);
    }

    /// <summary>Получить значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая значение из пары ключ-значение</returns>
    public StringPtr GetValueString(char Separator = '=')
    {
        var index = IndexOf(Separator);
        var pos = Pos;
        var str = Source;
        if (index <= 0) return new(str, pos, 0);
        return Substring(index + 1);
    }

    /// <summary>Получить вещественное значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Вещественное значение из пары ключ-значение</returns>
    public double GetValueDouble(char Separator = '=') => GetValueString(Separator).ParseDouble();

    /// <summary>Получить вещественное значение из пары ключ=значение</summary>
    /// <param name="Provider">Провайдер формата значения</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Вещественное значение из пары ключ-значение</returns>
    public double GetValueDouble(IFormatProvider Provider, char Separator = '=') => GetValueString(Separator).ParseDouble(Provider);

    /// <summary>Получить целочисленное значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Целочисленное значение из пары ключ-значение</returns>
    public int GetValueInt32(char Separator = '=') => GetValueString(Separator).ParseAsInt32();

    /// <summary>Попытаться получить вещественное значение из пары ключ=значение</summary>
    /// <param name="value">Вещественное значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в вещественное значение выполнено успешно</returns>
    public bool TryGetValueDouble(out double value, char Separator = '=') => GetValueString(Separator).TryParseDouble(out value);

    /// <summary>Попытаться получить вещественное значение из пары ключ=значение</summary>
    /// <param name="value">Вещественное значение из пары ключ-значение</param>
    /// <param name="Provider">Провайдер формата значения</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в вещественное значение выполнено успешно</returns>
    public bool TryGetValueDouble(IFormatProvider Provider, out double value, char Separator = '=') => GetValueString(Separator).TryParseDouble(Provider, out value);

    /// <summary>Попытаться получить целочисленное значение из пары ключ=значение</summary>
    /// <param name="value">Целочисленное значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в целочисленное значение выполнено успешно</returns>
    public bool TryGetValueInt32(out int value, char Separator = '=') => GetValueString(Separator).TryParseAsInt32(out value);

    /// <summary>Попытаться получить булево значение из пары ключ=значение</summary>
    /// <param name="value">Булево значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в булево значение выполнено успешно</returns>
    public bool TryGetValueBool(out bool value, char Separator = '=')
    {
        var str = GetValueString(Separator);
        value = false;

        if (str.IsEmpty) return false;

        if (str.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            value = true;
            return true;
        }

        if (str.Equals("false", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    /// <summary>Получить булево значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Булево значение из пары ключ-значение</returns>
    public bool GetValueBool(char Separator = '=') => TryGetValueBool(out var value, Separator)
        ? value
        : throw new FormatException("Строка имела неверный формат");

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Символы, обрезаемые в начале и конце строки по умолчанию</summary>
    private static readonly char[] __DefaultTrimChars = { ' ', '\0', '\r', '\n', '\t' };

    /// <summary>Удаление технических символов в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(out bool Trimmed) => TrimStart(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в начале строки</summary>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart() => TrimStart(__DefaultTrimChars);

    /// <summary>Удаление технических символов в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(out bool Trimmed) => TrimEnd(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в конце строки</summary>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd() => TrimEnd(__DefaultTrimChars);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool Trimmed) => Trim(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim() => Trim(__DefaultTrimChars);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(char c) => TrimStart(out _, c);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(out bool Trimmed, char c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && str[pos] == c) pos++;
        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos) : this;
    }

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(char c1, char c2) => TrimStart(out _, c1, c2);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(out bool Trimmed, char c1, char c2)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && (str[pos] == c1 || str[pos] == c2)) pos++;
        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos) : this;
    }

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(char c1, char c2, char c3) => TrimStart(out _, c1, c2, c3);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(out bool Trimmed, char c1, char c2, char c3)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && (str[pos] == c1 || str[pos] == c2 || str[pos] == c3)) pos++;
        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos) : this;
    }

    /// <summary>Удаление символов в начале строки</summary>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(params char[] c) => TrimStart(out _, c);

    /// <summary>Удаление символов в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimStart(out bool Trimmed, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (pos < end_pos && can_move)
        {
            var s = str[pos];
            can_move = false;
            foreach (var v in c)
                if (s == v)
                {
                    pos++;
                    can_move = true;
                    break;
                }
        }

        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos) : this;
    }

    /// <summary>Удаление символа в конца строки</summary>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(char c) => TrimEnd(out _, c);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(out bool Trimmed, char c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (end_pos > pos && str[end_pos] == c) end_pos--;

        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(char c1, char c2) => TrimEnd(out _, c1, c2);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(out bool Trimmed, char c1, char c2)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (end_pos > pos && str[end_pos] == c1 || str[end_pos] == c2) end_pos--;

        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(char c1, char c2, char c3) => TrimEnd(out _, c1, c2, c3);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(out bool Trimmed, char c1, char c2, char c3)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (end_pos > pos && str[end_pos] == c1 || str[end_pos] == c2 || str[end_pos] == c3) end_pos--;

        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление символов в конце строки</summary>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(params char[] c) => TrimEnd(out _, c);

    /// <summary>Удаление символов в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr TrimEnd(out bool Trimmed, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (end_pos > pos && can_move)
        {
            var s = str[end_pos];
            can_move = false;
            foreach (var v in c)
                if (s == v)
                {
                    end_pos--;
                    can_move = true;
                    break;
                }
        }

        Trimmed = pos != Pos;
        return Trimmed ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(char c) => Trim(out _, out _, c);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool Trimmed, char c)
    {
        var result = Trim(out var trimmed_start, out var trimmed_end, c);
        Trimmed = trimmed_start || trimmed_end;
        return result;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="TrimmedStart">Обрезание строки в начале было выполнено</param>
    /// <param name="TrimmedEnd">Обрезание строки в конце было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, char c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && str[pos] == c) pos++;
        while (pos < end_pos && str[end_pos] == c) end_pos--;
        TrimmedStart = pos != Pos;
        TrimmedEnd = end_pos != Pos + len - 1;
        return TrimmedStart || TrimmedEnd ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(char c1, char c2) => Trim(out _, out _, c1, c2);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool Trimmed, char c1, char c2)
    {
        var result = Trim(out var trimmed_start, out var trimmed_end, c1, c2);
        Trimmed = trimmed_start || trimmed_end;
        return result;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="TrimmedStart">Обрезание строки в начале было выполнено</param>
    /// <param name="TrimmedEnd">Обрезание строки в конце было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, char c1, char c2)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && str[pos] == c1 || str[pos] == c2) pos++;
        while (pos < end_pos && str[end_pos] == c1 || str[end_pos] == c2) end_pos--;
        TrimmedStart = pos != Pos;
        TrimmedEnd = end_pos != Pos + len - 1;
        return TrimmedStart || TrimmedEnd ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(char c1, char c2, char c3) => Trim(out _, out _, c1, c2, c3);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool Trimmed, char c1, char c2, char c3)
    {
        var result = Trim(out var trimmed_start, out var trimmed_end, c1, c2, c3);
        Trimmed = trimmed_start || trimmed_end;
        return result;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="TrimmedStart">Обрезание строки в начале было выполнено</param>
    /// <param name="TrimmedEnd">Обрезание строки в конце было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, char c1, char c2, char c3)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while (pos < end_pos && str[pos] == c1 || str[pos] == c2 || str[pos] == c3) pos++;
        while (pos < end_pos && str[end_pos] == c1 || str[end_pos] == c2 || str[end_pos] == c3) end_pos--;
        TrimmedStart = pos != Pos;
        TrimmedEnd = end_pos != Pos + len - 1;
        return TrimmedStart || TrimmedEnd ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(params char[] c) => Trim(out _, out _, c);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool Trimmed, params char[] c)
    {
        var result = Trim(out var trimmed_start, out var trimmed_end, c);
        Trimmed = trimmed_start || trimmed_end;
        return result;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="TrimmedStart">Обрезание строки в начале было выполнено</param>
    /// <param name="TrimmedEnd">Обрезание строки в конце было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (pos < end_pos && can_move)
        {
            var s = str[pos];
            can_move = false;
            foreach (var v in c)
                if (s == v)
                {
                    pos++;
                    can_move = true;
                    break;
                }
        }
        TrimmedStart = pos != Pos;

        can_move = true;
        while (pos < end_pos && can_move)
        {
            var s = str[end_pos];
            can_move = false;
            foreach (var v in c)
                if (s == v)
                {
                    end_pos--;
                    can_move = true;
                    break;
                }
        }
        TrimmedEnd = end_pos != Pos + len - 1;

        return TrimmedStart || TrimmedEnd ? Substring(pos, end_pos - pos + 1) : this;
    }

    /// <summary>Разделить строку на подстроки по указанному символам-разделителям</summary>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public Tokenizer Split(params char[] Separators) => new(this, Separators);

    /// <summary>Разделить строку на подстроки по указанному символам-разделителям</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public Tokenizer Split(bool SkipEmpty, params char[] Separators) => SkipEmpty
        ? new Tokenizer(this, Separators).SkipEmpty(true)
        : new(this, Separators);

    /// <summary>Разделитель строки на фрагменты по указанному символам-разделителям</summary>
    public readonly ref struct Tokenizer
    {
        /// <summary>Строковый буфер</summary>
        private readonly string _Buffer;

        /// <summary>Символы-разделители</summary>
        private readonly char[] _Separators;

        /// <summary>Начальное положение в буфере</summary>
        private readonly int _StartIndex;

        /// <summary>Длина подстроки для анализа</summary>
        private readonly int _Length;

        /// <summary>Пропускать пустые фрагменты</summary>
        private readonly bool _SkipEmpty;

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Str">Исходный фрагмент строки</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        public Tokenizer(StringPtr Str, char[] Separators) : this(Str.Source, Separators, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        public Tokenizer(string Buffer, char[] Separators) : this(Buffer, Separators, 0, Buffer.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        /// <param name="StartIndex">Индекс начала анализируемой подстроки</param>
        /// <param name="Length">Длина анализируемой подстроки</param>
        /// <param name="SkipEmpty">Пропускать пустые строковые фрагменты</param>
        public Tokenizer(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty = false)
        {
            _Buffer = Buffer;
            _Separators = Separators;
            _StartIndex = StartIndex;
            _Length = Length;
            _SkipEmpty = SkipEmpty;
        }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param name="Skip">Пропускать, или нет</param>
        /// <returns>Перечислитель строковых фрагментов с изменённым режимом пропуска строковых фрагментов</returns>
        public Tokenizer SkipEmpty(bool Skip) => new(_Buffer, _Separators, _StartIndex, _Length, Skip);

        /// <summary>Сформировать перечислитель строковых фрагментов</summary>
        /// <returns>Перечислитель строковых фрагментов</returns>
        public TokenEnumerator GetEnumerator() => new(_Buffer, _Separators, _StartIndex, _Length, _SkipEmpty);

        /// <summary>Перечислитель строковых фрагментов</summary>
        public ref struct TokenEnumerator
        {
            /// <summary>Строковый буфер</summary>
            private readonly string _Buffer;

            /// <summary>Символы-разделители</summary>
            private readonly char[] _Separators;

            /// <summary>Начальное положение в буфере</summary>
            private readonly int _StartIndex;

            /// <summary>Длина подстроки для анализа</summary>
            private readonly int _Length;

            /// <summary>Пропускать пустые фрагменты</summary>
            private readonly bool _SkipEmpty;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos;

            /// <summary>Инициализация нового перечислителя строковых фрагментов</summary>
            /// <param name="Buffer">Исходный строковый буфер</param>
            /// <param name="Separators">Символы-разделители</param>
            /// <param name="StartIndex">Начальное положение в строковом буфере</param>
            /// <param name="Length">Длина подстроки для анализа</param>
            /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
            public TokenEnumerator(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty)
            {
                _Buffer = Buffer;
                _Separators = Separators;
                _StartIndex = StartIndex;
                _CurrentPos = StartIndex;
                _Length = Length;
                _SkipEmpty = SkipEmpty;

                Current = default;
            }

            /// <summary>Текущий фрагмент строки</summary>
            public StringPtr Current { get; private set; }

            /// <summary>Перемещение к следующему фрагменту</summary>
            /// <returns>Истина, если перемещение выполнено успешно</returns>
            public bool MoveNext()
            {
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return true;
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;
            }

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <returns>Следующий фрагмент строки</returns>
            /// <exception cref="InvalidOperationException">Возникает в случае отсутствия возможности выделить следующий фрагмент строки</exception>
            public StringPtr MoveNextOrThrow() => MoveNext()
                ? Current
                : throw new InvalidOperationException($"Невозможно получить следующий фрагмент строки после разделителя {string.Join(",", _Separators.Select(c => $"'{c}'"))}");

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <typeparam name="TException">Генерируемое исключение в случае отсутствия возможности перемещения к следующей подстроке</typeparam>
            /// <returns>Следующий фрагмент строки</returns>
            public StringPtr MoveNextOrThrow<TException>() where TException : Exception, new() => MoveNext()
                ? Current
                : throw new TException();

            /// <summary>Найти следующую подстроку</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separators">Символы-разделители</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Найденная подстрока</returns>
            private static StringPtr GetNext(string Str, char[] Separators, int StartIndex, int EndIndex)
            {
                if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                var index = NextIndex(Str, Separators, StartIndex, EndIndex);
                return index < 0
                    ? new(Str, StartIndex, EndIndex - StartIndex)
                    : new(Str, StartIndex, index - StartIndex);
            }

            /// <summary>Индекс следующего разделителя в строке в заданном диапазоне</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separators">Символы-разделители</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Индекс искомого символа в подстроке, либо -1, если его найдено не было</returns>
            private static int NextIndex(string Str, char[] Separators, int StartIndex, int EndIndex)
            {
                var str_length = Str.Length;
                var separators_length = Separators.Length;
                for (var i = StartIndex; i < EndIndex && i < str_length; i++)
                {
                    var c = Str[i];
                    for (var j = 0; j < separators_length; j++)
                        if (Separators[j] == c)
                            return i;
                }

                return -1;
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="provider">Формат представления вещественного числа</param>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(IFormatProvider provider, out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(provider, out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(provider, out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в целое число</summary>
            /// <param name="value">Результат преобразования, либо 0, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в целое число выполнено успешно</returns>
            public bool TryParseNextAsInt32(out int value)
            {
                value = 0;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseAsInt32(out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseAsInt32(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в <see cref="bool"/> значение</summary>
            /// <param name="value">Результат преобразования, либо <c>false</c>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в <see cref="bool"/> значение выполнено успешно</returns>
            public bool TryParseNextAsBool(out bool value)
            {
                value = default;
                if (!MoveNext()) return false;
                if (Current.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    value = true;
                    return true;
                }

                if (Current.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    value = false;
                    return true;
                }

                return false;
            }

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в целое число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator int(TokenEnumerator Enumerator) => Enumerator.Current.ParseAsInt32();

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в вещественное число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator double(TokenEnumerator Enumerator) => Enumerator.Current.ParseDouble();
        }
    }

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public TokenizerSingleChar Split(char Separator) => new(this, Separator);

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public TokenizerSingleChar Split(bool SkipEmpty, char Separator) => SkipEmpty
        ? new TokenizerSingleChar(this, Separator).SkipEmpty()
        : new(this, Separator);

    /// <summary>Разделитель строки на фрагменты по указанному символу-разделителю</summary>
    public readonly ref struct TokenizerSingleChar
    {
        /// <summary>Строковый буфер</summary>
        private readonly string _Buffer;

        /// <summary>Символ-разделитель</summary>
        private readonly char _Separator;

        /// <summary>Начальное положение в буфере</summary>
        private readonly int _StartIndex;

        /// <summary>Длина подстроки для анализа</summary>
        private readonly int _Length;

        private readonly bool _SkipEmpty;

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Str">Исходный фрагмент строки</param>
        /// <param name="Separator">Символ-разделитель фрагментов строки</param>
        public TokenizerSingleChar(StringPtr Str, char Separator) : this(Str.Source, Separator, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separator">Символ-разделитель фрагментов строки</param>
        public TokenizerSingleChar(string Buffer, char Separator) : this(Buffer, Separator, 0, Buffer.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separator">Символ-разделитель фрагментов строки</param>
        /// <param name="StartIndex">Индекс начала анализируемой подстроки</param>
        /// <param name="Length">Длина анализируемой подстроки</param>
        /// <param name="SkipEmpty">Пропускать пустые строковые фрагменты</param>
        public TokenizerSingleChar(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmpty = false)
        {
            _Buffer = Buffer;
            _Separator = Separator;
            _StartIndex = StartIndex;
            _Length = Length;
            _SkipEmpty = SkipEmpty;
        }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param name="Skip">Пропускать, или нет</param>
        /// <returns>Перечислитель строковых фрагментов с изменённым режимом пропуска строковых фрагментов</returns>
        public TokenizerSingleChar SkipEmpty(bool Skip = true) => new(_Buffer, _Separator, _StartIndex, _Length, Skip);

        /// <summary>Сформировать перечислитель строковых фрагментов</summary>
        /// <returns>Перечислитель строковых фрагментов</returns>
        public TokenEnumerator GetEnumerator() => new(_Buffer, _Separator, _StartIndex, _Length, _SkipEmpty);

        /// <summary>Перечислитель строковых фрагментов</summary>
        public ref struct TokenEnumerator
        {
            /// <summary>Строковый буфер</summary>
            private readonly string _Buffer;

            /// <summary>Символ-разделитель</summary>
            private readonly char _Separator;

            /// <summary>Начальное положение в буфере</summary>
            private readonly int _StartIndex;

            /// <summary>Длина подстроки для анализа</summary>
            private readonly int _Length;

            /// <summary>Пропускать пустые фрагменты</summary>
            private readonly bool _SkipEmpty;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos;

            /// <summary>Инициализация нового перечислителя строковых фрагментов</summary>
            /// <param name="Buffer">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Начальное положение в строковом буфере</param>
            /// <param name="Length">Длина подстроки для анализа</param>
            /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
            public TokenEnumerator(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmpty)
            {
                _Buffer = Buffer;
                _Separator = Separator;
                _StartIndex = StartIndex;
                _CurrentPos = StartIndex;
                _Length = Length;
                _SkipEmpty = SkipEmpty;

                Current = default;
            }

            /// <summary>Текущий фрагмент строки</summary>
            public StringPtr Current { get; private set; }

            /// <summary>Перемещение к следующему фрагменту</summary>
            /// <returns>Истина, если перемещение выполнено успешно</returns>
            public bool MoveNext()
            {
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return true;
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;
            }

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <returns>Следующий фрагмент строки</returns>
            /// <exception cref="InvalidOperationException">Возникает в случае отсутствия возможности выделить следующий фрагмент строки</exception>
            public StringPtr MoveNextOrThrow() => MoveNext()
                ? Current
                : throw new InvalidOperationException($"Невозможно получить следующий фрагмент строки после разделителя {_Separator}");

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <typeparam name="TException">Генерируемое исключение в случае отсутствия возможности перемещения к следующей подстроке</typeparam>
            /// <returns>Следующий фрагмент строки</returns>
            public StringPtr MoveNextOrThrow<TException>() where TException : Exception, new() => MoveNext()
                ? Current
                : throw new TException();

            /// <summary>Найти следующую подстроку</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Найденная подстрока</returns>
            private static StringPtr GetNext(string Str, char Separator, int StartIndex, int EndIndex)
            {
                if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                var index = NextIndex(Str, Separator, StartIndex, EndIndex);
                return index < 0
                    ? new(Str, StartIndex, EndIndex - StartIndex)
                    : new(Str, StartIndex, index - StartIndex);
            }

            /// <summary>Индекс следующего разделителя в строке в заданном диапазоне</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Индекс искомого символа в подстроке, либо -1, если его найдено не было</returns>
            private static int NextIndex(string Str, char Separator, int StartIndex, int EndIndex)
            {
                var str_length = Str.Length;
                for (var i = StartIndex; i < EndIndex && i < str_length; i++)
                    if (Separator == Str[i])
                        return i;

                return -1;
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="provider">Формат представления вещественного числа</param>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(IFormatProvider provider, out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(provider, out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(provider, out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в целое число</summary>
            /// <param name="value">Результат преобразования, либо 0, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в целое число выполнено успешно</returns>
            public bool TryParseNextAsInt32(out int value)
            {
                value = 0;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseAsInt32(out value);
                }

                var str = _Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseAsInt32(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в <see cref="bool"/> значение</summary>
            /// <param name="value">Результат преобразования, либо <c>false</c>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в <see cref="bool"/> значение выполнено успешно</returns>
            public bool TryParseNextAsBool(out bool value)
            {
                value = default;
                if (!MoveNext()) return false;
                if (Current.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    value = true;
                    return true;
                }

                if (Current.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    value = false;
                    return true;
                }

                return false;
            }

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в целое число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator int(TokenEnumerator Enumerator) => Enumerator.Current.ParseAsInt32();

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в вещественное число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator double(TokenEnumerator Enumerator) => Enumerator.Current.ParseDouble();
        }
    }

    /* --------------------------------------------------------------------------------------- */

    /// <inheritdoc />
    public override bool Equals(object? obj) => false;

    //public override bool Equals(object? obj) => obj is StringPtr other && 
    //    other.Source == Source &&
    //    other.Length == Length && 
    //    other.Pos == Pos;

    /// <inheritdoc />
    public override string ToString() => Pos == 0 && Length == Source.Length
        ? Source
        : Source.Substring(Pos, Length);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = 397;
        var str = Source;
        unchecked
        {
            for (var (i, end) = (Pos, Pos + Length); i <= end; i++)
                hash = hash * 397 ^ str[i].GetHashCode();

            hash = hash * 397 ^ Pos.GetHashCode();
            hash = hash * 397 ^ Length.GetHashCode();
            return hash;
        }
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Преобразование подстроки в массив символов</summary>
    /// <returns>Массив символов подстроки</returns>
    public char[] ToCharArray()
    {
        var result = new char[Length];
        Source.CopyTo(Pos, result, 0, result.Length);
        return result;
    }

    /// <summary>Получить перечислитель символов подстроки</summary>
    /// <returns>Структура перечислителя символов подстроки</returns>
    public CharEnumerator GetEnumerator() => new(Source, Pos, Length);

    /// <summary>Перечислитель символов подстроки, необходимый для использования в цикле <c>foreach</c></summary>
    public ref struct CharEnumerator
    {
        /// <summary>Исходная строка</summary>
        private readonly string _Str;

        /// <summary>Начальное положение подстроки</summary>
        private readonly int _Pos;

        /// <summary>Длина подстроки</summary>
        private readonly int _Length;

        /// <summary>Текущий индекс в подстроке</summary>
        private int _Index;

        /// <summary>Текущий символ подстроки</summary>
        public char Current { get; private set; }

        /// <summary>Инициализация нового перечислителя подстроки</summary>
        /// <param name="Str">Исходная строка</param>
        /// <param name="Pos">Начальное положение подстроки</param>
        /// <param name="Length">Длина подстроки</param>
        public CharEnumerator(string Str, int Pos, int Length)
        {
            Current = '0';
            _Index = 0;

            _Str = Str;
            _Pos = Pos;
            _Length = Length;
        }

        /// <summary>Смещение перечислителя к следующему символу</summary>
        /// <returns>Истина, если текущее положение символа находится в пределах подстроки</returns>
        public bool MoveNext()
        {
            if (_Index >= _Length) return false;

            Current = _Str[_Pos + _Index];
            _Index++;
            return true;
        }
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного преобразования строки в фрагмент строки</summary>
    /// <param name="Source">Исходная строка</param>
    public static implicit operator StringPtr(string Source) => new(Source);

    /// <summary>Оператор неявного преобразования фрагмента строки в строку</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static implicit operator string(StringPtr Ptr) => Ptr.ToString();

    /// <summary>Оператор явного преобразования фрагмента строки в целое число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator int(StringPtr Ptr) => Ptr.ParseAsInt32();

    /// <summary>Оператор явного преобразования фрагмента строки в вещественное число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator double(StringPtr Ptr) => Ptr.ParseDouble();

    /* --------------------------------------------------------------------------------------- */
}