# Парсер PE-файлов (Portable Executable)

## Описание

Полнофункциональный парсер PE-файлов (исполняемые файлы и DLL для Windows) на C#. Поддерживает:

- ✅ Проверку корректности PE-файла
- ✅ Чтение заголовков файла (DOS, NT, Optional Header)
- ✅ Работу с разделами (Sections)
- ✅ Экспортируемые функции (функции, которые DLL предоставляет другим программам)
- ✅ Импортируемые функции (функции, которые программа использует из других DLL)
- ✅ Ресурсы (иконки, растровые изображения, звуки, диалоги, строки и т.д.)
- ✅ Преобразование RVA ↔ File Offset
- ✅ Извлечение ресурсов в отдельные файлы

## Основные классы

### PEFile
Главный класс для работы с PE-файлами.

```csharp
var peFile = new PEFile("C:\\Windows\\System32\\kernel32.dll");

// Проверка, является ли файл PE-файлом
if (peFile.IsPE)
{
    var header = peFile.GetHeader();
    Console.WriteLine($"Это PE-файл для платформы: {header.NT.FileHeader.Machine}");
}
```

### ExportedFunctions
Работа с экспортируемыми функциями из DLL.

```csharp
var exports = peFile.GetExports();
if (exports != null)
{
    Console.WriteLine($"Модуль: {exports.ModuleName}");
    Console.WriteLine($"Функций: {exports.FunctionCount}");
    
    foreach (var func in exports.Functions.Take(10))
    {
        Console.WriteLine($"  {func}");
    }
}
```

### ImportedFunctions
Работа с импортируемыми функциями.

```csharp
var imports = peFile.GetImports();
if (imports != null)
{
    foreach (var lib in imports.Libraries)
    {
        Console.WriteLine($"{lib.Name}: {lib.ImportedFunctions.Count} функций");
    }
}
```

### PEResources
Работа с ресурсами (иконками, звуками, изображениями).

```csharp
var resources = peFile.GetResources();
if (resources != null)
{
    foreach (var icon in resources.GetResourcesByType(ResourceType.Icon))
    {
        var data = resources.ExtractResourceData(icon.DataEntry);
        // Обработка данных иконки
    }
}
```

## Примеры использования

### Пример 1: Полный анализ файла

```csharp
var pe = new PEFile("C:\\Windows\\notepad.exe");

Console.WriteLine(pe.GetExportsSummary());
Console.WriteLine(pe.GetImportsSummary());
Console.WriteLine(pe.GetResourcesSummary());
```

### Пример 2: Извлечение всех иконок

```csharp
var pe = new PEFile("app.exe");

var outputDir = Path.Combine(Environment.CurrentDirectory, "extracted_icons");
foreach (var iconData in pe.ExtractIcons())
{
    var filename = Path.Combine(outputDir, $"icon_{Guid.NewGuid()}.ico");
    File.WriteAllBytes(filename, iconData);
}
```

### Пример 3: Получение информации о функциях

```csharp
var pe = new PEFile("mylib.dll");

// Экспортируемые функции
var exports = pe.GetExports();
Console.WriteLine($"Версия: {exports?.Version}");

// Импортируемые функции
var imports = pe.GetImports();
foreach (var lib in imports?.Libraries ?? [])
{
    Console.WriteLine($"Зависит от: {lib.Name}");
}
```

### Пример 4: Работа с RVA

```csharp
var pe = new PEFile("app.exe");
var header = pe.GetHeader();

// Преобразование RVA в смещение в файле
long fileOffset = pe.RvaToFileOffset(header.NT.OptionalHeader.AddressOfEntryPoint);

// Чтение данных из файла по RVA
var data = pe.ReadDataFromRVA(
    header.NT.OptionalHeader.BaseOfCode,
    256
);
```

## API Методы

### PEFile

| Метод | Описание |
|-------|---------|
| `GetHeader()` | Получить заголовок файла |
| `GetExports()` | Получить информацию об экспортируемых функциях |
| `GetImports()` | Получить информацию об импортируемых функциях |
| `GetResources()` | Получить информацию о ресурсах |
| `RvaToFileOffset(uint RVA)` | Преобразовать RVA в смещение в файле |
| `FileOffsetToRva(long offset)` | Преобразовать смещение в файле в RVA |
| `ReadDataFromRVA(uint RVA, int Size)` | Прочитать данные из файла по RVA |

### Вспомогательные методы (Extensions)

| Метод | Описание |
|-------|---------|
| `ExtractIcons(this PEFile)` | Получить все иконки |
| `ExtractBitmaps(this PEFile)` | Получить все растровые изображения |
| `ExtractSounds(this PEFile)` | Получить все звуки |
| `SaveIcons(this PEFile, string dir)` | Сохранить все иконки в директорию |
| `SaveBitmaps(this PEFile, string dir)` | Сохранить все растровые изображения |
| `SaveResourcesOfType(this PEFile, ResourceType, string dir)` | Сохранить ресурсы определённого типа |
| `GetExportsSummary(this PEFile)` | Получить текстовую сводку по экспортам |
| `GetImportsSummary(this PEFile)` | Получить текстовую сводку по импортам |
| `GetResourcesSummary(this PEFile)` | Получить текстовую сводку по ресурсам |

## Структуры данных

### Header
Основной заголовок PE-файла, содержит:
- `DOS` - DOS заголовок (MZ)
- `NT` - NT заголовок (PE\0\0)
- `Sections` - массив заголовков разделов

### ExportedFunction
Информация об экспортируемой функции:
- `Ordinal` - порядковый номер
- `Name` - имя функции (может быть null)
- `Address` - RVA функции

### ImportLibrary
Информация об импортируемой библиотеке:
- `Name` - имя DLL
- `ImportedFunctions` - список функций из этой DLL

### ResourceInfo
Информация о ресурсе:
- `Type` - тип ресурса (Icon, Bitmap, Sound и т.д.)
- `Name` - идентификатор или имя ресурса
- `DataEntry` - информация о расположении данных в файле

## Типы ресурсов

```csharp
public enum ResourceType : uint
{
    CursorGroup = 12,  // Группа курсоров
    Bitmap = 2,        // Растровое изображение
    IconGroup = 14,    // Группа иконок
    Icon = 3,          // Иконка
    Menu = 4,          // Меню
    Dialog = 5,        // Диалоговое окно
    String = 6,        // Строковая таблица
    FontDir = 7,       // Таблица шрифтов
    Font = 8,          // Шрифт
    Sound = 16,        // Звуковой ресурс (WAV)
    Custom = 256       // Пользовательский ресурс
}
```

## Совместимость

- .NET Standard 2.0 и выше
- .NET 8, 9, 10
- Кроссплатформа (чтение PE-файлов работает на Linux, macOS, Windows)

## Примеры в коде

Полные примеры использования находятся в классе `PEFileExamples`:

```csharp
using MathCore.PE.Examples;

// Пример 1: Полный анализ
PEFileExamples.Example8_FullAnalysis("kernel32.dll");

// Пример 2: Извлечение иконок
PEFileExamples.Example6_ExtractIcons("app.exe", "icons");

// Пример 3: Получение информации о ресурсах
PEFileExamples.Example5_GetResourcesInfo("app.exe");
```

## Примечания

1. **Производительность**: Заголовок файла кэшируется после первой загрузки для оптимизации производительности.

2. **Обработка ошибок**: Все методы защищены от исключений и возвращают null или пустые коллекции при ошибках.

3. **RVA преобразования**: Преобразование RVA ↔ File Offset выполняется на основе информации из заголовков разделов.

4. **Ресурсы**: Структура ресурсов в PE-файле иерархическая (Type → Language → Data).

## Источники информации

- [PE Format (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/debug/pe-format)
- [Windows PE File Format](https://en.wikipedia.org/wiki/Portable_Executable)
- [Структура PE-файла](https://www.reverseengineering.stackexchange.com/)
