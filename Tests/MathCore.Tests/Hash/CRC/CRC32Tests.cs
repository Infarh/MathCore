using System.Globalization;
using System.Text;

using MathCore.Hash.CRC;

// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable InconsistentNaming

namespace MathCore.Tests.Hash.CRC;

[TestClass]
public class CRC32Tests
{
    // https://microsin.net/programming/arm/crc32-demystified.html
    // https://crccalc.com/

    private const string __Table_poly_0x4C11DB7_str = """ 
        0x00000000 0x04c11db7 0x09823b6e 0x0d4326d9 0x130476dc 0x17c56b6b 0x1a864db2 0x1e475005
        0x2608edb8 0x22c9f00f 0x2f8ad6d6 0x2b4bcb61 0x350c9b64 0x31cd86d3 0x3c8ea00a 0x384fbdbd
        0x4c11db70 0x48d0c6c7 0x4593e01e 0x4152fda9 0x5f15adac 0x5bd4b01b 0x569796c2 0x52568b75
        0x6a1936c8 0x6ed82b7f 0x639b0da6 0x675a1011 0x791d4014 0x7ddc5da3 0x709f7b7a 0x745e66cd
        0x9823b6e0 0x9ce2ab57 0x91a18d8e 0x95609039 0x8b27c03c 0x8fe6dd8b 0x82a5fb52 0x8664e6e5
        0xbe2b5b58 0xbaea46ef 0xb7a96036 0xb3687d81 0xad2f2d84 0xa9ee3033 0xa4ad16ea 0xa06c0b5d
        0xd4326d90 0xd0f37027 0xddb056fe 0xd9714b49 0xc7361b4c 0xc3f706fb 0xceb42022 0xca753d95
        0xf23a8028 0xf6fb9d9f 0xfbb8bb46 0xff79a6f1 0xe13ef6f4 0xe5ffeb43 0xe8bccd9a 0xec7dd02d
        0x34867077 0x30476dc0 0x3d044b19 0x39c556ae 0x278206ab 0x23431b1c 0x2e003dc5 0x2ac12072
        0x128e9dcf 0x164f8078 0x1b0ca6a1 0x1fcdbb16 0x018aeb13 0x054bf6a4 0x0808d07d 0x0cc9cdca
        0x7897ab07 0x7c56b6b0 0x71159069 0x75d48dde 0x6b93dddb 0x6f52c06c 0x6211e6b5 0x66d0fb02
        0x5e9f46bf 0x5a5e5b08 0x571d7dd1 0x53dc6066 0x4d9b3063 0x495a2dd4 0x44190b0d 0x40d816ba
        0xaca5c697 0xa864db20 0xa527fdf9 0xa1e6e04e 0xbfa1b04b 0xbb60adfc 0xb6238b25 0xb2e29692
        0x8aad2b2f 0x8e6c3698 0x832f1041 0x87ee0df6 0x99a95df3 0x9d684044 0x902b669d 0x94ea7b2a
        0xe0b41de7 0xe4750050 0xe9362689 0xedf73b3e 0xf3b06b3b 0xf771768c 0xfa325055 0xfef34de2
        0xc6bcf05f 0xc27dede8 0xcf3ecb31 0xcbffd686 0xd5b88683 0xd1799b34 0xdc3abded 0xd8fba05a
        0x690ce0ee 0x6dcdfd59 0x608edb80 0x644fc637 0x7a089632 0x7ec98b85 0x738aad5c 0x774bb0eb
        0x4f040d56 0x4bc510e1 0x46863638 0x42472b8f 0x5c007b8a 0x58c1663d 0x558240e4 0x51435d53
        0x251d3b9e 0x21dc2629 0x2c9f00f0 0x285e1d47 0x36194d42 0x32d850f5 0x3f9b762c 0x3b5a6b9b
        0x0315d626 0x07d4cb91 0x0a97ed48 0x0e56f0ff 0x1011a0fa 0x14d0bd4d 0x19939b94 0x1d528623
        0xf12f560e 0xf5ee4bb9 0xf8ad6d60 0xfc6c70d7 0xe22b20d2 0xe6ea3d65 0xeba91bbc 0xef68060b
        0xd727bbb6 0xd3e6a601 0xdea580d8 0xda649d6f 0xc423cd6a 0xc0e2d0dd 0xcda1f604 0xc960ebb3
        0xbd3e8d7e 0xb9ff90c9 0xb4bcb610 0xb07daba7 0xae3afba2 0xaafbe615 0xa7b8c0cc 0xa379dd7b
        0x9b3660c6 0x9ff77d71 0x92b45ba8 0x9675461f 0x8832161a 0x8cf30bad 0x81b02d74 0x857130c3
        0x5d8a9099 0x594b8d2e 0x5408abf7 0x50c9b640 0x4e8ee645 0x4a4ffbf2 0x470cdd2b 0x43cdc09c
        0x7b827d21 0x7f436096 0x7200464f 0x76c15bf8 0x68860bfd 0x6c47164a 0x61043093 0x65c52d24
        0x119b4be9 0x155a565e 0x18197087 0x1cd86d30 0x029f3d35 0x065e2082 0x0b1d065b 0x0fdc1bec
        0x3793a651 0x3352bbe6 0x3e119d3f 0x3ad08088 0x2497d08d 0x2056cd3a 0x2d15ebe3 0x29d4f654
        0xc5a92679 0xc1683bce 0xcc2b1d17 0xc8ea00a0 0xd6ad50a5 0xd26c4d12 0xdf2f6bcb 0xdbee767c
        0xe3a1cbc1 0xe760d676 0xea23f0af 0xeee2ed18 0xf0a5bd1d 0xf464a0aa 0xf9278673 0xfde69bc4
        0x89b8fd09 0x8d79e0be 0x803ac667 0x84fbdbd0 0x9abc8bd5 0x9e7d9662 0x933eb0bb 0x97ffad0c
        0xafb010b1 0xab710d06 0xa6322bdf 0xa2f33668 0xbcb4666d 0xb8757bda 0xb5365d03 0xb1f740b4
        """;

    private static uint[] Table_poly_0x4C11DB7 => __Table_poly_0x4C11DB7_str
      .EnumLines()
      .SelectMany(line => line.Split(' '))
      .ToArray(s => uint.Parse(s.AsSpan(2), NumberStyles.HexNumber));

    [TestMethod]
    public void GetTable_Poly_0x04C11DB7_RefIn_False()
    {
        const uint poly = 0x04C11DB7;
        var expected_table = Table_poly_0x4C11DB7;

        var actual_table = CRC32.GetTable(poly, RefIn: false);

        actual_table.Length.AssertEquals(expected_table.Length);

        for (var i = 0; i < actual_table.Length; i++)
            if (actual_table[i] != expected_table[i])
                Assert.Fail($"""
                     Значение 
                       Actual[{i}]=0x{actual_table[i]:x8} !=
                     Expected[{i}]=0x{expected_table[i]:x8}
                     """);
    }

    [TestMethod]
    public void StaticHash_123456789_Standard_CRC32()
    {
        // CRC-32/ISO-HDLC стандарт: https://crccalc.com/?crc=123456789&method=CRC-32/ISO-HDLC
        var data = "123456789"u8.ToArray();
        const uint expected = 0xCBF43926;

        // Используем отраженный полином для стандартного CRC32
        const uint poly = 0xEDB88320; // Отраженный вариант 0x04C11DB7
        const uint init = 0xFFFFFFFF;
        const uint xor = 0xFFFFFFFF;

        var result = CRC32.Hash(data, poly, init, RefIn: true, RefOut: false, xor);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void InstanceCompute_123456789_Standard_CRC32()
    {
        var data = "123456789"u8.ToArray();
        const uint expected = 0xCBF43926;

        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result = crc.Compute(data);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void IncrementalCompute_Split_Data()
    {
        var full_data = "123456789"u8.ToArray();
        var part1 = "12345"u8.ToArray();
        var part2 = "6789"u8.ToArray();

        const uint expected = 0xCBF43926;

        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        crc.ContinueCompute(part1);
        crc.ContinueCompute(part2);
        var result = crc.GetResult();

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void Reset_ClearsState()
    {
        var data = "123456789"u8.ToArray();
        const uint expected = 0xCBF43926;

        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result1 = crc.Compute(data);
        crc.Reset();
        var result2 = crc.Compute(data);

        result1.AssertEquals(expected);
        result2.AssertEquals(expected);
        result1.AssertEquals(result2);
    }

    [TestMethod]
    public void ComputeChecksumBytes_Returns_4_Bytes()
    {
        var data = "123456789"u8.ToArray();
        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var checksum_bytes = crc.ComputeChecksumBytes(data);

        checksum_bytes.Length.AssertEquals(4);
    }

    [TestMethod]
    public void EmptyArray_Gives_InitialValue_XOR()
    {
        var empty_data = Array.Empty<byte>();
        const uint expected = 0x00000000; // 0xFFFFFFFF ^ 0xFFFFFFFF

        var crc = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result = crc.Compute(empty_data);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void Mode_ZIP_HelloWorld()
    {
        // CRC-32 (ZIP) для "Hello World!" - используем отраженный полином
        var data = "Hello World!"u8.ToArray();
        const uint expected = 0x1C291CA3;

        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином для ZIP
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result = crc.Compute(data);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void Mode_POSIX_Test()
    {
        // CRC-32/POSIX для "123456789"
        var data = "123456789"u8.ToArray();
        const uint expected = 0x765E7680;

        var result = CRC32.Hash(
            data,
            Polynomial: (uint)CRC32.Mode.POSIX,
            InitialCRC: 0x00000000,
            RefIn: false,
            RefOut: false,
            XOROut: 0xFFFFFFFF);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void Properties_AreCorrect()
    {
        const uint poly = 0x04C11DB7;
        const uint init_value = 0xFFFFFFFF;
        const uint xor_out = 0xFFFFFFFF;
        const bool ref_in = true;
        const bool ref_out = true;

        var crc = new CRC32(poly, init_value, xor_out, ref_in, ref_out);

        crc.Polynomial.AssertEquals(poly);
        crc.InitialValue.AssertEquals(init_value);
        crc.XOROut.AssertEquals(xor_out);
        crc.RefIn.AssertEquals(ref_in);
        crc.RefOut.AssertEquals(ref_out);
    }

    [TestMethod]
    public void State_CanBeModified()
    {
        var crc = new CRC32();
        const uint new_state = 0x12345678;

        crc.State = new_state;

        crc.State.AssertEquals(new_state);
    }

    [TestMethod]
    public void MultipleCompute_Same_Results()
    {
        var data = "Test Data"u8.ToArray();
        var crc = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result1 = crc.Compute(data);
        var result2 = crc.Compute(data);

        result1.AssertEquals(result2);
    }

    [TestMethod]
    public void ContinueCompute_WithReset_Same_As_Compute()
    {
        var data = "Test Data"u8.ToArray();
        var crc1 = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);
        
        var crc2 = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result_compute = crc1.Compute(data);
        
        crc2.Reset();
        crc2.ContinueCompute(data);
        var result_continue = crc2.GetResult();

        result_compute.AssertEquals(result_continue);
    }

#if NET5_0_OR_GREATER
    [TestMethod]
    public void Compute_Stream_Same_As_Array()
    {
        var data = "Test Stream Data"u8.ToArray();
        
        var crc_array = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var crc_stream = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result_array = crc_array.Compute(data);

        using var stream = new MemoryStream(data);
        var result_stream = crc_stream.Compute(stream);

        result_array.AssertEquals(result_stream);
    }

    [TestMethod]
    public async Task ComputeAsync_Stream_Same_As_Sync()
    {
        var data = "Test Stream Data Async"u8.ToArray();
        
        var crc_sync = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var crc_async = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        using var stream_sync = new MemoryStream(data);
        var result_sync = crc_sync.Compute(stream_sync);

        using var stream_async = new MemoryStream(data);
        var result_async = await crc_async.ComputeAsync(stream_async);

        result_sync.AssertEquals(result_async);
    }

    [TestMethod]
    public async Task StaticHashAsync_Works()
    {
        var data = "123456789"u8.ToArray();
        const uint expected_crc = 0xCBF43926; // Стандартный CRC32 для "123456789"

        using var stream = new MemoryStream(data);
        var result = await CRC32.HashAsync(
            stream,
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialCRC: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false,
            XOROut: 0xFFFFFFFF);

        result.AssertEquals(expected_crc);
    }

    [TestMethod]
    public void StaticHash_Stream_Works()
    {
        var data = "123456789"u8.ToArray();
        const uint expected = 0xCBF43926;

        using var stream = new MemoryStream(data);
        var result = CRC32.Hash(
            stream,
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialCRC: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false,
            XOROut: 0xFFFFFFFF);

        result.AssertEquals(expected);
    }

    [TestMethod]
    public void Compute_Span_Works()
    {
        var data = "123456789"u8.ToArray();
        const uint expected = 0xCBF43926;

        var crc = new CRC32(
            Polynomial: 0xEDB88320, // Отраженный полином
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result = crc.Compute(data.AsSpan());

        result.AssertEquals(expected);
    }
#endif

    [TestMethod]
    public void DefaultConstructor_Uses_ZipInv()
    {
        var crc = new CRC32();

        crc.Polynomial.AssertEquals((uint)CRC32.Mode.ZipInv);
    }

    [TestMethod]
    public void TableCache_Reuses_Tables()
    {
        const uint poly = 0x04C11DB7;
        
        var table1 = CRC32.GetTable(poly, RefIn: false);
        var table2 = CRC32.GetTable(poly, RefIn: false);

        // Кэш переиспользует одну и ту же таблицу
        Assert.That.Value(ReferenceEquals(table1, table2)).IsTrue();
    }

    [TestMethod]
    public void LargeData_Incremental_Same_As_Single()
    {
        var large_data = new byte[10000];
        for (var i = 0; i < large_data.Length; i++)
            large_data[i] = (byte)(i % 256);

        var crc_single = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var crc_incremental = new CRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        var result_single = crc_single.Compute(large_data);

        for (var i = 0; i < large_data.Length; i += 100)
        {
            var chunk_size = Math.Min(100, large_data.Length - i);
            var chunk = new byte[chunk_size];
            Array.Copy(large_data, i, chunk, 0, chunk_size);
            crc_incremental.ContinueCompute(chunk);
        }
        var result_incremental = crc_incremental.GetResult();

        result_single.AssertEquals(result_incremental);
    }
}
