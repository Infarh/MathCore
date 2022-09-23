using MathCore.Algorithms.Numbers;

var (mantissa, exp, sign) = DoubleIEEE754.Parse(0.5);

var value = DoubleIEEE754.Create((1L << 50) | (1L << 51), 0, false);

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
