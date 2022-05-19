
using System.Diagnostics;

using MathCore;
using MathCore.IO;
using MathCore.Threading;

var x = new Complex(5, 7);
var y = x + (5, 7.5);

Console.WriteLine(y);

//var messages = Enumerable.Range(1, 1000).Select(i => $"Message-{i}");

//using (var thread_pool = new InstanceThreadPool(10))
//{
//    foreach (var message in messages)
//        thread_pool.Execute(message, o =>
//        {
//            var msg = (string?)o;
//            Console.WriteLine(">>{0} processing...", msg);
//            Thread.Sleep(5000);
//            Console.WriteLine(">>{0} processing complete", msg);
//        });


//    Console.ReadLine();
//}

Console.ReadLine();
