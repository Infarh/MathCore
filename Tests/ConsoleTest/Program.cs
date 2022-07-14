
using ConsoleTest;

using MathCore;


var x_str = " 3.14 ".AsStringPtr().Trim();

var x = x_str.ParseDouble();

var str = "Val1=123;Val2=321;Val3=3.14;Val4=true;Val5=Hello World!".AsStringPtr();

var value = new Test();
foreach (var val in str.Split(';').SkipEmpty())
    switch (val.GetName())
    {
        case "Val1": value.Value1 = (int)val.GetValueString(); break;
        case "Val2": value.Value2 = val.GetValueInt32(); break;
        case "Val3": value.Value3 = val.GetValueDouble(); break;
        case "Val4": value.Value4 = val.GetValueBool(); break;
        case "Val5": value.Value5 = val.GetValueString(); break;
    }


Console.ReadLine();


class Test
{
    public int Value1 { get; set; }
    public int Value2 { get; set; }
    public double Value3 { get; set; }
    public bool Value4 { get; set; }
    public string Value5 { get; set; }
}