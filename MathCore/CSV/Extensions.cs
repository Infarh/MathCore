using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MathCore.CSV
{
    public static class Extensions
    {
        public static CSVQuery OpenCSV(this FileInfo file) => new CSVQuery(file);

        public static CSVWriter<T> AsCSV<T>(this IEnumerable<T> items) => new CSVWriter<T>(items);
    }
}
