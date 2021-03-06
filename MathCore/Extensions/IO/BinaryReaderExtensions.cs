﻿using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class BinaryReaderExtensions
    {
        /// <summary>Признак конца потока</summary>
        /// <param name="reader">Объект чтения потока</param>
        /// <returns>Истина, если поток закончен</returns>
        [DST]
        public static bool IsEOF([NotNull] this BinaryReader reader) => reader.BaseStream.Position == reader.BaseStream.Length;

        /// <summary>Получить перечисление, содержащее массивы байт заданной длины из потока</summary>
        /// <param name="reader">Объект чтения потока данных</param>
        /// <param name="BufferSize">Размер буфера</param>
        /// <returns>Перечислитель</returns>
        [ItemNotNull]
        public static IEnumerable<byte[]> GetByteBuffer([NotNull] this BinaryReader reader, int BufferSize)
        {
            while (!reader.IsEOF())
            {
                var buffer = new byte[BufferSize];
                reader.Read(buffer, 0, BufferSize);
                yield return buffer;
            }
        }

        /// <summary>Получить перечисление, содержащее массивы байт заданной длины из потока</summary>
        /// <param name="reader">Объект чтения потока данных</param>
        /// <param name="Buffer">Буфер чтения</param>
        /// <returns>Перечислитель</returns>
        [ItemNotNull]
        public static IEnumerable<byte[]> GetByteBuffer([NotNull] this BinaryReader reader, byte[] Buffer)
        {
            if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));
            if (Buffer.Length == 0) throw new ArgumentException("Размер буфера должен быть больше 0", nameof(Buffer));

            var buffer_size = Buffer.Length;
            while (!reader.IsEOF())
            {
                reader.Read(Buffer, 0, buffer_size);
                yield return Buffer;
            }
        }

        [ItemNotNull]
        public static IEnumerable<char[]> GetCharBuffer([NotNull] this BinaryReader reader, int BufferSize)
        {
            while (!reader.IsEOF())
            {
                var buffer = new char[BufferSize];
                reader.Read(buffer, 0, BufferSize);
                yield return buffer;
            }
        }

        public static T ReadStructure<T>([NotNull] this BinaryReader reader) where T : struct
        {
            var data = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var ptr = gch.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                gch.Free();
            }
        }

        public static void WriteStructure<T>([NotNull] this BinaryWriter writer, T value) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(value)]; // создать массив
            var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned); // зафиксировать в памяти
            try
            {
                var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); // и взять его адрес
                Marshal.StructureToPtr(value, p, true); // копировать в массив
                writer.Write(buffer);
            }
            finally
            {
                g_lock.Free(); // снять фиксацию
            }
        }

        public static async Task<double> ReadDoubleAsync(this BinaryReader Reader, CancellationToken Cancel = default)
        {
            const int size = 8;
            var buffer = new byte[8];
            await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false);
            return BitConverter.ToDouble(buffer, 0);
        }

        public static async Task<float> ReadSingleAsync(this BinaryReader Reader, CancellationToken Cancel = default)
        {
            const int size = 4;
            var buffer = new byte[size];
            await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static async Task<long> ReadInt64Async(this BinaryReader Reader, CancellationToken Cancel = default)
        {
            const int size = 8;
            var buffer = new byte[8];
            await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static async Task<int> ReadInt32Async(this BinaryReader Reader, CancellationToken Cancel = default)
        {
            const int size = 4;
            var buffer = new byte[size];
            await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static async Task<bool> ReadBooleanAsync(this BinaryReader Reader, CancellationToken Cancel = default)
        {
            const int size = 1;
            var buffer = new byte[size];
            await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false);
            return BitConverter.ToBoolean(buffer, 0);
        }
    }
}