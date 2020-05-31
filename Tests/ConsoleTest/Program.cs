using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using MathCore.Trees;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            var dir = new DirectoryInfo(".");

            var dir_tree = dir.ToTreeItem(d => d.EnumerateDirectories(), d => d.Parent);

            char b = 'B', c = '\x64', d = '\uffff';
            Console.WriteLine("{0}, {1}, {2}", b, c, d);
            Console.WriteLine("{0}, {1}, {2}", char.ToLower(b), char.ToUpper(c), char.GetNumericValue(d));

            Console.ReadLine();


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

            var times = GetData(request_uri)
               .First()
               .Split(',')
               .Skip(4)
               .Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture))
               .ToArray();

            var countries_data = GetData(request_uri)
               .Skip(1)
               .Select(line => line.Split(','))
               .Select(values => new
                {
                    Country = values[1],
                    Counts = values
                       .Skip(4)
                       .Select(S => double.Parse(S, CultureInfo.InvariantCulture))
                       .ToArray()
                })
               .ToArray();
        }
    }
}