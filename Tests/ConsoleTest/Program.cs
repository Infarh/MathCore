using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MathCore.Trees;
using Microsoft.Data.Analysis;
using Dirs = System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>;
// ReSharper disable ConvertToUsingDeclaration

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            var d1 = new Dictionary<int, string>();
            var d2 = new SimpleDictionary<int, string>();

            d1.Add(42, "Hello!");
            d1.Add(12, "World!");
            d1.Add(3, "123!");
            d1.Add(2, "321!");

            d2.Add(42, "Hello!");
            d2.Add(12, "World!");
            d2.Add(3, "123!");
            d2.Add(2, "321!");

            var time = DateTime.Parse("09/04/2002", CultureInfo.InvariantCulture);


            //SchedulersTests.Start();

            //Console.ReadLine();
            //return;

            var str = "Hello World 123 123 123 123 123 123 123 123 123 !!!";
            var compressed = str.Compress();
            var str2 = compressed.DecompressAsString();
            //                        3222 2222 2222 1111 1111 1             
            //                        1098 7654 3210 9876 5432 1098 7654 3210
            var res = (int)Math.Log(0b0000_0001_1000_0000_0000_0001_0000_0000, 2); //24

            var times = new PrimitiveDataFrameColumn<DateTime>("Date");
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(2)));
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
            times.Append(DateTime.Now);

            var ints = new PrimitiveDataFrameColumn<int>("ints", 3);
            var strs = new StringDataFrameColumn("strings", 3);

            var data = new DataFrame(times, ints, strs) {[0, 1] = 10};

            Console.ReadLine();
        }
    }
}