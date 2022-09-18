using MathCore.PE;

var pe_file = new PEFile("c:\\123\\user32.dll");

//pe_file.ReadData();

//var is_pe = pe_file.IsPE;
var header = pe_file.GetHeader();

var range = new Range(-10, 5);

Console.WriteLine("End.");
Console.ReadLine();