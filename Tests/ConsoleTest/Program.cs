
using OxyPlot.Axes;

var x0 = 0.01;
var y0 = x0.RoundAdaptive(2);

var x1 = 9999.123456;
var x2 = 1.000001234;
var x3 = 1234567.89;

var y1 = RoundToTheHighestDigits(x1, 5);
var y2 = RoundToTheHighestDigits(x2, 5);
var y3 = RoundToTheHighestDigits(x3, 5);

double RoundToTheHighestDigits(double x, int n)
{
    if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Число разрядов должно быть > 0");
    if (x == 0) return 0;
    if (x < 0) return RoundToTheHighestDigits(-x, n);


    var d = Math.Ceiling(Math.Log10(x));

    var q = Math.Pow(10, n - d);
    var y = Math.Floor(x * q) / q;

    return y;
}


//const string file_name = @"d:\123\test.txt";

//var watcher = new TextFileContentMonitor(file_name);

//watcher.NewContent += (s, e) =>
//{
//    Console.WriteLine("--------------");
//    Console.WriteLine(e.ToString());
//};

//watcher.Start();

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

