
using System.IO.Compression;

namespace ConsoleTest;
public static class IOTest
{
    public static void Run()
    {
        var file = new FileStream("c:\\123.txt", FileMode.Create, FileAccess.Write, FileShare.Read | FileShare.Write, 1024);
        var buffer = new BufferedStream(file, 1024 * 4 /*4096*/);

        var compressor = new DeflateStream(buffer, CompressionLevel.SmallestSize, true);
        var decompressor = new DeflateStream(buffer, CompressionMode.Decompress, true);

        //var gzip = new GZipStream(file, CompressionLevel.SmallestSize);

        ////file.Position = 0;

        //var bin = new StreamReader(file);

        ////File.AppendAllLines();

        ////bin.ReadB

        //DriveInfo dd;
        //DirectoryInfo dir;

    }
}
