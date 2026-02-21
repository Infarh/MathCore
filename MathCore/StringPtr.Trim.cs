namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Символы, обрезаемые в начале и конце строки по умолчанию</summary>
    private static readonly char[] __DefaultTrimChars = [' ', '\0', '\r', '\n', '\t'];

    /// <summary>Удаление технических символов в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("  text").TrimStart(out var trimmed);
    /// ]]>
    /// </example>
    public StringPtr TrimStart(out bool Trimmed) => TrimStart(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в начале строки</summary>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("  text").TrimStart();
    /// ]]>
    /// </example>
    public StringPtr TrimStart() => TrimStart(__DefaultTrimChars);

    /// <summary>Удаление технических символов в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text  ").TrimEnd(out var trimmed);
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(out bool Trimmed) => TrimEnd(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в конце строки</summary>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text  ").TrimEnd();
    /// ]]>
    /// </example>
    public StringPtr TrimEnd() => TrimEnd(__DefaultTrimChars);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("  text  ").Trim(out var trimmed);
    /// ]]>
    /// </example>
    public StringPtr Trim(out bool Trimmed) => Trim(out Trimmed, __DefaultTrimChars);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("  text  ").Trim();
    /// ]]>
    /// </example>
    public StringPtr Trim() => Trim(__DefaultTrimChars);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart('-');
    /// ]]>
    /// </example>
    public StringPtr TrimStart(char c) => TrimStart(out _, c);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart(out var trimmed, '-');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart('-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimStart(char c1, char c2) => TrimStart(out _, c1, c2);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart('-', '+', '*');
    /// ]]>
    /// </example>
    public StringPtr TrimStart(char c1, char c2, char c3) => TrimStart(out _, c1, c2, c3);

    /// <summary>Удаление символа в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart(out var trimmed, '-', '+', '*');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart('-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimStart(params char[] c) => TrimStart(out _, c);

    /// <summary>Удаление символов в начале строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text").TrimStart(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimStart(out bool Trimmed, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (pos < end_pos && can_move) // пропускаем начальные символы
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd('-');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(char c) => TrimEnd(out _, c);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd(out var trimmed, '-');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd('-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(char c1, char c2) => TrimEnd(out _, c1, c2);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(out bool Trimmed, char c1, char c2)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while ((end_pos > pos && str[end_pos] == c1) || str[end_pos] == c2) end_pos--;

        Trimmed = end_pos != pos + len - 1;
        return Trimmed ? Substring(pos - Pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd('-', '+', '*');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(char c1, char c2, char c3) => TrimEnd(out _, c1, c2, c3);

    /// <summary>Удаление символа в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd(out var trimmed, '-', '+', '*');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(out bool Trimmed, char c1, char c2, char c3)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while ((end_pos > pos && str[end_pos] == c1) || str[end_pos] == c2 || str[end_pos] == c3) end_pos--;

        Trimmed = end_pos != pos + len - 1;
        return Trimmed ? Substring(pos - Pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление символов в конце строки</summary>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd('-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(params char[] c) => TrimEnd(out _, c);

    /// <summary>Удаление символов в конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("text--").TrimEnd(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
    public StringPtr TrimEnd(out bool Trimmed, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (end_pos > pos && can_move) // пропускаем конечные символы
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim('-');
    /// ]]>
    /// </example>
    public StringPtr Trim(char c) => Trim(out _, out _, c);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed, '-');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed_start, out var trimmed_end, '-');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim('-', '+');
    /// ]]>
    /// </example>
    public StringPtr Trim(char c1, char c2) => Trim(out _, out _, c1, c2);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed_start, out var trimmed_end, '-', '+');
    /// ]]>
    /// </example>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, char c1, char c2)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        while ((pos < end_pos && str[pos] == c1) || str[pos] == c2) pos++;
        while ((pos < end_pos && str[end_pos] == c1) || str[end_pos] == c2) end_pos--;
        TrimmedStart = pos != Pos;
        TrimmedEnd = end_pos != Pos + len - 1;
        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim('-', '+', '*');
    /// ]]>
    /// </example>
    public StringPtr Trim(char c1, char c2, char c3) => Trim(out _, out _, c1, c2, c3);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c1">Первый удаляемый символ</param>
    /// <param name="c2">Второй удаляемый символ</param>
    /// <param name="c3">Третий удаляемый символ</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed, '-', '+', '*');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed_start, out var trimmed_end, '-', '+', '*');
    /// ]]>
    /// </example>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, char c1, char c2, char c3)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;

        while ((pos < end_pos && str[pos] == c1) || str[pos] == c2 || str[pos] == c3) pos++;
        while ((pos < end_pos && str[end_pos] == c1) || str[end_pos] == c2 || str[end_pos] == c3) end_pos--;

        TrimmedStart = pos != Pos;
        TrimmedEnd = end_pos != Pos + len - 1;

        return TrimmedStart || TrimmedEnd ? Substring(pos - Pos, end_pos - pos + 1) : this;
    }

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim('-', '+');
    /// ]]>
    /// </example>
    public StringPtr Trim(params char[] c) => Trim(out _, out _, c);

    /// <summary>Удаление технических символов в начале и конце строки</summary>
    /// <param name="Trimmed">Обрезание строки было выполнено</param>
    /// <param name="c">Удаляемые символы</param>
    /// <returns>Обрезанная строка</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed, '-', '+');
    /// ]]>
    /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("--text--").Trim(out var trimmed_start, out var trimmed_end, '-', '+');
    /// ]]>
    /// </example>
    public StringPtr Trim(out bool TrimmedStart, out bool TrimmedEnd, params char[] c)
    {
        var pos = Pos;
        var len = Math.Min(Length, Source.Length);
        var end_pos = pos + len - 1;
        var str = Source;
        var can_move = true;
        while (pos < end_pos && can_move) // пропускаем начальные символы
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
        while (pos < end_pos && can_move) // пропускаем конечные символы
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
}
