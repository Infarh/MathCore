using System.Collections.Generic;

namespace System.IO
{
    public static class StreamReaderExtensions
    {
        public static IEnumerable<string> GetStringLines(this StreamReader reader)
        {
            while(!reader.EndOfStream) yield return reader.ReadLine();
        }

        public static IEnumerable<char[]> GetCharBuffer(this StreamReader reader, int BufferLength)
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