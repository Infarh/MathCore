using System;
using System.Runtime.Serialization;
using Microsoft.Data.Analysis;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {

            //                        3222 2222 2222 1111 1111 1             
            //                        1098 7654 3210 9876 5432 1098 7654 3210
            var res = (int)Math.Log(0b0000_0001_1000_0000_0000_0001_0000_0000, 2); //24

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