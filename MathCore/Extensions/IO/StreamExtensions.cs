using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class StreamExtensions
    {
        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this Stream stream)
        {
            using var sha256 = new Security.Cryptography.SHA256Managed();
            return sha256.ComputeHash(stream);
        }

        [NotNull]
        public static byte[] ComputeMD5([NotNull] this Stream stream)
        {
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(stream);
        }

        /// <summary>Создать буферизованный поток данных</summary>
        /// <param name="DataStream">Исходный поток данных</param>
        /// <param name="BufferSize">Размер буфера (по умолчанию 4096 байта)</param>
        /// <returns>Буферизованный поток данных</returns>
        [DST, NotNull]
        public static BufferedStream GetBufferedStream([NotNull] this Stream DataStream, int BufferSize = 4096) => new(DataStream, BufferSize);

        [DST, NotNull]
        public static StreamWrapper GetWrapper([NotNull] this Stream BaseStream) => new(BaseStream);

        [DST]
        public static T ReadStructure<T>([NotNull] this Stream stream)
        {
            var size = Marshal.SizeOf(typeof(T));
            var data = new byte[size];
            stream.Read(data, 0, size);
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var ptr = gch.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            } finally
            {
                gch.Free();
            }
        }

        public static void WriteStructure<T>([NotNull] this Stream stream, T value) where T : struct
        {
            var size = Marshal.SizeOf(value);
            var buffer = new byte[size]; // создать массив
            var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned); // зафиксировать в памяти
            try
            {
                var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); // и взять его адрес
                Marshal.StructureToPtr(value, p, true); // копировать в массив
            } finally
            {
                g_lock.Free(); // снять фиксацию
            }
            stream.Write(buffer, 0, size);
        }

        [NotNull]
        public static byte[] ToArray([NotNull] this Stream stream)
        {
            var array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            return array;
        }

        [NotNull, ItemNotNull]
        public static async Task<byte[]> ToArrayAsync([NotNull] this Stream stream, CancellationToken cancel = default(CancellationToken))
        {
            var array = new byte[stream.Length];
            await stream.ReadAsync(array, 0, array.Length, cancel).ConfigureAwait(false);
            return array;
        }
    }
}