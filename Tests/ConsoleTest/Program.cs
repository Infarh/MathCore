using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using MathCore.CSV;
using MathCore.PE;

var str = "123.456E-3";

var d = double.Parse(str, CultureInfo.InvariantCulture);

var pstr = str.AsStringPtr();
var d2   = pstr.ParseDouble(CultureInfo.InvariantCulture);


var src_str           = "md5";
var src_str_bytes = Encoding.UTF8.GetBytes(src_str);
var exp_md5_bytes_str = "1bc29b36f623ba82aaf6724fd3b16718".AsSpan();
var exp_md5_bytes = new byte[exp_md5_bytes_str.Length / 2];
for (var i = 0; i < exp_md5_bytes.Length; i++)
    exp_md5_bytes[i] = byte.Parse(exp_md5_bytes_str.Slice(i * 2, 2), NumberStyles.HexNumber);

var md5_expected = MD5.HashData(src_str_bytes);

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