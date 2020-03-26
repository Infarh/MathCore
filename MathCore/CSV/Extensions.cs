using System.Collections.Generic;
using System.IO;
using System.Text;
using MathCore.Annotations;

namespace MathCore.CSV
{
    public static class Extensions
    {
        public static CSVQuery OpenCSV([NotNull] this FileInfo file) => new CSVQuery(file);

        public static CSVWriter<T> AsCSV<T>([NotNull] this IEnumerable<T> items) => new CSVWriter<T>(items);
    }
}
