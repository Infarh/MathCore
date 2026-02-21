# Модуль MathCore.CSV

## Описание

Модуль `MathCore.CSV` предоставляет полнофункциональный набор инструментов для работы с данными в формате CSV (Comma-Separated Values). Модуль поддерживает:

- **Гибкое чтение CSV-данных** с поддержкой различных разделителей, кодировок и форматов
- **Типизированный доступ к значениям** через структуру `Value` с преобразованием в различные типы данных
- **Запись данных в CSV** с конфигурацией колонок и форматирования
- **Парсинг и формирование строк** с корректной обработкой экранированных значений
- **Ленивое чтение** данных для работы с большими файлами

## Архитектура модуля

### Основные типы

#### 1. **CSVQuery** — Чтение CSV-данных
Структура для конфигурации параметров чтения и итерации по строкам данных.

**Конструктор:**
```csharp
CSVQuery(Func<TextReader> ReaderFactory, char Separator = ',')
```

**Fluent API методы:**
- `WithHeader(bool)` — указать наличие строки заголовка
- `SkipRowsBeforeHeader(int)` — пропустить строк перед заголовком
- `SkipRowsAfterHeader(int)` — пропустить строк после заголовка
- `TakeRows(int)` — ограничить количество читаемых строк (-1 = все)
- `ValuesSeparator(char)` — установить разделитель
- `WithCulture(CultureInfo)` — установить культуру для преобразований
- `WithEoL(string)` — установить символы конца строки
- `AddColumn(name, index)` — добавить колонку с псевдонимом
- `RemoveColumn(name/index)` — удалить колонку

**Пример использования:**
```csharp
var file = new FileInfo("sales.csv");
var query = file.OpenCSV(';')
    .WithHeader()
    .SkipRowsBeforeHeader(2)
    .TakeRows(100);

foreach (var row in query)
{
    int id = row[0].Int32Value;
    string name = row["Product"].StringValue;
    decimal price = row["Price"].DecimalValue;
    
    Console.WriteLine($"ID: {id}, Продукт: {name}, Цена: {price}");
}
```

#### 2. **CSVQueryRow** — Строка данных
Структура, представляющая одну строку из CSV-источника с доступом по индексу или по имени колонки.

**Свойства:**
- `Index` — номер строки (0-based)
- `ItemsCount` — количество ячеек в строке
- `SourceLine` — исходная текстовая строка
- `StartPos` — начальное положение в потоке (байты)
- `EndPos` — конечное положение в потоке (байты)
- `Headers` — словарь заголовков (имя → индекс)

**Доступ к значениям:**
```csharp
// По индексу
int value = row[0].Int32Value;
bool? nullable = row[1].AsBoolOrNull();

// По имени колонки
decimal price = row["Price"].DecimalValue;

// С преобразованием типа
int id = row.ValueAs<int>(0);
```

**Методы:**
- `this[int/string]` — получить типизированное значение
- `Value(int/string)` — получить ссылку на строковое значение для изменения
- `ToDictionary()` — преобразовать в словарь

#### 3. **Value** — Типизированное значение
Структура-обёртка над строковым значением с методами преобразования в различные типы.

**Прямые свойства:**
- `StringValue` — исходная строка
- `TrimmedStringValue` — строка без пробелов и кавычек
- `Int32Value`, `Int64Value`, `FloatValue`, `DoubleValue`, `DecimalValue` — преобразованные значения
- `BoolValue` — логическое значение

**Безопасные преобразования (возвращают null):**
```csharp
int? value = csvValue.AsInt32OrNull();
bool? flag = csvValue.AsBoolOrNull();
```

**Преобразования с значением по умолчанию:**
```csharp
int age = csvValue.AsInt32OrDefault(0);
decimal price = csvValue.AsDecimalOrDefault(0m);
```

**Преобразование enum-ов:**
```csharp
Status status = csvValue.AsEnum<Status>();
Priority? priority = csvValue.AsEnumOrNull<Priority>();
```

**Пример использования:**
```csharp
var row = ...;
Value price = row["Price"];

// Различные варианты преобразования
decimal exactPrice = price.DecimalValue;           // точное преобразование
decimal? nullablePrice = price.AsDecimalOrNull();  // nullable
decimal defaultPrice = price.AsDecimalOrDefault(99.99m);  // с значением по умолчанию
```

#### 4. **CSVWriter<T>** — Запись CSV-данных
Структура для конфигурации и записи объектов в CSV-формат.

**Конструктор и методы:**
```csharp
// Создание
var writer = new CSVWriter<Product>(products, ',');

// Fluent API
writer
    .AddDefaultHeaders()              // добавить колонки по свойствам T
    .AddColumn("Total", p => p.Qty * p.Price)  // добавить вычисляемую колонку
    .RemoveColumn("InternalId")       // удалить колонку
    .Separator(';')                   // изменить разделитель
    .WriteHeader(true);               // включить заголовок
```

**Методы записи:**
```csharp
writer.WriteTo("output.csv");
writer.WriteTo(new FileInfo("output.csv"));
writer.WriteTo(stream);
writer.WriteTo(textWriter);

// Асинхронно
await writer.WriteToAsync("output.csv", Encoding.UTF8);
await writer.WriteToAsync(stream, cancellationToken: token);
```

**Пример использования:**
```csharp
var products = new[]
{
    new { Id = 1, Name = "Item A", Price = 100m },
    new { Id = 2, Name = "Item B", Price = 200m }
};

products.AsCSV(';')
    .AddDefaultHeaders()
    .AddColumn("Currency", _ => "RUB")
    .WriteTo("products.csv");
```

#### 5. **CSVParser** — Парсинг строк
Статический класс для разбора и формирования CSV-строк.

**Методы:**
```csharp
// Разбор строки на элементы
IEnumerable<string> items = CSVParser.ParseLine("name,\"John, Jr.\",age", ',', trim: true);
// Результат: ["name", "John, Jr.", "age"]

// Формирование CSV-строки
string line = CSVParser.CreateLine(new[] { "Name", "John, Jr.", "Age" }, ',');
// Результат: "Name,\"John, Jr.\",Age"
```

#### 6. **Extensions** — Методы расширений
Удобные методы расширений для работы с CSV.

```csharp
// Открыть файл для чтения
var file = new FileInfo("data.csv");
var query = file.OpenCSV(',');

// Преобразовать перечисление в CSV
var items = new[] { 1, 2, 3 };
items.AsCSV().WriteTo("numbers.csv");
```

## Алгоритмы и особенности

### Парсинг CSV-строк

Модуль поддерживает стандарт RFC 4180 с экранированием разделителей и кавычек:

- **Экранирование разделителя:** значение `"A, B"` не разбивается
- **Экранирование кавычек:** внутри кавычек кавычки удваиваются
- **Trim опция:** удаляет пробелы и кавычки из начала/конца элементов

### Ленивое чтение

Данные читаются по мере необходимости при итерации через `foreach`. Это позволяет:
- Работать с очень большими файлами
- Останавливать чтение в любой момент
- Экономить память

### Позиционирование в потоке

Каждая строка содержит информацию о её положении в исходном файле:
- `StartPos` — начало строки
- `EndPos` — конец строки
- Позволяет внедрить поиск, индексацию и навигацию

## Рекомендации по использованию

### Параметры конфигурации

| Параметр | Значение по умолчанию | Применение |
|----------|---------------------|-----------|
| Separator | `,` | Использовать `;` для европейских форматов, `\t` для TSV |
| WithHeader | false | Установить `true`, если первая строка — заголовок |
| SkipRows | 0 | Пропустить описание, комментарии в начале |
| TakeRows | -1 | Ограничить чтение для тестирования |
| Culture | CurrentCulture | Установить en-US или de-DE для чисел/дат |

### Обработка ошибок

```csharp
// Опасно — может выбросить исключение при некорректном формате
int id = row["ID"].Int32Value;

// Безопасно — возвращает null при ошибке
int? id = row["ID"].AsInt32OrNull();

// С значением по умолчанию
int id = row["ID"].AsInt32OrDefault(-1);
```

### Производительность

1. **Для больших файлов:** используйте ленивое чтение с `foreach`
2. **Для частого доступа:** кэшируйте значения в памяти (например, LINQ `.ToList()`)
3. **Для асинхронной записи:** используйте `WriteToAsync()` с CancellationToken

### Типичные сценарии

#### Чтение данных с заголовком
```csharp
var query = new FileInfo("data.csv")
    .OpenCSV()
    .WithHeader();

foreach (var row in query)
{
    var name = row["Name"].StringValue;
    var age = row["Age"].AsInt32OrNull();
}
```

#### Пропуск служебных строк
```csharp
var query = new FileInfo("data.csv")
    .OpenCSV()
    .SkipRowsBeforeHeader(3)  // пропустить 3 строки
    .WithHeader()
    .SkipRowsAfterHeader(1);  // пропустить 1 строку после заголовка
```

#### Ограничение количества строк
```csharp
var query = new FileInfo("data.csv")
    .OpenCSV()
    .WithHeader()
    .TakeRows(1000);  // прочитать только первые 1000 строк

foreach (var row in query)
    ProcessRow(row);
```

#### Запись с пользовательскими колонками
```csharp
var items = GetData();

items.AsCSV(';')
    .AddColumn("ID", i => i.Id)
    .AddColumn("Display Name", i => $"{i.LastName} {i.FirstName}")
    .RemoveColumn("InternalCode")
    .WriteTo("export.csv");
```

#### Асинхронная запись с отменой
```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

try
{
    await items.AsCSV()
        .AddDefaultHeaders()
        .WriteToAsync("output.csv", Encoding.UTF8, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Запись отменена");
}
```

## Общие исключения

| Исключение | Причина | Решение |
|-----------|--------|---------|
| `FormatException` | Пустая строка заголовка | Проверьте наличие данных в файле |
| `KeyNotFoundException` | Колонка не найдена | Проверьте имя колонки в заголовке |
| `FormatException` | Ошибка при преобразовании типа | Используйте `AsXxxOrNull()` вместо `XxxValue` |
| `ArgumentNullException` | Null в обязательном параметре | Проверьте значения перед передачей |

## Лучшие практики

1. **Всегда указывайте культуру** при работе с числами разных локалей:
   ```csharp
   query.WithCulture(CultureInfo.GetCultureInfo("de-DE"))
   ```

2. **Используйте метод-расширение `OpenCSV()`** для файлов:
   ```csharp
   var query = file.OpenCSV(',').WithHeader();
   ```

3. **Обрабатывайте ошибки типизированно**:
   ```csharp
   int? value = row["Amount"].AsInt32OrNull();
   if (!value.HasValue)
       LogWarning($"Строка {row.Index}: некорректное значение Amount");
   ```

4. **Кэшируйте заголовок**, если читаете один файл несколько раз:
   ```csharp
   var header = query.GetHeader();
   var indexed_query = query.Header(header);
   ```

5. **Используйте `AddDefaultHeaders()` для простых типов**:
   ```csharp
   records.AsCSV().AddDefaultHeaders().WriteTo("file.csv");
   ```

## Примеры кода

### Пример 1: Чтение и фильтрация
```csharp
var file = new FileInfo("sales.csv");
var query = file.OpenCSV(';')
    .WithHeader()
    .SkipRowsBeforeHeader(1);

var highValueSales = query
    .Where(row => row["Amount"].AsDecimalOrDefault(0) > 1000)
    .Select(row => new
    {
        Id = row["ID"].Int32Value,
        Date = row["Date"].StringValue,
        Amount = row["Amount"].DecimalValue
    })
    .ToList();
```

### Пример 2: Преобразование и экспорт
```csharp
var source = new FileInfo("input.csv").OpenCSV().WithHeader();

var transformed = source.Select(row => new Output
{
    FullName = $"{row["FirstName"]} {row["LastName"]}",
    Age = row["Age"].AsInt32OrDefault(0),
    IsActive = row["Active"].AsBoolOrDefault(false)
});

transformed.AsCSV()
    .AddDefaultHeaders()
    .WriteTo("output.csv");
```

### Пример 3: Валидация данных
```csharp
var errors = new List<string>();
var query = new FileInfo("data.csv").OpenCSV().WithHeader();

foreach (var row in query)
{
    if (row["Email"].AsInt32OrNull() is null)
        errors.Add($"Строка {row.Index + 1}: некорректный Email");
    
    if (row["Phone"].StringValue.Length < 5)
        errors.Add($"Строка {row.Index + 1}: слишком короткий телефон");
}

if (errors.Count > 0)
    Console.WriteLine(string.Join("\n", errors));
```

## Дополнительные сведения

- **Кодировка:** По умолчанию используется кодировка файла. Явно укажите при необходимости.
- **Размер файла:** Модуль оптимизирован для работы с файлами до нескольких ГБ благодаря ленивому чтению.
- **Потокобезопасность:** Структуры предназначены для однопоточного использования. Для многопоточности используйте блокировки.
- **Производительность:** Используйте `ToList()` только если действительно нужны все данные сразу.
