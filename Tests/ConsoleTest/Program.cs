using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

var rnd = new Random(2);

int[] xx = [50, 49, 51, 48, 52, 47, 53, 46, 54, 45,  55, 44, 56, 43, 57, 42, 58, 41, 59, 40, 60];

var max = new RollingMax<int>(5, Inverted: false);

var i = 0;
foreach(var x in xx)
{
    var value = max.Add(x);

    Debug.WriteLineIf(x == value, $"MaxCount:{max}");

    i++;
}

Console.WriteLine("End.");

return;