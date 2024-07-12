using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.IO.Compression.ZipCompression;
public readonly struct EndCentralDirectory
{
    public const uint SignatureValue = 0x06054b50;


#if NET8_0_OR_GREATER
    public static void Find()
    {
        Span<byte> buffer = stackalloc byte[22];

        ReadOnlySpan<byte> signature = [0x50, 0x4b, 0x05, 0x06];

        var index = buffer.IndexOf(signature);
    }
#endif
}
