#nullable enable

using MathCore.PE.Tables;

namespace MathCore.PE.Examples;

/// <summary>
/// Примеры использования парсера PE-файлов
/// </summary>
public static class PEFileExamples
{
    /// <summary>
    /// Пример 1: Проверка, является ли файл PE-файлом
    /// </summary>
    public static void Example1_CheckIfPEFile(string filePath)
    {
        var pe = new PEFile(filePath);
        
        if (!pe.Exists)
        {
            Console.WriteLine("Файл не существует");
            return;
        }

        if (pe.IsPE)
        {
            Console.WriteLine("Это PE-файл (исполняемый файл Windows)");
        }
        else
        {
            Console.WriteLine("Это не PE-файл");
        }
    }

    /// <summary>
    /// Пример 2: Получение информации о заголовке файла
    /// </summary>
    public static void Example2_GetHeaderInfo(string filePath)
    {
        var pe = new PEFile(filePath);
        var header = pe.GetHeader();

        Console.WriteLine($"Магия DOS: {header.DOS.MagicStr}");
        Console.WriteLine($"Смещение PE заголовка: 0x{header.DOS.lfanew:X}");
        Console.WriteLine($"Архитектура: {header.NT.FileHeader.Machine}");
        Console.WriteLine($"Количество разделов: {header.NT.FileHeader.NumberOfSections}");
        Console.WriteLine($"Размер образа: {header.NT.OptionalHeader.SizeOfImage} байт");
        Console.WriteLine($"Размер кода: {header.NT.OptionalHeader.SizeOfCode} байт");
    }

    /// <summary>
    /// Пример 3: Получение экспортируемых функций
    /// </summary>
    public static void Example3_GetExportedFunctions(string filePath)
    {
        var pe = new PEFile(filePath);
        var exports = pe.GetExports();

        if (exports is null)
        {
            Console.WriteLine("Нет информации об экспортах");
            return;
        }

        Console.WriteLine($"Модуль: {exports.ModuleName}");
        Console.WriteLine($"Версия: {exports.Version}");
        Console.WriteLine($"Количество функций: {exports.FunctionCount}");
        Console.WriteLine($"Количество именованных функций: {exports.NamedFunctionCount}");
        Console.WriteLine();

        foreach (var func in exports.Functions.Take(10))
        {
            Console.WriteLine($"  {func}");
        }

        if (exports.Functions.Count > 10)
        {
            Console.WriteLine($"  ... и ещё {exports.Functions.Count - 10} функций");
        }
    }

    /// <summary>
    /// Пример 4: Получение импортируемых функций
    /// </summary>
    public static void Example4_GetImportedFunctions(string filePath)
    {
        var pe = new PEFile(filePath);
        var imports = pe.GetImports();

        if (imports is null)
        {
            Console.WriteLine("Нет информации об импортах");
            return;
        }

        Console.WriteLine($"Импортируемые библиотеки: {imports.Libraries.Count}");
        Console.WriteLine();

        foreach (var library in imports.Libraries)
        {
            Console.WriteLine($"  {library.Name} ({library.ImportedFunctions.Count} функций)");
            
            foreach (var func in library.ImportedFunctions.Take(5))
            {
                Console.WriteLine($"    - {func}");
            }

            if (library.ImportedFunctions.Count > 5)
            {
                Console.WriteLine($"    ... и ещё {library.ImportedFunctions.Count - 5} функций");
            }
        }
    }

    /// <summary>
    /// Пример 5: Получение информации о ресурсах
    /// </summary>
    public static void Example5_GetResourcesInfo(string filePath)
    {
        var pe = new PEFile(filePath);
        var resources = pe.GetResources();

        if (resources is null)
        {
            Console.WriteLine("Нет информации о ресурсах");
            return;
        }

        Console.WriteLine($"Всего ресурсов: {resources.Resources.Count}");
        Console.WriteLine();

        var groupedByType = resources.Resources.GroupBy(r => r.Type);
        foreach (var group in groupedByType)
        {
            var typeName = Enum.IsDefined(typeof(ResourceType), group.Key)
                ? ((ResourceType)group.Key).ToString()
                : $"Type({group.Key})";
            Console.WriteLine($"  {typeName}: {group.Count()}");
        }
    }

    /// <summary>
    /// Пример 6: Извлечение иконок из файла
    /// </summary>
    public static void Example6_ExtractIcons(string filePath, string outputDir)
    {
        var pe = new PEFile(filePath);
        var iconCount = 0;

        foreach (var iconData in pe.ExtractIcons())
        {
            var fileName = Path.Combine(outputDir, $"icon_{iconCount}.ico");
            File.WriteAllBytes(fileName, iconData);
            Console.WriteLine($"Сохранена иконка: {fileName}");
            iconCount++;
        }

        if (iconCount == 0)
            Console.WriteLine("Иконки не найдены");
    }

    /// <summary>
    /// Пример 7: Извлечение растровых изображений из файла
    /// </summary>
    public static void Example7_ExtractBitmaps(string filePath, string outputDir)
    {
        var pe = new PEFile(filePath);
        
        Console.WriteLine("Извлечение растровых изображений...");
        pe.SaveBitmaps(outputDir);
        Console.WriteLine($"Растровые изображения сохранены в: {outputDir}");
    }

    /// <summary>
    /// Пример 8: Полный анализ файла
    /// </summary>
    public static void Example8_FullAnalysis(string filePath)
    {
        var pe = new PEFile(filePath);

        if (!pe.IsPE)
        {
            Console.WriteLine("Это не PE-файл!");
            return;
        }

        Console.WriteLine("=== АНАЛИЗ PE-ФАЙЛА ===");
        Console.WriteLine($"Файл: {filePath}");
        Console.WriteLine();

        // Заголовок
        var header = pe.GetHeader();
        Console.WriteLine("=== ЗАГОЛОВОК ===");
        Console.WriteLine($"Архитектура: {header.NT.FileHeader.Machine}");
        Console.WriteLine($"Количество разделов: {header.NT.FileHeader.NumberOfSections}");
        Console.WriteLine();

        // Экспорты
        Console.WriteLine("=== ЭКСПОРТЫ ===");
        Console.WriteLine(pe.GetExportsSummary());
        Console.WriteLine();

        // Импорты
        Console.WriteLine("=== ИМПОРТЫ ===");
        Console.WriteLine(pe.GetImportsSummary());
        Console.WriteLine();

        // Ресурсы
        Console.WriteLine("=== РЕСУРСЫ ===");
        Console.WriteLine(pe.GetResourcesSummary());
    }

    /// <summary>
    /// Пример 9: Преобразование RVA в смещение в файле
    /// </summary>
    public static void Example9_RVAConversion(string filePath)
    {
        var pe = new PEFile(filePath);
        var header = pe.GetHeader();

        // Пример: преобразование RVA точки входа
        var entryPointRVA = header.NT.OptionalHeader.AddressOfEntryPoint;
        var fileOffset = pe.RvaToFileOffset(entryPointRVA);

        if (fileOffset >= 0)
        {
            Console.WriteLine($"Точка входа RVA: 0x{entryPointRVA:X}");
            Console.WriteLine($"Смещение в файле: 0x{fileOffset:X}");
        }
        else
        {
            Console.WriteLine("Не удалось преобразовать RVA");
        }
    }

    /// <summary>
    /// Пример 10: Чтение данных из файла по RVA
    /// </summary>
    public static void Example10_ReadDataByRVA(string filePath)
    {
        var pe = new PEFile(filePath);
        var header = pe.GetHeader();

        // Читаем первые 16 байт кода
        var codeData = pe.ReadDataFromRVA(
            header.NT.OptionalHeader.BaseOfCode,
            16
        );

        Console.WriteLine("Первые 16 байт кода:");
        for (int i = 0; i < codeData.Length; i++)
        {
            Console.Write($"{codeData[i]:X2} ");
            if ((i + 1) % 16 == 0)
                Console.WriteLine();
        }
    }
}
