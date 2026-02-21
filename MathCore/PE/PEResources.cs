#nullable enable

using System.Runtime.InteropServices;

using MathCore.PE.Headers;
using MathCore.PE.Tables;

namespace MathCore.PE;

/// <summary>Информация о ресурсах в PE-файле</summary>
public class PEResources
{
    private readonly PEFile _PEFile;
    private readonly NT.ImageOptionalHeader.ImageDataDirectory.ImageDataDirectoryValue _ResourceDirectory;
    private ResourceInfo[]? _Resources;
    private IMAGE_RESOURCE_DIRECTORY? _RootDirectory;

    internal PEResources(PEFile PEFile, NT.ImageOptionalHeader.ImageDataDirectory.ImageDataDirectoryValue ResourceDirectory)
    {
        _PEFile = PEFile;
        _ResourceDirectory = ResourceDirectory;
    }

    /// <summary>Относительный виртуальный адрес директории ресурсов</summary>
    public uint RootDirectoryRVA => _ResourceDirectory.VirtualAddress;

    /// <summary>Получает корневую директорию ресурсов</summary>
    public IMAGE_RESOURCE_DIRECTORY RootDirectory
    {
        get
        {
            if (_RootDirectory.HasValue)
                return _RootDirectory.Value;

            try
            {
                var data = _PEFile.ReadDataFromRVA(_ResourceDirectory.VirtualAddress, Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>());
                var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    var ptr = gch.AddrOfPinnedObject();
                    _RootDirectory = Marshal.PtrToStructure<IMAGE_RESOURCE_DIRECTORY>(ptr);
                }
                finally
                {
                    gch.Free();
                }
            }
            catch { }

            return _RootDirectory ?? default;
        }
    }

    /// <summary>Получает все ресурсы в файле</summary>
    public IReadOnlyList<ResourceInfo> Resources
    {
        get
        {
            if (_Resources is not null)
                return _Resources;

            _Resources = EnumerateResources();
            return _Resources;
        }
    }

    /// <summary>Получает ресурсы определённого типа</summary>
    public IEnumerable<ResourceInfo> GetResourcesByType(ResourceType Type)
    {
        return Resources.Where(r => r.Type == (uint)Type);
    }

    /// <summary>Извлекает данные ресурса</summary>
    public byte[] ExtractResourceData(IMAGE_RESOURCE_DATA_ENTRY DataEntry)
    {
        return _PEFile.ReadDataFromRVA(DataEntry.RVA, (int)DataEntry.Size);
    }

    private ResourceInfo[] EnumerateResources()
    {
        var resources = new List<ResourceInfo>();

        try
        {
            var root_dir = RootDirectory; // корневая директория ресурсов (уровень типов)
            var type_entries = ReadDirectoryEntries(root_dir, _ResourceDirectory.VirtualAddress); // записи типов

            foreach (var type_entry in type_entries)
            {
                var type_id = type_entry.NameId;
                var type_dir = ReadDirectory(type_entry.SubdirectoryRVA); // директория имён для данного типа
                var name_entries = ReadDirectoryEntries(type_dir, type_entry.SubdirectoryRVA); // записи имён

                foreach (var name_entry in name_entries)
                {
                    var name_id = name_entry.NameId;
                    var lang_dir = ReadDirectory(name_entry.SubdirectoryRVA); // директория языков для данного имени
                    var lang_entries = ReadDirectoryEntries(lang_dir, name_entry.SubdirectoryRVA); // записи языков

                    foreach (var lang_entry in lang_entries)
                    {
                        var lang_id = lang_entry.NameId;
                        var data_entry = ReadResourceDataEntry(lang_entry.DataRVA);

                        resources.Add(new ResourceInfo
                        {
                            Type = type_id,
                            Name = name_id,
                            Language = lang_id,
                            DataEntry = data_entry
                        });
                    }
                }
            }
        }
        catch { }

        return [.. resources];
    }

    private IMAGE_RESOURCE_DIRECTORY_ENTRY[] ReadDirectoryEntries(IMAGE_RESOURCE_DIRECTORY Directory, uint BaseRVA)
    {
        var entries = new List<IMAGE_RESOURCE_DIRECTORY_ENTRY>();
        var totalEntries = Directory.NumberOfNamedEntries + Directory.NumberOfIdEntries;

        try
        {
            // Первые NumberOfNamedEntries - это именованные записи, потом идут записи с ID
            var entrySize = Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>();
            var offset = BaseRVA + (uint)Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>();

            for (int i = 0; i < totalEntries; i++)
            {
                var data = _PEFile.ReadDataFromRVA(offset + (uint)(i * entrySize), entrySize);
                var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    var ptr = gch.AddrOfPinnedObject();
                    var entry = Marshal.PtrToStructure<IMAGE_RESOURCE_DIRECTORY_ENTRY>(ptr);
                    entries.Add(entry);
                }
                finally
                {
                    gch.Free();
                }
            }
        }
        catch { }

        return [.. entries];
    }

    private IMAGE_RESOURCE_DIRECTORY ReadDirectory(uint RVA)
    {
        try
        {
            var data = _PEFile.ReadDataFromRVA(RVA, Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>());
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var ptr = gch.AddrOfPinnedObject();
                return Marshal.PtrToStructure<IMAGE_RESOURCE_DIRECTORY>(ptr);
            }
            finally
            {
                gch.Free();
            }
        }
        catch
        {
            return default;
        }
    }

    private IMAGE_RESOURCE_DATA_ENTRY ReadResourceDataEntry(uint RVA)
    {
        try
        {
            var data = _PEFile.ReadDataFromRVA(RVA, Marshal.SizeOf<IMAGE_RESOURCE_DATA_ENTRY>());
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var ptr = gch.AddrOfPinnedObject();
                return Marshal.PtrToStructure<IMAGE_RESOURCE_DATA_ENTRY>(ptr);
            }
            finally
            {
                gch.Free();
            }
        }
        catch
        {
            return default;
        }
    }
}

/// <summary>Информация о ресурсе в PE-файле</summary>
public class ResourceInfo
{
    /// <summary>Тип ресурса</summary>
    public uint Type { get; set; }

    /// <summary>Имя или идентификатор ресурса</summary>
    public uint Name { get; set; }

    /// <summary>Язык ресурса</summary>
    public uint Language { get; set; }

    /// <summary>Данные ресурса</summary>
    public IMAGE_RESOURCE_DATA_ENTRY DataEntry { get; set; }

    public override string ToString()
    {
        var typeName = Enum.IsDefined(typeof(ResourceType), Type)
            ? ((ResourceType)Type).ToString()
            : $"Type({Type})";

        return $"{typeName} - Name:{Name}, Language:{Language}";
    }
}
