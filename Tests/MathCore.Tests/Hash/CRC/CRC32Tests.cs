﻿using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

using MathCore.Hash.CRC;

namespace MathCore.Tests.Hash.CRC;

[TestClass]
public class CRC32Tests
{
    [TestMethod]
    public void TableCheck_POSIX()
    {
        var table_04c11db7 = """ 
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

        // https://github.com/Michaelangel007/crc32
        var table_edb88320 = """ 
            0x00000000 0xedb88320 0x36c98560 0xdb710640 0x6d930ac0 0x802b89e0 0x5b5a8fa0 0xb6e20c80
            0xdb261580 0x369e96a0 0xedef90e0 0x005713c0 0xb6b51f40 0x5b0d9c60 0x807c9a20 0x6dc41900
            0x5bf4a820 0xb64c2b00 0x6d3d2d40 0x8085ae60 0x3667a2e0 0xdbdf21c0 0x00ae2780 0xed16a4a0
            0x80d2bda0 0x6d6a3e80 0xb61b38c0 0x5ba3bbe0 0xed41b760 0x00f93440 0xdb883200 0x3630b120
            0xb7e95040 0x5a51d360 0x8120d520 0x6c985600 0xda7a5a80 0x37c2d9a0 0xecb3dfe0 0x010b5cc0
            0x6ccf45c0 0x8177c6e0 0x5a06c0a0 0xb7be4380 0x015c4f00 0xece4cc20 0x3795ca60 0xda2d4940
            0xec1df860 0x01a57b40 0xdad47d00 0x376cfe20 0x818ef2a0 0x6c367180 0xb74777c0 0x5afff4e0
            0x373bede0 0xda836ec0 0x01f26880 0xec4aeba0 0x5aa8e720 0xb7106400 0x6c616240 0x81d9e160
            0x826a23a0 0x6fd2a080 0xb4a3a6c0 0x591b25e0 0xeff92960 0x0241aa40 0xd930ac00 0x34882f20
            0x594c3620 0xb4f4b500 0x6f85b340 0x823d3060 0x34df3ce0 0xd967bfc0 0x0216b980 0xefae3aa0
            0xd99e8b80 0x342608a0 0xef570ee0 0x02ef8dc0 0xb40d8140 0x59b50260 0x82c40420 0x6f7c8700
            0x02b89e00 0xef001d20 0x34711b60 0xd9c99840 0x6f2b94c0 0x829317e0 0x59e211a0 0xb45a9280
            0x358373e0 0xd83bf0c0 0x034af680 0xeef275a0 0x58107920 0xb5a8fa00 0x6ed9fc40 0x83617f60
            0xeea56660 0x031de540 0xd86ce300 0x35d46020 0x83366ca0 0x6e8eef80 0xb5ffe9c0 0x58476ae0
            0x6e77dbc0 0x83cf58e0 0x58be5ea0 0xb506dd80 0x03e4d100 0xee5c5220 0x352d5460 0xd895d740
            0xb551ce40 0x58e94d60 0x83984b20 0x6e20c800 0xd8c2c480 0x357a47a0 0xee0b41e0 0x03b3c2c0
            0xe96cc460 0x04d44740 0xdfa54100 0x321dc220 0x84ffcea0 0x69474d80 0xb2364bc0 0x5f8ec8e0
            0x324ad1e0 0xdff252c0 0x04835480 0xe93bd7a0 0x5fd9db20 0xb2615800 0x69105e40 0x84a8dd60
            0xb2986c40 0x5f20ef60 0x8451e920 0x69e96a00 0xdf0b6680 0x32b3e5a0 0xe9c2e3e0 0x047a60c0
            0x69be79c0 0x8406fae0 0x5f77fca0 0xb2cf7f80 0x042d7300 0xe995f020 0x32e4f660 0xdf5c7540
            0x5e859420 0xb33d1700 0x684c1140 0x85f49260 0x33169ee0 0xdeae1dc0 0x05df1b80 0xe86798a0
            0x85a381a0 0x681b0280 0xb36a04c0 0x5ed287e0 0xe8308b60 0x05880840 0xdef90e00 0x33418d20
            0x05713c00 0xe8c9bf20 0x33b8b960 0xde003a40 0x68e236c0 0x855ab5e0 0x5e2bb3a0 0xb3933080
            0xde572980 0x33efaaa0 0xe89eace0 0x05262fc0 0xb3c42340 0x5e7ca060 0x850da620 0x68b52500
            0x6b06e7c0 0x86be64e0 0x5dcf62a0 0xb077e180 0x0695ed00 0xeb2d6e20 0x305c6860 0xdde4eb40
            0xb020f240 0x5d987160 0x86e97720 0x6b51f400 0xddb3f880 0x300b7ba0 0xeb7a7de0 0x06c2fec0
            0x30f24fe0 0xdd4accc0 0x063bca80 0xeb8349a0 0x5d614520 0xb0d9c600 0x6ba8c040 0x86104360
            0xebd45a60 0x066cd940 0xdd1ddf00 0x30a55c20 0x864750a0 0x6bffd380 0xb08ed5c0 0x5d3656e0
            0xdcefb780 0x315734a0 0xea2632e0 0x079eb1c0 0xb17cbd40 0x5cc43e60 0x87b53820 0x6a0dbb00
            0x07c9a200 0xea712120 0x31002760 0xdcb8a440 0x6a5aa8c0 0x87e22be0 0x5c932da0 0xb12bae80
            0x871b1fa0 0x6aa39c80 0xb1d29ac0 0x5c6a19e0 0xea881560 0x07309640 0xdc419000 0x31f91320
            0x5c3d0a20 0xb1858900 0x6af48f40 0x874c0c60 0x31ae00e0 0xdc1683c0 0x07678580 0xeadf06a0
            """;

        var values = table_edb88320
           .EnumLines()
           .SelectMany(line => line.Split(' '))
           .ToArray(s => uint.Parse(s.AsSpan(2), NumberStyles.HexNumber));

        var crc = new CRC32(0xedb88320);
        var crc_table_info = crc.GetType().GetField("_Table", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var table = (uint[])crc_table_info.GetValue(crc)!;

        table.Length.AssertEquals(values.Length);

        for (var i = 0; i < table.Length; i++)
            if (table[i] != values[i])
                Assert.Fail($"Значение \r\n   Table[{i}]=0x{table[i]:x8} !=\r\nExpected[{i}]=0x{values[i]:x8}");
    }

    [TestMethod]
    public void Poly_04C11DB7_initial_00000000_data_3FA2132103_crc_2AB29C5EU()
    {
        // https://crccalc.com/?crc=3FA2132103&method=CRC-32/POSIX&datatype=hex&outtype=0
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        //const uint expected_crc = 0xD54D63A1 ^ 0xFFFFFFFF;
        const uint expected_crc = 0x2AB29C5EU;

        var crc = new CRC32(CRC32.Mode.POSIX);

        var actual_crc = crc.Compute(data);
        //var inv_crc  = actual_crc ^ 0xFFFFFFFF;

        Debug.WriteLine("Actual   0x{0:X4}", actual_crc);
        Debug.WriteLine("Expected 0x{0:X4}", expected_crc);

        $"0x{actual_crc:X4}".AssertEquals($"0x{expected_crc:X4}");
    }

    [TestMethod]
    public void StaticHash()
    {
        var data = "Hello World!"u8.ToArray();
        const uint expected_crc = 0x7AC1161F;

        var poly = CRC32.Mode.Zip;
        var initial_crc = 0xFFFFFFFF;
        var xor = 0xFFFFFFFF;

        var actual_crc = CRC32.Hash(data, poly, initial_crc, xor);
        var crc_coder = new CRC32(poly) { State = initial_crc, XOR = xor };
        var computed_crc = crc_coder.Compute(data);

        var crc32_actual = $"0x{actual_crc:X8}";
        var crc32_computed = $"0x{computed_crc:X8}";
        var crc32_expected = $"0x{expected_crc:X8}";
        crc32_actual.ToDebug();
        crc32_computed.ToDebug();
        crc32_expected.ToDebug();

        crc32_actual.AssertEquals(crc32_expected);
    }
}
