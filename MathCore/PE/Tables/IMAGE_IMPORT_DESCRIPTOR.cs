namespace MathCore.PE.Tables;

public readonly struct IMAGE_IMPORT_DESCRIPTOR
{
    public const int Length = 20;

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