#nullable enable
using System.Text;

namespace MathCore.CSV;

/// <summary>Парсер строк с разделителем</summary>
public static class CSVParser
{
    private static readonly char[] __TrimChars = { ' ', '"' };

    /// <summary>Разбор строки на элементы с учётом возможности экранирования разделителей кавычками</summary>
    /// <param name="Line">Разделяемая строка</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="Trim">Обрезать служебные символы в начале и конце строки</param>
    /// <returns>Перечисление элементов строки</returns>
    public static IEnumerable<string> ParseLine(string Line, char Separator = ',', bool Trim = false)
    {
        const char quote = '"';
        for (int start = 0, end; start < Line.Length; start = end + 1)
        {
            end = Line.IndexOf(Separator, start + 1);
            if (end < 0)
                end = Line.Length;

            if (Line.IndexOf(quote, start, end - start) is > 0 and var start_quote_index)
            {
                var close_quote_index = Line.IndexOf(quote, start_quote_index + 1);
                if (close_quote_index < 0) yield break;
                end = Line.IndexOf(Separator, close_quote_index + 1);
            }

            var result = Line[start..end];
            yield return Trim && result.Length > 2 
                ? result.Trim(__TrimChars) 
                : result;
        }
    }

    /// <summary>Объединение строк в одну с добавлением разделителя между элементами с возможностью экранирования разделителей в строках-компонентах</summary>
    /// <param name="Values">Строки, объединяемые в единую строку</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Строка, Составленная из указанного набора элементов</returns>
    public static string CreateLine(IEnumerable<string> Values, char Separator = ',')
    {
        var result = new StringBuilder();
        foreach (var value in Values)
        {
            if (value.IndexOf(Separator) < 0)
                result.Append(value);
            else
            {
                result.Append('"');
                result.Append(value);
                result.Append('"');
            }
            result.Append(Separator);
        }

        if (result.Length > 0)
            result.Length--;
        return result.ToString();
    }
}