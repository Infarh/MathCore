using System.Drawing;
using System.Globalization;

var str = "123.456E-3";

var d = double.Parse(str, CultureInfo.InvariantCulture);

var pstr = str.AsStringPtr();
var d2   = pstr.ParseDouble(CultureInfo.InvariantCulture);


var p0 = "5-7i";

var point_type = typeof(Point);

var converter = point_type.GetTypeConverter();

var qqq = p0.TryConvertTo(out Complex pp0);

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