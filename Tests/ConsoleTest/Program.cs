
using ConsoleTest;

var (m, n) = (4, 2);
Matrix A = new((m, n), 2, 1, 1, 3, 4, 2, 3, 4);

var a = A.GetData();

MatrixTST.SVD(a, out var s, out var u, out var vt);

var W = (Matrix)s;
var U = (Matrix)u;
var Vt = (Matrix)vt;

//var (q, r) = A.QR();
//var s = new double[Math.Min(n, m)];
//for (var i = 0; i < s.Length; i++)
//    s[i] = r[i, i];

Console.WriteLine($"A = \r\n{A:f0}");
Console.WriteLine();

Console.WriteLine($"W = \r\n{W:+0.0000;-0.0000}");
Console.WriteLine();

Console.WriteLine($"U = \r\n{U:+0.0000;-0.0000}");
Console.WriteLine();

Console.WriteLine($"Vt = \r\n{Vt:+0.0000;-0.0000}");
Console.WriteLine();

//Console.WriteLine($"q = \r\n{q:+0.0000;-0.0000}");
//Console.WriteLine();
//Console.WriteLine($"s = {s.Select(v => v.ToString("f4")).JoinStrings(", ")}");
//Console.WriteLine();
//Console.WriteLine($"V = \r\n{V:f4}");
Console.WriteLine();

var A1 = U * Matrix.Diagonal(s) * Vt;

Console.WriteLine($"A1 = \r\n{A1:+0.0000;-0.0000}");
Console.WriteLine();

var A2 = Vt.T * Matrix.Diagonal(s.ToArray(x => 1/x)) * U.T;

Console.WriteLine($"A2 = \r\n{A2:+0.0000;-0.0000}");
Console.WriteLine();

var B = Matrix.Col(5, 6, 11, 12);
var x0 = A2 * B;

Console.WriteLine($"x0 = \r\n{x0:+0.0000;-0.0000}");
Console.WriteLine();

Console.WriteLine("End.");
return;