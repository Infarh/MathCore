 using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов-расширений для массивов байт</summary>
    public static class ByteArrayExtensions
    {
        /// <summary>Вычислить контрольную сумму массива с применением алгоритма SHA256</summary>
        /// <param name="bytes">Массив байт для которого производится вычисление суммы SHA256</param>
        /// <returns>Массив байт рассчитанной суммы SHA256</returns>
        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this byte[] bytes)
        {
            using var sha256 = new Security.Cryptography.SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>Вычислить контрольную сумму массива с применением алгоритма SHA256</summary>
        /// <param name="bytes">Массив байт для которого производится вычисление суммы SHA256</param>
        /// <param name="offset">Индекс элемента массива с которого требуется вычислить контрольную суммы</param>
        /// <param name="count">Число элементов массива, участвующих в расчёте суммы</param>
        /// <returns>Массив байт рассчитанной суммы SHA256</returns>
        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this byte[] bytes, int offset, int count)
        {
            using var sha256 = new Security.Cryptography.SHA256Managed();
            return sha256.ComputeHash(bytes, offset, count);
        }

        /// <summary>Вычислить контрольную сумму массива с применением алгоритма MD5</summary>
        /// <param name="bytes">Массив байт для которого производится вычисление суммы MD5</param>
        /// <returns>Массив байт рассчитанной суммы MD5</returns>
        [NotNull]
        public static byte[] ComputeMD5([NotNull] this byte[] bytes)
        {
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        /// <summary>Вычислить контрольную сумму массива с применением алгоритма MD5</summary>
        /// <param name="bytes">Массив байт для которого производится вычисление суммы MD5</param>
        /// <param name="offset">Индекс элемента массива с которого требуется вычислить контрольную суммы</param>
        /// <param name="count">Число элементов массива, участвующих в расчёте суммы</param>
        /// <returns>Массив байт рассчитанной суммы MD5</returns>
        [NotNull]
        public static byte[] ComputeMD5([NotNull] this byte[] bytes, int offset, int count)
        {
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes, offset, count);
        }

        /// <summary>Преобразовать массив байт в массив целых чисел длиной два байта каждое</summary>
        /// <param name="array">Массив байт чётной</param>
        /// <param name="Destination">Массив двухбайтовых целых чисел, на который осуществляется проекция в памяти массива байт</param>
        public static void ToInt16Array([NotNull] this byte[] array, [NotNull] short[] Destination) => 
            Buffer.BlockCopy(array, 0, Destination, 0, Math.Min(array.Length, Destination.Length * 2));

        /// <summary>Преобразовать массив байт в массив целых чисел длиной два байта каждое</summary>
        /// <param name="array">Массив байт чётной</param>
        /// <returns>Новый массив двухбайтовых целых чисел, на который осуществляется проекция в памяти массива байт</returns>
        [NotNull]
        public static short[] ToInt16Array([NotNull] this byte[] array)
        {
            var result = new short[array.Length / 2];
            array.ToInt16Array(result);
            return result;
        }

        /// <summary>Преобразовать массив байт в массив целых чисел длиной четыре байта каждое</summary>
        /// <param name="array">Массив байт чётной</param>
        /// <param name="Destination">Массив четырёхбайтных целых чисел, на который осуществляется проекция в памяти массива байт</param>
        public static void ToInt32Array([NotNull] this byte[] array, [NotNull] int[] Destination) => 
            Buffer.BlockCopy(array, 0, Destination, 0, Math.Min(array.Length, Destination.Length * 4));

        /// <summary>Преобразовать массив байт в массив целых чисел длиной четыре байта каждое</summary>
        /// <param name="array">Массив байт чётной</param>
        /// <returns>Новый массив четырёхбайтных целых чисел, на который осуществляется проекция в памяти массива байт</returns>
        [NotNull]
        public static int[] ToInt32Array([NotNull] this byte[] array)
        {
            var result = new int[array.Length / 4];
            Buffer.BlockCopy(array, 0, result, 0, array.Length);
            return result;
        }

        /// <summary>Рассчитать четырёхбайтную сумму массива байт с переполнением</summary>
        /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
        /// <returns>Целое четырёхбайтное со знаком, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
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

        /// <summary>Рассчитать четырёхбайтную (без знака) сумму массива байт с переполнением</summary>
        /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
        /// <returns>Целое четырёхбайтное без знака, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
        public static uint ToUInt([NotNull] this byte[] array)
        {
            var result = 0U;
            unchecked
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < array.Length; i++)
                    result += (uint)(array[i] << (8 * i));
            }
            return result;
        }

        /// <summary>Рассчитать восьмибайтную сумму массива байт с переполнением</summary>
        /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
        /// <returns>Целое восьмибайтное со знаком, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
        public static long ToLong([NotNull] this byte[] array)
        {
            var result = 0L;
            unchecked
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < array.Length; i++)
                    result += array[i] << (8 * i);
            }
            return result;
        }

        /// <summary>Рассчитать восьмибайтную (без знака) сумму массива байт с переполнением</summary>
        /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
        /// <returns>Целое восьмибайтное без знака, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
        public static ulong ToULong([NotNull] this byte[] array)
        {
            var result = 0UL;
            unchecked
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < array.Length; i++)
                    result += (ulong) (array[i] << (8 * i));
            }
            return result;
        }
    }
}