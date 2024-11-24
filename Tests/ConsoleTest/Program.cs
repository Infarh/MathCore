
using System.Formats.Asn1;

var (m, n) = (4, 2);
Matrix A = new((m, n), 2, 1, 1, 3, 4, 2, 3, 4);

var (q, r) = A.QR();
var s = new double[Math.Min(n, m)];
for (var i = 0; i < s.Length; i++)
    s[i] = r[i, i];

Console.WriteLine($"A = \r\n{A:f0}");
Console.WriteLine();

Console.WriteLine($"q = \r\n{q:+0.0000;-0.0000}");
Console.WriteLine();
Console.WriteLine($"s = {s.Select(v => v.ToString("f4")).JoinStrings(", ")}");
Console.WriteLine();
//Console.WriteLine($"V = \r\n{V:f4}");
Console.WriteLine();

//var A1 = U * Matrix.Diagonal(s) * V;

//Console.WriteLine($"A1 = \r\n{A1:f4}");
//Console.WriteLine();

Console.WriteLine("End.");
return;