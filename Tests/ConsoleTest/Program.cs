using System.Globalization;
using System.Text.RegularExpressions;

using MathCore.CSV;
using MathCore.PE;

bool Selector(FileInfo f)
{
    var name = f.Name;
    if (name.EndsWith(".g.cs")) return false;
    if (name.EndsWith(".g.i.cs")) return false;
    if (f.FullName.Contains(@"\bin\", StringComparison.OrdinalIgnoreCase)) return false;
    if (f.FullName.Contains(@"\obj\", StringComparison.OrdinalIgnoreCase)) return false;
    if (f.FullName.Contains("RRJ-Express.3D.Unity")) return false;
    if (f.FullName.EndsWith("assemblyinfo.cs", StringComparison.OrdinalIgnoreCase)) return false;
    if (f.FullName.Contains(@"Algorithms\RRJ", StringComparison.OrdinalIgnoreCase)) return false;
    if (f.FullName.Contains(@"\Migrations\", StringComparison.OrdinalIgnoreCase)) return false;
    if (f.FullName.EndsWith(@"Designer.cs", StringComparison.OrdinalIgnoreCase)) return false;
    return true;
}

var src_dir   = new DirectoryInfo(@"C:\Users\shmac\src");
var src_files = src_dir.EnumerateFiles("*.cs", SearchOption.AllDirectories)
   .Where(Selector)
   .ToArray();

var total_lines = 0;
var culture     = new NumberFormatInfo { NumberGroupSeparator = "'" };
for (var i = 0; i < src_files.Length; i++)
{
    var file = src_files[i];

    total_lines   += file.ReadLines().Count(l => !string.IsNullOrWhiteSpace(l) && !l.TrimStart(' ').StartsWith("//"));
    Console.Title =  $"Lines {total_lines.ToString("N0", culture)} - {(i + 1d) / src_files.Length:p2}";
    Console.WriteLine(file.FullName.TrimByLength(Console.BufferWidth));
}

Console.WriteLine("Total lines {0}", total_lines.ToString("N0", culture));

var str = " 123;qwe;asd;zxc;000;111;456 ";

var values = str.AsStringPtr().Trim().Split(';');
if (values is [ ['1', .., '3'] a, var b, .. var ss, var c, var d])
{
    Process((int)a, b, (double)c, d, ss);
}

void Process(int a, string b, double c, string d, string sss)
{
    Console.WriteLine($"a:{a}, b:{b}, c:{c}, d:{d} - sss:{sss}");
}


Console.ReadLine();

return;

//var pe_file = new PEFile("c:\\123\\user32.dll");

////pe_file.ReadData();

////var is_pe = pe_file.IsPE;
//var header = pe_file.GetHeader();

//var range = new Range(-10, 5);

//Console.WriteLine("End.");
//Console.ReadLine();