using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MathCore.CSV;
using Microsoft.Data.Analysis;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            const string base_url = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/";
            const string confirmed_file = "time_series_covid19_confirmed_global.csv";

            static async Task<TextReader> GetReaderAsync(string address)
            {
                var client = new HttpClient { BaseAddress = new Uri(base_url) };
                var response = await client.GetAsync(address);
                return new StreamReader(await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync());
            }

            var confirmed = new CSVQuery(() => GetReaderAsync(confirmed_file).Result)
                .WithHeader()
                .AddColumn("Province", 0)
                .AddColumn("Name", 1);


            var countries = confirmed.Select(country => (Country: country["Name"], Count: country.LastValue<int>()));

            //foreach (var country in countries.OrderByDescending(c => c.Count))
            //    Console.WriteLine(country);

            var countries_v = confirmed.Select(country => (
                    name: country["Name"].Trim('"', '*'),
                    province: country["Province"],
                    values: country
                       .Skip(6)
                       .Select(value => (
                            date: DateTime.Parse(value.Header, CultureInfo.InvariantCulture),
                            count: double.Parse(value.Value, CultureInfo.InvariantCulture)
                            )
                        )
                       .ToArray()))
               .GroupBy(c => c.name, c => (c.province, c.values))
               .Select(c => (
                    name: c.Key,
                    count: c.Sum(p => p.values.Sum(x => x.count)),
                    delta: c.Sum(p => p.values[p.values.Length - 1].count - p.values[p.values.Length - 2].count),
                    provinces: c.ToArray()
                    )
                )
               .OrderByDescending(c => c.delta).ThenBy(c => c.name)
               .ToArray();

            var index = 1;
            foreach (var (name, count, delta, _) in countries_v)
                Console.WriteLine("{3, 3}{0, 35}:{1}({2})", name, count, delta, index++);


            Console.ReadLine();

            ////                        3222 2222 2222 1111 1111 1             
            ////                        1098 7654 3210 9876 5432 1098 7654 3210
            //var res = (int)Math.Log(0b0000_0001_1000_0000_0000_0001_0000_0000, 2); //24

            //var times = new PrimitiveDataFrameColumn<DateTime>("Date");
            //times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(2)));
            //times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
            //times.Append(DateTime.Now);

            //var ints = new PrimitiveDataFrameColumn<int>("ints", 3);
            //var strs = new StringDataFrameColumn("strings", 3);

            //var data = new DataFrame(times, ints, strs) {[0, 1] = 10};


            //Console.ReadLine();
        }
    }
}