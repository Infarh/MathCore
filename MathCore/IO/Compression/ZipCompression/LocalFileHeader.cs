using System.Runtime.InteropServices;

namespace MathCore.IO.Compression.ZipCompression;

[StructLayout(LayoutKind.Sequential)]
public readonly struct LocalFileHeader()
{
    public const uint SignatureValue = 0x04034b50;

    /// <summary>Должно иметь значение 0x04034b50</summary>
    public uint Signature { get; init; } = SignatureValue;

    /// <summary>Должно иметь значение 0x1404</summary>
    public ushort VersionToExtract { get; init; } = 0x1404;

    /// <summary>Бит общего назначения</summary>
    public ushort GeneralProposeBit { get; init; }

    /// <summary>Метод сжатия (0 - без сжатия, 8 - deflate)</summary>
    public ushort CompressionMethod { get; init; }

    /// <summary>Время модификации файла</summary>
    public ushort ModificationTime { get; init; }

    /// <summary>Дата модификации файла</summary>
    public ushort ModificationDate { get; init; }

    /// <summary>Контрольная сумма</summary>
    public uint CRC32 { get; init; }
    
    /// <summary>Сжатый размер</summary>
    public uint CompressedSize { get; init; }
    
    /// <summary>Несжатый размер</summary>
    public uint UncompressedSize { get; init; }
    
    /// <summary>Длина имени файла в байтах</summary>
    public uint FilenameLength { get; init; }
    
    /// <summary>Длина поля с дополнительными данными</summary>
    public uint ExtraFieldLength { get; init; }
}
