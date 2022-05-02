
using System.Diagnostics;

using MathCore.IO;
using MathCore.Threading;

var test_file = new FileInfo("test-file.txt");
await using var writer = test_file.CreateText();

var process11 = test_file.GetLockingProcesses();

const string path = @"c:\123\qwe.png";

var current_process = Process.GetCurrentProcess();
var parent = current_process.GetParentProcess()!.GetParentProcess()!;
var childs = parent.GetChildProcesses().ToArray();

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
