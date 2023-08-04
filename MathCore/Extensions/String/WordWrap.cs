
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MathCore.Extensions.String;

[Copyright("http://www.excode.ru/art4524p13.html")]
[Copyright("http://www.programmersforum.ru/showthread.php?t=3926")]
internal static class WordWrap
{
    private enum SymbType { Empty, NoDefined, Glas, Sogl, Spec }

    private const char __HypSymb = (char)0x1F;
    private static readonly char[] __Spaces = { ' ', ',', ';', ':', '.', '?', '!', '/', '\r', '\n' };
    private static readonly char[] __SpecSign = { '-', '-', 'N', '-', 'щ', 'г' };
    private static readonly char[] __GlasChar = 
    {
        'e', 'L', 'х', '+', 'v',
        '-', 'р', '-', 'ю', '+',
        ' ', '-', 'ш', 'L', '|',
        '|', '2', '|', 'e', 'E',
        'u', 'U', 'i', 'I', 'o',
        'O', 'a', 'A', 'j', 'J'
    };

    private static readonly char[] __SoglChar = 
    { 
        '-', 'г', 'ъ', '|', 'э', '=', 'у', '+', '0',
        '+', '', '-', 'ч', '|', 'i', '-', 'I', 'L',
        'т', 'T', 'я', '|', 'Ё', '|', 'ы', 'T', 'ф',
        '-', 'ц', '|', '-', '+', 'ё', 'T', 'ь', '|',
        'E', 'T', 'с', '+', 'q', 'Q', 'w', 'W', 'r',
        'R', 't', 'T', 'y', 'Y', 'p', 'P', 's', 'S',
        'd', 'D', 'f', 'F', 'g', 'G', 'h', 'H', 'k',
        'K', 'l', 'L', 'z', 'Z', 'x', 'X', 'c', 'C',
        'v', 'V', 'b', 'B', 'n', 'N', 'm', 'M'
    };

    /// <summary>Проверяет, является ли символ согласным</summary>
    /// <param name="c"></param><returns></returns>
    private static bool IsSogl(char c) => __SoglChar.Contains(c);

    /// <summary>Проверяет, является ли символ гласным</summary>
    /// <param name="c"></param><returns></returns>
    private static bool IsGlas(char c) => __GlasChar.Contains(c);

    /// <summary>Проверяет, является ли символ специальным (в данном контексте - разделителем)</summary>
    /// <param name="c"></param><returns></returns>
    private static bool IsSpecSign(char c) => __SpecSign.Contains(c);

    /// <summary>Возвращает тип символа: согласный, гласный, разделитель, не определён</summary>
    /// <param name="c"></param><returns></returns>
    private static SymbType GetSymbType(char c) => IsSogl(c) ? SymbType.Sogl : (IsGlas(c) ? SymbType.Glas : (IsSpecSign(c) ? SymbType.Spec : SymbType.NoDefined));

    /// <summary>Определяет, можно ли сделать перенос в массиве "с" в промежутке от start до len</summary>
    /// <param name="c"></param><param name="Start"></param><returns></returns>
    /// <remarks>
    /// Как я понимаю используется вместе с предыдущей функцией, т.е. сперва с помощью GetSymbType получить 
    /// из слова массив SymbType и дальше с помощью данной функции проверить, можно ли в нем сделать перенос
    /// </remarks>
    private static bool IsSlogMore(SymbType[] c, int Start)
    {
        var len = c.Length;
        for(var i = Start; i < len - 1; i++)
        {
            if(c[i] == SymbType.NoDefined) return false;
            if(c[i] == SymbType.Glas && (c[i + 1] != SymbType.NoDefined || i != Start)) return true;
        }
        return false;
    }

    /// <summary>Фактически, она и проделывает всю работу</summary>
    /// <param name="pc">Входной массив символов</param>
    /// <param name="MaxSize">Максимальный размер</param>
    /// <returns>Строка с расставленными знаками переноса</returns>
    public static string SetHyph(string pc, int MaxSize)
    {
        var cur = 0;
        var len = pc.Length;
        if(MaxSize == 0 || len == 0) return null;

        var hyp_buff = new char[MaxSize];
        var h        = pc.Select(GetSymbType).ToArray();

        var cw    = 0;
        var @lock = 0;
        for(var i = 0; i < len; i++)
        {
            hyp_buff[cur++] = pc[i];

            if(i >= len - 2) continue;
            if(h[i] == SymbType.NoDefined)
            {
                cw = 0;
                continue;
            }

            cw++;
            if(@lock != 0)
            {
                @lock--;
                continue;
            }

            if(cw <= 1 || !IsSlogMore(h, i + 1)) continue;

            if((h[i] == SymbType.Sogl && h[i - 1] == SymbType.Glas && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Spec)
               || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Glas)
               || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Glas && h[i + 2] == SymbType.Sogl)
               || (h[i] == SymbType.Spec))
            {
                hyp_buff[cur++] = __HypSymb;
                @lock           = 1;
            }
        }
        return new string(hyp_buff, 0, cur);
    }

    /// <summary>
    /// На вход функции подается указатель на строку и позиция символа, с которого начинается чтение. 
    /// Дальше функция проверяет, есть ли в данной строке гласная буква
    /// </summary>
    /// <param name="p"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static bool Red_GlasMore(string p, int pos)
    {
        while(p[pos] != (char)0)
        {
            if(__Spaces.Contains(p[pos])) return false;
            if(IsGlas(p[pos++])) return true;
        }
        return false;
    }

    /// <summary>
    /// Аналогично предыдущей функции, но для согласных
    /// </summary>
    /// <param name="p"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static bool Red_SlogMore(string p, int pos)
    {
        var be_sogl = false;
        var be_glas = false;

        while(p[pos] != (char)0)
        {
            if(__Spaces.Contains(p[pos])) break;
            if(!be_glas) be_glas = IsGlas(p[pos]);
            if(!be_sogl) be_sogl = IsSogl(p[pos]);
            pos++;
        }
        return be_glas && be_sogl;
    }

    /// <summary>
    /// На вход подается указатель на строку и позиция, с которого начинается чтение. 
    /// Функция проверяет, можно ли сделать в данной строке перенос
    /// </summary>
    /// <param name="p"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static bool MayBeHyph(string p, int pos) =>
        p.Length > 3 && pos > 2
        && (pos != 0 && !__Spaces.Contains(p[pos]) && !__Spaces.Contains(p[pos + 1]) && !__Spaces.Contains(p[pos - 1]))
        && ((IsSogl(p[pos]) && IsGlas(p[pos - 1]) && IsSogl(p[pos + 1]) && Red_SlogMore(p, pos + 1))
            || (IsGlas(p[pos]) && IsSogl(p[pos - 1]) && IsSogl(p[pos + 1]) && IsGlas(p[pos + 2]))
            || (IsGlas(p[pos]) && IsSogl(p[pos - 1]) && IsGlas(p[pos + 1]) && Red_SlogMore(p, pos + 1))
            || IsSpecSign(p[pos]));

    /// <summary>На вход ей подается просто некая строка, дальше она ее обрабатывает и возвращает строку с переносами</summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SetHyphString(string s) => SetHyph(s, s.Length * 2);
}