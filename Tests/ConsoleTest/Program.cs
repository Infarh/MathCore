const uint poly = 0xEDB88320;

var table = new uint[256];

FillTableCRC32RefOut(table, poly);

var data = "Hello, CRC32!"u8.ToArray();

Console.WriteLine($"data: {data.Select(b => b.ToString("x2")).JoinStrings("")}");

var crc = 0xFFFFFFFFu;

foreach (var b in data)
{
    var i = (crc ^ b) & 0xFF;
    crc = (crc >> 8) ^ table[i];
}

crc ^= 0xFFFFFFFFu;

Console.WriteLine($"{crc:x8}");

Console.WriteLine("End.");
return;

static uint ComputeCRC32(
    byte[] data,
    uint[] table,
    uint CRC = 0xFFFFFFFFu,
    uint XOR = 0xFFFFFFFFu,
    bool RefIn = false,
    bool RefOut = false)
{
    var crc = CRC;

    if (RefIn)
        foreach (var b in data)
            crc = (crc >> 8) ^ table[(crc ^ b) & 0xFF];
    else
        foreach (var b in data)
            crc = (crc << 8) ^ table[((crc >> 24) ^ b) & 0xFF];

    crc ^= XOR;

    return RefOut
        ? crc.ReverseBits()
        : crc;
}

static uint ComputeCRC32WithoutTable(
    byte[] data,
    uint poly = 0xEDB88320,
    uint CRC = 0xFFFFFFFFu,
    uint XOR = 0xFFFFFFFFu,
    bool RefIn = false,
    bool RefOut = false)
{
    var crc = CRC;

    foreach (var b in data)
    {
        crc ^= RefIn ? b.ReverseBits() : b;

        for (var i = 0; i < 8; i++)
            crc = (crc & 1) != 0
                ? (crc >> 1) ^ poly
                : (crc >> 1);
    }

    crc ^= XOR;

    return RefOut
        ? crc.ReverseBits()
        : crc;
}

static void FillTableCRC32RefOut(uint[] table, uint poly, bool RefIn = true, bool RefOut = false)
{
    table.AssertLength(256);

    if (RefOut)
    {
        if (RefIn)
            for (var i = 0u; i < table.Length; i++) // RefIn RefOut
            {
                var t = i;

                for (var j = 0; j < 8; j++)
                    t = (t & 1) == 1
                        ? (t >> 1) ^ poly
                        : (t >> 1);

                table[i] = t.ReverseBits();
            }
        else                                        // RefOut
            for (var i = 0u; i < table.Length; i++)
            {
                var t = i;

                for (var j = 0; j < 8; j++)
                    t = (t & 0x80000000) != 0
                        ? (t << 1) ^ poly
                        : (t << 1);

                table[i] = t.ReverseBits();
            }

        return;
    }

    if (RefIn)                                  // RefIn
        for (var i = 0u; i < table.Length; i++)
        {
            ref var t = ref table[i];
            t = i;

            for (var j = 0; j < 8; j++)
                t = (t & 1) == 1
                    ? (t >> 1) ^ poly
                    : (t >> 1);
        }
    else                                        // !RefIn && !RefOut
        for (var i = 0u; i < table.Length; i++)
        {
            ref var t = ref table[i];
            t = i;

            for (var j = 0; j < 8; j++)
                t = (t & 0x80000000) != 0
                    ? (t << 1) ^ poly
                    : (t << 1);
        }
}
