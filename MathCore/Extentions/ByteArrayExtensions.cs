 using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class ByteArrayExtensions
    {
        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this byte[] bytes)
        {
            using var sha256 = new Security.Cryptography.SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this byte[] bytes, int offset, int count)
        {
            using var sha256 = new Security.Cryptography.SHA256Managed();
            return sha256.ComputeHash(bytes, offset, count);
        }

        [NotNull]
        public static byte[] ComputeMD5([NotNull] this byte[] bytes)
        {
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        [NotNull]
        public static byte[] ComputeMD5([NotNull] this byte[] bytes, int offset, int count)
        {
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes, offset, count);
        }

        [NotNull]
        public static short[] ToInt16Array([NotNull] this byte[] array)
        {
            var result = new short[array.Length / 2];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);
            return result;
        }

        [NotNull]
        public static int[] ToInt32Array([NotNull] this byte[] array)
        {
            var result = new int[array.Length / 4];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);
            return result;
        }

        public static int ToInt([NotNull] this byte[] array)
        {
            var result = 0;
            unchecked
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                for(var i = 0; i < array.Length; i++)
                    result += array[i] << (8 * i);
            }
            return result;
        }
    }
}