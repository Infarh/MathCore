using System.IO;
using System.Runtime.InteropServices;

namespace MathCore.PE.Headers;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DOS
{
    public static DOS Load(Stream DataStream)
    {
        var dos = DataStream.ReadStructure<DOS>();
        return dos;
    }

    /// <summary>Размер заголовка 62 байта</summary>
    public const int Length = 62;

    /// <summary>Должно быть равно 'M', 'Z' = 4D5A</summary>
    public ushort Magic { get; init; } // 0x00

    public const ushort CorrectMagicValue = 'M' | 'Z' << 8;

    public string MagicStr => $"{(char)(Magic & 0xFF)}{(char)(Magic >> 8)}";

    public ushort cblp { get; init; } // 0x02

    public ushort cp { get; init; } // 0x04

    public ushort crlc { get; init; } // 0x06

    public ushort cparhdr { get; init; } // 0x08

    public ushort MinAlloc { get; init; } // 0x0a

    public ushort MaxAlloc { get; init; } // 0x0c

    public ushort ss { get; init; } // 0x0e

    public ushort sp { get; init; } // 0x10

    public ushort csum { get; init; } // 0x12

    public ushort ip { get; init; } // 0x14

    public ushort cs { get; init; } // 0x16

    public ushort lfarlc { get; init; } // 0x18

    public ushort ovno { get; init; } // 0x1a

    public ushort res0 { get; init; } // 0x1c

    public ushort res1 { get; init; } // 0x1e

    public ushort res2 { get; init; } // 0x20

    public ushort res3 { get; init; } // 0x22

    public ushort OemId { get; init; } // 0x24

    public ushort OemInfo { get; init; } // 0x26

    public ushort res20 { get; init; } // 0x28
    public ushort res21 { get; init; } // 0x2a
    public ushort res22 { get; init; } // 0x2c
    public ushort res23 { get; init; } // 0x2e
    public ushort res24 { get; init; } // 0x30
    public ushort res25 { get; init; } // 0x32
    public ushort res26 { get; init; } // 0x34
    public ushort res27 { get; init; } // 0x36
    public ushort res28 { get; init; } // 0x38
    public ushort res29 { get; init; } // 0x3a

    /// <summary>Смещение PE-заголовка</summary>
    public ushort lfanew { get; init; } // 0x3c
}