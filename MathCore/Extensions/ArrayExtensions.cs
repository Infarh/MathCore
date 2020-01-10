using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable ForCanBeConvertedToForeach

// ReSharper disable once CheckNamespace
namespace System
{
    ///<summary>Методы расширения для массивов</summary>
    public static class ArrayExtensions
    {
        public static void Deconstruct<T>([NotNull] this T[,] array, out int N, out int M)
        {
            N = array.GetLength(0);
            M = array.GetLength(1);
        }
        public static void Deconstruct<T>([NotNull] this T[,,] array, out int N, out int M, out int K)
        {
            N = array.GetLength(0);
            M = array.GetLength(1);
            K = array.GetLength(2);
        }

        [NotNull]
        public static IEnumerable<T> AsRandomEnumerable<T>([NotNull] this T[] Items, [CanBeNull] Random Rnd = null)
        {
            if (Rnd is null) Rnd = new Random();
            var index = CreateSequence(Items.Length).MixRef(Rnd);
            for (var i = 0; i < Items.Length; i++)
                yield return Items[index[i]];
        }

        [NotNull]
        public static IEnumerable<T> TakeLast<T>([NotNull] this T[] Items, int Count)
        {
            if (Count <= 0) yield break;

            for (var i = Math.Max(0, Items.Length - Count); i < Items.Length; i++)
                yield return Items[i];
        }

        [NotNull]
        public static IEnumerable<T> TakeLastElements<T>([NotNull] this IEnumerable<T> Items, int Count)
        {
            var buffer = new T[Count];
            var index = 0;
            foreach (var item in Items)
            {
                buffer[index % Count] = item;
                index++;
            }

            var count = Math.Min(Count, index);
            for (var i = 0; i < count; i++)
                yield return buffer[(index - count + i) % Count];
        }

        [NotNull]
        public static IEnumerable<T> EnumerateElements<T>([NotNull] this T[,] array)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    yield return array[i, j];
        }

        public static bool Exist<T>([NotNull] this T[] array, T value) => Array.IndexOf(array, value, 0, array.Length) >= 0;

        /// <summary>Разделить входной массив на подмассивы указанным методом</summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="array">Разделяемый массив</param>
        /// <param name="Splitter">Метод, возвращающий истину, когда надо начать новый подмассив</param>
        /// <returns>
        /// Массив подмассивов элементов исходного массива, разделённый выбранными указанным методом элементами.
        /// Выбранные элементы в результат не входят.
        /// </returns>
        [DST, NotNull]
        public static T[][] Split<T>([NotNull] this T[] array, [NotNull] Func<T, bool> Splitter)
        {
            var result = new List<T[]>(array.Length);
            var aggregator = new List<T>(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                var value = array[i];
                if (Splitter(value) && aggregator.Count != 0)
                {
                    result.Add(aggregator.ToArray());
                    aggregator.Clear();
                }
                else
                    aggregator.Add(value);
            }
            if (aggregator.Count != 0)
                result.Add(aggregator.ToArray());

            return result.ToArray();
        }

        ///// <summary>
        ///// Быстрая сортировка Хоара
        ///// </summary>
        ///// <param name="A"></param>
        ///// <param name="low"></param>
        ///// <param name="high"></param>
        //public static void qSort([NotNull] this int[] A, int low, int high)
        //{
        //    var i = low;
        //    var j = high;
        //    var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
        //    do
        //    {
        //        while(A[i] < x) ++i;  // поиск элемента для переноса в старшую часть
        //        while(A[j] > x) --j;  // поиск элемента для переноса в младшую часть
        //        if(i > j) continue;
        //        // обмен элементов местами:
        //        var temp = A[i];
        //        A[i] = A[j];
        //        A[j] = temp;
        //        // переход к следующим элементам:
        //        i++;
        //        j--;
        //    } while(i < j);
        //    if(low < j) qSort(A, low, j);
        //    if(i < high) qSort(A, i, high);
        //}

        /// <summary>Быстрая сортировка Хоара</summary>
        /// <typeparam name="T">Тип сортируемых элементов</typeparam>
        /// <param name="A">Сортируемый массив элементов</param>
        /// <param name="low">Нижняя граница индекса сортировки</param>
        /// <param name="high">Верхняя граница индекса сортировки</param>
        public static void QuickSort<T>([NotNull] this T[] A, int low, int high) where T : IComparable
        {
            var i = low;
            var j = high;
            var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
            do
            {
                while (A[i].CompareTo(x) < 0) ++i;  // поиск элемента для переноса в старшую часть
                while (A[j].CompareTo(x) > 0) --j;  // поиск элемента для переноса в младшую часть
                if (i > j) continue;
                // обмен элементов местами:
                var temp = A[i];
                A[i] = A[j];
                A[j] = temp;
                // переход к следующим элементам:
                i++;
                j--;
            } while (i < j);
            if (low < j) QuickSort(A, low, j);
            if (i < high) QuickSort(A, i, high);
        }

        /// <summary>Быстрая сортировка Хоара</summary>
        /// <typeparam name="T">Тип сортируемых элементов</typeparam>
        /// <param name="A">Сортируемый массив элементов</param>
        /// <param name="low">Нижняя граница индекса сортировки</param>
        /// <param name="high">Верхняя граница индекса сортировки</param>
        public static void QuickSortT<T>([NotNull] this T[] A, int low, int high) where T : IComparable<T>
        {
            var i = low;
            var j = high;
            var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
            do
            {
                while (A[i].CompareTo(x) < 0) ++i;  // поиск элемента для переноса в старшую часть
                while (A[j].CompareTo(x) > 0) --j;  // поиск элемента для переноса в младшую часть
                if (i > j) continue;
                // обмен элементов местами:
                var temp = A[i];
                A[i] = A[j];
                A[j] = temp;
                // переход к следующим элементам:
                i++;
                j--;
            } while (i < j);
            if (low < j) QuickSortT(A, low, j);
            if (i < high) QuickSortT(A, i, high);
        }

        /// <summary>Рассчёт хеш-суммы всех элементов массива</summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="Objects">Массив элементов</param>
        /// <returns>Хеш-сумма элементов массива</returns>
        public static int GetComplexHashCode<T>([NotNull] this T[] Objects)
        {
            if (Objects.Length == 0) return 0;

            var hash = Objects[0].GetHashCode();
            for (var i = 1; i < Objects.Length; i++)
                unchecked
                {
                    hash = (hash * 397) ^ Objects.GetHashCode();
                }

            return hash;
        }

        ///<summary>Объединение с массивом элементов</summary>
        ///<param name="A">Исходный массив</param>
        ///<param name="B">Присоединяемый массив</param>
        ///<typeparam name="TArray">Тип элементов массива</typeparam>
        ///<returns>Массив из объединенных элементов</returns>
        [DST, NotNull]
        public static TArray[] Concatenate<TArray>([NotNull] this TArray[] A, [NotNull] params TArray[] B)
        {
            var Result = new TArray[A.Length + B.Length];
            A.CopyTo(Result, 0);
            B.CopyTo(Result, A.Length);
            return Result;
        }

        [NotNull]
        public static TArray[] Concatenate<TArray>([NotNull] this TArray[] A, [NotNull] params TArray[][] B)
        {
            var result = new TArray[A.Length + B.Sum(l => l.Length)];
            A.CopyTo(result, 0);
            var pos = A.Length;
            for (var i = 0; i < B.Length; i++)
            {
                var b = B[i];
                b.CopyTo(result, pos += b.Length);
            }

            return result;
        }

        ///<summary>Получить элемент массива</summary>
        ///<param name="A">Массив элементов</param>
        ///<param name="Selector">Метод выбора элемента массива</param>
        ///<typeparam name="TArray">Тип элементов массива</typeparam>
        ///<typeparam name="TOut">Тип выходного элемента</typeparam>
        ///<returns>Выбранный элемент массива</returns>
        [DST, CanBeNull]
        public static TOut GetSelectedValue<TArray, TOut>([NotNull] this TArray[] A, [NotNull] Func<TArray, TOut, TOut> Selector)
        {
            var result = default(TOut);
            var len = A.Length;
            for (var i = 0; i < len; i++)
                result = Selector(A[i], result);
            return result;
        }

        ///<summary>Преобразовать тип элементов массива</summary>
        ///<param name="In">Исходный массив элементов</param>
        ///<param name="converter">Метод преобразования элемента массива</param>
        ///<typeparam name="TIn">Исходный тип элементов массива</typeparam>
        ///<typeparam name="TOut">Требуемый тип элементов массива</typeparam>
        ///<returns>Массив преобразованных элементов</returns>
        [DST, NotNull]
        public static TOut[] ConvertTo<TIn, TOut>([NotNull] this TIn[] In, [NotNull] Converter<TIn, TOut> converter)
        {
            var result = new TOut[In.Length];
            for (var i = 0; i < In.Length; i++)
                result[i] = converter(In[i]);
            return result;
        }

        ///<summary>Выполнение действия для всех элементов массива</summary>
        ///<param name="array">Массив элементов</param>
        ///<param name="action">Выполняемой действие</param>
        ///<typeparam name="TArray">Тип элементов массива</typeparam>
        [DST]
        public static void Foreach<TArray>([NotNull] this TArray[] array, [NotNull] Action<TArray> action) => Array.ForEach(array, action);

        ///<summary>Выполнение действия для всех элементов массива с обработкой исключений</summary>
        ///<param name="array">Массив элементов</param>
        ///<param name="action">Выполняемое действие</param>
        ///<param name="ErrorHandler">Обработчик исключения</param>
        ///<typeparam name="TArray">Тип элементов массива</typeparam>
        [DST]
        public static void Foreach<TArray>([NotNull] this TArray[] array, [NotNull] Action<TArray> action, [NotNull] Func<Exception, bool> ErrorHandler) => array.Foreach<TArray, Exception>(action, ErrorHandler);

        /// <summary>Выполнение действия для всех элементов массива</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="action">Выполняемое действие</param>
        /// <param name="ErrorHandler">Обработчик исключений</param>
        /// <exception cref="ApplicationException">Возникает в случае если в методе action возникшее исключение не было обработано</exception>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TException">Тип исключений</typeparam>
        [DST]
        public static void Foreach<TArray, TException>
        (
            [NotNull] this TArray[] array,
            [NotNull] Action<TArray> action,
            [NotNull] Func<TException, bool> ErrorHandler
        ) where TException : Exception
        {
            var length = array.Length;
            for (var i = 0; i < length; i++)
                try
                {
                    action(array[i]);
                }
                catch (TException e)
                {
                    if (!ErrorHandler(e))
                        throw new InvalidOperationException("Ошибка выполнения", e);
                }
        }

        ///<summary>Определение значения функции для всех элементов массива</summary>
        ///<param name="array">Массив элементов</param>
        ///<param name="f">Вычисляемая функция</param>
        ///<typeparam name="TIn">Тип элементов массива области определения</typeparam>
        ///<typeparam name="TOut">Тип элементов массива области значения</typeparam>
        ///<returns>Массив значений функции</returns>
        [DST, NotNull]
        public static TOut[] Function<TIn, TOut>([NotNull] this TIn[] array, [NotNull] Func<TIn, TOut> f) => array.Select(f).ToArray();

        /// <summary>Получить массив, индексы элементов которого имеют обратный порядок</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Переворачиваемый массив</param>
        /// <returns>Перевёрнутый массив</returns>
        [DST, NotNull]
        public static TArray[] GetReversed<TArray>([NotNull] this TArray[] array)
        {
            var len = array.Length;
            var result = new TArray[len];
            for (var i = 0; i < len; i++)
                result[len - i - 1] = array[i];
            return result;
        }

        [DST, NotNull]
        public static TArray[] GetSubArray<TArray>([NotNull] this TArray[] array, int Length, int Start = 0)
        {
            var result = new TArray[Length];

            var j_Length = Start + Length;
            for (int i = 0, j = Start; i < Length && j < j_Length; i++, j++)
                result[i] = array[j];

            return result;
        }

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Инициализированный массив</param>
        /// <param name="Initializer">Метод инициализации</param>
        /// <returns>Инициализированный массив</returns>
        [DST, NotNull]
        public static TArray[] Initialize<TArray>
        (
            [NotNull] this TArray[] array,
            [NotNull] Func<int, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(i);
            return array;
        }

        /// <summary>Инициализация массива одним значением</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Инициализируемый массив</param>
        /// <param name="value">Значение, размещаемое во всех элементах массива</param>
        /// <returns>Инициализированный массив</returns>
        [DST, NotNull]
        public static TArray[] Initialize<TArray>
        (
            [NotNull] this TArray[] array,
            TArray value
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = value;
            return array;
        }

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TValue">Тип элементов массива</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="array">Инициализированный массив</param>
        /// <param name="p">Параметр инициализации</param>
        /// <param name="Initializer">Метод инициализации</param>
        /// <returns>Инициализированный массив</returns>
        [DST, NotNull]
        public static TValue[] Initialize<TValue, TP>
        (
            [NotNull] this TValue[] array,
            [CanBeNull] TP p,
            [NotNull] Func<int, TP, TValue> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(i, p);
            return array;
        }



        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TValue">Тип элементов массива</typeparam>
        /// <typeparam name="TP1">Тип первого параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип второго параметра инициализации</typeparam>
        /// <param name="array">Инициализированный массив</param>
        /// <param name="p1">Первый параметр инициализации</param>
        /// <param name="p2">Второй параметр инициализации</param>
        /// <param name="Initializer">Метод инициализации</param>
        /// <returns>Инициализированный массив</returns>
        [DST, NotNull]
        public static TValue[] Initialize<TValue, TP1, TP2>
        (
            [NotNull] this TValue[] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [NotNull] Func<int, TP1, TP2, TValue> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(i, p1, p2);
            return array;
        }

        [DST, NotNull]
        public static TArray[] Initialize<TArray>
        (
            [NotNull] this TArray[] array,
            [NotNull] Func<TArray, int, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(array[i], i);
            return array;
        }

        [DST, NotNull]
        public static TArray[] Initialize<TArray, TP>
        (
            [NotNull] this TArray[] array,
            [CanBeNull] TP p,
            [NotNull] Func<TArray, int, TP, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(array[i], i, p);
            return array;
        }

        [DST, NotNull]
        public static TArray[] Initialize<TArray, TP1, TP2>
        (
            [NotNull] this TArray[] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [NotNull] Func<TArray, int, TP1, TP2, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = Initializer(array[i], i, p1, p2);
            return array;
        }

        [DST, NotNull]
        public static TArray[,] Initialize<TArray>
        (
            [NotNull] this TArray[,] array,
            [NotNull] Func<int, int, TArray> Initializer
        )
        {
            var length_i = array.GetLength(0);
            var length_j = array.GetLength(1);
            for (var i = 0; i < length_i; i++)
                for (var j = 0; j < length_j; j++)
                    array[i, j] = Initializer(i, j);
            return array;
        }

        [DST, NotNull]
        public static TArray[,] Initialize<TArray, TP>
        (
            [NotNull] this TArray[,] array,
            [CanBeNull] TP p,
            [NotNull] Func<int, int, TP, TArray> Initializer
        )
        {
            var length_i = array.GetLength(0);
            var length_j = array.GetLength(1);
            for (var i = 0; i < length_i; i++)
                for (var j = 0; j < length_j; j++)
                    array[i, j] = Initializer(i, j, p);
            return array;
        }

        [DST, NotNull]
        public static TArray[,] Initialize<TArray, TP1, TP2>
        (
            [NotNull] this TArray[,] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [NotNull] Func<int, int, TP1, TP2, TArray> Initializer
        )
        {
            var length_i = array.GetLength(0);
            var length_j = array.GetLength(1);
            for (var i = 0; i < length_i; i++)
                for (var j = 0; j < length_j; j++)
                    array[i, j] = Initializer(i, j, p1, p2);
            return array;
        }

        [DST, NotNull]
        public static TArray[,] Initialize<TArray, TP1, TP2, TP3>
        (
            [NotNull] this TArray[,] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [CanBeNull] TP3 p3,
            [NotNull] Func<int, int, TP1, TP2, TP3, TArray> Initializer
        )
        {
            var length_i = array.GetLength(0);
            var length_j = array.GetLength(1);
            for (var i = 0; i < length_i; i++)
                for (var j = 0; j < length_j; j++)
                    array[i, j] = Initializer(i, j, p1, p2, p3);
            return array;
        }

        [DST, NotNull]
        public static TArray[][] Initialize<TArray>
        (
            [NotNull] this TArray[][] array,
            [NotNull] Func<int, TArray[]> ArrayInitializer,
            [NotNull] Func<int, int, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = ArrayInitializer(i);
                for (var j = 0; j < array[i].Length; j++)
                    array[i][j] = Initializer(i, j);
            }
            return array;
        }

        [DST, NotNull]
        public static TArray[][] Initialize<TArray, TP>
        (
            [NotNull] this TArray[][] array,
            [CanBeNull] TP p,
            [NotNull] Func<int, TP, TArray[]> ArrayInitializer,
            [NotNull] Func<int, int, TP, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = ArrayInitializer(i, p);
                for (var j = 0; j < array[i].Length; j++)
                    array[i][j] = Initializer(i, j, p);
            }
            return array;
        }

        [DST, NotNull]
        public static TArray[][] Initialize<TArray, TP1, TP2>
        (
            [NotNull] this TArray[][] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [NotNull] Func<int, TP1, TP2, TArray[]> ArrayInitializer,
            [NotNull] Func<int, int, TP1, TP2, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = ArrayInitializer(i, p1, p2);
                for (var j = 0; j < array[i].Length; j++)
                    array[i][j] = Initializer(i, j, p1, p2);
            }
            return array;
        }

        [DST, NotNull]
        public static TArray[][] Initialize<TArray, TP1, TP2, TP3>
        (
            [NotNull] this TArray[][] array,
            [CanBeNull] TP1 p1,
            [CanBeNull] TP2 p2,
            [CanBeNull] TP3 p3,
            [NotNull] Func<int, TP1, TP2, TP3, TArray[]> ArrayInitializer,
            [NotNull] Func<int, int, TP1, TP2, TP3, TArray> Initializer
        )
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = ArrayInitializer(i, p1, p2, p3);
                for (var j = 0; j < array[i].Length; j++)
                    array[i][j] = Initializer(i, j, p1, p2, p3);
            }
            return array;
        }

        [DST]
        public static void Reverse<TArray>([NotNull] this TArray[] array) => Array.Reverse(array);

        [DST]
        public static void SetValues<TArray>([NotNull] this TArray[] array, int StartIndex, [NotNull] params TArray[] Values)
        {
            for (var i = 0; i + StartIndex < array.Length && i < Values.Length; i++)
                array[i + StartIndex] = Values[i];
        }

        [DST, NotNull]
        public static TArray[,] ToAligned<TArray>([NotNull] this TArray[][] array)
        {
            var l = array.Select(a => a.Length).ToArray();
            return new TArray[array.Length, l.Max()].Initialize(array, l, (i, j, a, ll) => j >= ll[i] ? default : a[i][j]);
        }

        [DST, NotNull]
        public static TArray[][] ToNonAligned<TArray>([NotNull] this TArray[,] array) =>
            new TArray[array.GetLength(0)][]
               .Initialize(array, (i, a) => new TArray[a.GetLength(1)].Initialize(i, a, (j, ii, aa) => aa[ii, j]));

        [DST]
        public static void SetCol<TArray>([NotNull] this TArray[,] array, [NotNull] TArray[] Col, int m = 0)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (N != Col.Length)
                throw new ArgumentException($"Длина столбца {Col.Length} не совпадает с высотой массива {N}");
            if (m < 0 || m >= M)
                throw new ArgumentOutOfRangeException(nameof(m), m, $"Указанный номер столбца {m} выходит за пределы размеров массива {M}");
            for (var i = 0; i < N; i++)
                array[i, m] = Col[i];
        }

        [DST]
        public static void SetRow<TArray>([NotNull] this TArray[,] array, [NotNull]  TArray[] Row, int n = 0)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (M != Row.Length)
                throw new ArgumentException($"Длина столбца {Row.Length} не совпадает с высотой массива {N}");
            if (n < 0 || n >= N)
                throw new ArgumentOutOfRangeException(nameof(n), n, $"Указанный номер столбца {n} выходит за пределы размеров массива {N}");
            for (var i = 0; i < M; i++)
                array[n, i] = Row[i];
        }

        [DST, NotNull]
        public static TArray[] GetCol<TArray>([NotNull] this TArray[,] array, int m)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (m < 0 || m >= M)
                throw new ArgumentOutOfRangeException(nameof(m), m, $"Указанный номер столбца {m} выходит за пределы размеров массива {M}");

            var result = new TArray[N];
            for (var n = 0; n < N; n++)
                result[n] = array[n, m];
            return result;
        }

        [DST, NotNull]
        public static TArray[] GetRow<TArray>([NotNull] this TArray[,] array, int n)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (n < 0 || n >= N)
                throw new ArgumentOutOfRangeException(nameof(n), n, $"Указанный номер столбца {n} выходит за пределы размеров массива {N}");

            var result = new TArray[M];
            for (var m = 0; m < M; m++)
                result[m] = array[n, m];
            return result;
        }

        [DST]
        public static void SwapRows<T>([NotNull] this T[,] array, int i1, int i2)
        {
            var N = array.GetLength(0);
            for (var j = 0; j < N; j++)
            {
                var tmp = array[i1, j];
                array[i1, j] = array[i2, j];
                array[i2, j] = tmp;
            }
        }

        /// <summary>Поменять местами два столбца двумерного массива</summary>
        /// <param name="array">Двумерный массив</param>
        /// <param name="j1">Номер первого столбца</param>
        /// <param name="j2">Номер второго столбца</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        [DST]
        public static void SwapCols<T>([NotNull] this T[,] array, int j1, int j2)
        {
            var M = array.GetLength(1);
            for (var i = 0; i < M; i++)
            {
                var tmp = array[i, j1];
                array[i, j1] = array[i, j2];
                array[i, j2] = tmp;
            }
        }

        public static int GetMinIndex<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare)
        {
            var i_min = 0;
            var min = array[i_min];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (compare(min, v) >= 0) continue;
                i_min = i;
                min = v;
            }
            return i_min;
        }

        public static int GetMinIndex<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter)
        {
            var i_max = 0;
            var min = converter(array[i_max]);
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var y = converter(v);
                if (y >= min) continue;
                i_max = i;
                min = y;
            }
            return i_max;
        }

        public static int GetMinIndex<T>([NotNull] this T[] array) where T : IComparable<T>
        {
            var i_min = 0;
            var min = array[i_min];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Equals(min, default(T)) || min.CompareTo(v) >= 0) continue;
                i_min = i;
                min = v;
            }
            return i_min;
        }

        public static int GetMaxIndex<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare)
        {
            var i_max = 0;
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (compare(max, v) <= 0) continue;
                i_max = i;
                max = v;
            }
            return i_max;
        }

        public static int GetMaxIndex<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter)
        {
            var i_max = 0;
            var max = converter(array[i_max]);
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var y = converter(v);
                if (y <= max) continue;
                i_max = i;
                max = y;
            }
            return i_max;
        }

        public static int GetMaxIndex<T>([NotNull] this T[] array) where T : IComparable<T>
        {
            var i_max = 0;
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (max is null || max.CompareTo(v) <= 0) continue;
                i_max = i;
                max = v;
            }
            return i_max;
        }

        /// <summary>Создать массив последовательных значений длины <paramref name="length"/> начиная с <paramref name="offset"/></summary>
        /// <param name="length">Длина массива</param>
        /// <param name="offset">НАчальное значение</param>
        /// <returns>Массив чисел длины <paramref name="length"/> начиная с <paramref name="offset"/></returns>
        [DST, NotNull]
        public static int[] CreateSequence(int length, int offset = 0)
        {
            var result = new int[length];
            for (var i = 0; i < length; i++)
                result[i] = i + offset;
            return result;
        }

        /// <summary>Создать копию массива с перемешанным содержимым</summary>
        /// <param name="array">Исходный массив</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Копия исходного массива с перемешанным содержимым</returns>
        [DST, NotNull]
        public static T[] Mix<T>([NotNull] this T[] array) => ((T[])array.Clone()).MixRef();

        /// <summary>Создать копию массива с перемешанным содержимым</summary>
        /// <param name="array">Исходный массив</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="rnd">Генератор случайных чисел</param>
        /// <returns>Копия исходного массива с перемешанным содержимым</returns>
        [DST, NotNull]
        public static T[] Mix<T>([NotNull] this T[] array, Random rnd) => ((T[])array.Clone()).MixRef(rnd);

        /// <summary>Перемешать массив</summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="array">Перемешиваемый массив</param>
        /// <param name="rnd">Генератор случайных чисел</param>
        /// <returns>Исходный массив с перемешанным содержимым</returns>
        [DST, NotNull]
        public static T[] MixRef<T>([NotNull] this T[] array, Random rnd)
        {
            var length = array.Length;
            if (rnd is null) rnd = new Random();
            var temp = array[0];
            var index = 0;
            for (var i = 1; i <= length; i++)
                array[index] = array[index = rnd.Next(length)];
            array[index] = temp;

            return array;
        }

        /// <summary>Перемешать массив</summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="array">Перемешиваемый массив</param>
        /// <returns>Исходный массив с перемешанным содержимым</returns>
        [DST, NotNull]
        public static T[] MixRef<T>([NotNull] this T[] array)
        {
            var length = array.Length - 1;
            var rnd = new Random();
            var temp = array[0];
            var index = 0;
            for (var i = 1; i <= length; i++)
                array[index] = array[index = rnd.Next(length)];
            array[index] = temp;

            return array;
        }

        /// <summary>Последовательно скопировать набор массивов в буфер</summary>
        /// <param name="A">Буферный массив соответствующей длины</param>
        /// <param name="B">Перечень устанавливаемых значений</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        [DST]
        public static void SetSubArrays<T>([NotNull] this T[] A, [NotNull] params T[][] B)
        {
            var index = 0;
            B.Foreach(b =>
            {
                b.CopyTo(A, index);
                index += b.Length;
            });
        }

        [DST]
        public static bool IsContains<T>([NotNull] this T[] array, T item) => Array.IndexOf(array, item) != -1;

        [DST, NotNull]
        public static T[] Liniarize<T>([NotNull] this T[][] array)
        {
            var result_length = array.Sum(a => a.Length);
            if (result_length == 0) return new T[0];
            var result = new T[result_length];
            for (int i = 0, k = 0; i < array.Length; i++)
            {
                var sub_array = array[i];
                for (var j = 0; j < sub_array.Length; j++)
                    result[k++] = sub_array[j];
            }
            return result;
        }

        [DST]
        public static void Foreach<T>([NotNull] this T[,] array, [NotNull] Action<T> action)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    action(array[i, j]);
        }

        [DST]
        public static void Foreach<T>([NotNull] this T[,] array, [NotNull] Action<int, int, T> action)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    action(i, j, array[i, j]);
        }

        [DST, NotNull]
        public static IEnumerable<Q> Select<T, Q>([NotNull] this T[,] array, [NotNull] Func<T, Q> selector)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    yield return selector(array[i, j]);
        }

        [DST, NotNull]
        public static IEnumerable<Q> Select<T, Q>([NotNull] this T[,] array, [NotNull] Func<int, int, T, Q> selector)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    yield return selector(i, j, array[i, j]);
        }

        [DST, CanBeNull]
        public static string ToStringView<T>([CanBeNull] this T[,] matrix, [CanBeNull] string Splitter = "\t")
        {
            if (matrix is null) return null;
            var N = matrix.GetLength(0);
            var M = matrix.GetLength(1);
            if (N == 0 || M == 0) return "";
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(matrix[i, 0]);
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(matrix[i, j]);
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }

        [DST, CanBeNull]
        public static string ToStringFormatView<T>
        (
            [CanBeNull] this T[,] matrix,
            [NotNull] string Format = "r",
            [CanBeNull] string Splitter = "\t",
            [CanBeNull] IFormatProvider provider = null
        ) where T : IFormattable
        {
            if (matrix is null) return null;
            var N = matrix.GetLength(0);
            var M = matrix.GetLength(1);
            if (N == 0 || M == 0) return "";
            if (provider is null) provider = CultureInfo.InvariantCulture;
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(matrix[i, 0].ToString(Format, provider));
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(matrix[i, j].ToString(Format, provider));
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }
    }
}