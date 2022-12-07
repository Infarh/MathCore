//using MathCore.Algorithms.Numbers;

//Pascal.Run(16);

//return;

//DoubleIEEE754.TestParsingStrings();

//var xx = 123.456;
//var yy = xx.Exp10(-3);

//var (mantissa123, exp123, sign123) = DoubleIEEE754.Parse(123.456);
//var (mantissa321, exp321, sign321) = DoubleIEEE754.Parse(0.123456);


////CalculatorPI.IterationPI();

//var (m1, k1) = DoubleIEEE754.GetPower2(+8);
//var (m2, k2) = DoubleIEEE754.GetPower2(-8);
//var (m3, k3) = DoubleIEEE754.GetPower21(+8);
//var (m4, k4) = DoubleIEEE754.GetPower21(-8);

//var (m, k) = DoubleIEEE754.GetPower21(-8);

//var x = 123.456;
//var x0 = 123.456e-3;

//var mm = 0;
//var y = x;
//while (y > 2)
//    (y, mm) = (y / 2, mm + 1);

//var (m0, k0) = DoubleIEEE754.GetPower2(-3);

//var y1 = y * k0 * Math.Pow(2, mm + m0);

//var (mantissa, exp, sign) = DoubleIEEE754.Parse(x);

//var value = DoubleIEEE754.Create(mantissa, (short)(exp + m0), sign);
//var v1 = value * k0;
//var (mantissa1, exp1, sign1) = DoubleIEEE754.Parse(v1);
//var (mantissa2, exp2, sign2) = DoubleIEEE754.Parse(0.123456);
//var value1 = DoubleIEEE754.Create(mantissa2, exp2, sign2);

//var qqq = double.Parse("0.123456".AsSpan(), NumberStyles.Any, CultureInfo.InvariantCulture);

//var www = Convert.ToDouble("0.123456");

//var b1 = Convert.ToString(mantissa, 2);
//var b2 = Convert.ToString(mantissa1, 2);
//var b3 = Convert.ToString(mantissa2, 2);


//var items = DoubleIEEE754.Decode(x);

//const int digits_count = 100000;
//CalculatorPI.Calculate(digits_count);
//Console.WriteLine();
//Console.WriteLine("--------------------------------");
//var progress = new ConsoleProgressBar(80);
//Console.WriteLine();
//var pi = await CalculatorPI.CalculateParallelAsync(digits_count/*, progress*/);

Console.WriteLine("..");
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

//Console.WriteLine("End.");
//Console.ReadLine();


//static uint DigitsToUInt32(ReadOnlySpan<char> p, int count)
//{
//    var res = (uint)(p[0] - '0');
//    for (var i = 1; i < count; i++)
//        res = res * 10 + (uint)(p[i] - '0');

//    return res;
//}

//static ulong DigitsToUInt64(ReadOnlySpan<char> p, int count)
//{
//    var res = (ulong)(p[0] - '0');
//    for (var i = 1; i < count; i++)
//        res = res * 10 + (ulong)(p[i] - '0');
//    return res;
//}