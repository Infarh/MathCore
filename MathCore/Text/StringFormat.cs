using System.Text;
using System.Text.RegularExpressions;

namespace MathCore.Text;

/// <summary>
/// Парсер строк по пользовательскому шаблону с поддержкой именованных полей<br/>
/// Позволяет извлекать данные из строк, используя формат с placeholder'ами вида {FieldName}
/// </summary>
/// <remarks>
/// StringFormat преобразует пользовательский шаблон в регулярное выражение и извлекает 
/// значения именованных полей из входной строки.<br/>
/// <br/>
/// <b>Синтаксис шаблона:</b><br/>
/// - <c>{FieldName}</c> — обязательное поле (минимум 1 символ)<br/>
/// - <c>{FieldName?}</c> — необязательное поле (может быть пустым)<br/>
/// - Любой другой текст в шаблоне должен точно совпадать с входной строкой<br/>
/// <br/>
/// <b>Особенности:</b><br/>
/// - Поля извлекаются жадным образом (максимально возможное совпадение)<br/>
/// - Поддерживаются различные опции регулярных выражений<br/>
/// - Можно использовать одно и то же имя поля несколько раз<br/>
/// <br/>
/// <b>Производительность:</b><br/>
/// Регулярное выражение компилируется один раз при создании объекта,
/// что делает повторное использование экземпляра эффективным
/// </remarks>
/// <example>
/// <![CDATA[
/// // Простой пример: парсинг даты и времени
/// var format1 = new StringFormat("Date: {date}, Time: {time}");
/// var result1 = format1.Parse("Date: 2024-01-15, Time: 14:30");
/// // result1["date"] = "2024-01-15"
/// // result1["time"] = "14:30"
/// 
/// // Пример с необязательными полями
/// var format2 = new StringFormat("Name: {name}, Age: {age?}");
/// var result2 = format2.Parse("Name: John, Age: ");
/// // result2["name"] = "John"
/// // result2["age"] = ""
/// 
/// // Парсинг URL
/// var url_format = new StringFormat("https://{domain}/{path?}");
/// var url_data = url_format.Parse("https://example.com/api/users");
/// // url_data["domain"] = "example.com"
/// // url_data["path"] = "api/users"
/// 
/// // Парсинг лог-файла
/// var log_format = new StringFormat("[{level}] {timestamp}: {message}");
/// var log_entry = log_format.Parse("[ERROR] 2024-01-15 10:30:45: Connection timeout");
/// // log_entry["level"] = "ERROR"
/// // log_entry["timestamp"] = "2024-01-15 10:30:45"
/// // log_entry["message"] = "Connection timeout"
/// 
/// // Использование опций регулярных выражений (игнорировать регистр)
/// var format3 = new StringFormat(
///     "Status: {status}", 
///     RegexOptions.IgnoreCase
/// );
/// var result3 = format3.Parse("STATUS: OK");
/// // result3["status"] = "OK"
/// 
/// // Обработка неуспешного парсинга
/// var format4 = new StringFormat("Value: {value}");
/// var result4 = format4.Parse("Invalid string");
/// // result4.Count == 0 (пустой словарь)
/// ]]>
/// </example>
#if NET8_0_OR_GREATER
public partial class StringFormat(string Format, RegexOptions Opts = default)
#else
public class StringFormat(string Format, RegexOptions Opts = default)
#endif
{
#if NET8_0_OR_GREATER
    [GeneratedRegex(@"\{(?<Name>[^}]+)\}", RegexOptions.Compiled)]
    private static partial Regex GetValueInfoRegex();

    private static readonly Regex __ValueInfoRegex = GetValueInfoRegex();
#else
    private static readonly Regex __ValueInfoRegex = new(@"\{(?<Name>[^}]+)\}", RegexOptions.Compiled);
#endif

    private readonly Regex _Regex = GetRegex(Format, Opts);

    private static Regex GetRegex(string format, RegexOptions opts)
    {
        var pattern = new StringBuilder();

        var last_pos = 0;
        foreach (Match match in __ValueInfoRegex.Matches(format))
        {
            pattern.Append(format[last_pos..match.Index]);
            last_pos = match.Index + match.Length;

            var field_name = match.Groups["Name"].Value
#if NET8_0_OR_GREATER
                .AsSpan();
#else
                .AsStringPtr();
#endif

            var is_required = field_name[^1] == '?';
            if (is_required)
                field_name = field_name.TrimEnd('?');

            pattern.Append($"(?<{field_name}>.{(is_required ? '*' : '+')}?)");
        }

        pattern.Append(format[last_pos..]);

        return new(pattern.ToString(), opts);
    }

    /// <summary>Парсит входную строку согласно заданному шаблону и извлекает значения именованных полей</summary>
    /// <param name="str">Строка для парсинга</param>
    /// <returns>
    /// Словарь, где ключ — имя поля из шаблона, значение — извлеченная подстрока.
    /// Если строка не соответствует шаблону, возвращается пустой словарь
    /// </returns>
    /// <remarks>
    /// Метод выполняет сопоставление входной строки с регулярным выражением,
    /// сгенерированным из шаблона.<br/>
    /// <br/>
    /// <b>Важно:</b><br/>
    /// - При неуспешном парсинге возвращается пустой словарь (не null)<br/>
    /// - Все значения возвращаются как строки, необходима дополнительная конвертация типов<br/>
    /// - Если поле используется в шаблоне несколько раз, в словарь попадет только первое вхождение
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var format = new StringFormat("ID: {id}, Name: {name}, Score: {score}");
    /// 
    /// // Успешный парсинг
    /// var result = format.Parse("ID: 42, Name: Alice, Score: 95.5");
    /// if (result.Count > 0)
    /// {
    ///     var id = int.Parse(result["id"]);           // 42
    ///     var name = result["name"];                   // "Alice"
    ///     var score = double.Parse(result["score"]);   // 95.5
    /// }
    /// 
    /// // Неуспешный парсинг
    /// var failed = format.Parse("Invalid format");
    /// if (failed.Count == 0)
    /// {
    ///     Console.WriteLine("Строка не соответствует шаблону");
    /// }
    /// 
    /// // Использование LINQ для пакетной обработки
    /// var logs = new[]
    /// {
    ///     "User: john logged in at 10:00",
    ///     "User: alice logged in at 10:05",
    ///     "Invalid log entry"
    /// };
    /// 
    /// var log_format = new StringFormat("User: {user} logged in at {time}");
    /// var parsed_logs = logs
    ///     .Select(log => log_format.Parse(log))
    ///     .Where(result => result.Count > 0)
    ///     .Select(result => new 
    ///     { 
    ///         User = result["user"], 
    ///         Time = TimeSpan.Parse(result["time"]) 
    ///     })
    ///     .ToList();
    /// // Результат: 2 записи (john и alice), третья отфильтрована
    /// ]]>
    /// </example>
    public Dictionary<string, string> Parse(string str)
    {
        if (_Regex.Match(str) is not { Success: true, Groups: var values })
            return [];

#if NET8_0_OR_GREATER
        return values
            .Cast<Group>()
            .Skip(1)
            .Distinct(v => v!.Name)
            .ToDictionary(v => v.Name, v => v.Value);
#else
        var result = new Dictionary<string, string>();
        foreach (var group_name in _Regex.GetGroupNames())
            if (!result.ContainsKey(group_name))
                result.Add(group_name, values[group_name].Value);

        return result;
#endif
    }
}
