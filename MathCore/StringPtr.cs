#nullable enable
using System.Globalization;

// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global

namespace MathCore;

/// <summary>Указатель на позицию в строке</summary>
public readonly ref partial struct StringPtr
{
    public delegate T Selector<out T>(StringPtr p);

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Исходная строка</summary>
    public string Source { [DST] get; }

    /// <summary>Положение начала в строке</summary>
    public int Pos { [DST] get; }

    /// <summary>Длина подстроки</summary>
    public int Length { [DST] get; }

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
    public StringPtr this[int index, int length] => Substring(index, length);

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового указателя на положение в строке</summary>
    /// <param name="Source">Исходная строка</param>
    public StringPtr(string Source) : this(Source, 0, Source.Length) { }

    /// <summary>Инициализация нового указателя на положение в строке</summary>
    /// <param name="Source">Исходная строка</param>
    /// <param name="Pos">Положение в исходной строке</param>
    /// <param name="Length">Длина подстроки</param>
    public StringPtr(string Source, int Pos, int Length)
    {
        this.Source = Source;
        this.Pos = Pos;

        if (Length < 0)
            Length = Source.Length + Length - Pos;

        this.Length = Math.Max(Math.Min(Length, Source.Length - Pos), 0);
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Подстрока</summary>
    /// <param name="Offset">Смещение в текущей подстроке</param>
    /// <returns>Новый указатель на подстроку, смещённую на указанное значение символов относительно текущей подстроки</returns>
    public StringPtr Substring(int Offset)
    {
        if (Offset < 0) return Substring(Length + Offset);
        if (Offset > Length) return new(Source, Pos + Length, 0);
        if (Offset + Length == 0) return new(Source, Pos, 0);
        return new(Source, Pos + Offset, Length - Offset);
    }

    /// <summary>Подстрока</summary>
    /// <param name="Offset">Смещение в текущей подстроке</param>
    /// <param name="Count">Число символов в новой подстроке</param>
    /// <returns>Указатель на подстроку, смещённую на указанное значение символов относительно текущей подстроки</returns>
    public StringPtr Substring(int Offset, int Count) => new(Source, Pos + Offset, Count < 0 ? Length + Count - Offset : Count);

    public StringPtr SubstringIndex(int Start, int End) => Substring(Start, Math.Max(0, End - Start));

    /// <summary>
    /// Получить подстроку до указанного символа. <br/>
    /// Если символ отсутствует, то возвращается вся строка целиком.
    /// </summary>
    /// <param name="Separator">Искомый символ-разделитель</param>
    /// <returns>Подстрока до указанного разделителя</returns>
    public StringPtr SubstringBefore(char Separator)
    {
        var index = IndexOf(Separator);
        if (index < 0) return this;
        return SubstringIndex(0, index);
    }

    /// <summary>
    /// Получить подстроку после указанного разделителя. <br/>
    /// Если разделитель не найден, то возвращается пустая подстрока.
    /// </summary>
    /// <param name="Separator">Искомый символ-разделитель</param>
    /// <returns>Подстрока после указанного символа разделителя</returns>
    public StringPtr SubstringAfter(char Separator)
    {
        var index = LastIndexOf(Separator);
        if (index < 0) return new(Source, Pos + Length, 0);
        return Substring(index + 1);
    }

    public bool IsInBracket(char Open, char Close) => Length >= 2 && Source[Pos] == Open && Source[Pos + Length - 1] == Close;

    public bool IsInBracket(string Open, string Close)
    {
        var open_length  = Open.Length;
        var close_length = Close.Length;
        var length       = Length;
        var pos          = Pos;
        var source        = Source;
        return length >= open_length + close_length
            && string.Compare(source, pos, Open, 0, open_length) == 0
            && string.Compare(source, pos + length - close_length, Close, 0, close_length) == 0;
    }

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
    public int LastIndexOf(char c)
    {
        if (Source.LastIndexOf(c, Pos + Length - 1, Length - Pos) is >= 0 and var index)
            return index - Pos;

        return -1;
    }

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
        str.Length <= Length && Source.IndexOf(str, Pos, Length, Comparison) is >= 0 and var index
            ? index - Pos
            : -1;

    /// <summary>Индекс последнего вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Индекс последнего вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int LastIndexOf(string str, StringComparison Comparison) =>
        str.Length <= Length && Source.LastIndexOf(str, Pos + Length - 1, Length, Comparison) is >= 0 and var index
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

    public bool Equals(int x)
    {
        var len = Length;
        var str = Trim();
        if (len == 0) return false;

        if (x == 0)
        {
            for (var i = 0; i < len; i++)
                if (str[i] != '0')
                    return false;

            return true;
        }

        if (x < 0)
        {
            if (str[0] != '-')
                return false;

            str = str.TrimStart('-');
            len--;
            x = -x;
        }

        len--;
        while (len >= 0)
        {
            if (str[len] - '0' != x % 10)
                return false;

            len--;
            x /= 10;
        }

        return len == -1 && x == 0;
    }

    public bool Equals(double x) => Length > 0 && TryParseDouble() == x;

    public bool Equals(char c) => Length == 1 && char.Equals(this[0], c);

    public bool Equals(char c, StringComparison Comparison)
    {
        if (Length != 1) return false;

        if(Comparison is StringComparison.OrdinalIgnoreCase or StringComparison.InvariantCultureIgnoreCase or StringComparison.CurrentCultureIgnoreCase)
            return char.Equals(char.ToUpper(this[0]), char.ToUpper(c));
        return Equals(c);
    }

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

    public static bool operator ==(StringPtr ptr, int x) => ptr.Equals(x);

    public static bool operator ==(int x, StringPtr ptr) => ptr == x;

    public static bool operator !=(StringPtr ptr, int x) => !(ptr == x);
    public static bool operator !=(int x, StringPtr ptr) => !(ptr == x);

    public static bool operator ==(StringPtr ptr, double x) => ptr.Equals(x);

    public static bool operator ==(double x, StringPtr ptr) => ptr == x;

    public static bool operator !=(StringPtr ptr, double x) => !(ptr == x);
    public static bool operator !=(double x, StringPtr ptr) => !(ptr == x);

    public static bool operator ==(StringPtr ptr, char c) => ptr.Equals(c);
    public static bool operator ==(char c, StringPtr ptr) => ptr.Equals(c);
    public static bool operator !=(StringPtr ptr, char c) => !(ptr == c);
    public static bool operator !=(char c, StringPtr ptr) => !(c == ptr);

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

    /// <summary>Получить имя (ключ) из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая имя (ключ) из пары ключ-значение</returns>
    public StringPtr GetName(char Separator = '=') => SubstringBefore(Separator);

    /// <summary>Получить значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая значение из пары ключ-значение</returns>
    public StringPtr GetValueString(char Separator = '=') => SubstringAfter(Separator);

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
    public int GetValueInt32(char Separator = '=') => GetValueString(Separator).ParseInt32();

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
    public bool TryGetValueInt32(out int value, char Separator = '=') => GetValueString(Separator).TryParseInt32(out value);

    /// <summary>Попытаться получить булево значение из пары ключ=значение</summary>
    /// <param name="value">Булево значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в булево значение выполнено успешно</returns>
    public bool TryGetValueBool(out bool value, char Separator = '=')
    {
        var str = GetValueString(Separator);
        value = false;

        if (str.IsEmpty) return false;

        if (!str.Equals("true", StringComparison.OrdinalIgnoreCase)) 
            return str.Equals("false", StringComparison.OrdinalIgnoreCase);

        value = true;
        return true;
    }

    /// <summary>Получить булево значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Булево значение из пары ключ-значение</returns>
    public bool GetValueBool(char Separator = '=') => TryGetValueBool(out var value, Separator)
        ? value
        : throw new FormatException("Строка имела неверный формат");

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Символы, обрезаемые в начале и конце строки по умолчанию</summary>
    private static readonly char[] __DefaultTrimChars = [' ', '\0', '\r', '\n', '\t'];

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
        return Trimmed ? Substring(pos - Pos) : this;
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
        return Trimmed ? Substring(pos - Pos) : this;
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
        return Trimmed ? Substring(pos - Pos) : this;
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
        return Trimmed ? Substring(pos - Pos) : this;
    }

    /// <summary>Удаление символа в конце строки</summary>
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
        var trimmed_len = 0;
        var str = Source;
        while (trimmed_len < len && str[pos + len - trimmed_len - 1] == c) trimmed_len++;

        Trimmed = trimmed_len > 0;
        return Trimmed ? Substring(0, len - trimmed_len) : this;
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

        Trimmed = end_pos != pos + len - 1;
        return Trimmed ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

        Trimmed = end_pos != pos + len - 1;
        return Trimmed ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

        Trimmed = end_pos != pos + len - 1;
        return Trimmed ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
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
        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
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

    public (string Key, int Value) SplitKeyValueInt32(char Separator)
    {
        var value = SubstringAfter(Separator).ParseInt32();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (string Key, double Value) SplitKeyValueDouble(char Separator)
    {
        var value = SubstringAfter(Separator).ParseDouble();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (string Key, double Value) SplitKeyValueDouble(char Separator, IFormatProvider format)
    {
        var value = SubstringAfter(Separator).ParseDouble(format);
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (int First, int Last) SplitIntervalInt32(char Separator)
    {
        var first = SubstringBefore(Separator).ParseInt32();
        var last = SubstringAfter(Separator).ParseInt32();
        return (first, last);
    }

    public (double First, double Last) SplitIntervalDouble(char Separator, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble();
        var last = SubstringAfter(Separator).ParseDouble();
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }

    public (double First, double Last) SplitIntervalDouble(char Separator, IFormatProvider format, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble(format);
        var last = SubstringAfter(Separator).ParseDouble(format);
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }

    /* --------------------------------------------------------------------------------------- */

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj switch
    {
        string str => Equals(str),
        int x => Equals(x),
        double x => Equals(x),
        _ => false
    };

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

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного преобразования строки во фрагмент строки</summary>
    /// <param name="Source">Исходная строка</param>
    public static implicit operator StringPtr(string Source) => new(Source);

    /// <summary>Оператор неявного преобразования фрагмента строки в строку</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static implicit operator string(StringPtr Ptr) => Ptr.ToString();

    /// <summary>Оператор явного преобразования фрагмента строки в целое число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator int(StringPtr Ptr) => Ptr.ParseInt32();
    public static explicit operator int?(StringPtr Ptr) => Ptr.ParseInt32();

    /// <summary>Оператор явного преобразования фрагмента строки в вещественное число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator double(StringPtr Ptr) => Ptr.ParseDouble();
    public static explicit operator double?(StringPtr Ptr) => Ptr.TryParseDouble();

    public static explicit operator bool(StringPtr Ptr) => (bool?)Ptr ?? throw new FormatException("Строка имела неверный формат");

    public static explicit operator bool?(StringPtr Ptr)
    {
        var ptr = Ptr.Trim();
        if (ptr.Equals(bool.TrueString, StringComparison.InvariantCulture))
            return true;
        if (ptr.Equals(bool.FalseString, StringComparison.InvariantCulture))
            return false;
        return null;
    }

    /* --------------------------------------------------------------------------------------- */
}