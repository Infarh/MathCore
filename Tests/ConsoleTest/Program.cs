
using System.IO.Compression;

var str = "Hello World!";

const string test_file_path = "test.txt";

File.WriteAllText(test_file_path, str);

var test_file_info = new FileInfo(test_file_path);

var test_file_crc32 = test_file_info.ComputeCRC32();

Console.WriteLine($"Test file info CRC32: {test_file_crc32:X8}");

var test_file_zip = ZipFile.CreateFromFile(test_file_path, "test.zip");

using (var zip = new ZipArchive(File.OpenRead(test_file_zip.FullName)))
{
    var test_file_entry = zip.GetEntry(test_file_info.Name);
    var entry_crc32 = test_file_entry.Crc32;
    Console.WriteLine($"Zip entry CRC32: {entry_crc32:X8}");
}

Console.WriteLine("End.");
return;

file static class Ex
{
    extension(ZipFile)
    {
        public static FileInfo CreateFromFile(string SourceFilePath, string ZipFilePath)
        {
            File.Delete(ZipFilePath);
            using var zip = ZipFile.Open(ZipFilePath, ZipArchiveMode.Create);
            var source_file = new FileInfo(SourceFilePath);
            zip.CreateEntryFromFile(SourceFilePath, source_file.Name, CompressionLevel.Optimal);
            return new FileInfo(ZipFilePath);
        }
    }
}