using System;
using System.Linq;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MathCore.Extensions.String
{
    [Copyright("http://www.excode.ru/art4524p13.html")]
    [Copyright("http://www.programmersforum.ru/showthread.php?t=3926")]
    internal static class WordWrap
    {
        private enum SymbType { Empty, NoDefined, Glas, Sogl, Spec }

        private const char HypSymb = (char)0x1F;
        private static readonly char[] Spaces = { ' ', ',', ';', ':', '.', '?', '!', '/', '\r', '\n' };
        private static readonly char[] SpecSign = { '-', '-', 'N', '-', 'щ', 'г' };
        private static readonly char[] GlasChar = {'e', 'L', 'х', '+', 'v', '-','р', '-', 'ю', '+', ' ', '-', 'ш', 'L', 
            '|', '|', '2', '|', 'e', 'E', 'u', 'U','i', 'I', 'o', 'O', 'a', 'A', 'j', 'J'};

        private static readonly char[] SoglChar = { '-', 'г', 'ъ', '|', 'э', '=', 'у', '+', '0', '+', '', '-', 'ч', '|',
            'i', '-', 'I', 'L', 'т', 'T', 'я', '|', 'Ё', '|', 'ы', 'T', 'ф', '-', 'ц', '|', '-', '+', 'ё', 'T', 'ь', '|', 'E', 
            'T', 'с', '+', 'q', 'Q', 'w', 'W', 'r', 'R', 't', 'T', 'y', 'Y', 'p', 'P', 's', 'S', 'd', 'D', 'f', 'F', 'g', 'G', 
            'h', 'H', 'k', 'K', 'l', 'L', 'z', 'Z', 'x', 'X', 'c', 'C', 'v', 'V', 'b', 'B', 'n', 'N', 'm', 'M' };

        /// <summary>Проверяет, является ли символ согласным</summary>
        /// <param name="c"></param><returns></returns>
        private static bool isSogl(char c) => SoglChar.Contains(c);

        /// <summary>Проверяет, является ли символ гласным</summary>
        /// <param name="c"></param><returns></returns>
        private static bool isGlas(char c) => GlasChar.Contains(c);

        /// <summary>Проверяет, является ли символ специальным (в данном контексте - разделителем)</summary>
        /// <param name="c"></param><returns></returns>
        private static bool isSpecSign(char c) => SpecSign.Contains(c);

        /// <summary>Возвращает тип символа: согласный, гласный, разделитель, не определён</summary>
        /// <param name="c"></param><returns></returns>
        private static SymbType GetSymbType(char c) => isSogl(c) ? SymbType.Sogl : (isGlas(c) ? SymbType.Glas : (isSpecSign(c) ? SymbType.Spec : SymbType.NoDefined));

        /// <summary>Определяет, можно ли сделать перенос в массиве "с" в промежутке от start до len</summary>
        /// <param name="c"></param><param name="Start"></param><returns></returns>
        /// <remarks>
        /// Как я понимаю используется вместе с предыдущей функцией, т.е. сперва с помощью GetSymbType получить 
        /// из слова массив SymbType и дальше с помощью данной функции проверить, можно ли в нем сделать перенос
        /// </remarks>
        private static bool isSlogMore(SymbType[] c, int Start)
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
            var Cur = 0;
            var len = pc.Length;
            if(MaxSize == 0 || len == 0) return null;

            var HypBuff = new char[MaxSize];
            var h = pc.Select(GetSymbType).ToArray();

            var cw = 0;
            var Lock = 0;
            for(var i = 0; i < len; i++)
            {
                HypBuff[Cur++] = pc[i];

                if(i >= len - 2) continue;
                if(h[i] == SymbType.NoDefined)
                {
                    cw = 0;
                    continue;
                }

                cw++;
                if(Lock != 0)
                {
                    Lock--;
                    continue;
                }

                if(cw <= 1 || !isSlogMore(h, i + 1)) continue;

                if((h[i] == SymbType.Sogl && h[i - 1] == SymbType.Glas && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Spec)
                    || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Sogl && h[i + 2] == SymbType.Glas)
                    || (h[i] == SymbType.Glas && h[i - 1] == SymbType.Sogl && h[i + 1] == SymbType.Glas && h[i + 2] == SymbType.Sogl)
                    || (h[i] == SymbType.Spec))
                {
                    HypBuff[Cur++] = HypSymb;
                    Lock = 1;
                }
            }
            return new string(HypBuff, 0, Cur);
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
                if(Spaces.Contains(p[pos])) return false;
                if(isGlas(p[pos++])) return true;
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
            var BeSogl = false;
            var BeGlas = false;

            while(p[pos] != (char)0)
            {
                if(Spaces.Contains(p[pos])) break;
                if(!BeGlas) BeGlas = isGlas(p[pos]);
                if(!BeSogl) BeSogl = isSogl(p[pos]);
                pos++;
            }
            return BeGlas && BeSogl;
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
                         && (pos != 0 && !Spaces.Contains(p[pos]) && !Spaces.Contains(p[pos + 1]) && !Spaces.Contains(p[pos - 1]))
                         && ((isSogl(p[pos]) && isGlas(p[pos - 1]) && isSogl(p[pos + 1]) && Red_SlogMore(p, pos + 1))
                             || (isGlas(p[pos]) && isSogl(p[pos - 1]) && isSogl(p[pos + 1]) && isGlas(p[pos + 2]))
                             || (isGlas(p[pos]) && isSogl(p[pos - 1]) && isGlas(p[pos + 1]) && Red_SlogMore(p, pos + 1))
                             || isSpecSign(p[pos]));

        /// <summary>На вход ей подается просто некая строка, дальше она ее обрабатывает и возвращает строку с переносами</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SetHyphString(string s) => SetHyph(s, s.Length * 2);
    }
}