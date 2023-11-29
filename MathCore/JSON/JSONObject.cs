#nullable enable
using System.Collections;
using System.Text;

namespace MathCore.JSON;

/// <summary>Парсер структуры JSON</summary>
public sealed class JSONObject : IEnumerable<JSONObject>
{
    //internal class TextOBJ
    //{
    //    public string Name { get; set; }
    //    public bool Value { get; set; }
    //}

    //private class TestObj
    //{
    //    public string Name { get; set; }
    //    public int Value { get; set; }
    //    public TextOBJ Field { get; set; }
    //}

    //internal static void Test()
    //{
    //    var test = new TestObj
    //    {
    //        Name = "Name",
    //        Value = 123,
    //        Field = new TextOBJ
    //        {
    //            Name = "ASD",
    //            Value = true
    //        }
    //    };

    //    var json = test.GetJSON()
    //        .AddField("Name", o => o.Name)
    //        .AddField("Value", o => o.Value)
    //        .AddField("Field", o => o.Field.GetJSON()
    //            .AddField("NAME", f => f.Name)
    //            .AddField("VALUE", f => f.Value))
    //        .Create();

    //    Console.WriteLine(json);
    //}

    /// <summary>Имя структуры</summary>
    private readonly string? _Name;

    /// <summary>Значение структуры</summary>
    private readonly string _Data = null!;

    /// <summary>Словарь полей структуры по ключу-имени поля</summary>
    private readonly JSONObject[]? _Fields;

    /// <summary>Имя структуры</summary>
    public string? Name => _Name;

    /// <summary>Значение структуры</summary>
    public string Data => _Data;

    /// <summary>Признак того, что структура является сложной - со вложенными полями</summary>
    public bool IsComplex => _Fields is { Length: > 0 };

    /// <summary>Перечисление имён полей структуры</summary>
    public IEnumerable<string> Fields => _Fields?.Select(f => f.Name).WhereNotNull() ?? Enumerable.Empty<string>();

    /// <summary>Перечисление полей структуры по указанному имени</summary>
    /// <param name="Field">Имя поля структуры</param>
    /// <returns>Вложенные поля с указанным именем</returns>
    public IEnumerable<JSONObject> this[string Field] => _Fields?.Where(f => string.Equals(Field, f._Name)) ?? Enumerable.Empty<JSONObject>();

    #region Конструкторы

    public JSONObject(params JSONObject[] fields) : this((IEnumerable<JSONObject>)fields) { }

    public JSONObject(IEnumerable<JSONObject> Fields)
    {
        var data                       = new StringBuilder();
        var fields                     = Fields.ForeachLazy(f => data.AppendLine($"\"{f._Name}\" : {f._Data}")).ToArray();
        if (fields.Length > 0) _Fields = fields;
    }

    /// <summary>Инициализация новой структуры JSON из строкового представления</summary>
    /// <param name="str">Строковое представление структуры JSON</param>
    public JSONObject(string str) : this(null, str) { }

    /// <summary>Инициализация новой структуры JSON из строкового представления с указанием имени</summary>
    /// <param name="Name">Имя структуры</param>
    /// <param name="str">Строковое представление структуры JSON</param>
    public JSONObject(string? Name, string str)
    {
        _Name = Name;
        _Data = str.Trim();

        if (_Data is not [ '{', .., '}' ]) return;

        str = str.Trim('{', '}', ' ');

        var fields = new List<JSONObject>();
        var len = str.Length;
        var pos = 0;
        while (pos < len)
        {
            var name = GetText(str, ref pos, "\"", "\"");

            while (pos < len && (char.IsSeparator(str, pos) || str[pos] == ':')) pos++;

            var body = str[pos] == '{'
                ? $"{{{GetText(str, ref pos, "{", "}")}}}"
                : GetText(str, ref pos, "\"", "\"") ?? throw new InvalidOperationException("Пустая ссылка на значение");

            fields.Add(new JSONObject(name, body));

            while (pos < len && (char.IsSeparator(str, pos) || str[pos] == ',')) pos++;
        }
        if (fields.Count > 0) _Fields = [.. fields];
    }

    #endregion

    /// <summary>Преобразование значения структуры к целому числу</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Целочисленное значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
    public int? ToInt(int? Default = null) => int.TryParse(_Data, out var v) ? v : Default;

    /// <summary>Преобразование значения структуры к вещественному числу</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Вещественное значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
    public double? ToDouble(double? Default = null) => double.TryParse(_Data, out var v) ? v : Default;

    /// <summary>Преобразование значения структуры к логическому значению</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Логическое значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
    public bool? ToBool(bool? Default = null) => bool.TryParse(_Data, out var v) ? v : Default;

    /// <summary>Строковое представление структуры</summary>
    /// <returns>Строковое представление структуры</returns>
    public override string ToString() => _Name.IsNullOrWhiteSpace() ? _Data : $"{_Name} : {_Data}";

    /// <summary>Оператор неявного приведения структуры JSON к строковому значению</summary>
    /// <param name="json">Структура JSON</param>
    public static implicit operator string(JSONObject json) => json._Data;

    /// <summary>
    /// Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения
    /// </summary>
    /// <param name="Str">Входная строка</param>
    /// <param name="Offset">
    /// Смещение во входной строке начала поиска - в конце работы метода соответствует месту окончания поиска
    /// </param>
    /// <param name="Open">Шаблон начала подстроки</param>
    /// <param name="Close">Шаблон окончания подстроки</param>
    /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
    /// <exception cref="FormatException">
    /// Если шаблон завершения строки не найден, либо если количество шаблонов начала строки превышает 
    /// количество шаблонов окончания во входной строке
    /// </exception>
    public static string? GetText(string Str, ref int Offset, string Open, string Close)
    {
        var start_index = Str.IndexOf(Open, Offset, StringComparison.Ordinal);

        if (start_index == -1) return null;

        var stop_index = Str.IndexOf(Close, start_index + 1, StringComparison.Ordinal);

        if (stop_index == -1) throw new FormatException();

        var start = start_index;
        do
        {
            start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);

            if (start != -1 && start < stop_index)
                stop_index = Str.IndexOf(Close, stop_index + 1, StringComparison.Ordinal);

        } 
        while (start != -1 && start < stop_index);

        if (stop_index == -1 || stop_index < start_index) throw new FormatException();

        Offset      =  stop_index + Close.Length;
        start_index += Open.Length;

        return Str[start_index..stop_index];
    }

    IEnumerator<JSONObject> IEnumerable<JSONObject>.GetEnumerator() => (_Fields ?? Enumerable.Empty<JSONObject>()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this).GetEnumerator();
}