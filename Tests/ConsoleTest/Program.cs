using System.Text.RegularExpressions;

using MathCore.CSV;
using MathCore.PE;

//var line = """
//    "value1","val2,5",value3
//    """;

//const char separator = ',';
//var splitter = new Regex($@"(?<=(?:{separator}|\n|^))(""(?:(?:"""")*[^""]*)*""|[^""{separator}\n]*|(?:\n|$))", RegexOptions.Compiled);
//var headers = splitter
//   .Matches(line)
//   .Select(m => m.Value is ['"', .. var ss, '"'] ? ss : m.Value)
//   .ToArray();

var line = """
    "value1","val2,5",value3
    """;

const char separator = ',';
var values = Regex
       .Matches(line, $@"(?<=(?:{separator}|\n|^))(""(?:(?:"""")*[^""]*)*""|[^""{separator}\n]*|(?:\n|$))")
       .Select(m => m.Value is ['"', .. var ss, '"'] ? ss : m.Value)
       .ToArray();






Console.ReadLine();

return;

//var pe_file = new PEFile("c:\\123\\user32.dll");

////pe_file.ReadData();

////var is_pe = pe_file.IsPE;
//var header = pe_file.GetHeader();

//var range = new Range(-10, 5);

//Console.WriteLine("End.");
//Console.ReadLine();