using System.Runtime.InteropServices;

namespace MathCore.PE.Headers;

[StructLayout(LayoutKind.Sequential)]
public readonly struct IMAGE_EXPORT_DIRECTORY
{
    public const int Length = 40;

    /// <summary>Зарезервировано, всегда равно 0</summary>
    public uint Characteristics { get; init; }
    /// <summary>Дата и время создания таблицы экспорта в формате Unix</summary>
    public uint TimeDateStamp { get; init; }
    /// <summary>Старшая цифра номера версии, не используется</summary>
    public ushort MajorVersion { get; init; }
    /// <summary>Младшая цифра номера версии, не используется</summary>
    public ushort MinorVersion { get; init; }
    /// <summary>RVA ASCIIZ-строки, содержащей имя данного файла</summary>
    public uint Name { get; init; }
    /// <summary>Начальный номер экспортируемых символов (больше или равен 1)</summary>
    public uint Base { get; init; }
    /// <summary>Количество элементов в таблице адресов</summary>
    public uint NumberOfFunctions { get; init; }
    /// <summary>Количество элементов в таблице имен и таблице номеров</summary>
    public uint NumberOfNames { get; init; }
    /// <summary>RVA таблицы адресов</summary>
    public uint AddressOfFunctions { get; init; }
    /// <summary>RVA таблицы имен</summary>
    public uint AddressOfNames { get; init; }
    /// <summary>RVA таблицы номеров</summary>
    public uint AddressOfNameOrdinals { get; init; }
}

public readonly struct IMAGE_IMPORT_DESCRIPTOR
{
    /// <summary>RVA таблицы имен импорта (INT)</summary>
    public uint OriginalFirstThunk { get; init; }
    /// <summary>Дата и время</summary>
    public uint TimeDateStamp { get; init; }
    /// <summary>Индекс первого перенаправленного символа</summary>
    public uint ForwarderChain { get; init; }
    /// <summary>RVA ASCIIZ-строки, содержащей имя DLL</summary>
    public uint Name { get; init; }
    /// <summary>RVA таблицы адресов импорта (IAT)</summary>
    public uint FirstThunk { get; init; }

}