﻿#nullable enable


// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class StreamReaderExtensions
{
    public static IEnumerable<string> GetStringLines(this StreamReader reader)
    {
        while (!reader.EndOfStream && reader.ReadLine() is { } line) 
            yield return line;
    }

    public static IEnumerable<char[]> GetCharBuffer(this StreamReader reader, int BufferLength)
    {
        while (!reader.EndOfStream)
        {
            var buffer = new char[BufferLength];
            var readed = reader.Read(buffer, 0, BufferLength);
            if (readed != BufferLength)
                Array.Resize(ref buffer, readed);
            yield return buffer;
        }
    }

    public readonly struct ReadedCharBuffer(char[] buffer, int readed)
    {
        public char[] Buffer => buffer;
        public int Readed => readed;
        public bool IsFull => readed == Buffer.Length;

        public char[] GetReaded()
        {
            var buffer = Buffer;
            if (Readed < buffer.Length)
                Array.Resize(ref buffer, Readed);
            return buffer;
        }

        public static implicit operator char[](ReadedCharBuffer buffer) => buffer.GetReaded();

        public static implicit operator int(ReadedCharBuffer buffer) => buffer.Readed;
        public static implicit operator bool(ReadedCharBuffer buffer) => buffer.IsFull;
    }

    public static IEnumerable<ReadedCharBuffer> GetCharBuffer(this StreamReader reader, char[] Buffer)
    {
        if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));

        var buffer_length = Buffer.Length;
        if (buffer_length == 0) throw new ArgumentException("Размер буфера чтения должен быть больше 0");

        while (!reader.EndOfStream)
        {
            var readed = reader.Read(Buffer, 0, buffer_length);
            yield return new(Buffer, readed);
        }
    }
}