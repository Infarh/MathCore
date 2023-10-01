using MathCore.Algorithms.Collections;
using MathCore.Text;

var bas = StringValueComparer.Base;
//StringValueComparer.Base = 842003174;
Console.WriteLine(StringValueComparer.Base);
var dict = new CustomDictionary<string, int>(new StringValueComparer());

for (var i = 0; i < 1000; i++)
{
    dict.Add($"value-{i}", i);
}

for (var i = 0; i < 1000; i++)
{
    var value = dict[$"value-{i}"];
    if (value != i)
    {

    }
}

//dict.Add("222", 222);
//dict.Add("444", 444);
//dict.Add("777", 777);
//dict.Add("123", 123);
//dict.Add("321", 321);
//dict.Add("111", 111);

//var v222 = dict["222"];
//var v444 = dict["444"];
//var v777 = dict["777"];
//var v123 = dict["123"];
//var v321 = dict["321"];
//var v111 = dict["111"];

Console.WriteLine("End.");
//Console.ReadLine();

