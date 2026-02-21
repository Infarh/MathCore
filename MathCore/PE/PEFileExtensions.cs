#nullable enable

using System.Text;

using MathCore.PE.Tables;

namespace MathCore.PE;

/// <summary>Вспомогательные методы для работы с PE-файлами</summary>
public static class PEFileExtensions
{
    /// <summary>Получает все иконки из PE-файла</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <returns>Список данных иконок</returns>
    public static IEnumerable<byte[]> ExtractIcons(this PEFile PEFile)
    {
        var resources = PEFile.GetResources();
        if (resources is null)
            return [];

        var icons = new List<byte[]>();
        foreach (var icon in resources.GetResourcesByType(ResourceType.Icon))
        {
            try
            {
                icons.Add(resources.ExtractResourceData(icon.DataEntry));
            }
            catch { }
        }

        return [.. icons];
    }

    /// <summary>Получает все растровые изображения из PE-файла</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <returns>Список данных растровых изображений</returns>
    public static IEnumerable<byte[]> ExtractBitmaps(this PEFile PEFile)
    {
        var resources = PEFile.GetResources();
        if (resources is null)
            yield break;

        var bitmaps = new List<byte[]>();
        foreach (var bitmap in resources.GetResourcesByType(ResourceType.Bitmap))
        {
            try
            {
                bitmaps.Add(resources.ExtractResourceData(bitmap.DataEntry));
            }
            catch { }
        }

        foreach (var bitmap in bitmaps)
        {
            yield return bitmap;
        }
    }

    /// <summary>Получает все звуковые ресурсы из PE-файла</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <returns>Список данных звуков</returns>
    public static IEnumerable<byte[]> ExtractSounds(this PEFile PEFile)
    {
        var resources = PEFile.GetResources();
        if (resources is null)
            yield break;

        var sounds = new List<byte[]>();
        foreach (var sound in resources.GetResourcesByType(ResourceType.Sound))
        {
            try
            {
                sounds.Add(resources.ExtractResourceData(sound.DataEntry));
            }
            catch { }
        }

        foreach (var sound in sounds)
        {
            yield return sound;
        }
    }

    /// <summary>Сохраняет все ресурсы указанного типа в директорию</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <param name="Type">Тип ресурса</param>
    /// <param name="OutputDirectory">Директория для сохранения</param>
    /// <param name="Prefix">Префикс для имён файлов</param>
    public static void SaveResourcesOfType(this PEFile PEFile, ResourceType Type, string OutputDirectory, string Prefix = "resource")
    {
        var resources = PEFile.GetResources();
        if (resources is null)
            return;

        Directory.CreateDirectory(OutputDirectory);

        var extension = GetFileExtensionForResourceType(Type);
        var index = 0;

        foreach (var resource in resources.GetResourcesByType(Type))
        {
            try
            {
                var data = resources.ExtractResourceData(resource.DataEntry);
                var filename = Path.Combine(OutputDirectory, $"{Prefix}_{index}{extension}");
                File.WriteAllBytes(filename, data);
                index++;
            }
            catch { }
        }
    }

    /// <summary>Сохраняет все иконки в директорию</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <param name="OutputDirectory">Директория для сохранения</param>
    public static void SaveIcons(this PEFile PEFile, string OutputDirectory)
    {
        PEFile.SaveResourcesOfType(ResourceType.Icon, OutputDirectory, "icon");
    }

    /// <summary>Сохраняет все растровые изображения в директорию</summary>
    /// <param name="PEFile">PE-файл</param>
    /// <param name="OutputDirectory">Директория для сохранения</param>
    public static void SaveBitmaps(this PEFile PEFile, string OutputDirectory)
    {
        PEFile.SaveResourcesOfType(ResourceType.Bitmap, OutputDirectory, "bitmap");
    }

    /// <summary>Получает информацию об экспортируемых функциях в виде текста</summary>
    public static string GetExportsSummary(this PEFile PEFile)
    {
        var exports = PEFile.GetExports();
        if (exports is null)
            return "Нет информации об экспортах";

        var sb = new StringBuilder();
        sb.AppendLine($"Модуль: {exports.ModuleName}");
        sb.AppendLine($"Дата создания: {exports.TimeDateStamp}");
        sb.AppendLine($"Версия: {exports.Version}");
        sb.AppendLine($"Количество функций: {exports.FunctionCount}");
        sb.AppendLine($"Количество именованных функций: {exports.NamedFunctionCount}");
        sb.AppendLine();
        sb.AppendLine("Экспортируемые функции:");

        foreach (var func in exports.Functions.Take(20))
        {
            sb.AppendLine($"  {func}");
        }

        if (exports.Functions.Count > 20)
        {
            sb.AppendLine($"  ... и ещё {exports.Functions.Count - 20} функций");
        }

        return sb.ToString();
    }

    /// <summary>Получает информацию об импортируемых функциях в виде текста</summary>
    public static string GetImportsSummary(this PEFile PEFile)
    {
        var imports = PEFile.GetImports();
        if (imports is null)
            return "Нет информации об импортах";

        var sb = new StringBuilder();
        sb.AppendLine($"Импортируемые библиотеки: {imports.Libraries.Count}");
        sb.AppendLine();

        foreach (var library in imports.Libraries.Take(10))
        {
            sb.AppendLine($"  {library.Name}");
            foreach (var func in library.ImportedFunctions.Take(5))
            {
                sb.AppendLine($"    - {func}");
            }

            if (library.ImportedFunctions.Count > 5)
            {
                sb.AppendLine($"    ... и ещё {library.ImportedFunctions.Count - 5} функций");
            }
        }

        if (imports.Libraries.Count > 10)
        {
            sb.AppendLine($"  ... и ещё {imports.Libraries.Count - 10} библиотек");
        }

        return sb.ToString();
    }

    /// <summary>Получает информацию о ресурсах в виде текста</summary>
    public static string GetResourcesSummary(this PEFile PEFile)
    {
        var resources = PEFile.GetResources();
        if (resources is null)
            return "Нет информации о ресурсах";

        var sb = new StringBuilder();
        sb.AppendLine($"Всего ресурсов: {resources.Resources.Count}");
        sb.AppendLine();

        var groupedByType = resources.Resources.GroupBy(r => r.Type);
        foreach (var group in groupedByType)
        {
            var typeName = Enum.IsDefined(typeof(ResourceType), group.Key)
                ? ((ResourceType)group.Key).ToString()
                : $"Type({group.Key})";
            sb.AppendLine($"  {typeName}: {group.Count()}");
        }

        return sb.ToString();
    }

    private static string GetFileExtensionForResourceType(ResourceType Type) => Type switch
    {
        ResourceType.Icon => ".ico",
        ResourceType.Bitmap => ".bmp",
        ResourceType.Sound => ".wav",
        ResourceType.Dialog => ".dlg",
        ResourceType.Menu => ".mnu",
        ResourceType.String => ".str",
        _ => ".bin"
    };
}
