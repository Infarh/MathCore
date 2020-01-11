using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Data
{
    public static class IDataReaderExtensions
    {
        [NotNull]
        public static IEnumerable<T> ReadToEnd<T>([NotNull] this IDataReader Reader, Func<IDataRecord, T> Read)
        {
            var items = new List<T>();

            while(Reader.Read())
                items.Add(Read(Reader));

            return items;
        }

        public static IEnumerable<T> Enumerable<T>([NotNull] this IDataReader Reader, Func<IDataRecord, T> Read)
        {
            while(Reader.Read()) yield return Read(Reader);
        }
    }
}