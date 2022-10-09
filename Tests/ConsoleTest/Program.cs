using System.Xml;
using System.Xml.Serialization;

using MathCore.CSV;
using MathCore.PE;

var str = " 123;qwe;asd;zxc;000;111;456 ";

var xml_serializer  = new XmlSerializer(typeof(string));
var writer = new XmlTextWriter(Console.Out);
writer.Formatting = Formatting.Indented;
xml_serializer.Serialize(writer, str);

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