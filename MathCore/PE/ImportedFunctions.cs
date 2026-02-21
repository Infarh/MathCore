#nullable enable

using System.Runtime.InteropServices;
using System.Text;

using MathCore.PE.Headers;
using MathCore.PE.Tables;

namespace MathCore.PE;

/// <summary>Информация об импортируемых функциях из PE-файла</summary>
public class ImportedFunctions
{
    private readonly PEFile _PEFile;
    private readonly NT.ImageOptionalHeader.ImageDataDirectory.ImageDataDirectoryValue _ImportDirectory;
    private ImportLibrary[]? _Libraries;

    internal ImportedFunctions(PEFile PEFile, NT.ImageOptionalHeader.ImageDataDirectory.ImageDataDirectoryValue ImportDirectory)
    {
        _PEFile = PEFile;
        _ImportDirectory = ImportDirectory;
    }

    /// <summary>Получает список всех импортируемых библиотек</summary>
    public IReadOnlyList<ImportLibrary> Libraries
    {
        get
        {
            if (_Libraries is not null)
                return _Libraries;

            _Libraries = LoadLibraries();
            return _Libraries;
        }
    }

    private ImportLibrary[] LoadLibraries()
    {
        try
        {
            var libraries = new List<ImportLibrary>();
            var descriptorSize = Marshal.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();
            var offset = _PEFile.RvaToFileOffset(_ImportDirectory.VirtualAddress);

            if (offset < 0)
                return [];

            using var file = _PEFile.FileInfo.OpenRead();
            file.Seek(offset, SeekOrigin.Begin);

            while (true)
            {
                var descriptor = file.ReadStructure<IMAGE_IMPORT_DESCRIPTOR>();
                
                if (descriptor.Name == 0)
                    break;

                var libraryName = ReadNullTerminatedString(_PEFile, descriptor.Name);
                var importedFunctions = LoadImportedFunctions(_PEFile, descriptor);

                libraries.Add(new ImportLibrary
                {
                    Name = libraryName,
                    Characteristics = descriptor.OriginalFirstThunk,
                    TimeDateStamp = descriptor.TimeDateStamp,
                    ForwarderChain = descriptor.ForwarderChain,
                    ImportedFunctions = importedFunctions
                });
            }

            return [..libraries];
        }
        catch
        {
            return [];
        }
    }

    private List<ImportedFunction> LoadImportedFunctions(PEFile pefFile, IMAGE_IMPORT_DESCRIPTOR descriptor)
    {
        var functions = new List<ImportedFunction>();
        
        try
        {
            var iatRVA = descriptor.FirstThunk;
            var lookupTableRVA = descriptor.OriginalFirstThunk;
            
            uint index = 0;
            const uint max_imports = 10000; // Защита от бесконечного цикла
            while (index < max_imports)
            {
                // Безопасное вычисление RVA с проверкой переполнения
                var offset_bytes = checked((uint)(index * sizeof(uint)));
                var entry_rva = checked(lookupTableRVA + offset_bytes);
                
                var lookupData = pefFile.ReadDataFromRVA(entry_rva, sizeof(uint));
                var lookupValue = BitConverter.ToUInt32(lookupData, 0);

                if (lookupValue == 0)
                    break;

                var (importName, ordinal) = ParseImportLookupEntry(pefFile, lookupValue);
                
                functions.Add(new ImportedFunction
                {
                    Name = importName,
                    Ordinal = ordinal,
                    Hint = ordinal
                });

                index++;
            }
        }
        catch { }

        return functions;
    }

    private (string?, uint) ParseImportLookupEntry(PEFile pefFile, uint lookupValue)
    {
        if ((lookupValue & 0x80000000) != 0)
        {
            // Импорт по номеру
            var ordinal = lookupValue & 0xFFFF;
            return (null, ordinal);
        }
        else
        {
            // Импорт по имени
            var nameRVA = lookupValue & 0x7FFFFFFF;
            var nameData = pefFile.ReadDataFromRVA(nameRVA, 256);
            
            var hint = BitConverter.ToUInt16(nameData, 0);
            var nullIndex = Array.IndexOf(nameData, (byte)0, 2);
            var name = Encoding.ASCII.GetString(nameData, 2, nullIndex >= 0 ? nullIndex - 2 : nameData.Length - 2);

            return (name, hint);
        }
    }

    private static string ReadNullTerminatedString(PEFile pefFile, uint rva)
    {
        try
        {
            var data = pefFile.ReadDataFromRVA(rva, 256);
            var nullIndex = Array.IndexOf(data, (byte)0);
            return Encoding.ASCII.GetString(data, 0, nullIndex >= 0 ? nullIndex : data.Length);
        }
        catch
        {
            return string.Empty;
        }
    }
}

/// <summary>Информация об одной импортируемой библиотеке</summary>
public class ImportLibrary
{
    /// <summary>Имя библиотеки (DLL)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Характеристики импорта</summary>
    public uint Characteristics { get; set; }

    /// <summary>Дата и времени связывания</summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>Цепь переадресации</summary>
    public uint ForwarderChain { get; set; }

    /// <summary>Импортируемые функции из этой библиотеки</summary>
    public List<ImportedFunction> ImportedFunctions { get; set; } = [];

    public override string ToString() => $"{Name} ({ImportedFunctions.Count} функций)";
}

/// <summary>Информация об одной импортируемой функции</summary>
public class ImportedFunction
{
    /// <summary>Имя функции (может быть null если импортируется по номеру)</summary>
    public string? Name { get; set; }

    /// <summary>Порядковый номер (ordinal) или hint</summary>
    public uint Ordinal { get; set; }

    /// <summary>Hint для поиска функции</summary>
    public uint Hint { get; set; }

    public override string ToString() => Name ?? $"#{Ordinal}";
}
