
using System.IO.Compression;

using MathCore.IO.Compression.ZipCompression;

using var zip = Zip.Open("d:\\123\\data.zip");

foreach (var entry in zip)
{

}


return;