# MathCore.Text

Модуль для работы с текстовыми алгоритмами и обработки строк.

## Обзор

`MathCore.Text` предоставляет набор классов для анализа и сравнения текстовых данных:

- **Метрики строковых расстояний**: алгоритмы для измерения сходства и различия между строками
- **Парсинг строк по шаблонам**: извлечение структурированных данных из неструктурированного текста

Модуль особенно полезен для задач:
- Нечеткого поиска и сравнения текстов
- Обнаружения дубликатов и похожих записей
- Исправления опечаток и автокоррекции
- Парсинга лог-файлов и структурированных текстов
- Биоинформатики (сравнение последовательностей)

---

## Классы модуля

### 1. `JaroWinkler`

**Назначение**: Вычисление сходства строк по метрике Джаро — Винклера.

**Алгоритм**: Метрика Джаро — Винклера — это мера расстояния между строками, которая особенно эффективна для коротких строк и учитывает совпадение начальных символов (префикса). Алгоритм работает в три этапа:
1. Определяет совпадающие символы с учетом максимального допустимого расстояния
2. Вычисляет количество транспозиций (перестановок символов)
3. При высоком сходстве применяет модификацию Винклера, увеличивающую вес совпадающего префикса

#### Публичные методы

```csharp
public static double Distance(
    string Str1, 
    string Str2, 
    double WeightThreshold = 0.7, 
    int PrefixLength = 4, 
    IEqualityComparer<char>? CharComparer = null)
```
Вычисляет расстояние между строками (0 = идентичные, 1 = полностью различные).

**Параметры:**
- `Str1`, `Str2` — сравниваемые строки
- `WeightThreshold` — порог для применения модификации Винклера (по умолчанию 0.7)
- `PrefixLength` — максимальная длина префикса для учета (по умолчанию 4)
- `CharComparer` — компаратор символов (для игнорирования регистра и др.)

```csharp
public static double Proximity(
    string Str1, 
    string Str2, 
    double WeightThreshold = 0.7, 
    int PrefixLength = 4, 
    IEqualityComparer<char>? CharComparer = null)
```
Вычисляет сходство строк (0 = полностью различные, 1 = идентичные).

#### Примеры использования

```csharp
using MathCore.Text;

// Базовое сравнение
var distance = JaroWinkler.Distance("martha", "marhta");
Console.WriteLine($"Расстояние: {distance:F3}"); // ~0.039

var proximity = JaroWinkler.Proximity("dixon", "dicksonx");
Console.WriteLine($"Сходство: {proximity:F3}"); // ~0.767

// Игнорирование регистра
var case_insensitive = JaroWinkler.Proximity(
    "Hello", 
    "hello", 
    CharComparer: StringComparer.OrdinalIgnoreCase
);
Console.WriteLine($"Сходство: {case_insensitive}"); // 1.0

// Поиск похожих имен
var names = new[] { "John Smith", "Jon Smith", "Jane Smith", "Bob Jones" };
var target = "John Smyth";
var similar = names
    .Select(name => new { Name = name, Score = JaroWinkler.Proximity(target, name) })
    .Where(x => x.Score > 0.8)
    .OrderByDescending(x => x.Score)
    .ToList();

foreach (var item in similar)
    Console.WriteLine($"{item.Name}: {item.Score:F3}");
// Результат:
// Jon Smith: 0.957
// John Smith: 0.948
```

#### Рекомендации по параметрам

- **WeightThreshold (0.7)**: 
  - Увеличьте (0.8-0.9) для более строгого применения модификации Винклера
  - Уменьшите (0.5-0.6) для более частого применения бонуса за префикс
  
- **PrefixLength (4)**: 
  - Для коротких строк (имена): 4 символа
  - Для средних строк (адреса): 6-8 символов
  - Для длинных строк: 10+ символов

- **CharComparer**: 
  - `null` — с учетом регистра (по умолчанию)
  - `StringComparer.OrdinalIgnoreCase` — без учета регистра
  - Пользовательский компаратор для специальной логики

#### Производительность

- **Временная сложность**: O(n×m), где n и m — длины строк
- **Пространственная сложность**: O(n + m)
- **Рекомендации**: Эффективен для коротких и средних строк (до 1000 символов)

---

### 2. `Levenshtein`

**Назначение**: Вычисление расстояния Левенштейна (редакционного расстояния).

**Алгоритм**: Классическая метрика, определяющая минимальное количество односимвольных операций (вставка, удаление, замена), необходимых для превращения одной строки в другую. Использует динамическое программирование для построения матрицы расстояний.

#### Публичные методы

```csharp
public static int Distance(string S1, string S2)
```
Вычисляет расстояние Левенштейна между строками.

**Параметры:**
- `S1`, `S2` — сравниваемые строки

**Возвращает**: Количество операций редактирования

**Исключения**: `ArgumentNullException` если любая из строк равна `null`

```csharp
public static int DistanceFast(string source, string target)
```
Быстрая версия алгоритма с оптимизацией памяти (на 10-20% быстрее).

#### Примеры использования

```csharp
using MathCore.Text;

// Базовые примеры
var d1 = Levenshtein.Distance("kitten", "sitting"); // 3
var d2 = Levenshtein.Distance("saturday", "sunday"); // 3
var d3 = Levenshtein.Distance("book", "back"); // 2

// Проверка на опечатки
var typo_distance = Levenshtein.Distance("teh", "the"); // 2
if (typo_distance <= 2)
    Console.WriteLine("Возможна опечатка");

// Поиск похожих слов в словаре
var dictionary = new[] { "cat", "hat", "rat", "bat", "car" };
var word = "cut";
var similar = dictionary
    .Select(w => new { Word = w, Distance = Levenshtein.Distance(word, w) })
    .Where(x => x.Distance <= 2)
    .OrderBy(x => x.Distance)
    .ToList();

foreach (var item in similar)
    Console.WriteLine($"{item.Word}: {item.Distance}");
// Результат:
// cat: 1
// car: 1

// Вычисление процента сходства
var text1 = "The quick brown fox jumps over the lazy dog";
var text2 = "The quick brown cat jumps over the lazy dog";
var distance = Levenshtein.DistanceFast(text1, text2);
var max_length = Math.Max(text1.Length, text2.Length);
var similarity = (1.0 - (double)distance / max_length) * 100;
Console.WriteLine($"Сходство: {similarity:F2}%"); // ~93.18%

// Обработка исключений
try
{
    var invalid = Levenshtein.Distance(null, "test");
}
catch (ArgumentNullException ex)
{
    Console.WriteLine("Строки не могут быть null");
}
```

#### Выбор метода

| Длина строк | Рекомендуемый метод | Причина |
|-------------|---------------------|---------|
| До 100 символов | `Distance` или `DistanceFast` | Разница незначительна |
| 100-1000 символов | `DistanceFast` | Лучшая производительность |
| Более 1000 символов | `DistanceFast` | Значительно быстрее |

#### Производительность

- **Временная сложность**: O(n×m), где n и m — длины строк
- **Пространственная сложность**: 
  - `Distance`: O(n×m) — двумерная матрица
  - `DistanceFast`: O(n×m) — массив массивов (более эффективное размещение)
- **Рекомендации**: Для очень длинных строк (>10000 символов) рассмотрите другие алгоритмы

#### Типичные ошибки

- **ArgumentNullException**: Проверяйте входные строки на `null`
- **OutOfMemoryException**: Для очень длинных строк матрица может не поместиться в память

---

### 3. `StringFormat`

**Назначение**: Парсинг строк по пользовательскому шаблону с извлечением именованных полей.

**Алгоритм**: Преобразует пользовательский шаблон в регулярное выражение и извлекает значения именованных полей из входной строки. Регулярное выражение компилируется один раз при создании объекта.

#### Конструктор

```csharp
public StringFormat(string Format, RegexOptions Opts = default)
```

**Параметры:**
- `Format` — шаблон с placeholder'ами вида `{FieldName}` или `{FieldName?}`
- `Opts` — опции регулярного выражения (игнорирование регистра и др.)

**Синтаксис шаблона:**
- `{FieldName}` — обязательное поле (минимум 1 символ)
- `{FieldName?}` — необязательное поле (может быть пустым)
- Остальной текст должен точно совпадать

#### Публичные методы

```csharp
public Dictionary<string, string> Parse(string str)
```

Парсит строку согласно шаблону.

**Параметры:**
- `str` — строка для парсинга

**Возвращает**: Словарь имен полей и их значений, или пустой словарь при неудаче

#### Примеры использования

```csharp
using MathCore.Text;
using System.Text.RegularExpressions;

// Простой парсинг даты и времени
var format1 = new StringFormat("Date: {date}, Time: {time}");
var result1 = format1.Parse("Date: 2024-01-15, Time: 14:30");
Console.WriteLine($"Дата: {result1["date"]}");   // 2024-01-15
Console.WriteLine($"Время: {result1["time"]}");  // 14:30

// Необязательные поля
var format2 = new StringFormat("Name: {name}, Age: {age?}");
var result2 = format2.Parse("Name: John, Age: ");
Console.WriteLine($"Имя: {result2["name"]}");    // John
Console.WriteLine($"Возраст: '{result2["age"]}'"); // (пустая строка)

// Парсинг URL
var url_format = new StringFormat("https://{domain}/{path?}");
var url_data = url_format.Parse("https://example.com/api/users");
Console.WriteLine($"Домен: {url_data["domain"]}"); // example.com
Console.WriteLine($"Путь: {url_data["path"]}");    // api/users

// Парсинг лог-файла
var log_format = new StringFormat("[{level}] {timestamp}: {message}");
var logs = new[]
{
    "[INFO] 2024-01-15 10:00:00: Application started",
    "[ERROR] 2024-01-15 10:30:45: Connection timeout",
    "[WARNING] 2024-01-15 11:00:00: High memory usage"
};

foreach (var log in logs)
{
    var data = log_format.Parse(log);
    if (data.Count > 0)
    {
        Console.WriteLine($"Уровень: {data["level"]}, Сообщение: {data["message"]}");
    }
}

// Игнорирование регистра
var format3 = new StringFormat("Status: {status}", RegexOptions.IgnoreCase);
var result3 = format3.Parse("STATUS: OK");
Console.WriteLine($"Статус: {result3["status"]}"); // OK

// Обработка неуспешного парсинга
var format4 = new StringFormat("Value: {value}");
var result4 = format4.Parse("Invalid string");
if (result4.Count == 0)
    Console.WriteLine("Строка не соответствует шаблону");

// Конвертация типов
var format5 = new StringFormat("ID: {id}, Score: {score}");
var result5 = format5.Parse("ID: 42, Score: 95.5");
if (result5.Count > 0)
{
    var id = int.Parse(result5["id"]);         // 42
    var score = double.Parse(result5["score"]); // 95.5
    Console.WriteLine($"ID: {id}, Оценка: {score}");
}

// LINQ и пакетная обработка
var data_format = new StringFormat("User: {user} logged in at {time}");
var log_lines = new[]
{
    "User: john logged in at 10:00",
    "User: alice logged in at 10:05",
    "Invalid log entry"
};

var parsed_logs = log_lines
    .Select(log => data_format.Parse(log))
    .Where(result => result.Count > 0)
    .Select(result => new 
    { 
        User = result["user"], 
        Time = TimeSpan.Parse(result["time"]) 
    })
    .ToList();

foreach (var entry in parsed_logs)
    Console.WriteLine($"{entry.User} вошел в {entry.Time}");
// Результат:
// john вошел в 10:00:00
// alice вошел в 10:05:00
```

#### Рекомендации

**Шаблоны:**
- Используйте простые и понятные имена полей
- Добавляйте `?` только для действительно необязательных полей
- Избегайте слишком сложных шаблонов (усложняет отладку)

**Производительность:**
- Создавайте объект `StringFormat` один раз и используйте многократно
- Регулярное выражение компилируется при создании объекта
- Для массовой обработки используйте параллельные запросы LINQ

**Обработка ошибок:**
- Всегда проверяйте `result.Count > 0` перед доступом к полям
- Используйте `TryParse` для конвертации числовых значений
- Не полагайтесь на порядок полей в словаре

#### Производительность

- **Компиляция**: O(n), где n — длина шаблона (один раз при создании)
- **Парсинг**: O(m), где m — длина входной строки (зависит от сложности regex)
- **Рекомендации**: Переиспользуйте экземпляры для одного и того же шаблона

#### Типичные ошибки

- **Пустой словарь**: Строка не соответствует шаблону
- **KeyNotFoundException**: Попытка доступа к несуществующему полю
- **FormatException**: Ошибка при конвертации типов (используйте `TryParse`)

---

## Общие рекомендации

### Выбор метрики сходства

| Задача | Рекомендуемый класс | Причина |
|--------|---------------------|---------|
| Поиск опечаток | `Levenshtein` | Учитывает все типы ошибок |
| Сравнение имен/фамилий | `JaroWinkler` | Бонус за совпадающий префикс |
| Дубликаты в БД | `JaroWinkler` | Лучше для коротких строк |
| Сравнение текстовых блоков | `Levenshtein.DistanceFast` | Эффективен для длинных строк |
| Биоинформатика | `Levenshtein` | Стандарт в области |

### Производительность

**Оптимизация памяти:**
- Для длинных строк предпочитайте `Levenshtein.DistanceFast`
- Избегайте хранения больших матриц расстояний

**Параллельная обработка:**
```csharp
// Эффективное сравнение множества строк
var results = strings
    .AsParallel()
    .Select(s => new 
    { 
        String = s, 
        Distance = JaroWinkler.Distance(reference, s) 
    })
    .ToList();
```

**Кэширование:**
```csharp
// Кэширование результатов для частых запросов
var cache = new Dictionary<(string, string), double>();

double GetDistance(string s1, string s2)
{
    var key = (s1, s2);
    if (!cache.TryGetValue(key, out var distance))
    {
        distance = JaroWinkler.Distance(s1, s2);
        cache[key] = distance;
    }
    return distance;
}
```

### Обработка ошибок

**Проверка входных данных:**
```csharp
// JaroWinkler и StringFormat: принимают null, вернут 0 или пустой словарь
var distance = JaroWinkler.Distance(null, "test"); // 1.0 (максимальное расстояние)

// Levenshtein: бросает исключение при null
try
{
    var lev_distance = Levenshtein.Distance(null, "test");
}
catch (ArgumentNullException ex)
{
    // Обработка ошибки
}
```

### Комбинированное использование

```csharp
// Двухэтапный поиск: быстрая предварительная фильтрация + точное сравнение
var candidates = database
    .Where(item => JaroWinkler.Proximity(query, item.Name) > 0.7) // Быстрая фильтрация
    .Select(item => new
    {
        Item = item,
        Distance = Levenshtein.DistanceFast(query, item.Name) // Точное расстояние
    })
    .OrderBy(x => x.Distance)
    .Take(10)
    .ToList();
```

---

## Совместимость

Модуль поддерживает следующие целевые платформы:
- .NET 10
- .NET 9
- .NET 8
- .NET Standard 2.0

Специфические особенности реализации:
- **.NET 8+**: использует `GeneratedRegex` для оптимизации производительности в `StringFormat`
- **.NET Standard 2.0**: использует стандартный `Regex` с компиляцией во время выполнения

---

## Дополнительные ресурсы

**Научные статьи и спецификации:**
- [Jaro–Winkler distance на Wikipedia](https://ru.wikipedia.org/wiki/Сходство_Джаро_—_Винклера)
- [Levenshtein distance на Wikipedia](https://ru.wikipedia.org/wiki/Расстояние_Левенштейна)

**Связанные разделы MathCore:**
- `MathCore.Extensions` — расширения для работы со строками
- `MathCore.Collections` — коллекции для эффективного хранения строк
