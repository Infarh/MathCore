using System.Collections.Generic;

namespace System.Data
{
    public static class IDataReaderExtensions
    {
        public static IEnumerable<T> ReadToEnd<T>(this IDataReader Reader, Func<IDataRecord, T> Read)
        {
            var items = new List<T>();

            while(Reader.Read())
                items.Add(Read(Reader));

            return items;
        }

        public static IEnumerable<T> Enumerable<T>(this IDataReader Reader, Func<IDataRecord, T> Read)
        {
            while(Reader.Read()) yield return Read(Reader);
        }
    }
}