using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

using MathCore.CSV;
using MathCore.PE;

var str = "123.456E-3";

var d = double.Parse(str, CultureInfo.InvariantCulture);

var pstr = str.AsStringPtr();
var d2   = pstr.ParseDouble(CultureInfo.InvariantCulture);


Console.WriteLine("End.");
Console.ReadLine();

return;

//var pe_file = new PEFile("c:\\123\\user32.dll");

////pe_file.ReadData();

////var is_pe = pe_file.IsPE;
//var header = pe_file.GetHeader();

//var range = new Range(-10, 5);

//Console.WriteLine("End.");
//Console.ReadLine();