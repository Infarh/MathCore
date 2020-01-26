using System;
using System.Runtime.Serialization;
using Microsoft.Data.Analysis;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var times = new PrimitiveDataFrameColumn<DateTime>("Date");
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(2)));
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
            times.Append(DateTime.Now);

            var ints = new PrimitiveDataFrameColumn<int>("ints", 3);
            var strs = new StringDataFrameColumn("strings", 3);

            var data = new DataFrame(times, ints, strs);

            data[0, 1] = 10;

            Console.ReadLine();
        }
    }
}