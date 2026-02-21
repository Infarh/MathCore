// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global

namespace MathCore;

/// <summary>Указатель на позицию в строке</summary>
public readonly ref partial struct StringPtr
{
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
        var open_length = Open.Length;
        var close_length = Close.Length;
        var length = Length;
        var pos = Pos;
        var source = Source;
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
        if (Source.LastIndexOf(c, Pos + Length - 1, Length) is >= 0 and var index)
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

    /// <summary>
    /// Определяет 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public bool Equals(double x) => Length > 0 && TryParseDouble() == x;

    public bool Equals(char c) => Length == 1 && char.Equals(this[0], c);

    public bool Equals(char c, StringComparison Comparison)
    {
        if (Length != 1) return false;

        if (Comparison is StringComparison.OrdinalIgnoreCase or StringComparison.InvariantCultureIgnoreCase or StringComparison.CurrentCultureIgnoreCase)
            return char.Equals(char.ToUpper(this[0]), char.ToUpper(c));
        return Equals(c);
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Получить имя (ключ) из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая имя (ключ) из пары ключ-значение</returns>
    public StringPtr GetName(char Separator = '=') => SubstringBefore(Separator);

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
}