using System.Numerics;
using System.Runtime.Intrinsics;

var hh = Vector256.IsHardwareAccelerated;

var b1 = new short[1024];
for (var i = 0; i < b1.Length; i++)
    b1[i] = (short)i;


var w_size = Vector<short>.Count;

var v1 = new Vector<short>(b1, 0);

var s = Vector.Dot(v1, Vector<short>.One);

Console.WriteLine("End.");
//Console.ReadLine();

