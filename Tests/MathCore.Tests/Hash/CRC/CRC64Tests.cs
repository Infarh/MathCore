using System.Diagnostics;

using MathCore.Hash.CRC;

namespace MathCore.Tests.Hash.CRC;

[TestClass]
public class CRC64Tests
{
    [TestMethod]
    public void ISO_Hello_World()
    {
        var data = "Hello World!"u8.ToArray();
        const ulong expected_hash = 0xCA64C7DA170C6241;
        //const ulong expected_hash = 0x7DB9CF17F71CD9AC;

        const CRC64.Mode iso3309 = CRC64.Mode.ISO3309;
        //const ulong xor = 0xFFFFFFFFFFFFFFFF;
        //const ulong initial = 0xFFFFFFFFFFFFFFFF;
        var crc = new CRC64(iso3309)
        {
            //XOR = xor,
            //State = initial,
        };

        var actual_hash_1 = crc.Compute([0x01]);
        //var actual_hash_1 = crc.Compute(data);
        //var actual_hash_2 = CRC64.Hash(data: data, mode: iso3309, crc: initial, xor: xor);

        var actual_hash1 = $"0x{actual_hash_1:X16}";
        //var actual_hash2 = $"0x{actual_hash_2:X16}";

        actual_hash1.ToDebug();
    }
}
