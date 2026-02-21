using MathCore.PE.Headers;
using MathCore.PE.Tables;

namespace MathCore.PE;

/// <summary>Парсер PE-файлов (Portable Executable)</summary>
public class PEFile(FileInfo File)
{
    // #define MakePtr(Type, Base, Offset) ((Type)(DWORD(Base) + (DWORD)(Offset)))
    internal static uint MakePtr(uint Base, uint Offset) => Base + Offset;

    private readonly FileInfo _File = File;
    private Header? _Header;

    /// <summary>Информация о файле</summary>
    public FileInfo FileInfo => _File;

    public bool Exists => _File.Exists;

    /// <summary>Проверяет, является ли файл корректным PE-файлом</summary>
    public bool IsPE
    {
        get
        {
            if (!Exists) return false;

            // DOS заголовок минимум 64 байта, NT заголовок требует еще данных
            if (_File.Length < 512) return false;

            try
            {
                using var file = _File.OpenRead();
                _ = Header.Load(file);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>Создаёт экземпляр парсера PE-файла из пути к файлу</summary>
    public PEFile(string FilePath) : this(new FileInfo(FilePath)) { }

    /// <summary>Загружает и кэширует заголовок PE-файла</summary>
    public Header GetHeader()
    {
        if (_Header is { } header)
            return header;

        using var file = _File.OpenRead();
        _Header = Header.Load(file);
        return _Header.Value;
    }

    /// <summary>Преобразует RVA (Relative Virtual Address) в смещение в файле</summary>
    /// <param name="RVA">Относительный виртуальный адрес</param>
    /// <returns>Смещение в файле, или -1 если RVA находится вне разделов</returns>
    public long RvaToFileOffset(uint RVA)
    {
        var header = GetHeader();

        foreach (var section in header.Sections)
        {
            // RVA находится в диапазоне виртуального адреса раздела
            if (RVA >= section.VirtualAddress && RVA < section.VirtualAddress + section.VirtualSize)
            {
                // Но смещение файла должно быть в пределах реального размера раздела в файле
                var offset_in_section = RVA - section.VirtualAddress;
                if (offset_in_section < section.SizeOfRawData)
                {
                    var offset = offset_in_section + section.PointerToRawData;
                    return offset;
                }
                // RVA в диапазоне виртуального размера, но вне реального файла
                return -1;
            }
        }

        return -1;
    }

    /// <summary>Преобразует смещение в файле в RVA (Relative Virtual Address)</summary>
    /// <param name="FileOffset">Смещение в файле</param>
    /// <returns>RVA, или 0 если смещение находится вне разделов</returns>
    public uint FileOffsetToRva(long FileOffset)
    {
        var header = GetHeader();

        foreach (var section in header.Sections)
        {
            if (FileOffset >= section.PointerToRawData &&
                FileOffset < section.PointerToRawData + section.SizeOfRawData)
            {
                var rva = (uint)(FileOffset - section.PointerToRawData) + section.VirtualAddress;
                return rva;
            }
        }

        return 0;
    }

    /// <summary>Читает данные из файла по RVA</summary>
    /// <param name="RVA">Относительный виртуальный адрес</param>
    /// <param name="Size">Количество байт для чтения</param>
    /// <returns>Прочитанные данные</returns>
    public byte[] ReadDataFromRVA(uint RVA, int Size)
    {
        if (Size <= 0)
            throw new ArgumentException("Размер должен быть положительным", nameof(Size));

        var offset = RvaToFileOffset(RVA);
        if (offset < 0)
            throw new InvalidOperationException($"RVA 0x{RVA:X8} находится вне разделов файла");

        using var file = _File.OpenRead();
        file.Seek(offset, SeekOrigin.Begin);

        var data = new byte[Size];
        var read = file.Read(data, 0, Size);

        if (read != Size)
            throw new InvalidOperationException($"Не удалось прочитать полный объём данных из RVA 0x{RVA:X8}: прочитано {read} байт вместо {Size}");

        return data;
    }

    /// <summary>Получает информацию об экспортируемых функциях</summary>
    public ExportedFunctions? GetExports()
    {
        try
        {
            var header = GetHeader();
            var exportDir = header.NT.OptionalHeader.DataDirectory.Export;

            if (exportDir.VirtualAddress == 0 || exportDir.Size == 0)
                return null;

            using var file = _File.OpenRead();
            var offset = RvaToFileOffset(exportDir.VirtualAddress);
            if (offset < 0)
                return null;

            file.Seek(offset, SeekOrigin.Begin);
            var exportsData = file.ReadStructure<IMAGE_EXPORT_DIRECTORY>();

            return new ExportedFunctions(this, exportsData);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Получает информацию о ресурсах в файле</summary>
    public PEResources? GetResources()
    {
        try
        {
            var header = GetHeader();
            var resourceDir = header.NT.OptionalHeader.DataDirectory.Resource;

            if (resourceDir.VirtualAddress == 0 || resourceDir.Size == 0)
                return null;

            return new PEResources(this, resourceDir);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Получает информацию об импортируемых функциях</summary>
    public ImportedFunctions? GetImports()
    {
        try
        {
            var header = GetHeader();
            var importDir = header.NT.OptionalHeader.DataDirectory.Import;

            if (importDir.VirtualAddress == 0 || importDir.Size == 0)
                return null;

            return new ImportedFunctions(this, importDir);
        }
        catch
        {
            return null;
        }
    }
}