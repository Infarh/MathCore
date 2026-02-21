#nullable enable

using System.Text;

using MathCore.PE.Headers;
using MathCore.PE.Tables;

namespace MathCore.PE;

/// <summary>Информация об экспортируемых функциях из PE-файла</summary>
public class ExportedFunctions
{
    private readonly PEFile _PEFile;
    private readonly IMAGE_EXPORT_DIRECTORY _ExportDirectory;
    private ExportedFunction[]? _Functions;
    private string? _ModuleName;

    internal ExportedFunctions(PEFile PEFile, IMAGE_EXPORT_DIRECTORY ExportDirectory)
    {
        _PEFile = PEFile;
        _ExportDirectory = ExportDirectory;
    }

    /// <summary>Имя модуля (DLL)</summary>
    public string ModuleName
    {
        get
        {
            if (_ModuleName is not null)
                return _ModuleName;

            try
            {
                var nameData = _PEFile.ReadDataFromRVA(_ExportDirectory.Name, 256);
                var nullIndex = Array.IndexOf(nameData, (byte)0);
                _ModuleName = Encoding.ASCII.GetString(nameData, 0, nullIndex >= 0 ? nullIndex : nameData.Length);
            }
            catch
            {
                _ModuleName = string.Empty;
            }

            return _ModuleName;
        }
    }

    /// <summary>Дата и время создания таблицы экспорта</summary>
    public DateTime TimeDateStamp => UnixTimeStampToDateTime(_ExportDirectory.TimeDateStamp);

    /// <summary>Номер версии (старший.младший)</summary>
    public Version Version => new(
        _ExportDirectory.MajorVersion,
        _ExportDirectory.MinorVersion
    );

    /// <summary>Начальный номер экспортируемых функций</summary>
    public uint Base => _ExportDirectory.Base;

    /// <summary>Количество экспортируемых функций</summary>
    public uint FunctionCount => _ExportDirectory.NumberOfFunctions;

    /// <summary>Количество именованных функций</summary>
    public uint NamedFunctionCount => _ExportDirectory.NumberOfNames;

    /// <summary>Получает список всех экспортируемых функций</summary>
    public IReadOnlyList<ExportedFunction> Functions
    {
        get
        {
            if (_Functions is not null)
                return _Functions;

            _Functions = LoadFunctions();
            return _Functions;
        }
    }

    private ExportedFunction[] LoadFunctions()
    {
        try
        {
            var functions = new List<ExportedFunction>();

            // Читаем таблицу адресов функций
            var addressTableData = _PEFile.ReadDataFromRVA(
                _ExportDirectory.AddressOfFunctions,
                (int)_ExportDirectory.NumberOfFunctions * sizeof(uint)
            );

            // Читаем таблицу имён (если есть)
            string[] names = [];
            uint[] nameOrdinals = [];
            
            if (_ExportDirectory.NumberOfNames > 0)
            {
                var nameTableData = _PEFile.ReadDataFromRVA(
                    _ExportDirectory.AddressOfNames,
                    (int)_ExportDirectory.NumberOfNames * sizeof(uint)
                );

                var ordinalsData = _PEFile.ReadDataFromRVA(
                    _ExportDirectory.AddressOfNameOrdinals,
                    (int)_ExportDirectory.NumberOfNames * sizeof(ushort)
                );

                names = new string[_ExportDirectory.NumberOfNames];
                nameOrdinals = new uint[_ExportDirectory.NumberOfNames];

                for (int i = 0; i < _ExportDirectory.NumberOfNames; i++)
                {
                    var nameRVA = BitConverter.ToUInt32(nameTableData, i * sizeof(uint));
                    var ordinal = BitConverter.ToUInt16(ordinalsData, i * sizeof(ushort));

                    try
                    {
                        var nameData = _PEFile.ReadDataFromRVA(nameRVA, 256);
                        var nullIndex = Array.IndexOf(nameData, (byte)0);
                        names[i] = Encoding.ASCII.GetString(nameData, 0, nullIndex >= 0 ? nullIndex : nameData.Length);
                        nameOrdinals[i] = ordinal;
                    }
                    catch { }
                }
            }

            // Создаём структуры функций
            var nameDict = new Dictionary<uint, string>();
            for (int i = 0; i < names.Length; i++)
            {
                if (!string.IsNullOrEmpty(names[i]))
                    nameDict[nameOrdinals[i]] = names[i];
            }

            for (uint i = 0; i < _ExportDirectory.NumberOfFunctions; i++)
            {
                var functionRVA = BitConverter.ToUInt32(addressTableData, (int)i * sizeof(uint));
                var ordinal = (uint)(_ExportDirectory.Base + i);
                var name = nameDict.TryGetValue(i, out var foundName) ? foundName : null;

                functions.Add(new ExportedFunction
                {
                    Ordinal = ordinal,
                    Name = name,
                    Address = functionRVA
                });
            }

            return [..functions];
        }
        catch
        {
            return [];
        }
    }

    private static DateTime UnixTimeStampToDateTime(uint unixTimeStamp)
    {
        if (unixTimeStamp == 0)
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        try
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}

/// <summary>Информация об одной экспортируемой функции</summary>
public class ExportedFunction
{
    /// <summary>Порядковый номер (ordinal) функции</summary>
    public uint Ordinal { get; set; }

    /// <summary>Имя функции (может быть null если функция экспортируется только по номеру)</summary>
    public string? Name { get; set; }

    /// <summary>Относительный виртуальный адрес (RVA) функции</summary>
    public uint Address { get; set; }

    public override string ToString() => Name is not null 
        ? $"{Name}@{Ordinal} (RVA: 0x{Address:X8})"
        : $"@{Ordinal} (RVA: 0x{Address:X8})";
}
