namespace MathCore.CSV;

/// <summary>
/// Расширения для работы с CSV-данными
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Открыть CSV-файл для чтения данных с настройкой параметров
    /// </summary>
    /// <param name="file">Файл для открытия</param>
    /// <param name="Separator">Символ-разделитель значений (по умолчанию ',')</param>
    /// <returns>Объект CSVQuery для конфигурации и чтения данных</returns>
    /// <remarks>
    /// Метод создаёт фабрику для ленивого открытия файла.
    /// Файл открывается только при начале итерации через GetEnumerator().
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var file = new FileInfo("data.csv");
    /// var query = file.OpenCSV(',')
    ///     .WithHeader()
    ///     .SkipRowsBeforeHeader(1);
    /// 
    /// foreach (var row in query)
    ///     Console.WriteLine(row["Name"]);
    /// ]]>
    /// </example>
    public static CSVQuery OpenCSV(this FileInfo file, char Separator = ',') => new(file.OpenText, Separator);

    /// <summary>
    /// Преобразовать перечисление элементов в построитель CSV-записи
    /// </summary>
    /// <typeparam name="T">Тип элементов перечисления</typeparam>
    /// <param name="items">Элементы для записи в CSV</param>
    /// <param name="Separator">Символ-разделитель значений (по умолчанию ',')</param>
    /// <returns>Построитель CSVWriter<T> для конфигурации записи</returns>
    /// <remarks>
    /// Метод позволяет использовать fluent API для построения CSV-файла.
    /// Данные не записываются до вызова одного из методов WriteTo.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var people = new List<Person> 
    /// { 
    ///     new { Id = 1, Name = "Alice" },
    ///     new { Id = 2, Name = "Bob" }
    /// };
    /// 
    /// people.AsCSV(',')
    ///     .AddDefaultHeaders()
    ///     .WriteTo("output.csv");
    /// ]]>
    /// </example>
    public static CSVWriter<T> AsCSV<T>(this IEnumerable<T> items, char Separator = ',') => new(items, Separator);
}