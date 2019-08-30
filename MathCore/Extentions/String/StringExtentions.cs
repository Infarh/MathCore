using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Методы-расширения класса <see cref="T:System.String">строк</see></summary>
    public static class StringExtentions
    {
        public static string JoinStrings(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

        public static byte[] ComputeSHA256(this string text, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.Default;
            return encoding.GetBytes(text).ComputeSHA256();
        }

        public static byte[] ComputeMD5(this string text, Encoding encoding = null)
        {
            if(encoding == null) encoding = Encoding.Default;
            return encoding.GetBytes(text).ComputeMD5();
        }

        /// <summary>Перечисление подстрок, разделяемых указанным строковым шаблоном</summary>
        /// <param name="Str">Разбиваемая строка</param>
        /// <param name="EndPattern">Строковый шаблон разбиения</param>
        /// <returns>Перечисление подстрок</returns>
        public static IEnumerable<string> FindBlock(this string Str, string EndPattern)
        {
            if(string.IsNullOrEmpty(Str)) yield break;
            var len = Str.Length;
            var PatternLen = EndPattern.Length;
            var pos = 0;
            do
            {
                var index = Str.IndexOf(EndPattern, StringComparison.Ordinal);
                yield return Str.Substring(pos, index - pos + PatternLen);
                pos = index + PatternLen + 1;
            } while(pos < len);
        }

        /// <summary>
        /// Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения
        /// </summary>
        /// <param name="Str">Входная строка</param>
        /// <param name="Offset">
        /// Смещеине во входной строке начала поиска - в конце работы метода соответствует месту окончания поиска
        /// </param>
        /// <param name="Open">Шаблон начала подстроки</param>
        /// <param name="Close">Шаблон окончания подстроки</param>
        /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
        /// <exception cref="FormatException">
        /// Если шаблон завершения строки на нейден, либо если количество шаблонов начала строки превышает 
        /// количество шаблонов окончания во входной строке
        /// </exception>
        public static string GetBracketText(this string Str, ref int Offset, string Open = "(", string Close = ")")
        {
            var Start = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
            if(Start == -1) return null;
            var Stop = Str.IndexOf(Close, Start + 1, StringComparison.Ordinal);
            if(Stop == -1)
                throw new FormatException();
            var start = Start;
            do
            {
                start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
                if(start != -1 && start < Stop)
                    Stop = Str.IndexOf(Close, Stop + 1, StringComparison.Ordinal);
            } while(start != -1 && start < Stop);
            if(Stop == -1 || Stop < Start)
                throw new FormatException();
            Offset = Stop + Close.Length;
            Start += Open.Length;
            return Str.Substring(Start, Stop - Start);
        }

        public static string GetBracketText(this string Str, ref int Offset, string Open, string Close,
            out string TextBefore, out string TextAfter)
        {
            TextAfter = null;
            var Start = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
            if(Start == -1)
            {
                TextBefore = Str.Substring(Offset, Str.Length - Offset);
                return null;
            }
            var Stop = Str.IndexOf(Close, Start + 1, StringComparison.Ordinal);
            if(Stop == -1)
                throw new FormatException();
            var start = Start;
            do
            {
                start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
                if(start != -1 && start < Stop)
                    Stop = Str.IndexOf(Close, Stop + 1, StringComparison.Ordinal);
            } while(start != -1 && start < Stop);
            if(Stop == -1 || Stop < Start)
                throw new FormatException();
            TextBefore = Str.Substring(Offset, Start - Offset);
            Offset = Stop + Close.Length;
            TextAfter = Str.Length - Offset > 0 ? Str.Substring(Offset, Str.Length - Offset) : "";
            Start += Open.Length;
            return Str.Substring(Start, Stop - Start);
        }

        /// <summary>Проверка строки на пустоту, либо нулевую ссылку</summary>
        /// <param name="Str">Проверяемая строка</param>
        /// <returns>Истина, если трока пуста, либо если передана нулевая ссылка</returns>
        [DST, Pure]
        public static bool IsNullOrEmpty(this string Str) => string.IsNullOrEmpty(Str);

        /// <summary>Строка присутствует и не пуста</summary>
        /// <param name="Str">Проверяемая строка</param>
        /// <returns>Истина, если трокане  пуста, и если передана ненулевая ссылка</returns>
        [DST, Pure]
        public static bool IsNotNullOrEmpty(this string Str) => !string.IsNullOrEmpty(Str);

        [DST, Pure]
        public static bool IsNullOrWhiteSpace(this string Str)
        {
            Contract.Ensures(Contract.Result<bool>() == string.IsNullOrWhiteSpace(Str));
            return string.IsNullOrWhiteSpace(Str);
        }

        [DST, Pure]
        public static bool IsNotNullOrWhiteSpace(this string Str)
        {
            Contract.Ensures(Contract.Result<bool>() == !string.IsNullOrWhiteSpace(Str));
            return !string.IsNullOrWhiteSpace(Str);
        }

        /// <summary>Удаление символов в начале строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <param name="symbols">Перечень удаляемых символов</param>
        /// <returns>Новая строка с удалёнными символами в начале</returns>
        public static string ClerSymbolsAtBegin(this string str, params char[] symbols)
        {
            var i = 0;
            var len = str.Length;
            while(i < len && symbols.IsContains(str[i])) i++;

            return i == 0 || len == 0 ? str : str.Substring(i, len - i);
        }

        /// <summary>Удаление символов в конце строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <param name="symbols">Перечень удаляемых символов</param>
        /// <returns>Новая строка с удалёнными символами в конце</returns>
        public static string ClerSymbolsAtEnd(this string str, params char[] symbols)
        {
            var len = str.Length;
            var i = 0;
            while(i < len && symbols.IsContains(str[len - i - 1])) i++;

            return len == 0 || i == 0 ? str : str.Substring(0, len - i);
        }

        /// <summary>Удаление символов в начале и конце строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <param name="symbols">Перечень удаляемых символов</param>
        /// <returns>Новая строка с удалёнными символами в начале и конце</returns>
        public static string ClearSymbolsAtBeginAndEnd(this string str, params char[] symbols) => str.ClerSymbolsAtBegin(symbols).ClerSymbolsAtEnd(symbols);

        /// <summary>Удаление служебных символов в начале и конце строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <returns>Новая строка с удалёнными служебными символами в началеи конце</returns>
        public static string ClearSystemSymbolsAtBeginAndEnd(this string str) => str.ClearSymbolsAtBeginAndEnd(' ', '\n', '\r');
    }
}
