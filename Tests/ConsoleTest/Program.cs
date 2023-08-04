using MathCore.Extensions;

var person = new Person { Age = 18 };

person.SetPropertyValue("Age", 20);

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

class Person
{
    public int Age { get; set; }

    public override string ToString() => $"Age {Age}";
}