using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathCore;
using MathCore.Annotations;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterTypeCanBeEnumerable.Global

// ReSharper disable once CheckNamespace
namespace System
{
    ///<summary>Методы расширения для массивов</summary>
    public static class ArrayExtensions
    {
        public static int[] InitializeRange(this int[] array, int StartValue = 0, int Step = 1)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = StartValue + i * Step;
            return array;
        }

        public static double[] InitializeRange(this double[] array, double StartValue = 0, double Step = 1)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = StartValue + i * Step;
            return array;
        }

        public static Complex[] InitializeRange(this Complex[] array, Complex StartValue, Complex Step)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = StartValue + i * Step;
            return array;
        }

        /// <summary>Перемешать элементы массива</summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="items">Перемешиваемый массив</param>
        /// <param name="rnd">Генератор случайных чисел</param>
        /// <returns>Перечисление элементов массива в случайном порядке без повторений</returns>
        public static IEnumerable<T> Shuffle<T>(this T[] items, Random rnd = null)
        {
            if (items.Length is not (> 0 and var length))
                yield break;
            rnd ??= new Random();
            var index = new int[length];
            for (var i = 0; i < length; i++)
            {
                var j = rnd.Next(i, length);
                yield return index[j] > 0 
                    ? items[index[j] - 1] 
                    : items[j];
                index[j] = index[i] > 0
                    ? index[i]
                    : i + 1;
            }
        }

        /// <summary>Деконструктор двумерного массива на размеры по первому и второму измерению</summary>
        /// <param name="array">Деконструируемый массив</param>
        /// <param name="N">Число строк (первое измерение)</param>
        /// <param name="M">Число столбцов (второе измерение)</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        public static void Deconstruct<T>([NotNull] this T[,] array, out int N, out int M)
        {
            N = array.GetLength(0);
            M = array.GetLength(1);
        }

        /// <summary>Деконструктор трёхмерного массива на размеры по первому, второму и третьему измерению</summary>
        /// <param name="array">Деконструируемый массив</param>
        /// <param name="N">Число строк (первое измерение)</param>
        /// <param name="M">Число столбцов (второе измерение)</param>
        /// <param name="K">Глубина (третье измерение)</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        public static void Deconstruct<T>([NotNull] this T[,,] array, out int N, out int M, out int K)
        {
            N = array.GetLength(0);
            M = array.GetLength(1);
            K = array.GetLength(2);
        }

        /// <summary>Перебрать элементы массива случайным образом без повторений</summary>
        /// <param name="Items">Массив элементов</param>
        /// <param name="Rnd">Генератор случайных чисел</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Перечисление элементов массива в случайном порядке</returns>
        [NotNull]
        public static IEnumerable<T> AsRandomEnumerable<T>([NotNull] this T[] Items, [CanBeNull] Random Rnd = null)
        {
            Rnd ??= new Random();
            var index = CreateSequence(Items.Length).MixRef(Rnd);
            for (var i = 0; i < Items.Length; i++)
                yield return Items[index[i]];
        }

        /// <summary>Получить последовательность из последних элементов массива (в прямом порядке)</summary>
        /// <param name="Items">Перебираемый массив элементов</param>
        /// <param name="Count">Число элементов с конца, которые надо перебрать</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Перечисление элементов в конце массива в указанном количестве</returns>
        [NotNull]
        public static IEnumerable<T> TakeLast<T>([NotNull] this T[] Items, int Count)
        {
            if (Count <= 0) yield break;

            for (var i = Math.Max(0, Items.Length - Count); i < Items.Length; i++)
                yield return Items[i];
        }

        /// <summary>Получить последовательность из последних элементов массива (в обратном порядке)</summary>
        /// <param name="Items">Перебираемый массив элементов</param>
        /// <param name="Count">Число элементов с конца, которые надо перебрать</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Перечисление элементов в конце массива в указанном количестве</returns>
        [NotNull]
        public static IEnumerable<T> TakeLastInverted<T>([NotNull] this T[] Items, int Count)
        {
            if (Count <= 0) yield break;

            for (int i = Items.Length - 1, I = Math.Max(0, Items.Length - Count); i >= I; i--)
                yield return Items[i];
        }

        /// <summary>Получить последние элементы перечисления</summary>
        /// <param name="Items">Перечисление элементов</param>
        /// <param name="Count">Количество элементов с конца перечисления, которые требуется получить</param>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <returns>Перечисление последних <paramref name="Count"/> элементов <paramref name="Items"/></returns>
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

        /// <summary>Перечисление элементов двумерного массива по строкам</summary>
        /// <param name="array">Двумерный массив, элементы которого требуется перечислить</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Перечисление элементов двумерного массива по строкам</returns>
        [NotNull]
        public static IEnumerable<T> EnumerateElementsByRows<T>([NotNull] this T[,] array)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    yield return array[i, j];
        }

        /// <summary>Перечисление элементов двумерного массива по столбцам</summary>
        /// <param name="array">Двумерный массив, элементы которого требуется перечислить</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Перечисление элементов двумерного массива по столбцам</returns>
        [NotNull]
        public static IEnumerable<T> EnumerateElementsByCols<T>([NotNull] this T[,] array)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            for (var j = 0; j < M; j++)
                for (var i = 0; i < N; i++)
                    yield return array[i, j];
        }

        /// <summary>Проверка на наличие элемента в массиве</summary>
        /// <param name="array">Массив, проверка наличия элемента в котором выполняется</param>
        /// <param name="value">Искомый элемент</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Истина, если элемент найден</returns>
        public static bool Exist<T>([NotNull] this T[] array, T value) => Array.IndexOf(array, value, 0, array.Length) >= 0;

        /// <summary>Проверка на отсутствие элемента в массиве</summary>
        /// <param name="array">Массив, проверка отсутствия элемента в котором выполняется</param>
        /// <param name="value">Искомый элемент</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Истина, если элемент в массиве отсутствует</returns>
        public static bool NotExist<T>([NotNull] this T[] array, T value) => Array.IndexOf(array, value, 0, array.Length) < 0;

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

        ///// <summary>Быстрая сортировка Хоара</summary>
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
        //        while(A[i] < x) i++;  // поиск элемента для переноса в старшую часть
        //        while(A[j] > x) j--;  // поиск элемента для переноса в младшую часть
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
            var i = low;  // i -  левая граница интервала сортировки
            var j = high; // j - правая граница интервала сортировки
            // Захватываем элемент из середины интервала
            var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
            do
            {
                // Сдвигаем левую границу вправо до тех пор, пока элемент на левой границе < x
                while (A[i].CompareTo(x) < 0) i++;  // поиск элемента для переноса в старшую часть
                // Сдвигаем правую границу влево до тех пор, пока элемент на левой границе > x
                while (A[j].CompareTo(x) > 0) j--;  // поиск элемента для переноса в младшую часть
                if (i > j) break;
                // обмен элементов местами:
                var temp = A[i];
                A[i] = A[j];
                A[j] = temp;
                // переход к следующим элементам:
                i++; // Двигаем левую  границу вправо
                j--; // Двигаем правую границу влево
            } while (i < j); // Делаем всё это до тех пор, пока границы не пересекутся
            if (low < j) QuickSort(A, low, j);
            if (i < high) QuickSort(A, i, high);
        }

        /// <summary>Асинхронная быстрая сортировка Хоара</summary>
        /// <typeparam name="T">Тип сортируемых элементов</typeparam>
        /// <param name="A">Сортируемый массив элементов</param>
        /// <param name="low">Нижняя граница индекса сортировки</param>
        /// <param name="high">Верхняя граница индекса сортировки</param>
        /// <returns>Задача, выполняющая процесс быстрой сортировки</returns>
        public static async Task QuickSortAsync<T>([NotNull] this T[] A, int low, int high) where T : IComparable
        {
            var i = low;
            var j = high;
            await Task.Run(() =>
            {
                var x = A[(low + high) / 2];
                do
                {
                    while (A[i].CompareTo(x) < 0) i++;
                    while (A[j].CompareTo(x) > 0) j--;
                    if (i > j) break;
                    var temp = A[i];
                    A[i] = A[j];
                    A[j] = temp;
                    i++;
                    j--;
                } while (i < j);
            })
            .ConfigureAwait(false);

            var sort_tasks = new List<Task>(2);
            if (low < j) sort_tasks.Add(QuickSortAsync(A, low, j));
            if (i < high) sort_tasks.Add(QuickSortAsync(A, i, high));
            if (sort_tasks.Count > 0)
                await Task.WhenAll(sort_tasks);
        }

        /// <summary>Быстрая сортировка Хоара</summary>
        /// <typeparam name="T">Тип сортируемых элементов</typeparam>
        /// <param name="A">Сортируемый массив элементов</param>
        /// <param name="low">Нижняя граница индекса сортировки</param>
        /// <param name="high">Верхняя граница индекса сортировки</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух объектов <typeparamref name="T"/></param>
        public static void QuickSort<T>([NotNull] this T[] A, int low, int high, [NotNull] IComparer<T> Comparer)
        {
            var i = low;
            var j = high;
            var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
            do
            {
                while (Comparer.Compare(A[i], x) < 0) i++;  // поиск элемента для переноса в старшую часть
                while (Comparer.Compare(A[j], x) > 0) j--;  // поиск элемента для переноса в младшую часть
                if (i > j) break;
                // обмен элементов местами:
                var temp = A[i];
                A[i] = A[j];
                A[j] = temp;
                // переход к следующим элементам:
                i++;
                j--;
            } while (i < j);
            if (low < j) QuickSort(A, low, j, Comparer);
            if (i < high) QuickSort(A, i, high, Comparer);
        }

        /// <summary>Асинхронная быстрая сортировка Хоара</summary>
        /// <typeparam name="T">Тип сортируемых элементов</typeparam>
        /// <param name="A">Сортируемый массив элементов</param>
        /// <param name="low">Нижняя граница индекса сортировки</param>
        /// <param name="high">Верхняя граница индекса сортировки</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух объектов <typeparamref name="T"/></param>
        /// <returns>Задача, выполняющая процесс быстрой сортировки</returns>
        public static async Task QuickSortAsync<T>([NotNull] this T[] A, int low, int high, [NotNull] IComparer<T> Comparer)
        {
            var i = low;
            var j = high;
            await Task.Run(() =>
            {
                var x = A[(low + high) / 2];
                do
                {
                    while (Comparer.Compare(A[i], x) < 0) i++;
                    while (Comparer.Compare(A[j], x) > 0) j--;
                    if (i > j) break;
                    var temp = A[i];
                    A[i] = A[j];
                    A[j] = temp;
                    i++;
                    j--;
                } while (i < j);
            })
           .ConfigureAwait(false);

            var sort_tasks = new List<Task>(2);
            if (low < j) sort_tasks.Add(QuickSortAsync(A, low, j, Comparer));
            if (i < high) sort_tasks.Add(QuickSortAsync(A, i, high, Comparer));
            if (sort_tasks.Count > 0)
                await Task.WhenAll(sort_tasks);
        }

        /// <summary>Расчёт хеш-суммы всех элементов массива</summary>
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
            var result = new TArray[A.Length + B.Length];
            A.CopyTo(result, 0);
            B.CopyTo(result, A.Length);
            return result;
        }

        /// <summary>Конкатенация массивов</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="A">Исходный массив</param>
        /// <param name="B">Присоединяемые массивы</param>
        /// <returns>Массив, содержащий все элементы объединяемых массивов</returns>
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
        public static TOut GetSelectedValue<TArray, TOut>(
            [NotNull] this TArray[] A,
            [NotNull] Func<TArray, TOut, TOut> Selector)
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
        public static void Foreach<TArray>(
            [NotNull] this TArray[] array,
            [NotNull] Action<TArray> action)
            => Array.ForEach(array, action);

        ///<summary>Выполнение действия для всех элементов массива с обработкой исключений</summary>
        ///<param name="array">Массив элементов</param>
        ///<param name="action">Выполняемое действие</param>
        ///<param name="ErrorHandler">Обработчик исключения</param>
        ///<typeparam name="TArray">Тип элементов массива</typeparam>
        [DST]
        public static void Foreach<TArray>(
            [NotNull] this TArray[] array,
            [NotNull] Action<TArray> action,
            [NotNull] Func<Exception, bool> ErrorHandler)
            => array.Foreach<TArray, Exception>(action, ErrorHandler);

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

        /// <summary>Извлечь подмассив</summary>
        /// <param name="array">Исходный массив</param>
        /// <param name="Length">Длина извлекаемого участка</param>
        /// <param name="Start">Начальное положение</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <returns>
        /// Новый экземпляр массива, содержащий последовательность элементов
        /// начиная с указанного <paramref name="Start"/> положения и указанной длины <paramref name="Length"/>
        /// </returns>
        [DST, NotNull]
        public static TArray[] GetSubArray<TArray>([NotNull] this TArray[] array, int Length, int Start = 0)
        {
            var result = new TArray[Length];
            Array.Copy(array, Start, result, 0, Length);
            return result;
        }

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Инициализируемый массив</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра номер элемента,
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
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
        /// <param name="array">Инициализируемый массив</param>
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
        /// <param name="array">Инициализируемый массив</param>
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

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Инициализируемый массив</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметров инициализируемый массив и номер элемента,
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный массив</returns>
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

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый массив</param>
        /// <param name="p">Параметр инициализации</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметров инициализируемый массив
        /// и номер элемента и параметр <paramref name="p"/>, 
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный массив</returns>
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

        /// <summary>Инициализация массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый массив</param>
        /// <param name="p1">Параметр 1 инициализации</param>
        /// <param name="p2">Параметр 2 инициализации</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметров инициализируемый массив
        /// и номер элемента, параметр <paramref name="p1"/> и параметр <paramref name="p2"/>, 
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный массив</returns>
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

        /// <summary>Инициализация двумерного массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <param name="array">Инициализируемый двумерный массив</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра номер строки и номер столбца элемента,
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный двумерный массив</returns>
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

        /// <summary>Инициализация двумерного массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый двумерный массив</param>
        /// <param name="p">Параметр инициализации</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра номер строки и номер столбца элемента,
        /// параметр инициализации <paramref name="p"/>
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный двумерный массив</returns>
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

        /// <summary>Инициализация двумерного массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый двумерный массив</param>
        /// <param name="p1">Параметр 1 инициализации</param>
        /// <param name="p2">Параметр 2 инициализации</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра номер строки и номер столбца элемента,
        /// параметры инициализации <paramref name="p1"/> и <paramref name="p2"/>
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный двумерный массив</returns>
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

        /// <summary>Инициализация двумерного массива</summary>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра инициализации</typeparam>
        /// <typeparam name="TP3">Тип 3 параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый двумерный массив</param>
        /// <param name="p1">Параметр 1 инициализации</param>
        /// <param name="p2">Параметр 2 инициализации</param>
        /// <param name="p3">Параметр 3 инициализации</param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра номер строки и номер столбца элемента,
        /// параметры инициализации <paramref name="p1"/>, <paramref name="p2"/> и <paramref name="p3"/>
        /// а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный двумерный массив</returns>
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

        /// <summary>Инициализация невыровненного двумерного массива элементов</summary>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <param name="array">Инициализируемый невыровненный массив</param>
        /// <param name="ArrayInitializer">
        /// Метод создания очередного одномерного массива, получающий в качестве параметра индекс инициализируемого
        /// одномерного массива, а в результате своей работы он должен вернуть одномерный массив элементов,
        /// который будет помещён в ячейку массива верхнего уровня
        /// </param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра индекс массива и индекс элемента
        /// в одномерном массиве, а результатом его вызова должно быть значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный невыровненный двумерный массив</returns>
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

        /// <summary>Инициализация невыровненного двумерного массива элементов</summary>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый невыровненный массив</param>
        /// <param name="p">Параметр процесса инициализации</param>
        /// <param name="ArrayInitializer">
        /// Метод создания очередного одномерного массива, получающий в качестве параметра индекс инициализируемого
        /// одномерного массива и параметр, а в результате своей работы он должен вернуть одномерный массив
        /// элементов, который будет помещён в ячейку массива верхнего уровня
        /// </param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра индекс массива и индекс элемента
        /// в одномерном массиве, а также параметр инициализации, а результатом его вызова должно быть значение,
        /// помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный невыровненный двумерный массив</returns>
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

        /// <summary>Инициализация невыровненного двумерного массива элементов</summary>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый невыровненный массив</param>
        /// <param name="p1">Параметр 1 процесса инициализации</param>
        /// <param name="p2">Параметр 2 процесса инициализации</param>
        /// <param name="ArrayInitializer">
        /// Метод создания очередного одномерного массива, получающий в качестве параметра индекс инициализируемого
        /// одномерного массива и два параметра, а в результате своей работы он должен вернуть одномерный массив
        /// элементов, который будет помещён в ячейку массива верхнего уровня
        /// </param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра индекс массива и индекс элемента
        /// в одномерном массиве, а также два параметра инициализации, а результатом его вызова должно быть
        /// значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный невыровненный двумерный массив</returns>
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

        /// <summary>Инициализация невыровненного двумерного массива элементов</summary>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра инициализации</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра инициализации</typeparam>
        /// <typeparam name="TP3">Тип 3 параметра инициализации</typeparam>
        /// <param name="array">Инициализируемый невыровненный массив</param>
        /// <param name="p1">Параметр 1 процесса инициализации</param>
        /// <param name="p2">Параметр 2 процесса инициализации</param>
        /// <param name="p3">Параметр 3 процесса инициализации</param>
        /// <param name="ArrayInitializer">
        /// Метод создания очередного одномерного массива, получающий в качестве параметра индекс инициализируемого
        /// одномерного массива и три параметра, а в результате своей работы он должен вернуть одномерный массив
        /// элементов, который будет помещён в ячейку массива верхнего уровня
        /// </param>
        /// <param name="Initializer">
        /// Метод инициализации, получающий в качестве параметра индекс массива и индекс элемента
        /// в одномерном массиве, а также три параметра инициализации, а результатом его вызова должно быть
        /// значение, помещаемое в ячейку массива
        /// </param>
        /// <returns>Инициализированный невыровненный двумерный массив</returns>
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

        /// <summary>Обращение порядка элементов в массиве</summary>
        /// <param name="array">
        /// Обращаемый массив, порядок следования элементов которого требуется перевернуть
        /// </param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST]
        public static void Reverse<TArray>([NotNull] this TArray[] array) =>
            Array.Reverse(array ?? throw new ArgumentNullException(nameof(array)));

        /// <summary>Установить значения ячеек массива</summary>
        /// <param name="array">Массив, значения ячеек которого требуется установить</param>
        /// <param name="StartIndex">Индекс первой устанавливаемой ячейки</param>
        /// <param name="Values">Значения, Которые требуется внести с <paramref name="array"/></param>
        /// <typeparam name="TArray">Тип ячеек массива</typeparam>
        [DST]
        public static void SetValues<TArray>(
            [NotNull] this TArray[] array,
            int StartIndex,
            [NotNull] params TArray[] Values)
            => Array.Copy(Values, 0, array, StartIndex, Math.Min(Values.Length, array.Length - StartIndex));

        /// <summary>
        /// Преобразование массива массивов в двумерный массив, где исходные массивы располагаются построчно
        /// </summary>
        /// <param name="array">Массив массивов элементов</param>
        /// <typeparam name="TArray">Тим элементов</typeparam>
        /// <returns>Двумерный массив, составленный построчно из исходных массивов</returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST, NotNull]
        public static TArray[,] ToAlignedRows<TArray>([NotNull] this TArray[][] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var lengths = array.Select(a => a.Length).ToArray();
            var rows_count = array.Length;
            var cols_count = lengths.Max();
            var result = new TArray[rows_count, cols_count];
            for (var i = 0; i < rows_count; i++)
                for (int j = 0, row_length = lengths[i]; j < row_length; j++)
                    result[i, j] = array[i][j];

            return result;
        }

        /// <summary>
        /// Преобразование массива массивов в двумерный массив, где исходные массивы располагаются по столбцам
        /// </summary>
        /// <param name="array">Массив массивов элементов</param>
        /// <typeparam name="TArray">Тим элементов</typeparam>
        /// <returns>Двумерный массив, составленный из исходных массивов, расположенных по столбцам</returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST, NotNull]
        public static TArray[,] ToAlignedCols<TArray>([NotNull] this TArray[][] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var lengths = array.Select(a => a.Length).ToArray();
            var cols_count = array.Length;
            var rows_count = lengths.Max();
            var result = new TArray[rows_count, cols_count];
            for (var j = 0; j < cols_count; j++)
                for (int i = 0, col_length = lengths[j]; i < col_length; i++)
                    result[i, j] = array[i][j];

            return result;
        }

        /// <summary>Преобразовать двумерный массив в массив массивов элементов строк</summary>
        /// <param name="array">Исходный двумерный массив</param>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <returns>Массив строк двумерного массива <paramref name="array"/></returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST, NotNull]
        public static TArray[][] ToNonAlignedRows<TArray>([NotNull] this TArray[,] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var rows_count = array.GetLength(0);
            var cols_count = array.GetLength(1);
            var result = new TArray[rows_count][];
            for (var i = 0; i < rows_count; i++)
            {
                var row = new TArray[cols_count];
                for (var j = 0; j < cols_count; j++)
                    row[j] = array[i, j];
                result[i] = row;
            }

            return result;
        }

        /// <summary>Преобразовать двумерный массив в массив массивов элементов столбцов</summary>
        /// <param name="array">Исходный двумерный массив</param>
        /// <typeparam name="TArray">Тип элементов массивов</typeparam>
        /// <returns>Массив столбцов двумерного массива <paramref name="array"/></returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST, NotNull]
        public static TArray[][] ToNonAlignedCols<TArray>([NotNull] this TArray[,] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var rows_count = array.GetLength(0);
            var cols_count = array.GetLength(1);
            var result = new TArray[cols_count][];
            for (var j = 0; j < cols_count; j++)
            {
                var col = new TArray[cols_count];
                for (var i = 0; i < rows_count; i++)
                    col[i] = array[i, j];
                result[j] = col;
            }

            return result;
        }

        /// <summary>Установить значения колонки (второй индекс) двумерного массива</summary>
        /// <param name="array">Двумерный массив, значение столбца которого требуется установить</param>
        /// <param name="Col">Массив элементов столбца, которые надо установить в <paramref name="array"/></param>
        /// <param name="m">Номер колонки (второй индекс массива)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentException">
        /// Если длина <paramref name="Col"/> неравна числу строк (первая размерность) <paramref name="array"/>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если номер колонки <paramref name="m"/> больше, либо равен числу колонок массива (вторая размерность)
        /// <paramref name="array"/>, либо меньше 0
        /// </exception>
        [DST]
        public static void SetCol<TArray>([NotNull] this TArray[,] array, [NotNull] TArray[] Col, int m = 0)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (N != Col.Length)
                throw new ArgumentException($"Длина столбца {Col.Length} не совпадает с числом строк массива {N}");
            if (m < 0 || m >= M)
                throw new ArgumentOutOfRangeException(nameof(m), m,
                    $"Указанный номер столбца {m} выходит за пределы размеров массива (числа столбцов) {M}");

            for (var i = 0; i < N; i++)
                array[i, m] = Col[i];
        }

        /// <summary>Установить значения строки (первый индекс) двумерного массива</summary>
        /// <param name="array">Двумерный массив, значение строки которого требуется установить</param>
        /// <param name="Row">Массив элементов строки, которые надо установить в <paramref name="array"/></param>
        /// <param name="n">Номер строки (первый индекс массива)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentException">Если длина <paramref name="Row"/> неравна числу столбцов (вторая размерность) <paramref name="array"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если номер колонки <paramref name="n"/> больше, либо равен числу строк массива (первая размерность)
        /// <paramref name="array"/>, либо меньше 0
        /// </exception>
        [DST]
        public static void SetRow<TArray>([NotNull] this TArray[,] array, [NotNull] TArray[] Row, int n = 0)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (M != Row.Length)
                throw new ArgumentException($"Длина столбца {Row.Length} не совпадает с числом столбцов массива {M}");
            if (n < 0 || n >= N)
                throw new ArgumentOutOfRangeException(nameof(n), n,
                    $"Указанный номер столбца {n} выходит за пределы размеров массива (числа строк) {N}");

            for (var i = 0; i < M; i++)
                array[n, i] = Row[i];
        }

        /// <summary>Получить столбец двумерного массива (второй индекс)</summary>
        /// <param name="array">Двумерный массив из которого требуется извлечь столбец значений</param>
        /// <param name="m">Номер столбца массива (второй индекс)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <returns>
        /// Новый массив, значения которого скопированы из столбца элементов двумерного массива
        /// с выбранным индексом <paramref name="m"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если номер столбца больше, либо равен числу столбцов массива (вторая размерность), либо меньше 0
        /// </exception>
        [DST, NotNull]
        public static TArray[] GetCol<TArray>([NotNull] this TArray[,] array, int m)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (m < 0 || m >= M)
                throw new ArgumentOutOfRangeException(nameof(m), m, $"Указанный номер столбца {m} выходит за пределы размеров массива (числа столбцов) {M}");

            var result = new TArray[N];
            for (var n = 0; n < N; n++)
                result[n] = array[n, m];
            return result;
        }

        /// <summary>Получить столбец двумерного массива (второй индекс)</summary>
        /// <param name="array">Двумерный массив из которого требуется извлечь столбец значений</param>
        /// <param name="m">Номер столбца массива (второй индекс)</param>
        /// <param name="Col">Массив столбца (если передана пустая ссылка, то будет создан новый)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если номер столбца больше, либо равен числу столбцов массива (вторая размерность), либо меньше 0
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Если размер массива, переданного для копирования элементов столбца,
        /// не равен числу строк исходного двумерного массива
        /// </exception>
        [DST]
        public static void GetCol<TArray>([NotNull] this TArray[,] array, int m, [CanBeNull] ref TArray[] Col)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));

            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (m < 0 || m >= M)
                throw new ArgumentOutOfRangeException(nameof(m), m,
                    $"Указанный номер столбца {m} выходит за пределы размеров массива (числа столбцов) {M}");

            if (Col is null)
                Col = new TArray[N];
            else
                if (Col.Length != N)
                throw new ArgumentException(
                    $"Размер переданного массива элементов столбца имеет длину {Col.Length}, отличную от числа строк двумерного массива {N}",
                    nameof(Col));

            for (var n = 0; n < N; n++)
                Col[n] = array[n, m];
        }

        /// <summary>Получить строку двумерного массива (первый индекс)</summary>
        /// <param name="array">Двумерный массив из которого требуется извлечь строку значений</param>
        /// <param name="n">Номер строки массива (первый индекс)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <returns>Новый массив, значения которого скопированы из строки элементов двумерного массива с выбранным индексом <paramref name="n"/></returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentOutOfRangeException">Если номер строки больше, либо равен числу строк массива (первая размерность), либо меньше 0</exception>
        [DST, NotNull]
        public static TArray[] GetRow<TArray>([NotNull] this TArray[,] array, int n)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (n < 0 || n >= N)
                throw new ArgumentOutOfRangeException(nameof(n), n,
                    $"Указанный номер строки {n} выходит за пределы размеров массива (числа строк) {N}");

            var result = new TArray[M];
            for (var m = 0; m < M; m++)
                result[m] = array[n, m];
            return result;
        }

        /// <summary>Получить строку двумерного массива (первый индекс)</summary>
        /// <param name="array">Двумерный массив из которого требуется извлечь строку значений</param>
        /// <param name="n">Номер строки массива (первый индекс)</param>
        /// <param name="Row">Массив строки (если передана пустая ссылка, то будет создан новый)</param>
        /// <typeparam name="TArray">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если номер строки больше, либо равен числу строк массива (первая размерность), либо меньше 0
        /// </exception>
        [DST]
        public static void GetRow<TArray>([NotNull] this TArray[,] array, int n, [CanBeNull] ref TArray[] Row)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var N = array.GetLength(0);
            var M = array.GetLength(1);

            if (n < 0 || n >= N)
                throw new ArgumentOutOfRangeException(nameof(n), n,
                    $"Указанный номер строки {n} выходит за пределы размеров массива (числа строк) {N}");

            if (Row is null)
                Row = new TArray[M];
            else
            if (Row.Length != M)
                throw new ArgumentException(
                    $"Размер переданного массива элементов строки имеет длину {Row.Length}, отличную от числа столбцов двумерного массива {M}",
                    nameof(Row));

            for (var m = 0; m < M; m++)
                Row[m] = array[n, m];
        }

        /// <summary>Поменять местами элементы двух строк двумерного массива</summary>
        /// <param name="array">Массив в котором требуется поменять местами две строки</param>
        /// <param name="i1">Индекс первой строки</param>
        /// <param name="i2">Индекс второй строки</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST]
        public static void SwapRows<T>([NotNull] this T[,] array, int i1, int i2)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (i1 == i2) return;
            var N = array.GetLength(0);
            for (var j = 0; j < N; j++)
            {
                var tmp = array[i1, j];
                array[i1, j] = array[i2, j];
                array[i2, j] = tmp;
            }
        }

        /// <summary>Поменять местами два столбца двумерного массива</summary>
        /// <param name="array">Массив в котором требуется поменять местами два столбца</param>
        /// <param name="j1">Индекс первого столбца</param>
        /// <param name="j2">Индекс второго столбца</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на исходный массив</exception>
        [DST]
        public static void SwapCols<T>([NotNull] this T[,] array, int j1, int j2)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (j1 == j2) return;
            var M = array.GetLength(1);
            for (var i = 0; i < M; i++)
            {
                var tmp = array[i, j1];
                array[i, j1] = array[i, j2];
                array[i, j2] = tmp;
            }
        }

        /// <summary>Найти индекс минимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального элемента</returns>
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

        /// <summary>Найти минимальный элемента в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare)
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
            return ref array[i_min];
        }

        /// <summary>Найти минимальный элемента в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <param name="MinIndex">Индекс минимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare, out int MinIndex)
        {
            MinIndex = 0;
            var min = array[MinIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (compare(min, v) >= 0) continue;
                MinIndex = i;
                min = v;
            }
            return ref array[MinIndex];
        }

        /// <summary>Найти индекс минимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального элемента</returns>
        public static int GetMinIndex<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer)
        {
            var i_min = 0;
            var min = array[i_min];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(min, v) >= 0) continue;
                i_min = i;
                min = v;
            }
            return i_min;
        }

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer)
        {
            var i_min = 0;
            var min = array[i_min];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(min, v) >= 0) continue;
                i_min = i;
                min = v;
            }
            return ref array[i_min];
        }

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <param name="MinIndex">Индекс минимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer, out int MinIndex)
        {
            MinIndex = 0;
            var min = array[MinIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(min, v) >= 0) continue;
                MinIndex = i;
                min = v;
            }
            return ref array[MinIndex];
        }

        /// <summary>Найти индекс минимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального элемента</returns>
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

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter)
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
            return ref array[i_max];
        }

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <param name="MinIndex">Индекс минимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter, out int MinIndex)
        {
            MinIndex = 0;
            var min = converter(array[MinIndex]);
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var y = converter(v);
                if (y >= min) continue;
                MinIndex = i;
                min = y;
            }
            return ref array[MinIndex];
        }

        /// <summary>Найти индекс минимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Индекс минимального элемента</returns>
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

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array) where T : IComparable<T>
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
            return ref array[i_min];
        }

        /// <summary>Найти минимальный элемент в массиве и получить ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <param name="MinIndex">Индекс минимального элемента</param>
        /// <returns>Минимальный элемент массива по ссылке</returns>
        public static ref T GetMinRef<T>([NotNull] this T[] array, out int MinIndex) where T : IComparable<T>
        {
            MinIndex = 0;
            var min = array[MinIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Equals(min, default(T)) || min.CompareTo(v) >= 0) continue;
                MinIndex = i;
                min = v;
            }
            return ref array[MinIndex];
        }

        /// <summary>Найти индекс максимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс максимального элемента</returns>
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

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare)
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
            return ref array[i_max];
        }

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <param name="MaxIndex">Индекс максимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] Comparison<T> compare, out int MaxIndex)
        {
            MaxIndex = 0;
            var max = array[MaxIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (compare(max, v) <= 0) continue;
                MaxIndex = i;
                max = v;
            }
            return ref array[MaxIndex];
        }

        /// <summary>Найти индекс максимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс максимального элемента</returns>
        public static int GetMaxIndex<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer)
        {
            var i_max = 0;
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(max, v) <= 0) continue;
                i_max = i;
                max = v;
            }
            return i_max;
        }

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer)
        {
            var i_max = 0;
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(max, v) <= 0) continue;
                i_max = i;
                max = v;
            }
            return ref array[i_max];
        }

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <param name="MaxIndex">Индекс максимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] IComparer<T> Comparer, out int MaxIndex)
        {
            MaxIndex = 0;
            var max = array[MaxIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(max, v) <= 0) continue;
                MaxIndex = i;
                max = v;
            }
            return ref array[MaxIndex];
        }

        /// <summary>Найти индекс максимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс максимального элемента</returns>
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

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter)
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
            return ref array[i_max];
        }

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <param name="MaxIndex">Индекс максимального элемента</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter, out int MaxIndex)
        {
            MaxIndex = 0;
            var max = converter(array[MaxIndex]);
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var y = converter(v);
                if (y <= max) continue;
                MaxIndex = i;
                max = y;
            }
            return ref array[MaxIndex];
        }

        /// <summary>Найти индекс максимального элемента в массиве</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Индекс максимального элемента</returns>
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

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array) where T : IComparable<T>
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
            return ref array[i_max];
        }

        /// <summary>Найти максимальный элемент в массиве и вернуть ссылку на него</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="MaxIndex">Индекс максимального элемента</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Максимальный элемент массива по ссылке</returns>
        public static ref T GetMaxRef<T>([NotNull] this T[] array, out int MaxIndex) where T : IComparable<T>
        {
            MaxIndex = 0;
            var max = array[MaxIndex];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (max is null || max.CompareTo(v) <= 0) continue;
                MaxIndex = i;
                max = v;
            }
            return ref array[MaxIndex];
        }

        /// <summary>Найти индексы минимального и максимального элементов</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="compare">Метод сравнения двух объектов между собой</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального и максимального элементов массива</returns>
        public static (int MinIndex, int MaxIndex) GetMinMaxIndex<T>([NotNull] this T[] array, Comparison<T> compare)
        {
            var i_min = 0;
            var i_max = 0;
            var min = array[i_min];
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (compare(min, v) < 0)
                {
                    i_min = i;
                    min = v;
                }
                else if (compare(max, v) > 0)
                {
                    i_max = i;
                    max = v;
                }
            }

            return (i_min, i_max);
        }

        /// <summary>Найти индексы минимального и максимального элементов</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="Comparer">Объект, обеспечивающий сравнение двух элементов в массиве</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального и максимального элементов массива</returns>
        public static (int MinIndex, int MaxIndex) GetMinMaxIndex<T>([NotNull] this T[] array, IComparer<T> Comparer)
        {
            var i_min = 0;
            var i_max = 0;
            var min = array[i_min];
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                if (Comparer.Compare(min, v) < 0)
                {
                    i_min = i;
                    min = v;
                }
                else if (Comparer.Compare(max, v) > 0)
                {
                    i_max = i;
                    max = v;
                }
            }

            return (i_min, i_max);
        }

        /// <summary>Найти индексы минимального и максимального элементов</summary>
        /// <param name="array">Массив элементов</param>
        /// <param name="converter">Функция, обеспечивающая преобразования элемента в численное значение для сравнения</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Индекс минимального и максимального элементов массива</returns>
        public static (int MinIndex, int MaxIndex) GetMinMaxIndex<T>([NotNull] this T[] array, [NotNull] Func<T, double> converter)
        {
            var i_min = 0;
            var i_max = 0;
            var min = converter(array[i_min]);
            var max = min;
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var y = converter(v);
                if (y < min)
                {
                    i_min = i;
                    min = y;
                }
                else if (y > max)
                {
                    i_max = i;
                    max = y;
                }
            }

            return (i_min, i_max);
        }

        /// <summary>Найти индексы минимального и максимального элементов</summary>
        /// <param name="array">Массив элементов</param>
        /// <typeparam name="T">Тип элементов массива, поддерживающий возможность сравнения</typeparam>
        /// <returns>Индекс минимального и максимального элементов массива</returns>
        public static (int MinIndex, int MaxIndex) GetMinMaxIndex<T>([NotNull] this T[] array) where T : IComparable<T>
        {
            var i_min = 0;
            var i_max = 0;
            var min = array[i_min];
            var max = array[i_max];
            for (var i = 1; i < array.Length; i++)
            {
                var v = array[i];
                var min_changed = false;
                if (min is null || min.CompareTo(v) < 0)
                {
                    i_min = i;
                    min = v;
                    min_changed = true;
                }
                if (max is null || !min_changed && max.CompareTo(v) > 0)
                {
                    i_max = i;
                    max = v;
                }
            }

            return (i_min, i_max);
        }

        /// <summary>Создать массив последовательных значений длины <paramref name="length"/> начиная с <paramref name="offset"/></summary>
        /// <param name="length">Длина массива</param>
        /// <param name="offset">Начальное значение</param>
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
        public static T[] Mix<T>([NotNull] this T[] array, [CanBeNull] Random rnd) => ((T[])array.Clone()).MixRef(rnd);

        /// <summary>Перемешать массив</summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="array">Перемешиваемый массив</param>
        /// <param name="rnd">Генератор случайных чисел</param>
        /// <returns>Исходный массив с перемешанным содержимым</returns>
        [DST, NotNull]
        public static T[] MixRef<T>([NotNull] this T[] array, [CanBeNull] Random rnd)
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
            foreach (var array in B)
            {
                array.CopyTo(A, index);
                index += array.Length;
            }
        }

        /// <summary>Проверка, что элемент входит в состав массива</summary>
        /// <param name="array">Проверяемый массив элементов</param>
        /// <param name="item">Элемент, вхождение в состав массива <paramref name="array"/> которого проверяется</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Истина, если массив <paramref name="array"/> содержит среди своих элементов <paramref name="item"/></returns>
        [DST]
        public static bool IsContains<T>([NotNull] this T[] array, T item) => Array.IndexOf(array, item) != -1;

        /// <summary>Создать одномерный массив, содержащий все элементы указанных массивов</summary>
        /// <param name="array">Массив массивов элементов</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>Одномерный массив, содержащий все элементы массивов из <paramref name="array"/></returns>
        [DST, NotNull]
        public static T[] Linearize<T>([NotNull] this T[][] array)
        {
            var result_length = array.Sum(a => a.Length);
            if (result_length == 0) return Array.Empty<T>();
            var result = new T[result_length];
            for (int i = 0, k = 0; i < array.Length; i++)
            {
                var sub_array = array[i];
                for (var j = 0; j < sub_array.Length; j++)
                    result[k++] = sub_array[j];
            }
            return result;
        }

        /// <summary>Выполнить действие для всех элементов двумерного массива</summary>
        /// <param name="array">Двумерный массив элементов, действие для всех элементов которого следует выполнить</param>
        /// <param name="action">Выполняемое действие, получающее в качестве параметра элемент массива</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        [DST]
        public static void Foreach<T>([NotNull] this T[,] array, [NotNull] Action<T> action)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    action(array[i, j]);
        }

        /// <summary>Выполнить действие для всех элементов двумерного массива</summary>
        /// <param name="array">Двумерный массив элементов, действие для всех элементов которого следует выполнить</param>
        /// <param name="action">
        /// Выполняемое действие, получающее в качестве параметра номер строки (первый индекс),
        /// номер столбца (второй индекс) и элемент массива
        /// </param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        [DST]
        public static void Foreach<T>([NotNull] this T[,] array, [NotNull] Action<int, int, T> action)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    action(i, j, array[i, j]);
        }

        /// <summary>Получить перечисление значений на основе элементов двумерного массива</summary>
        /// <param name="array">Двумерный массив элементов</param>
        /// <param name="selector">
        /// Метод конвертации элемента массива <typeparamref name="TItem"/> в его значение
        /// <typeparamref name="TValue"/>, получающий в качестве параметра значение элемента массива
        /// </param>
        /// <typeparam name="TItem">Тип элементов массива</typeparam>
        /// <typeparam name="TValue">Тип результата элементов перечисления</typeparam>
        /// <returns>Перечисление значений, сформированное на основе элементов двумерного массива</returns>
        [DST, NotNull]
        public static IEnumerable<TValue> Select<TItem, TValue>(
            [NotNull] this TItem[,] array,
            [NotNull] Func<TItem, TValue> selector)
        {
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    yield return selector(array[i, j]);
        }

        /// <summary>Получить перечисление значений на основе элементов двумерного массива</summary>
        /// <param name="array">Двумерный массив элементов</param>
        /// <param name="selector">
        /// Метод конвертации элемента массива <typeparamref name="TItem"/> в его значение
        /// <typeparamref name="TValue"/>, получающий в качестве параметров номер строки (первый индекс),
        /// номер столбца (второй индекс) и значение элемента массива
        /// </param>
        /// <typeparam name="TItem">Тип элементов массива</typeparam>
        /// <typeparam name="TValue">Тип результата элементов перечисления</typeparam>
        /// <returns>Перечисление значений, сформированное на основе элементов двумерного массива</returns>
        [DST, NotNull]
        public static IEnumerable<TValue> Select<TItem, TValue>(
            [NotNull] this TItem[,] array,
            [NotNull] Func<int, int, TItem, TValue> selector)
        {
            var I = array.GetLength(0);
            var J = array.GetLength(1);
            for (var i = 0; i < I; i++)
                for (var j = 0; j < J; j++)
                    yield return selector(i, j, array[i, j]);
        }

        /// <summary>Представить двумерный массив в текстовом виде в виде матрицы</summary>
        /// <param name="array">Двумерный массив, текстовое представление которого требуется выполнить</param>
        /// <param name="Splitter">Строка-разделитель элементов матрицы</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>
        /// Строковое представление двумерного массива в котором строки массива разделены переносом строки
        /// </returns>
        [DST, CanBeNull]
        public static string ToStringView<T>([CanBeNull] this T[,] array, [CanBeNull] string Splitter = "\t")
        {
            if (array is null) return null;
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            if (N == 0 || M == 0) return string.Empty;
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(array[i, 0]);
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(array[i, j]);
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }

        /// <summary>Представить двумерный массив в текстовом виде в виде матрицы</summary>
        /// <param name="array">Двумерный массив, текстовое представление которого требуется выполнить</param>
        /// <param name="Selector">Метод преобразования значения элемента массива перед его записью</param>
        /// <param name="Splitter">Строка-разделитель элементов матрицы</param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <typeparam name="TValue">Тип данных, записываемых в текстовую форму представления</typeparam>
        /// <returns>
        /// Строковое представление двумерного массива в котором строки массива разделены переносом строки
        /// </returns>
        [DST, CanBeNull]
        public static string ToStringView<T, TValue>(
            [CanBeNull] this T[,] array,
            [NotNull] Func<T, TValue> Selector,
            [CanBeNull] string Splitter = "\t")
        {
            if (array is null) return null;
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            if (N == 0 || M == 0) return string.Empty;
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(Selector(array[i, 0]));
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(Selector(array[i, j]));
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }

        /// <summary>Представить двумерный массив в текстовом виде в виде матрицы</summary>
        /// <param name="array">Двумерный массив, текстовое представление которого требуется выполнить</param>
        /// <param name="Format">Строка формата данных каждого элемента</param>
        /// <param name="Splitter">Строка-разделитель элементов матрицы</param>
        /// <param name="provider">
        /// Объект, осуществляющий конечное форматирование элемента массива перед его выводом в текстовую форму
        /// </param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <returns>
        /// Строковое представление двумерного массива в котором строки массива разделены переносом строки
        /// </returns>
        [DST, CanBeNull]
        public static string ToStringFormatView<T>
        (
            [CanBeNull] this T[,] array,
            [NotNull] string Format = "r",
            [CanBeNull] string Splitter = "\t",
            [CanBeNull] IFormatProvider provider = null
        ) where T : IFormattable
        {
            if (array is null) return null;
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            if (N == 0 || M == 0) return string.Empty;
            if (provider is null) provider = CultureInfo.InvariantCulture;
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(array[i, 0].ToString(Format, provider));
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(array[i, j].ToString(Format, provider));
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }

        /// <summary>Представить двумерный массив в текстовом виде в виде матрицы</summary>
        /// <param name="array">Двумерный массив, текстовое представление которого требуется выполнить</param>
        /// <param name="Selector">Метод преобразования значения элемента массива перед его записью</param>
        /// <param name="Format">Строка формата данных каждого элемента</param>
        /// <param name="Splitter">Строка-разделитель элементов матрицы</param>
        /// <param name="provider">
        /// Объект, осуществляющий конечное форматирование элемента массива перед его выводом в текстовую форму
        /// </param>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <typeparam name="TValue">Тип данных, записываемых в текстовую форму представления</typeparam>
        /// <returns>
        /// Строковое представление двумерного массива в котором строки массива разделены переносом строки
        /// </returns>
        [DST, CanBeNull]
        public static string ToStringFormatView<T, TValue>
        (
            [CanBeNull] this T[,] array,
            [NotNull] Func<T, TValue> Selector,
            [NotNull] string Format = "r",
            [CanBeNull] string Splitter = "\t",
            [CanBeNull] IFormatProvider provider = null
        ) where TValue : IFormattable
        {
            if (array is null) return null;
            var N = array.GetLength(0);
            var M = array.GetLength(1);
            if (N == 0 || M == 0) return string.Empty;
            if (provider is null) provider = CultureInfo.InvariantCulture;
            var result = new StringBuilder();
            var line = new StringBuilder();

            for (var i = 0; i < N; i++)
            {
                line.Clear();
                line.Append(Selector(array[i, 0]).ToString(Format, provider));
                for (var j = 1; j < M; j++)
                {
                    line.Append(Splitter);
                    line.Append(Selector(array[i, j]).ToString(Format, provider));
                }
                result.AppendLine(line.ToString());
            }
            return result.ToString();
        }
    }
}