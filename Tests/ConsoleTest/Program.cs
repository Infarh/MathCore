using System;
using System.IO;
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
            const string confirmed = "time_series_covid19_confirmed_global.csv";

            static async Task<TextReader> GetReader(string address)
            {
                var client = new HttpClient { BaseAddress = new Uri(base_url) };
                var response = await client.GetAsync(address);
                return new StreamReader(await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync());
            }

            var confirmed_query = new CSVQuery(() => GetReader(confirmed).Result)
                .ValuesSeparator(',')
                .WithHeader();

            var header = confirmed_query.GetHeader();



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