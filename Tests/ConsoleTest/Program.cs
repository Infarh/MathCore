
using System.Diagnostics;

using MathCore.Threading;

const string path = @"c:\123\qwe.png";

var file = new FileInfo(path);

var execute_process = file.Execute();

var process = await execute_process!
   .WaitAsync();


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
