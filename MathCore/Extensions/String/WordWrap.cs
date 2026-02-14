// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MathCore.Extensions.String;

/// <summary>Класс для расстановки переносов в строках</summary>
[Copyright("http://www.excode.ru/art4524p13.html")]
[Copyright("http://www.programmersforum.ru/showthread.php?t=3926")]
internal static class WordWrap
{
    /// <summary>Тип символа для определения возможности переноса</summary>
    private enum SymbType { Empty, NoDefined, Glas, Sogl, Spec }

    /// <summary>Символ переноса</summary>
    private const char __HypSymb = (char)0x1F;
    /// <summary>Массив символов, считающихся пробелами и разделителями</summary>
    private static readonly char[] __Spaces = [' ', ',', ';', ':', '.', '?', '!', '/', '\r', '\n'];
    /// <summary>Массив специальных символов-разделителей</summary>
    private static readonly char[] __SpecSign = ['-', '-', 'N', '-', 'щ', 'г'];
    /// <summary>Массив гласных символов</summary>
    private static readonly char[] __GlasChar =
    [
        'e', 'L', 'х', '+', 'v',
        '-', 'р', '-', 'ю', '+',
        ' ', '-', 'ш', 'L', '|',
        '|', '2', '|', 'e', 'E',
        'u', 'U', 'i', 'I', 'o',
        'O', 'a', 'A', 'j', 'J'
    ];

    /// <summary>Массив согласных символов</summary>
    private static readonly char[] __SoglChar =
    [
        '-', 'г', 'ъ', '|', 'э', '=', 'у', '+', '0',
        '+', '\u0007', '-', 'ч', '|', 'i', '-', 'I', 'L',
        'т', 'T', 'я', '|', 'Ё', '|', 'ы', 'T', 'ф',
        '-', 'ц', '|', '-', '+', 'ё', 'T', 'ь', '|',
        'E', 'T', 'с', '+', 'q', 'Q', 'w', 'W', 'r',
        'R', 't', 'T', 'y', 'Y', 'p', 'P', 's', 'S',
        'd', 'D', 'f', 'F', 'g', 'G', 'h', 'H', 'k',
        'K', 'l', 'L', 'z', 'Z', 'x', 'X', 'c', 'C',
        'v', 'V', 'b', 'B', 'n', 'N', 'm', 'M'
    ];

    /// <summary>Проверяет, является ли символ согласным</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>True, если символ согласный</returns>
    private static bool IsSogl(char c) => __SoglChar.Contains(c);

    /// <summary>Проверяет, является ли символ гласным</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>True, если символ гласный</returns>
    private static bool IsGlas(char c) => __GlasChar.Contains(c);

    /// <summary>Проверяет, является ли символ специальным (разделителем)</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>True, если символ специальный</returns>
    private static bool IsSpecSign(char c) => __SpecSign.Contains(c);

    /// <summary>Возвращает тип символа: согласный, гласный, разделитель, не определён</summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns>Тип символа</returns>
    private static SymbType GetSymbType(char c) => IsSogl(c) ? SymbType.Sogl : (IsGlas(c) ? SymbType.Glas : (IsSpecSign(c) ? SymbType.Spec : SymbType.NoDefined));

    /// <summary>Определяет, можно ли сделать перенос в массиве типов символов с позиции Start</summary>
    /// <param name="c">Массив типов символов</param>
    /// <param name="Start">Начальная позиция</param>
    /// <returns>True, если перенос возможен</returns>
    private static bool IsSlogMore(SymbType[] c, int Start)
    {
        var len = c.Length;
        for (var i = Start; i < len - 1; i++)
            switch (c[i])
            {
                case SymbType.NoDefined: return false;
                case SymbType.Glas when c[i + 1] != SymbType.NoDefined || i != Start: return true;
            }

        return false;
    }

    /// <summary>Выполняет расстановку переносов в строке</summary>
    /// <param name="pc">Входная строка</param>
    /// <param name="MaxSize">Максимальный размер выходной строки</param>
    /// <returns>Строка с расставленными знаками переноса</returns>
    public static string SetHyph(string pc, int MaxSize)
    {
        var cur = 0;
        var len = pc.Length;
        if (MaxSize == 0 || len == 0) return string.Empty;

        var hyp_buff = new char[MaxSize];
        var h = pc.Select(GetSymbType).ToArray();

        var cw = 0;
        var @lock = 0;
        for (var i = 0; i < len; i++)
        {
            hyp_buff[cur++] = pc[i];

            if (i >= len - 2) continue;
            if (h[i] == SymbType.NoDefined)
            {
                cw = 0;
                continue;
            }

            cw++;
            if (@lock != 0)
            {
                @lock--;
                continue;
            }

            if (cw <= 1 || !IsSlogMore(h, i + 1)) continue;

            if ((h[i] == SymbType.Sogl && h[i - 1] == SymbType.Glas && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Spec)
               || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Glas)
               || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Glas && h[i + 2] == SymbType.Sogl)
               || (h[i] == SymbType.Spec))
            {
                hyp_buff[cur++] = __HypSymb;
                @lock = 1;
            }
        }
        return new(hyp_buff, 0, cur);
    }

    /// <summary>Проверяет, есть ли в строке гласная буква начиная с позиции pos</summary>
    /// <param name="p">Строка для проверки</param>
    /// <param name="pos">Начальная позиция</param>
    /// <returns>True, если найдена гласная буква</returns>
    private static bool Red_GlasMore(string p, int pos)
    {
        while (p[pos] != (char)0)
        {
            if (__Spaces.Contains(p[pos])) return false;
            if (IsGlas(p[pos++])) return true;
        }
        return false;
    }

    /// <summary>Проверяет, есть ли в строке согласная буква начиная с позиции pos</summary>
    /// <param name="p">Строка для проверки</param>
    /// <param name="pos">Начальная позиция</param>
    /// <returns>True, если найдена согласная буква</returns>
    private static bool Red_SlogMore(string p, int pos)
    {
        var be_sogl = false;
        var be_glas = false;

        while (p[pos] != (char)0)
        {
            if (__Spaces.Contains(p[pos])) break;
            if (!be_glas) be_glas = IsGlas(p[pos]);
            if (!be_sogl) be_sogl = IsSogl(p[pos]);
            pos++;
        }
        return be_glas && be_sogl;
    }

    /// <summary>Проверяет, можно ли сделать перенос в строке с позиции pos</summary>
    /// <param name="p">Строка для проверки</param>
    /// <param name="pos">Позиция для проверки</param>
    /// <returns>True, если перенос возможен</returns>
    private static bool MayBeHyph(string p, int pos) =>
        p.Length > 3 && pos > 2
        && !__Spaces.Contains(p[pos]) && !__Spaces.Contains(p[pos + 1]) && !__Spaces.Contains(p[pos - 1])
        && ((IsSogl(p[pos]) && IsGlas(p[pos - 1]) && IsSogl(p[pos + 1]) && Red_SlogMore(p, pos + 1))
            || (IsGlas(p[pos]) && IsSogl(p[pos - 1]) && IsSogl(p[pos + 1]) && IsGlas(p[pos + 2]))
            || (IsGlas(p[pos]) && IsSogl(p[pos - 1]) && IsGlas(p[pos + 1]) && Red_SlogMore(p, pos + 1))
            || IsSpecSign(p[pos]));

    /// <summary>Выполняет расстановку переносов в строке</summary>
    /// <param name="s">Входная строка</param>
    /// <returns>Строка с расставленными переносами</returns>
    public static string SetHyphString(string s) => SetHyph(s, s.Length * 2);
}