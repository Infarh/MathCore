using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using MathCore.Trees;
using Dirs = System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>;
// ReSharper disable ConvertToUsingDeclaration

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            //var dir = new DirectoryInfo(".");

            //var dir_tree = dir.AsTreeNode(d => d.EnumerateDirectories(), d => d.Parent);

            //char b = 'B', c = '\x64', d = '\uffff';
            //Console.WriteLine("{0}, {1}, {2}", b, c, d);
            //Console.WriteLine("{0}, {1}, {2}", char.ToLower(b), char.ToUpper(c), char.GetNumericValue(d));

            //Console.ReadLine();


            const string request_uri = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
            static string Request(string address) => new HttpClient()
               .GetAsync(request_uri, HttpCompletionOption.ResponseHeadersRead)
               .Result
               .Content
               .ReadAsStringAsync()
               .Result;

            static IEnumerable<string> GetLines(string str)
            {
                var reader = new StringReader(str);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    yield return line;
                }
            }

            static IEnumerable<string> GetData(string address) => GetLines(Request(address));

            //var times = GetData(request_uri)
            //   .First()
            //   .Split(',')
            //   .Skip(4)
            //   .Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture))
            //   .ToArray();

            //var countries_data = GetData(request_uri)
            //   .Skip(1)
            //   .Select(line => line.Split(','))
            //   .Select(values => new
            //    {
            //        Country = values[1],
            //        Counts = values
            //           .Skip(4)
            //           .Select(S => double.Parse(S, CultureInfo.InvariantCulture))
            //           .ToArray()
            //    })
            //   .ToArray();


            new DirectoryInfo("c:\\")
               .AsTreeNode((d, error) => Console.WriteLine("Error:{0}", d.FullName))
               .EnumerateChildValuesWithRoot(n => n.Level <= 3)
               .Count()
               .ToConsoleLN("Count:{0}");

            "c:\\ ".AsTreeNode(Directory.EnumerateDirectories)
               .EnumerateChildValuesWithRoot(n => n.Level <= 3)
               .Count()
               .ToConsoleLN("Count:{0}");
             
            //static Func<T, T> Y<T>(Func<Func<T, T>, Func<T, T>> F) => t => F(Y(F))(t);

            //var dirs = Y<Dirs>(f => dirs => dirs.SelectMany(d => d.EnumerateDirectories()));

            //var dir123 = new DirectoryInfo("c:\\123");
            //var directories = dirs(dir123.EnumerateDirectories());
            //var files = directories.SelectMany(d => d.EnumerateFiles());
            //directories.Count().ToConsoleLN("dirs: {0}");
            //files.Count().ToConsoleLN("files: {0}");


            ////new DirectoryInfo("c:\\")
            ////   .AsTreeNode(d => d.EnumerateDirectories(), d => d.Parent)
            ////   .EnumerateChilds()
            ////   .TakeWhile(d => d.Level <= 5)
            ////   .Foreach(d => Console.WriteLine(d.Value.FullName));

            Console.ReadLine();
        }
    }

    public static class FileTools
    {
        public static IEnumerable<string> GetLines(this FileInfo file)
        {
            using(var reader = file.OpenText())
                while (!reader.EndOfStream)
                    yield return reader.ReadLine();
        }

        public static IEnumerable<string> GetLines(this StreamReader reader)
        {
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }
    }
}