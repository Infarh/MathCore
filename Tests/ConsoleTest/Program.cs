
using System.IO.Compression;
using System.IO.Hashing;

using MathCore.Hash.CRC;

const string data_file_name = "data.file";
const string zip_file_name = $"{data_file_name}.zip";

using (var file = File.CreateText(data_file_name))
    for(var i = 0; i < 1000; i++)
        file.WriteLine("Hello World!!!");

File.Delete(zip_file_name);
using (var zip = ZipFile.Open(zip_file_name, ZipArchiveMode.Create))
{
    var entry = zip.CreateEntry(data_file_name, CompressionLevel.SmallestSize);
    using var entry_stream = entry.Open();
    using var file = File.OpenRead(data_file_name);
    file.CopyTo(entry_stream);
}

string zip_crc;
using (var zip = ZipFile.Open(zip_file_name, ZipArchiveMode.Read))
{
    var entry = zip.GetEntry(data_file_name);
    zip_crc = entry.Crc32.ToString("X8");
}

//var data_file = new FileInfo(data_file_name);
//var data = File.ReadAllBytes(data_file_name);
//var ccc = CRC32.Hash(data).ToString("X8");
//var cc1 = BitConverter.ToUInt32(Crc32.Hash(data)).ToString("X8");

var data1 = "Hello World!"u8.ToArray();

var zz = new Crc32();

var cc3 = Crc32.HashToUInt32(data1).ToString("b32");

//var q = CRC32.Hash(data1, poly: 0x77073096).ToString("b32");

var tabls = CRC32.GetTableReverseBits(0x77073096);

var q = Crc32.HashToUInt32("123456789"u8).ToString("X8");

for (var i = 0u; i < 256; i++)
{
    if(i % 8 == 0) Console.WriteLine();

    //var poly = CRC32.Table(i, 0x77073096);
    var poly = tabls[i];
    Console.Write($"0x{poly:X8} ");
}

var ccrc32 = new CRC32(0x77073096)
{
    State = 0xFFFFFFFF,
    XOR = 0xFFFFFFFF,
};
var cc4 = ccrc32.Compute(data1).ToString("b32");
//const uint expected_crc = 0x7AC1161F;


//var crc32 = data_file.ComputeCRC32().ToString("X8");
//var crc322 = data_file.ComputeCRC32().ToString("X8");

Console.WriteLine("End.");
return;