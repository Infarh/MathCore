using System.Runtime.InteropServices;

namespace MathCore.IO.Compression.ZipCompression;

[StructLayout(LayoutKind.Sequential)]
public struct LocalFileHeader()
{
    /// <summary>Должно иметь значение 0x04034b50</summary>
    public uint Signature { get; set; } = 0x04034b50;

    /// <summary>Должно иметь значение 0x1404</summary>
    public ushort VersionToExtract { get; set; } = 0x1404;

    /// <summary>Бит общего назначения</summary>
    public ushort GeneralProposeBit { get; set; }

    /// <summary>Метод сжатия (0 - без сжатия, 8 - deflate)</summary>
    public ushort CompressionMethod { get; set; }

    /// <summary>Время модификации файла</summary>
    public ushort ModificationTime { get; set; }

    /// <summary>Дата модификации файла</summary>
    public ushort ModificationDate { get; set; }

    /// <summary>Контрольная сумма</summary>
    public uint CRC32 { get; set; }
    
    /// <summary>Сжатый размер</summary>
    public uint CompressedSize { get; set; }
    
    /// <summary>Несжатый размер</summary>
    public uint UncompressedSize { get; set; }
    
    /// <summary>Длина имени файла в байтах</summary>
    public uint FilenameLength { get; set; }
    
    /// <summary>Длина поля с дополнительными данными</summary>
    public uint ExtraFieldLength { get; set; }
}
