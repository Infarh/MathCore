using System.Runtime.InteropServices;

using MathCore.PE;

var pe_file = new PEFile("c:\\123\\user32.dll");

//pe_file.ReadData();

//var is_pe = pe_file.IsPE;
var header = pe_file.GetHeader();


Console.WriteLine("End.");
Console.ReadLine();

