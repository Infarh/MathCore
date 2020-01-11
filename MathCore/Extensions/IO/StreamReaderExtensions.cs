using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class StreamReaderExtensions
    {
        [ItemCanBeNull]
        public static IEnumerable<string> GetStringLines([NotNull] this StreamReader reader)
        {
            while(!reader.EndOfStream) yield return reader.ReadLine();
        }

        [ItemNotNull]
        public static IEnumerable<char[]> GetCharBuffer([NotNull] this StreamReader reader, int BufferLength)
        {
            while(!reader.EndOfStream)
            {
                var buffer = new char[BufferLength];
                reader.Read(buffer, 0, BufferLength);
                yield return buffer;
            }
        }
    } 
}