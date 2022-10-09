using MathCore.Algorithms.Numbers;

CalculatorPI.IterationPI();

var (m1, k1) = DoubleIEEE754.GetPower2(+8);
var (m2, k2) = DoubleIEEE754.GetPower2(-8);
var (m3, k3) = DoubleIEEE754.GetPower21(+8);
var (m4, k4) = DoubleIEEE754.GetPower21(-8);

var (m, k) = DoubleIEEE754.GetPower21(-8);

var x = 654.321;
var x0 = 654.321e-8;

var mm = 0;
var y = x;
while (y > 2) 
    (y, mm) = (y / 2, mm + 1);

var y1 = y * k * Math.Pow(2, mm + m);

var (mantissa, exp, sign) = DoubleIEEE754.Parse(x);

var value = DoubleIEEE754.Create(mantissa, 0, false);

var items = DoubleIEEE754.Decode(x);

//const int digits_count = 100000;
//CalculatorPI.Calculate(digits_count);
//Console.WriteLine();
//Console.WriteLine("--------------------------------");
//var progress = new ConsoleProgressBar(80);
//Console.WriteLine();
//var pi = await CalculatorPI.CalculateParallelAsync(digits_count/*, progress*/);

Console.ReadLine();

//MatrixRef a = new double[,]
//{
//    { 1, 2, 3 },
//    { 4, 5, 6 },
//    { 7, 8, 9 },
//};

//MatrixRef b = new double[,]
//{
//    { 1, 4, 7 },
//    { 2, 5, 8 },
//    { 3, 6, 9 },
//};

//var c = a + b;

//Console.WriteLine(a.ToString());
//Console.WriteLine();
//Console.WriteLine(b.ToString());
//Console.WriteLine();
//Console.WriteLine(c.ToString());

//var k = BinomialRatio.Value(4, 10);

//NewtonPolynom.Test();

Console.WriteLine("End.");
Console.ReadLine();
