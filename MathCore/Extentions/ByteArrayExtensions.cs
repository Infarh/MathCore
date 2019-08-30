
 // ReSharper disable once CheckNamespace
namespace System
{
    public static class ByteArrayExtensions
    {
        public static byte[] ComputeSHA256(this byte[] bytes)
        {
            using(var sha256 = new Security.Cryptography.SHA256Managed())
                return sha256.ComputeHash(bytes);
        }

        public static byte[] ComputeSHA256(this byte[] bytes, int offset, int count)
        {
            using(var sha256 = new Security.Cryptography.SHA256Managed())
                return sha256.ComputeHash(bytes, offset, count);
        }

        public static byte[] ComputeMD5(this byte[] bytes)
        {
            using(var md5 = new Security.Cryptography.MD5CryptoServiceProvider())
                return md5.ComputeHash(bytes);
        }

        public static byte[] ComputeMD5(this byte[] bytes, int offset, int count)
        {
            using(var md5 = new Security.Cryptography.MD5CryptoServiceProvider())
                return md5.ComputeHash(bytes, offset, count);
        }

        public static short[] ToInt16Array(this byte[] array)
        {
            var result = new short[array.Length / 2];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);
            return result;
        }

        public static int[] ToInt32Array(this byte[] array)
        {
            var result = new int[array.Length / 4];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);
            return result;
        }

        public static int ToInt(this byte[] array)
        {
            var result = 0;
            unchecked
            {
                for(var i = 0; i < array.Length; i++)
                    result += array[i] << (8 * i);
            }
            return result;
        }
    }
}