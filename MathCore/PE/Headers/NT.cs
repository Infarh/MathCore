using System.IO;
using System.Runtime.InteropServices;

namespace MathCore.PE.Headers;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct NT
{
    public static NT Load(Stream DataStream)
    {
        var nt = DataStream.ReadStructure<NT>();
        return nt;
    }

    //public const int Length = ImageFileHeader.Length + ImageOptionalHeader.Length;
    public const int Length = 264;

    //public const uint CorrectSignatureValue = 0x50450000;
    public const uint CorrectSignatureValue = 0x4550;

    /// <summary>Должно быть равно "PE\0\0" = 0x50450000</summary>
    public uint Signature { get; init; }

    public string SignatureStr => $"{(char)(Signature & 0xff)}{(char)(Signature >> 8)}{(char)(Signature >> 16)}{(char)(Signature >> 24)}";

    public ImageFileHeader FileHeader { get; init; }

    public ImageOptionalHeader OptionalHeader { get; init; }
}