namespace ConsoleTest;

internal static class TestCRC64
{
    public static void Run()
    {
        //byte[] data = [0x80,];
        var data = "Hello World!"u8.ToArray();
        var crc1 = CRC64.Compute1(data, RefOut: false, RefIn: true, crc: 0, xor: 0).ToFormattedString("0x{0:X16}");
        var crc2 = CRC64.Compute2(data, RefOut: false, RefIn: true, crc: 0, xor: 0).ToFormattedString("0x{0:X16}");
        Console.WriteLine($"CRC64: {crc1}");
        Console.WriteLine($"CRC64: {crc2}");
    }

    private static class CRC64
    {
        // Таблица предварительно рассчитанных значений полинома
        private static readonly ulong[] __CRCTable = CreateTable(__Polynomial);

        // Полином для CRC64
        private const ulong __Polynomial = 0x000000000000001BUL;
        //private const ulong __Polynomial = 0x42F0E1EBA9EA3693UL;

        private static ulong[] CreateTable(ulong poly)
        {
            var table = new ulong[256];

            for (ulong i = 0; i < 256; i++)
            {
                var crc = i << 56;
                const ulong mask = 0x80_00_00_00_00_00_00_00UL;
                for (var bit = 0; bit < 8; bit++)
                    crc = (crc & mask) == mask
                        ? crc << 1 ^ poly
                        : crc << 1;

                table[i] = crc;
            }

            return table;
        }

        public static ulong Compute1(byte[] data, ulong crc = 0xFFFFFFFFFFFFFFFFUL, ulong xor = 0xFFFFFFFFFFFFFFFFUL, bool RefIn = false, bool RefOut = false)
        {
            if(RefIn)
                foreach (var b in data)
                {
                    var b0 = b.ReverseBits();
                    crc = (crc << 8) ^ __CRCTable[(crc >> 56) ^ b0];
                }
            else
                foreach (var b in data)
                    crc = (crc << 8) ^ __CRCTable[(crc >> 56) ^ b];

            crc ^= xor;

            return RefOut ? crc.ReverseBits() : crc;
        }

        public static ulong Compute2(byte[] data, ulong crc = 0xFFFFFFFFFFFFFFFFUL, ulong xor = 0xFFFFFFFFFFFFFFFFUL, bool RefIn = false, bool RefOut = false)
        {
            if (RefIn)
                foreach (var b in data)
                    crc = (crc << 8) ^ Table((crc >> 56) ^ b.ReverseBits(), __Polynomial);
            else
                foreach (var b in data)
                    crc = (crc << 8) ^ Table((crc >> 56) ^ b, __Polynomial);

            crc ^= xor;

            return RefOut ? crc.ReverseBits() : crc;

            static ulong Table(ulong i, ulong poly)
            {
                var crc = i << 56;
                const ulong mask = 0x80_00_00_00_00_00_00_00UL;
                for (var bit = 0; bit < 8; bit++)
                    crc = (crc & mask) == mask
                        ? crc << 1 ^ poly
                        : crc << 1;

                return crc;
            }
        }
    }
}
