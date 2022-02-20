#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;

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
    public bool IsEmpty => Length == 0;

    /// <summary>Индекс символа в подстроке</summary>
    /// <param name="index">Индекс в подстроке</param>
    /// <returns>Символ по указаному положению</returns>
    public char this[int index] => Source[Pos + index];

    /// <summary>Новая подстрока</summary>
    /// <param name="index">Индекс начала</param>
    /// <param name="length">Длина</param>
    /// <returns>Указатель на новое положение</returns>
    public StringPtr this[int index, int length] => new(Source, Pos + index, length);

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


    /// <summary>Заканчивается ли подстрока с указанной строки</summary>
    /// <param name="Str">Строка, на которую должна заканчиваться текущая подстрока</param>
    /// <returns>Истина, если текущая подстрока заканчивается на указанную строку</returns>
    public bool EndWith(string Str) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str, 0, Str.Length) == 0;

    /// <summary>Заканчивается ли подстрока с указанной строки</summary>
    /// <param name="Str">Строка, на которую должна заканчиваться текущая подстрока</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Истина, если текущая подстрока заканчивается на указанную строку</returns>
    public bool EndWith(string Str, StringComparison Comparison) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str, 0, Str.Length, Comparison) == 0;

    /// <summary>Начинается ли текущая подстрока с указанной подстроки</summary>
    /// <param name="Str">Подстрока, с которой должна начинаться текущая подстрока</param>
    /// <returns>Истина, если в начале текущей подстроки содержится указанная подстрока</returns>
    public bool EndWith(StringPtr Str) => Length >= Str.Length && string.Compare(Source, Pos + Length - Str.Length, Str.Source, Str.Pos, Str.Length) == 0;

    /// <summary>Начинается ли текущая подстрока с указанной подстроки</summary>
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

    /// <summary>Индекс первого вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <returns>Индекс первого вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int IndexOf(string str) => IndexOf(str, StringComparison.Ordinal);

    /// <summary>Индекс первого вхождения строки в подстроку</summary>
    /// <param name="str">Искомая строка</param>
    /// <param name="Comparison">Способ сравнения строк</param>
    /// <returns>Индекс первого вхождения указанной строки в подстроке, либо -1 в случае её отсутствия</returns>
    public int IndexOf(string str, StringComparison Comparison) =>
        str.Length <= Length && str.IndexOf(str, Pos, Length, Comparison) is >= 0 and var index
            ? index - Pos
            : -1;

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

    public static bool operator ==(StringPtr ptr, string str) => ptr.Equals(str);
    public static bool operator !=(StringPtr ptr, string str) => !(ptr == str);

    public static bool operator ==(string str, StringPtr ptr) => ptr.Equals(str);
    public static bool operator !=(string str, StringPtr ptr) => !(ptr == str);

    public static bool operator >(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) > 0;
    public static bool operator <(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) < 0;

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

        if (length == 0 || str[start + length] is '.' or ',')
        {
            value = double.NaN;
            return false;
        }

        //var format = NumberFormatInfo.GetInstance(Provider);

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

    /* --------------------------------------------------------------------------------------- */

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != typeof(StringPtr))
            return false;

        var other = (StringPtr)obj!;
        return other.Source == Source && other.Length == Length && other.Pos == Pos;
    }

    public override string ToString() => Source.Substring(Pos, Length);

    public override int GetHashCode()
    {
        var hash = Source.GetHashCode();
        unchecked
        {
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

    public static implicit operator StringPtr(string Source) => new(Source);

    public static implicit operator string(in StringPtr Ptr) => Ptr.ToString();

    /* --------------------------------------------------------------------------------------- */
}