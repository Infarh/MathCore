using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Text;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Методы-расширения класса <see cref="T:System.String">строк</see></summary>
    public static class StringExtentions
    {
        /// <summary>Сжать строку в последовательность байт</summary>
        /// <param name="str">Сжимаемая строка</param>
        /// <returns>Сжатая строка в виде последовательности байт</returns>
        [NotNull]
        public static byte[] Compress([NotNull] this string str)
        {
            using var input_stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            using var output_stream = new MemoryStream();
            using var g_zip_stream = new GZipStream(output_stream, CompressionMode.Compress);
            input_stream.CopyTo(g_zip_stream);

            return output_stream.ToArray();
        }

        /// <summary>Разархивировать последовательность байт в строку</summary>
        /// <param name="bytes">Сжатая последовательность бай, содержащая строку</param>
        /// <returns>Разжатая последовательность байт в строковом представлении</returns>
        [NotNull]
        public static string DecompressAsString([NotNull] this byte[] bytes)
        {
            using var input_stream = new MemoryStream(bytes);
            using var output_stream = new MemoryStream();
            using var g_zip_stream = new GZipStream(input_stream, CompressionMode.Decompress);
            g_zip_stream.CopyTo(output_stream);

            return Encoding.UTF8.GetString(output_stream.ToArray());
        }

        [NotNull]
        public static string JoinStrings([NotNull] this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this string text, Encoding encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeSHA256();

        [NotNull]
        public static byte[] ComputeMD5([NotNull] this string text, [CanBeNull] Encoding encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeMD5();

        /// <summary>Перечисление подстрок, разделяемых указанным строковым шаблоном</summary>
        /// <param name="Str">Разбиваемая строка</param>
        /// <param name="EndPattern">Строковый шаблон разбиения</param>
        /// <returns>Перечисление подстрок</returns>
        [ItemNotNull]
        public static IEnumerable<string> FindBlock([CanBeNull] this string Str, string EndPattern)
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

        /// <summary>Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения</summary>
        /// <param name="Str">Входная строка</param>
        /// <param name="Offset">Смещеине во входной строке начала поиска - в конце работы метода соответствует месту окончания поиска</param>
        /// <param name="Open">Шаблон начала подстроки</param>
        /// <param name="Close">Шаблон окончания подстроки</param>
        /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
        /// <exception cref="FormatException">
        /// Если шаблон завершения строки на нейден, либо если количество шаблонов начала строки превышает 
        /// количество шаблонов окончания во входной строке
        /// </exception>
        [CanBeNull]
        public static string GetBracketText([NotNull] this string Str, ref int Offset, [NotNull] string Open = "(", string Close = ")")
        {
            var Start = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
            if(Start == -1) return null;
            var Stop = Str.IndexOf(Close, Start + 1, StringComparison.Ordinal);
            if(Stop == -1) throw new FormatException();
            var start = Start;
            do
            {
                start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
                if(start != -1 && start < Stop)
                    Stop = Str.IndexOf(Close, Stop + 1, StringComparison.Ordinal);
            } while(start != -1 && start < Stop);
            if(Stop == -1 || Stop < Start) throw new FormatException();
            Offset = Stop + Close.Length;
            Start += Open.Length;
            return Str.Substring(Start, Stop - Start);
        }

        [CanBeNull]
        public static string GetBracketText(
            [NotNull] this string Str, 
            ref int Offset,
            [NotNull]
            string Open, 
            string Close,
            [NotNull] out string TextBefore, 
            [CanBeNull] out string TextAfter)
        {
            TextAfter = null;
            var Start = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
            if(Start == -1)
            {
                TextBefore = Str.Substring(Offset, Str.Length - Offset);
                return null;
            }
            var Stop = Str.IndexOf(Close, Start + 1, StringComparison.Ordinal);
            if(Stop == -1) throw new FormatException();
            var start = Start;
            do
            {
                start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
                if(start != -1 && start < Stop)
                    Stop = Str.IndexOf(Close, Stop + 1, StringComparison.Ordinal);
            } while(start != -1 && start < Stop);
            if(Stop == -1 || Stop < Start) throw new FormatException();
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
        public static bool IsNullOrEmpty([CanBeNull] this string Str) => string.IsNullOrEmpty(Str);

        /// <summary>Строка присутствует и не пуста</summary>
        /// <param name="Str">Проверяемая строка</param>
        /// <returns>Истина, если трокане  пуста, и если передана ненулевая ссылка</returns>
        [DST, Pure]
        public static bool IsNotNullOrEmpty([CanBeNull] this string Str) => !string.IsNullOrEmpty(Str);

        [DST, Pure]
        public static bool IsNullOrWhiteSpace([CanBeNull] this string Str) => string.IsNullOrWhiteSpace(Str);

        [DST, Pure]
        public static bool IsNotNullOrWhiteSpace([CanBeNull] this string Str) => !string.IsNullOrWhiteSpace(Str);

        /// <summary>Удаление символов в начале строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <param name="symbols">Перечень удаляемых символов</param>
        /// <returns>Новая строка с удалёнными символами в начале</returns>
        [NotNull]
        public static string ClerSymbolsAtBegin([NotNull] this string str, params char[] symbols)
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
        [NotNull]
        public static string ClerSymbolsAtEnd([NotNull] this string str, params char[] symbols)
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
        [NotNull]
        public static string ClearSymbolsAtBeginAndEnd([NotNull] this string str, params char[] symbols) => str.ClerSymbolsAtBegin(symbols).ClerSymbolsAtEnd(symbols);

        /// <summary>Удаление служебных символов в начале и конце строки</summary>
        /// <param name="str">Обрабатываемая строка</param>
        /// <returns>Новая строка с удалёнными служебными символами в началеи конце</returns>
        [NotNull]
        public static string ClearSystemSymbolsAtBeginAndEnd([NotNull] this string str) => str.ClearSymbolsAtBeginAndEnd(' ', '\n', '\r');

        /// <summary>Проверка на пустоту строки</summary>
        /// <param name="str">Проверяемая строка</param>
        /// <param name="ParameterName">Имя параметра, добавляемое в исключение в случае его генерации</param>
        /// <param name="Message">Сообщение, добавляемое в исключение в случае его генерации</param>
        /// <exception cref="ArgumentNullException">Если переданна пустая ссылка на строку <paramref name="str"/></exception>
        /// <exception cref="ArgumentException">Если переданная строка <paramref name="str"/> является пустой</exception>
        /// <returns>Строка, гарантированно не являющаяся пустой</returns>
        [NotNull]
        public static string NotEmpty([CanBeNull] this string str, string ParameterName, string Message = "Передана пустая строка") =>
            string.IsNullOrEmpty(str ?? throw new ArgumentNullException(ParameterName))
                ? throw new ArgumentException(Message ?? "Передана пустая строка", ParameterName)
                : str;

        [NotNull]
        public static string NotNull([CanBeNull] this string str, [CanBeNull] string Message = null) => str ?? throw new InvalidOperationException(Message ?? "Отсусттвует ссылка на объект");
    }
}
