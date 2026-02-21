using System.Text;

namespace MathCore.CSV;

/// <summary>
/// Парсер для разбора и формирования строк в формате CSV с поддержкой экранирования
/// </summary>
/// <remarks>
/// Класс предоставляет статические методы для:
/// - Разбора строки на отдельные элементы с учётом кавычек для экранирования разделителей
/// - Формирования CSV-строки из набора элементов
/// 
/// Особенности:
/// - Поддерживает экранирование разделителей и кавычек
/// - Удаляет служебные пробелы и кавычки при опции Trim
/// - Корректно обрабатывает пустые значения и значения с кавычками
/// </remarks>
public static class CSVParser
{
    private static readonly char[] __TrimChars = [' ', '"'];

    /// <summary>
    /// Разбить CSV-строку на элементы с поддержкой экранирования разделителей кавычками
    /// </summary>
    /// <param name="Line">Строка для разбора</param>
    /// <param name="Separator">Символ-разделитель (по умолчанию ',')</param>
    /// <param name="Trim">true = удалить пробелы и кавычки из начала и конца элементов</param>
    /// <returns>Перечисление элементов строки</returns>
    /// <remarks>
    /// Алгоритм:
    /// 1. Ищет каждый разделитель в строке
    /// 2. Если внутри значения обнаружена кавычка, ищет закрывающую кавычку
    /// 3. Продолжает поиск разделителя после закрывающей кавычки
    /// 4. Опционально удаляет пробелы и кавычки из результата
    /// 
    /// Примеры:
    /// - "a,b,c" -> ["a", "b", "c"]
    /// - "a,\"b,c\",d" -> ["a", "\"b,c\"", "d"]
    /// - "a, \"hello\", c" (с Trim) -> ["a", "hello", "c"]
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var line = "Name,\"John, Jr.\",Age";
    /// var items = CSVParser.ParseLine(line, ',', false);
    /// // Результат: ["Name", "\"John, Jr.\"", "Age"]
    /// ]]>
    /// </example>
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

    /// <summary>
    /// Создать CSV-строку из набора элементов с экранированием разделителей
    /// </summary>
    /// <param name="Values">Элементы для объединения</param>
    /// <param name="Separator">Символ-разделитель (по умолчанию ',')</param>
    /// <returns>Сформированная CSV-строка</returns>
    /// <remarks>
    /// Алгоритм:
    /// 1. Для каждого элемента проверяет наличие разделителя
    /// 2. Если разделитель найден, обёртывает значение в кавычки
    /// 3. Присоединяет элементы к результату с разделителем
    /// 4. Удаляет последний разделитель из результата
    /// 
    /// Примеры:
    /// - ["a", "b", "c"] -> "a,b,c"
    /// - ["a", "b,c", "d"] -> "a,\"b,c\",d"
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var items = new[] { "Name", "John, Jr.", "Age" };
    /// var line = CSVParser.CreateLine(items, ',');
    /// // Результат: "Name,\"John, Jr.\",Age"
    /// ]]>
    /// </example>
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