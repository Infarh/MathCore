#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using MathCore;
using MathCore.Annotations;
using MathCore.Values;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using InN = MathCore.Annotations.ItemNotNullAttribute;
using IcN = MathCore.Annotations.ItemCanBeNullAttribute;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Local
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    /// <summary>Класс методов-расширений для интерфейса перечисления</summary>
    [PublicAPI]
    public static class IEnumerableExtensions
    {
        /// <summary>Преобразовать последовательность в хеш-таблицу</summary>
        /// <typeparam name="T">Тип элемента последовательности</typeparam>
        /// <param name="items">Последовательность элементов, для которой надо создать хеш-таблицу</param>
        /// <returns>Новая хеш-таблица, созданная из указанной последовательности элементов</returns>
        public static HashSet<T> GetHashSet<T>(this IEnumerable<T> items) => new(items);

        public static HashSet<T> GetHashSet<T>(this IEnumerable<T> items, Func<T, T, bool> Comparer, Func<T, int> Hasher)
        {
            var set = new HashSet<T>(Comparer.Create(Hasher));
            foreach (var item in items)
                set.Add(item);
            return set;

        }

        /// <summary>Перечисление без повторений значений, определяемых лямбда-выражением</summary>
        /// <typeparam name="T">Тип перечисляемых объектов</typeparam>
        /// <typeparam name="TKey">Тип ключа значения</typeparam>
        /// <param name="enumerable">Исходное перечисление объектов</param>
        /// <param name="KeySelector">Критерий определения повторения значения</param>
        /// <returns>Перечисление, из которого исключены повторения по указанному критерию</returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> KeySelector) =>
            enumerable.Distinct(PropertyEqualityComparer.Create(KeySelector));

        /// <summary>Дисперсия значений</summary>
        /// <param name="enumerable">Объект-источник данных</param>
        /// <returns>Дисперсия значений</returns>
        public static double Dispersion(this IEnumerable<double> enumerable)
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

            if (enumerable is double[] array) return DoubleArrayExtensions.Dispersion(array);

            double sum;
            double sum2;
            long count;
            switch (enumerable)
            {
                case List<double> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0d;
                        sum2 = 0d;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                case IList<double> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0d;
                        sum2 = 0d;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                default:
                    {
                        using var enumerator = enumerable.GetEnumerator();
                        if (!enumerator.MoveNext()) return double.NaN;

                        sum = enumerator.Current;
                        sum2 = sum * sum;
                        count = 1;
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            sum += x;
                            sum2 += x * x;
                            count++;
                        }
                        break;
                    }
            }
            return count == 1 ? 0 : (sum2 - sum / count) / count;
        }

        /// <summary>Дисперсия значений</summary>
        /// <param name="enumerable">Объект-источник данных</param>
        /// <returns>Дисперсия значений</returns>
        public static double Dispersion(this IEnumerable<int> enumerable)
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

            long sum;
            long sum2;
            long count;
            switch (enumerable)
            {
                case int[] list:
                    {
                        if (list.Length == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Length;
                        break;
                    }

                case List<int> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                case IList<int> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                default:
                    {
                        using var enumerator = enumerable.GetEnumerator();
                        if (!enumerator.MoveNext()) return double.NaN;

                        sum = enumerator.Current;
                        sum2 = sum * sum;
                        count = 1;
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            sum += x;
                            sum2 += x * x;
                            count++;
                        }
                        break;
                    }
            }
            return count == 1 ? 0 : (sum2 - sum / (double)count) / count;
        }

        /// <summary>Дисперсия значений</summary>
        /// <param name="enumerable">Объект-источник данных</param>
        /// <returns>Дисперсия значений</returns>
        public static double Dispersion(this IEnumerable<long> enumerable)
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

            long sum;
            long sum2;
            long count;
            switch (enumerable)
            {
                case long[] list:
                    {
                        if (list.Length == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Length;
                        break;
                    }

                case List<long> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                case IList<long> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0;
                        sum2 = 0;
                        foreach (var x in list)
                        {
                            sum += x;
                            sum2 += x * x;
                        }

                        count = list.Count;
                        break;
                    }

                default:
                    {
                        using var enumerator = enumerable.GetEnumerator();
                        if (!enumerator.MoveNext()) return double.NaN;

                        sum = enumerator.Current;
                        sum2 = sum * sum;
                        count = 1;
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            sum += x;
                            sum2 += x * x;
                            count++;
                        }
                        break;
                    }
            }
            return count == 1 ? 0 : (sum2 - sum / (double)count) / count;
        }

        /// <summary>Дисперсия значений</summary>
        /// <param name="enumerable">Объект-источник данных</param>
        /// <param name="Selector">Метод получения вещественного значения</param>
        /// <returns>Дисперсия значений</returns>
        public static double Dispersion<T>(this IEnumerable<T> enumerable, Func<T, double> Selector)
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));
            if (Selector is null) throw new ArgumentNullException(nameof(Selector));

            double sum;
            double sum2;
            long count;
            switch (enumerable)
            {
                case T[] list:
                    {
                        if (list.Length == 0) return double.NaN;
                        sum = 0d;
                        sum2 = 0d;
                        foreach (var x in list)
                        {
                            var y = Selector(x);
                            sum += y;
                            sum2 += y * y;
                        }

                        count = list.Length;
                        break;
                    }

                case List<T> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0d;
                        sum2 = 0d;
                        foreach (var x in list)
                        {
                            var y = Selector(x);
                            sum += y;
                            sum2 += y * y;
                        }

                        count = list.Count;
                        break;
                    }

                case IList<T> list:
                    {
                        if (list.Count == 0) return double.NaN;

                        sum = 0d;
                        sum2 = 0d;
                        foreach (var x in list)
                        {
                            var y = Selector(x);
                            sum += y;
                            sum2 += y * y;
                        }

                        count = list.Count;
                        break;
                    }

                default:
                    {
                        using var enumerator = enumerable.GetEnumerator();
                        if (!enumerator.MoveNext()) return double.NaN;

                        sum = Selector(enumerator.Current);
                        sum2 = sum * sum;
                        count = 1;
                        while (enumerator.MoveNext())
                        {
                            var y = Selector(enumerator.Current);
                            sum += y;
                            sum2 += y * y;
                            count++;
                        }
                        break;
                    }
            }
            return count == 1 ? 0 : (sum2 - sum / count) / count;
        }

        /// <summary>Добавить элемент в начало последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="enumerable">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на первом месте</returns>
        public static IEnumerable<T> InsertBefore<T>(this IEnumerable<T>? enumerable, params T[]? values)
        {
            if (values != null) foreach (var v in values) yield return v;
            if (enumerable != null) foreach (var v in enumerable) yield return v;
        }

        /// <summary>Добавить элемент в начало последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="enumerable">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на первом месте</returns>
        public static IEnumerable<T> InsertBefore<T>(this IEnumerable<T>? enumerable, IEnumerable<T>? values)
        {
            if (values != null) foreach (var v in values) yield return v;
            if (enumerable != null) foreach (var v in enumerable) yield return v;
        }

        /// <summary>Добавить элемент в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на последнем месте</returns>
        public static IEnumerable<T> InsertAfter<T>(this IEnumerable<T> collection, params T[] values)
        {
            if (collection != null) foreach (var v in collection) yield return v;
            if (values != null) foreach (var v in values) yield return v;
        }

        /// <summary>Добавить элемент в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на последнем месте</returns>
        public static IEnumerable<T> InsertAfter<T>(this IEnumerable<T> collection, IEnumerable<T> values)
        {
            if (collection != null) foreach (var v in collection) yield return v;
            if (values != null) foreach (var v in values) yield return v;
        }

        /// <summary>Первый элемент перечисления, удовлетворяющий задаваемому критерию с параметром</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <param name="Selector">Критерий отбора элементов перечисления с параметром</param>
        /// <param name="DefaultValue">Значение по умолчанию, возвращаемое если ни один из элементов перечисления не соответствует критерию</param>
        /// <returns>Первый найденный элемент в перечислении, удовлетворяющий критерию, либо значение по умолчанию</returns>
        public static T? FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> Selector, in T? DefaultValue = default)
        {
            switch (enumerable)
            {
                case T[] { Length: 0 }: return DefaultValue;
                case List<T> { Count: 0 }: return DefaultValue;
                case IList<T> { Count: 0 }: return DefaultValue;

                case T[] list:
                    {
                        var item = list[0];
                        return Selector(item) ? item : DefaultValue;
                    }
                case List<T> list:
                    {
                        var item = list[0];
                        return Selector(item) ? item : DefaultValue;
                    }
                case IList<T> list:
                    {
                        var item = list[0];
                        return Selector(item) ? item : DefaultValue;
                    }

                default:
                    foreach (var item in enumerable)
                        if (Selector(item))
                            return item;
                    return DefaultValue;
            }
        }

        /// <summary>Первый элемент перечисления, удовлетворяющий задаваемому критерию с параметром</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <typeparam name="TP">Тип параметра</typeparam>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <param name="p">Параметр, передаваемый в критерий, необходимый для устранения замыкания</param>
        /// <param name="Selector">Критерий отбора элементов перечисления с параметром</param>
        /// <param name="DefaultValue">Значение по умолчанию, возвращаемое если ни один из элементов перечисления не соответствует критерию</param>
        /// <returns>Первый найденный элемент в перечислении, удовлетворяющий критерию, либо значение по умолчанию</returns>
        public static T? FirstOrDefault<T, TP>(this IEnumerable<T> enumerable, in TP p, Func<T, TP, bool> Selector, in T? DefaultValue = default)
        {
            switch (enumerable)
            {
                case T[] { Length: 0 }: return DefaultValue;
                case List<T> { Count: 0 }: return DefaultValue;
                case IList<T> { Count: 0 }: return DefaultValue;

                case T[] list:
                    {
                        var item = list[0];
                        return Selector(item, p) ? item : DefaultValue;
                    }
                case List<T> list:
                    {
                        var item = list[0];
                        return Selector(item, p) ? item : DefaultValue;
                    }
                case IList<T> list:
                    {
                        var item = list[0];
                        return Selector(item, p) ? item : DefaultValue;
                    }

                default:
                    foreach (var item in enumerable)
                        if (Selector(item, p))
                            return item;
                    return DefaultValue;
            }
        }

        /// <summary>Первый элемент перечисления, удовлетворяющий задаваемому критерию с параметром</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <typeparam name="TP1">Тип параметра 1</typeparam>
        /// <typeparam name="TP2">Тип параметра 2</typeparam>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <param name="p1">Параметр 1, передаваемый в критерий, необходимый для устранения замыкания</param>
        /// <param name="p2">Параметр 2, передаваемый в критерий, необходимый для устранения замыкания</param>
        /// <param name="Selector">Критерий отбора элементов перечисления с параметром</param>
        /// <param name="DefaultValue">Значение по умолчанию, возвращаемое если ни один из элементов перечисления не соответствует критерию</param>
        /// <returns>Первый найденный элемент в перечислении, удовлетворяющий критерию, либо значение по умолчанию</returns>
        public static T? FirstOrDefault<T, TP1, TP2>(
            this IEnumerable<T> enumerable,
            in TP1 p1,
            in TP2 p2,
            Func<T, TP1, TP2, bool> Selector,
            in T? DefaultValue = default)
        {
            switch (enumerable)
            {
                case T[] { Length: 0 }: return DefaultValue;
                case List<T> { Count: 0 }: return DefaultValue;
                case IList<T> { Count: 0 }: return DefaultValue;

                case T[] list:
                    {
                        var item = list[0];
                        return Selector(item, p1, p2) ? item : DefaultValue;
                    }
                case List<T> list:
                    {
                        var item = list[0];
                        return Selector(item, p1, p2) ? item : DefaultValue;
                    }
                case IList<T> list:
                    {
                        var item = list[0];
                        return Selector(item, p1, p2) ? item : DefaultValue;
                    }

                default:
                    foreach (var item in enumerable)
                        if (Selector(item, p1, p2))
                            return item;
                    return DefaultValue;
            }
        }

        /// <summary>Сделать перечисление плоским (линейным)</summary>
        /// <param name="enumerable">Перечисление перечислений элементов</param>
        /// <typeparam name="TSource">Тип перечисления первого уровня</typeparam>
        /// <typeparam name="TSubSource">Тип перечисления второго уровня</typeparam>
        /// <typeparam name="TItem">Тип перечисления результатов</typeparam>
        /// <returns>Перечисление вложенных элементов</returns>
        public static IEnumerable<TItem> SelectMany<TSource, TSubSource, TItem>(this TSource? enumerable)
            where TSource : IEnumerable<TSubSource>
            where TSubSource : IEnumerable<TItem>
        {
            if (enumerable is null) yield break;
            switch (enumerable)
            {
                case TSubSource[] list:
                    foreach (var items in list)
                        if (items != null)
                            foreach (var item in items)
                                yield return item;
                    break;

                case List<TSubSource> list:
                    foreach (var items in list)
                        if (items != null)
                            foreach (var item in items)
                                yield return item;
                    break;

                case IList<TSubSource> list:
                    foreach (var items in list)
                        if (items != null)
                            foreach (var item in items)
                                yield return item;
                    break;

                default:
                    foreach (var items in enumerable)
                        if (items != null)
                            foreach (var item in items)
                                yield return item;
                    break;
            }
        }

        /// <summary>Сформировать блочный перечислитель элементов</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="enumerable">Перечисление элементов, которое требуется разбить на блоки</param>
        /// <param name="BlockSize">Размер блоков, не которые будет разбито исходное перечисление</param>
        /// <param name="CreateNewBlocks">Создавать новые блоки (не использовать один и тот же буфер)</param>
        /// <returns>Перечисление блоков элементов</returns>
        public static IEnumerable<T?[]> AsBlockEnumerable<T>(this IEnumerable<T?> enumerable, int BlockSize, bool CreateNewBlocks = true)
        {
            if (BlockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BlockSize), "Размер блока должен быть положительным значением");

            using var e = enumerable.GetEnumerator();
            var block = new T?[BlockSize];
            while (e.MoveNext())
            {
                block[0] = e.Current;

                var i = 1;
                for (; i < block.Length && e.MoveNext(); i++) block[i] = e.Current;

                if (i == block.Length)
                    yield return block;
                else
                {
                    Array.Resize(ref block, i);
                    yield return block;
                    yield break;
                }

                if (CreateNewBlocks)
                    block = new T[BlockSize];
            }
        }

        /// <summary>Перечисление элементов за исключением указанного</summary>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="item">Элемент, исключаемый из перечисления</param>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <returns>Перечисление элементов за исключением указанного</returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T item)
        {
            switch (enumerable)
            {
                case T[] list:
                    foreach (var i in list)
                        if (!Equals(i, item))
                            yield return i;
                    break;

                case List<T> list:
                    foreach (var i in list)
                        if (!Equals(i, item))
                            yield return i;
                    break;

                case IList<T> list:
                    foreach (var i in list)
                        if (!Equals(i, item))
                            yield return i;
                    break;

                default:
                    foreach (var i in enumerable)
                        if (!Equals(i, item))
                            yield return i;
                    break;
            }
        }

        /// <summary>Индекс первого найденного элемента</summary>
        /// <typeparam name="T">Тип искомого элемента</typeparam>
        /// <param name="enumerable">Перечисление элементов, в котором требуется найти заданный</param>
        /// <param name="item">Искомый элемент</param>
        /// <returns>Индекс первого вхождения элемента в перечисление, либо -1 в случае, если он в ней отсутствует</returns>
        public static int FirstIndexOf<T>(this IEnumerable<T?> enumerable, in T? item)
        {
            var index = -1;
            switch (enumerable)
            {
                case T[] list:
                    foreach (var element in list)
                    {
                        index++;
                        if (Equals(element, item)) return index;
                    }
                    break;

                case List<T> list:
                    foreach (var element in list)
                    {
                        index++;
                        if (Equals(element, item)) return index;
                    }
                    break;

                case IList<T> list:
                    foreach (var element in list)
                    {
                        index++;
                        if (Equals(element, item)) return index;
                    }
                    break;

                default:
                    foreach (var element in enumerable)
                    {
                        index++;
                        if (Equals(element, item)) return index;
                    }
                    break;
            }
            return -1;
        }

        /// <summary>Индекс последнего найденного элемента</summary>
        /// <typeparam name="T">Тип искомого элемента</typeparam>
        /// <param name="enumerable">Перечисление элементов, в котором требуется найти заданный</param>
        /// <param name="item">Искомый элемент</param>
        /// <returns>Индекс последнего вхождения элемента в перечисление, либо -1 в случае, если он в ней отсутствует</returns>
        public static int LastIndexOf<T>(this IEnumerable<T?> enumerable, in T? item)
        {
            var i = 0;
            var index = -1;
            switch (enumerable)
            {
                case T[] list:
                    foreach (var element in list)
                    {
                        if (Equals(element, item)) index = i;
                        i++;
                    }
                    break;

                case List<T> list:
                    foreach (var element in list)
                    {
                        if (Equals(element, item)) index = i;
                        i++;
                    }
                    break;

                case IList<T> list:
                    foreach (var element in list)
                    {
                        if (Equals(element, item)) index = i;
                        i++;
                    }
                    break;

                default:
                    foreach (var element in enumerable)
                    {
                        if (Equals(element, item)) index = i;
                        i++;
                    }
                    break;
            }
            return index;
        }

        /// <summary>Объединение двух перечислений</summary>
        /// <param name="source">Первое перечисление элементов</param>
        /// <param name="other">Второе перечисление элементов</param>
        /// <returns>Перечисление элементов, составленное из элементов первого и второго перечисления</returns>
        public static IEnumerable Concat(this IEnumerable source, IEnumerable? other)
        {
            if (source != null) foreach (var src in source) yield return src;
            if (other != null) foreach (var oth in other) yield return oth;
        }

        /// <summary>Преобразовать объект в перечисление</summary>
        /// <param name="obj">Объект-источник элементов</param>
        /// <param name="Creator">Метод извлечения дочерних элементов объекта</param>
        /// <typeparam name="TObject">Тип объекта</typeparam>
        /// <typeparam name="TValue">Тип дочерних элементов</typeparam>
        /// <returns>Перечисление дочерних элементов</returns>
        public static IEnumerable<TValue> GetLambdaEnumerable<TObject, TValue>(
            this TObject obj,
            Func<TObject, IEnumerable<TValue>> Creator)
            => new LambdaEnumerable<TValue>(() => Creator(obj));

        /// <summary>Исключение пустых ссылок из перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <returns>Перечисление, составленное из элементов исходного перечисления, за исключением пустых ссылок</returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumerable)
            where T : class
            => enumerable.Where(i => i != null);

        /// <summary>Фильтрация последовательности строк по указанному регулярному выражению</summary>
        /// <param name="strings">Последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, удовлетворяющая регулярному выражению</returns>
        public static IEnumerable<string> Where(
            this IEnumerable<string> strings,
            Regex regex)
            => strings.Where(str => regex.IsMatch(str));

        /// <summary>Фильтрация последовательности строк, которые не удовлетворяют регулярному выражению</summary>
        /// <param name="strings">Фильтруемая последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, которые не удовлетворяют регулярному выражению</returns>
        public static IEnumerable<string> WhereNot(
            this IEnumerable<string> strings,
            Regex regex)
            => strings.WhereNot(str => regex.IsMatch(str));

        /// <summary>Фильтрация последовательности строк по указанному регулярному выражению</summary>
        /// <param name="strings">Последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, удовлетворяющая регулярному выражению</returns>
        public static IEnumerable<string> Where(
            this IEnumerable<string> strings,
            string regex)
            => strings.Where(s => Regex.IsMatch(s, regex));

        /// <summary>Фильтрация последовательности строк, которые не удовлетворяют регулярному выражению</summary>
        /// <param name="strings">Фильтруемая последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, которые не удовлетворяют регулярному выражению</returns>
        public static IEnumerable<string> WhereNot(
            this IEnumerable<string> strings,
            string regex)
            => strings.WhereNot(s => Regex.IsMatch(s, regex));

        /// <summary>Выполняет фильтрацию последовательности значений на основе заданного предиката</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, содержащий элементы входной последовательности, которые не удовлетворяют условию.
        /// </returns>
        /// <param name="enumerable">Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, подлежащий фильтрации.</param>
        /// <param name="NotSelector">Функция для проверки каждого элемента на не соответствие условию.</param>
        /// <typeparam name="T">Тип элементов последовательности <paramref name="enumerable"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="enumerable"/> или <paramref name="NotSelector"/> — null.</exception>
        public static IEnumerable<T> WhereNot<T>(
            this IEnumerable<T> enumerable,
            Func<T, bool> NotSelector)
            => enumerable.Where(t => !NotSelector(t));

        /// <summary>Возвращает цепочку элементов последовательности, удовлетворяющих указанному условию</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, содержащий элементы входной последовательности до первого элемента, который не прошел проверку.
        /// </returns>
        /// <param name="enumerable">Последовательность, из которой требуется возвратить элементы.</param>
        /// <param name="NotSelector">Функция для проверки каждого элемента на соответствие условию.</param>
        /// <typeparam name="T">Тип элементов последовательности <paramref name="enumerable"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="enumerable"/> или <paramref name="NotSelector"/> — null.</exception>
        public static IEnumerable<T> TakeWhileNot<T>(
            this IEnumerable<T> enumerable,
            Func<T, bool> NotSelector)
            => enumerable.TakeWhile(t => !NotSelector(t));

        /// <summary>Преобразование перечисления в массив с преобразованием элементов</summary>
        /// <typeparam name="T">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов результирующего массива</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий массив, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на массив
        /// </returns>
        public static TValue[] ToArray<T, TValue>(
            this IEnumerable<T> enumerable,
            Func<T, TValue> converter)
        {
            switch (enumerable)
            {
                default: return enumerable.Select(converter).ToArray();

                case T[] array:
                    {
                        var length = array.Length;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(array[i]);
                        return result;
                    }
                case List<T> list:
                    {
                        var length = list.Count;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(list[i]);
                        return result;
                    }
                case IList<T> list:
                    {
                        var length = list.Count;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(list[i]);
                        return result;
                    }
            }
        }

        /// <summary>Преобразование перечисления в массив с преобразованием элементов</summary>
        /// <typeparam name="T">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов результирующего массива</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов и индекса элемента</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий массив, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на массив
        /// </returns>
        public static TValue[] ToArray<T, TValue>(
            this IEnumerable<T> enumerable,
            Func<T, int, TValue> converter)
        {
            switch (enumerable)
            {
                default: return enumerable.Select(converter).ToArray();

                case T[] array:
                    {
                        var length = array.Length;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(array[i], i);
                        return result;
                    }
                case List<T> list:
                    {
                        var length = list.Count;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(list[i], i);
                        return result;
                    }
                case IList<T> list:
                    {
                        var length = list.Count;
                        if (length == 0) return Array.Empty<TValue>();
                        var result = new TValue[length];
                        for (var i = 0; i < length; i++)
                            result[i] = converter(list[i], i);
                        return result;
                    }
            }
        }

        /// <summary>Преобразование перечисления в список с преобразованием элементов</summary>
        /// <typeparam name="T">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов результирующего списка</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий список, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на список
        /// </returns>
        public static List<TValue> ToList<T, TValue>(
            this IEnumerable<T> enumerable,
            Func<T, TValue> converter)
        {
            switch (enumerable)
            {
                default: return enumerable.Select(converter).ToList();

                case T[] array:
                    {
                        var length = array.Length;
                        var result = new List<TValue>(length);
                        for (var i = 0; i < length; i++)
                            result.Add(converter(array[i]));
                        return result;
                    }
                case List<T> list:
                    {
                        var length = list.Count;
                        var result = new List<TValue>(length);
                        for (var i = 0; i < length; i++)
                            result.Add(converter(list[i]));
                        return result;
                    }
                case IList<T> list:
                    {
                        var length = list.Count;
                        var result = new List<TValue>(length);
                        for (var i = 0; i < length; i++)
                            result.Add(converter(list[i]));
                        return result;
                    }
            }
        }

        /// <summary>Преобразование перечисления в словарь с отбрасыванием повторяющихся значений ключей</summary>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <param name="KeySelector">Метод формирования ключа словаря</param>
        /// <param name="OverloadValues">Перезаписывать значения в словаре?</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <returns>Словарь, составленный из элементов перечисления без повторений</returns>
        public static Dictionary<TKey, T> ToDictionaryDistinctKeys<T, TKey>(
            this IEnumerable<T> enumerable,
            Func<T, TKey> KeySelector,
            bool OverloadValues = false)
        {
            var dic = new Dictionary<TKey, T>();
            switch (enumerable)
            {
                case T[] list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, item);
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = item;
                        }
                    break;

                case List<T> list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, item);
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = item;
                        }
                    break;

                case IList<T> list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, item);
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = item;
                        }
                    break;

                default:
                    if (OverloadValues)
                        foreach (var item in enumerable)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, item);
                        }
                    else
                        foreach (var item in enumerable)
                        {
                            var key = KeySelector(item);
                            dic[key] = item;
                        }
                    break;
            }
            return dic;
        }

        /// <summary>Преобразование перечисления в словарь с отбрасыванием повторяющихся значений ключей</summary>
        /// <param name="enumerable">Перечисление элементов</param>
        /// <param name="KeySelector">Метод формирования ключа словаря</param>
        /// <param name="ValueSelector">Метод вычисления значения словаря</param>
        /// <param name="OverloadValues">Перезаписывать значения в словаре?</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения записи словаря</typeparam>
        /// <returns>Словарь, составленный из элементов перечисления без повторений</returns>
        public static Dictionary<TKey, TValue> ToDictionaryDistinctKeys<T, TKey, TValue>(
            this IEnumerable<T> enumerable,
            Func<T, TKey> KeySelector,
            Func<T, TValue> ValueSelector,
            bool OverloadValues = false)
        {
            var dic = new Dictionary<TKey, TValue>();

            switch (enumerable)
            {
                case T[] list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, ValueSelector(item));
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = ValueSelector(item);
                        }
                    break;

                case List<T> list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, ValueSelector(item));
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = ValueSelector(item);
                        }
                    break;

                case IList<T> list:
                    if (OverloadValues)
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, ValueSelector(item));
                        }
                    else
                        foreach (var item in list)
                        {
                            var key = KeySelector(item);
                            dic[key] = ValueSelector(item);
                        }
                    break;

                default:
                    if (OverloadValues)
                        foreach (var item in enumerable)
                        {
                            var key = KeySelector(item);
                            if (dic.ContainsKey(key)) continue;
                            dic.Add(key, ValueSelector(item));
                        }
                    else
                        foreach (var item in enumerable)
                        {
                            var key = KeySelector(item);
                            dic[key] = ValueSelector(item);
                        }
                    break;
            }

            return dic;
        }

        /// <summary>Объединение перечисления строк в единую строку с разделителем - переносом строки</summary>
        /// <param name="Lines">Перечисление строк</param>
        /// <returns>Если ссылка на перечисление пуста, то пустая ссылка на строку, иначе - объединение строк с разделителем - переносом строки</returns>
        public static string? Aggregate(this IEnumerable<string>? Lines)
        {
            if (Lines is null) return null;

            var result = new StringBuilder();

            switch (Lines)
            {
                case string[] list:
                    foreach (var line in list)
                        result.AppendLine(line);
                    break;

                case List<string> list:
                    foreach (var line in list)
                        result.AppendLine(line);
                    break;

                case IList<string> list:
                    foreach (var line in list)
                        result.AppendLine(line);
                    break;

                default:
                    foreach (var line in Lines)
                        result.AppendLine(line);
                    break;
            }

            return result.ToString();
        }

        /// <summary>Добавить элементы перечисления в коллекцию</summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="source">Перечисление добавляемых элементов</param>
        /// <param name="collection">Коллекция-приёмник элементов</param>
        public static void AddTo<T>(this IEnumerable<T> source, ICollection<T> collection)
        {
            switch (source)
            {
                case T[] list:
                    foreach (var item in list)
                        collection.Add(item);
                    break;

                case List<T> list:
                    foreach (var item in list)
                        collection.Add(item);
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        collection.Add(item);
                    break;

                default:
                    foreach (var item in source)
                        collection.Add(item);
                    break;
            }
        }

        /// <summary>Удалить перечисление элементов из коллекции</summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="source">Перечисление удаляемых элементов</param>
        /// <param name="collection">Коллекция, из которой производится удаление</param>
        /// <returns>Перечисление результатов операций удаления для каждого из элементов исходного перечисления</returns>
        public static bool[] RemoveFrom<T>(this IEnumerable<T> source, ICollection<T> collection)
        {
            switch (source)
            {
                case T[] list:
                    {
                        var result = new bool[list.Length];
                        for (var i = 0; i < result.Length; i++)
                            result[i] = collection.Remove(list[i]);
                        return result;
                    }

                case List<T> list:
                    {
                        var result = new bool[list.Count];
                        for (var i = 0; i < result.Length; i++)
                            result[i] = collection.Remove(list[i]);
                        return result;
                    }

                case IList<T> list:
                    {
                        var result = new bool[list.Count];
                        for (var i = 0; i < result.Length; i++)
                            result[i] = collection.Remove(list[i]);
                        return result;
                    }

                default:
                    return source.Select(item => collection.Remove(item)).ToArray();
            }
        }


        /// <summary>Добавить в словарь</summary>
        /// <typeparam name="TKey">Тип ключей словаря</typeparam>
        /// <typeparam name="TValue">Тип значений словаря</typeparam>
        /// <param name="collection">Коллекция элементов, добавляемых в словарь</param>
        /// <param name="dictionary">Словарь, в который добавляются элементы</param>
        /// <param name="converter">Метод определения ключа элемента</param>
        public static void AddToDictionary<TKey, TValue>
        (
            this IEnumerable<TValue>? collection,
            IDictionary<TKey, TValue> dictionary,
            Func<TValue, TKey> converter
        )
        {
            if (collection is null) return;
            switch (collection)
            {
                case TValue[] list:
                    foreach (var value in list)
                        dictionary.Add(converter(value), value);
                    break;

                case List<TValue> list:
                    foreach (var value in list)
                        dictionary.Add(converter(value), value);
                    break;

                case IList<TValue> list:
                    foreach (var value in list)
                        dictionary.Add(converter(value), value);
                    break;

                default:
                    foreach (var value in collection)
                        dictionary.Add(converter(value), value);
                    break;
            }
        }

        /// <summary>Преобразовать последовательность одних элементов в последовательность других элементов с использованием механизма конвертации</summary>
        /// <typeparam name="TItem">Тип исходных элементов</typeparam>
        /// <typeparam name="TValue">Тип элементов, в которые преобразуются исходные</typeparam>
        /// <param name="collection">Последовательность исходных элементов</param>
        /// <returns>Последовательность элементов преобразованного типа</returns>
        public static IEnumerable<TValue>? ConvertToType<TItem, TValue>(this IEnumerable<TItem>? collection)
        {
            var target_type = typeof(TValue);
            var source_type = typeof(TItem);

            if (source_type == target_type) return (IEnumerable<TValue>)collection!;
            if (collection is null) return null;

            Func<object, object>? type_converter = null;
            var converter = source_type == typeof(object)
                        ? o => (TValue)target_type.Cast(o)
                        : (Func<TItem, TValue>)(o => (TValue)(type_converter ??= target_type.GetCasterFrom(source_type))(o));
            return collection.Select(converter);
        }

        /// <summary>Создать последовательность элементов, каждое значение в которой будет получено на основе двух значений исходной последовательности</summary>
        /// <typeparam name="TItem">Тип элементов исходной последовательности</typeparam>
        /// <typeparam name="TValue">Тип элементов последовательности конвертированных элементов</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="converter">
        /// Метод преобразования, в который передаётся исходный элемент последовательности, предыдущий элемент последовательности, 
        /// и на основе двух этих элементов, он определяет значение элемента результирующей последовательности</param>
        /// <returns>Последовательность элементов, составляемая из преобразованных элементов исходной последовательности, где метод преобразования учитывает значение предшествующего элемента</returns>
        public static IEnumerable<TValue> SelectWithLastValue<TItem, TValue>
        (
            this IEnumerable<TItem>? collection,
            Func<TItem?, TItem?, TValue> converter
        )
        {
            if (collection is null) yield break;
            TItem? last;
            switch (collection)
            {
                case TItem[] { Length: > 1 and var length } list:
                    last = list[0];
                    for (var i = 1; i < length; i++)
                        yield return converter(last, last = list[i]);
                    break;

                case List<TItem> { Count: > 1 and var count } list:
                    last = list[0];
                    for (var i = 1; i < count; i++)
                        yield return converter(last, last = list[i]);
                    break;

                case IList<TItem> { Count: > 1 and var count } list:
                    last = list[0];
                    for (var i = 1; i < count; i++)
                        yield return converter(last, last = list[i]);
                    break;

                default:
                    var first = true;
                    last = default;
                    foreach (var item in collection)
                    {
                        if (first)
                        {
                            last = item;
                            first = false;
                            continue;
                        }
                        yield return converter(last, item);
                        last = item;
                    }
                    break;
            }
        }

        /// <summary>Выполнить действие для первого элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, выполняемое для первого элемента последовательности в момент её перечисления</param>
        /// <returns>Исходная последовательность элементов</returns>
        public static IEnumerable<T> AtFirst<T>(this IEnumerable<T>? collection, Action<T> action)
        {
            switch (collection)
            {
                case null: yield break;

                case T[] { Length: > 0 } list:
                    action(list[0]);
                    foreach (var item in list)
                        yield return item;
                    break;

                case List<T> { Count: > 0 } list:
                    action(list[0]);
                    foreach (var item in list)
                        yield return item;
                    break;

                case IList<T> { Count: > 0 } list:
                    action(list[0]);
                    foreach (var item in list)
                        yield return item;
                    break;

                default:
                    var first = true;
                    foreach (var item in collection)
                    {
                        if (first)
                        {
                            action(item);
                            first = false;
                        }
                        yield return item;
                    }

                    break;
            }
        }

        /// <summary>Выполнить действие для последнего элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, выполняемое для последнего элемента в момент её перечисления. Если последовательность элементов просмотрена до конца</param>
        /// <returns>Исходная последовательность элементов</returns>
        public static IEnumerable<T> AtLast<T>(this IEnumerable<T>? collection, Action<T?> action)
        {
            switch (collection)
            {
                case null: yield break;

                case T[] { Length: > 0 } list:
                    foreach (var item in list)
                        yield return item;
                    action(list[^1]);
                    break;

                case List<T> { Count: > 0 } list:
                    foreach (var item in list)
                        yield return item;
                    action(list[^1]);
                    break;

                case IList<T> { Count: > 0 } list:
                    foreach (var item in list)
                        yield return item;
                    action(list[^1]);
                    break;

                default:
                    var last = default(T);
                    var any = false;
                    foreach (var item in collection)
                    {
                        any = true;
                        last = item;
                        yield return last;
                    }
                    if (any) action(last);
                    break;
            }
        }

        /// <summary> Выполнить действие до начала перечисления последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="ActionBefore">Действие, выполняемое до начала перечисления элементов последовательности</param>
        /// <returns>Исходная последовательность элементов</returns>
        public static IEnumerable<T> Before<T>(this IEnumerable<T>? collection, Action ActionBefore)
        {
            if (collection is null) yield break;
            ActionBefore();
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list) yield return item;
                    break;

                case List<T> list:
                    foreach (var item in list) yield return item;
                    break;

                case IList<T> list:
                    foreach (var item in list) yield return item;
                    break;

                default:
                    foreach (var item in collection) yield return item;
                    break;
            }
        }

        /// <summary>Выполнение действия по завершению перечисления коллекции</summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="CompleteAction">Действие, выполняемое по завершению перечисления коллекции</param>
        /// <returns>Исходная последовательность элементов</returns>
        public static IEnumerable<T> OnComplete<T>(this IEnumerable<T>? collection, Action CompleteAction)
        {
            switch (collection)
            {
                case null: yield break;

                case T[] list:
                    foreach (var item in list) yield return item;
                    break;

                case List<T> list:
                    foreach (var item in list) yield return item;
                    break;

                case IList<T> list:
                    foreach (var item in list) yield return item;
                    break;

                default:
                    foreach (var item in collection) yield return item;
                    break;
            }
            CompleteAction();
        }

        /// <summary>История перечисления последовательности элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        public sealed class EnumerableHistory<T> : IEnumerable<T>, IObservable<T>
        {
            /// <summary>Длина истории</summary>
            private int _HistoryLength;

            /// <summary>Список элементов в истории</summary>
            private readonly List<T> _Queue;

            /// <summary>Объект-наблюдения за историей</summary>
            private readonly SimpleObservableEx<T> _ObservableObject = new();

            /// <summary>Текущий элемент перечисления</summary>
            public T Current { get; private set; } = default!;

            /// <summary>Длина истории</summary>
            [MinValue(0)]
            public int Length { get => _HistoryLength; set { _HistoryLength = value; Check(); } }

            /// <summary>Количество элементов в истории</summary>
            [MinValue(0)]
            public int Count => _Queue.Count;

            /// <summary>Доступ к элементам истории начиная с текущего</summary>
            /// <param name="i">Индекс элемента в истории, где 0 - текущий элемент</param>
            /// <returns>Элемент истории перечисления</returns>
            public T this[[MinValue(0)] int i] => _Queue[^i];

            /// <summary>Инициализация нового экземпляра <see cref="EnumerableHistory{T}"/></summary>
            /// <param name="HistoryLength">Длина истории</param>
            public EnumerableHistory([MinValue(0)] int HistoryLength)
            {
                _HistoryLength = HistoryLength;
                _Queue = new List<T>(HistoryLength);
            }

            /// <summary>Удаление лишних элементов из истории</summary>
            private void Check()
            {
                while (_Queue.Count > _HistoryLength) _Queue.RemoveAt(0);
            }

            /// <summary>Добавить элемент в историю перечисления</summary>
            /// <param name="item">Добавляемый элемент</param>
            public EnumerableHistory<T> Add(T item)
            {
                _Queue.Add(item);
                Current = item;
                _ObservableObject.OnNext(item);
                Check();
                return this;
            }

            /// <summary>Получить перечислитель истории элементов</summary><returns>Перечислитель истории элементов</returns>
            public IEnumerator<T> GetEnumerator() => _Queue.GetEnumerator();

            /// <summary>Получить перечислитель истории элементов</summary><returns>Перечислитель истории элементов</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <summary>Подписка на изменения истории перечисления</summary>
            /// <param name="observer">Объект-подписчик, уведомляемый об изменениях в истории перечисления</param>
            /// <returns>Объект, осуществляющий возможность отписаться от уведомлений изменения истории перечисления</returns>
            public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);
        }

        /// <summary>Преобразование исходной последовательности элементов с учётом указанного размера истории перечисления</summary>
        /// <typeparam name="TIn">Тип элементов исходной коллекции</typeparam>
        /// <typeparam name="TOut">Тип элементов результирующей коллекции</typeparam>
        /// <param name="collection">Исходная коллекция элементов</param>
        /// <param name="Selector">Метод преобразования элементов коллекции на основе истории их перечисления</param>
        /// <param name="HistoryLength">Максимальная длина истории перечисления</param>
        /// <returns>Коллекция элементов, сформированная на основе исходной с учётом истории процесса перечисления исходной коллекции</returns>
        public static IEnumerable<TOut> SelectWithHistory<TIn, TOut>
        (
            this IEnumerable<TIn> collection,
            Func<EnumerableHistory<TIn>, TOut> Selector,
            [MinValue(0)] int HistoryLength
        )
        {
            var history = new EnumerableHistory<TIn>(HistoryLength);
            return collection.Select(item => Selector(history.Add(item)));
        }

        /// <summary>Оценка статистических параметров перечисления</summary>
        /// <param name="collection">Перечисление значений, статистику которых требуется получить</param>
        /// <param name="Length">Размер выборки для оценки</param>
        /// <returns>Оценка статистики</returns>
        public static StatisticValue GetStatistic(this IEnumerable<double> collection, [MinValue(0)] int Length = 0)
        {
            if (Length > 0)
                return new StatisticValue(Length)
                          .InitializeObject(collection, (sv, items) => sv!.AddEnumerable(items))
                       ?? throw new InvalidOperationException();
            var values = collection.ToArray();
            var result = new StatisticValue(values.Length);
            result.AddEnumerable(values);
            return result;
        }

        /// <summary>Отбросить нулевые значения с конца перечисления</summary>
        /// <param name="collection">Фильтруемое перечисление</param>
        /// <returns>Перечисление чисел, в котором отсутствуют хвостовые нули</returns>
        public static IEnumerable<double> FilterNullValuesFromEnd(this IEnumerable<double> collection)
        {
            var n = 0;
            foreach (var value in collection)
                if (value == 0) n++;
                else
                {
                    for (; n > 0; n--) yield return 0;
                    yield return value;
                }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="selector">Метод преобразования объектов в вещественные числа</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        public static void GetMinMax<T>
        (
            this IEnumerable<T> collection,
            Func<T, double> selector,
            out T? Min,
            out T? Max
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = default;
            Max = default;
            switch (collection)
            {
                case T[] { Length: 0 }: break;
                case T[] list:
                    foreach (var value in list)
                    {
                        var f = selector(value);
                        if (min.AddValue(f)) Min = value;
                        if (max.AddValue(f)) Max = value;
                    }
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> list:
                    foreach (var value in list)
                    {
                        var f = selector(value);
                        if (min.AddValue(f)) Min = value;
                        if (max.AddValue(f)) Max = value;
                    }
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> list:
                    foreach (var value in list)
                    {
                        var f = selector(value);
                        if (min.AddValue(f)) Min = value;
                        if (max.AddValue(f)) Max = value;
                    }
                    break;

                default:
                    foreach (var value in collection)
                    {
                        var f = selector(value);
                        if (min.AddValue(f)) Min = value;
                        if (max.AddValue(f)) Max = value;
                    }
                    break;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        public static void GetMinMax
        (
            this IEnumerable<double> collection,
            out double Min,
            out double Max
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = double.NaN;
            Max = double.NaN;
            switch (collection)
            {
                case double[] { Length: 0 }: break;
                case double[] list:
                    foreach (var value in list)
                    {
                        if (min.AddValue(value)) Min = value;
                        if (max.AddValue(value)) Max = value;
                    }
                    break;

                case List<double> { Count: 0 }: break;
                case List<double> list:
                    foreach (var value in list)
                    {
                        if (min.AddValue(value)) Min = value;
                        if (max.AddValue(value)) Max = value;
                    }
                    break;

                case IList<double> { Count: 0 }: break;
                case IList<double> list:
                    foreach (var value in list)
                    {
                        if (min.AddValue(value)) Min = value;
                        if (max.AddValue(value)) Max = value;
                    }
                    break;

                default:
                    foreach (var value in collection)
                    {
                        if (min.AddValue(value)) Min = value;
                        if (max.AddValue(value)) Max = value;
                    }
                    break;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="selector">Метод преобразования объектов в вещественные числа</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="MinIndex">Индекс минимального элемента в коллекции</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        /// <param name="MaxIndex">Индекс максимального элемента в коллекции</param>
        public static void GetMinMax<T>
        (
            this IEnumerable<T> collection,
            Func<T, double> selector,
            out T? Min,
            out int MinIndex,
            out T? Max,
            out int MaxIndex
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = default;
            Max = default;
            MinIndex = -1;
            MaxIndex = -1;
            switch (collection)
            {
                case T[] { Length: 0 }: break;
                case T[] { Length: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            var f = selector(item);
                            if (min.AddValue(f))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(f))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> { Count: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            var f = selector(item);
                            if (min.AddValue(f))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(f))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> { Count: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            var f = selector(item);
                            if (min.AddValue(f))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(f))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                default:
                    {
                        var i = 0;
                        foreach (var item in collection)
                        {
                            var f = selector(item);
                            if (min.AddValue(f))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(f))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                            i++;
                        }
                    }
                    break;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="MinIndex">Индекс минимального элемента в коллекции</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        /// <param name="MaxIndex">Индекс максимального элемента в коллекции</param>
        public static void GetMinMax
        (
            this IEnumerable<double> collection,
            out double Min,
            out int MinIndex,
            out double Max,
            out int MaxIndex
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = double.NaN;
            Max = double.NaN;
            MinIndex = -1;
            MaxIndex = -1;
            switch (collection)
            {
                case double[] { Length: 0 }: break;
                case double[] { Length: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            if (min.AddValue(item))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(item))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                case List<double> { Count: 0 }: break;
                case List<double> { Count: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            if (min.AddValue(item))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(item))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                case IList<double> { Count: 0 }: break;
                case IList<double> { Count: var count } list:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var item = list[i];
                            if (min.AddValue(item))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(item))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                        }
                    }
                    break;

                default:
                    {
                        var i = 0;
                        foreach (var item in collection)
                        {
                            if (min.AddValue(item))
                            {
                                Min = item;
                                MinIndex = i;
                            }
                            if (max.AddValue(item))
                            {
                                Max = item;
                                MaxIndex = i;
                            }
                            i++;
                        }
                    }
                    break;
            }
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static T? GetMax<T>(this IEnumerable<T> collection, Func<T, double> selector)
        {
            var max = new MaxValue();
            var result = default(T);
            switch (collection)
            {
                case T[] list:
                    foreach (var v in list)
                        if (max.AddValue(selector(v)))
                            result = v;
                    break;

                case List<T> list:
                    foreach (var v in list)
                        if (max.AddValue(selector(v)))
                            result = v;
                    break;

                case IList<T> list:
                    foreach (var v in list)
                        if (max.AddValue(selector(v)))
                            result = v;
                    break;

                default:
                    foreach (var v in collection)
                        if (max.AddValue(selector(v)))
                            result = v;
                    break;
            }
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static double GetMax(this IEnumerable<double> collection)
        {
            var max = new MaxValue();
            var result = double.NaN;
            switch (collection)
            {
                case double[] list:
                    foreach (var v in list)
                        result = max.Add(v);
                    break;

                case List<double> list:
                    foreach (var v in list)
                        result = max.Add(v);
                    break;

                case IList<double> list:
                    foreach (var v in list)
                        result = max.Add(v);
                    break;

                default:
                    foreach (var v in collection)
                        result = max.Add(v);
                    break;
            }
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <param name="index">Индекс максимального элемента в последовательности</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static T? GetMax<T>(this IEnumerable<T> collection, Func<T, double> selector, out int index)
        {
            var max = new MaxValue();
            var result = default(T);
            var i = 0;
            index = -1;
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case List<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case IList<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                default:
                    foreach (var item in collection)
                    {
                        if (max.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                        i++;
                    }
                    break;
            }
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="index">Индекс максимального элемента в последовательности</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static double GetMax(this IEnumerable<double> collection, out int index)
        {
            var max = new MaxValue();
            var result = double.NaN;
            var i = 0;
            index = -1;
            switch (collection)
            {
                case double[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case List<double> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case IList<double> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (max.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                default:
                    foreach (var item in collection)
                    {
                        if (max.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                        i++;
                    }

                    break;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static T? GetMin<T>(this IEnumerable<T> collection, Func<T, double> selector)
        {
            var min = new MinValue();
            var result = default(T);
            switch (collection)
            {
                case T[] list:
                    foreach (var v in list)
                        if (min.AddValue(selector(v)))
                            result = v;
                    break;
                case List<T> list:
                    foreach (var v in list)
                        if (min.AddValue(selector(v)))
                            result = v;
                    break;

                case IList<T> list:
                    foreach (var v in list)
                        if (min.AddValue(selector(v)))
                            result = v;
                    break;

                default:
                    foreach (var v in collection)
                        if (min.AddValue(selector(v)))
                            result = v;
                    break;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static double GetMin(this IEnumerable<double> collection)
        {
            var min = new MinValue();
            var result = double.NaN;
            switch (collection)
            {
                case double[] list:
                    foreach (var v in list)
                        result = min.Add(v);
                    break;

                case List<double> list:
                    foreach (var v in list)
                        result = min.Add(v);
                    break;

                case IList<double> list:
                    foreach (var v in list)
                        result = min.Add(v);
                    break;

                default:
                    foreach (var v in collection)
                        result = min.Add(v);
                    break;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <param name="index">Индекс минимального элемента в последовательности</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static T? GetMin<T>(this IEnumerable<T> collection, Func<T, double> selector, out int index)
        {
            var min = new MinValue();
            var result = default(T);
            var i = 0;
            index = -1;
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case List<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case IList<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                default:
                    foreach (var item in collection)
                    {
                        if (min.AddValue(selector(item)))
                        {
                            index = i;
                            result = item;
                        }
                        i++;
                    }
                    break;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="index">Индекс минимального элемента в последовательности</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static double GetMin(this IEnumerable<double> collection, out int index)
        {
            var min = new MinValue();
            var result = double.NaN;
            var i = 0;
            index = -1;
            switch (collection)
            {
                case double[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case List<double> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                case IList<double> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        var item = list[i];
                        if (min.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                    }
                    break;

                default:
                    foreach (var item in collection)
                    {
                        if (min.AddValue(item))
                        {
                            index = i;
                            result = item;
                        }
                        i++;
                    }
                    break;
            }
            return result;
        }

        /// <summary>Быстрое преобразование последовательности в список</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Enum">Последовательность, преобразуемая в список</param>
        /// <returns>Список элементов последовательности</returns>
        public static IList<T> ToListFast<T>(this IEnumerable<T> Enum) => Enum as IList<T> ?? Enum.ToList();

        /// <summary>Сумма последовательности комплексных чисел</summary>
        /// <param name="collection">Последовательность комплексных чисел</param>
        /// <returns>Комплексное число, являющееся суммой последовательности комплексных чисел</returns>
        [DST]
        public static Complex Sum(this IEnumerable<Complex> collection) => collection.Aggregate((Z, z) => Z + z);

        /// <summary>Комплексная сумма последовательности элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод формирования комплексного значения на основе очередного элемента последовательности</param>
        /// <returns>Комплексное число, являющееся суммой последовательности комплексных чисел</returns>
        [DST]
        public static Complex Sum<T>(this IEnumerable<T> collection, Func<T, Complex> selector) => collection.Select(selector).Aggregate((Z, z) => Z + z);

        /// <summary>Объединить элементы коллекции</summary>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        /// <param name="collection">Исходная коллекция элементов</param>
        /// <param name="Init">Исходное состояние результата объединения</param>
        /// <param name="func">Метод объединения</param>
        /// <param name="index">Индекс элемента коллекции</param>
        /// <returns>Результат объединения коллекции элементов</returns>
        [DST]
        public static TResult Aggregate<T, TResult>
        (
            this IEnumerable<T> collection,
            TResult Init,
            Func<TResult, T, int, TResult> func,
            int index = 0) =>
            collection.Aggregate(Init, (last, e) => func(last, e, index++));

        /// <summary>Проверка на наличие элемента в коллекции</summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="collection">Проверяемая коллекция</param>
        /// <param name="selector">Метод выбора</param>
        /// <returns>Истина, если выполняется предикат хотя бы на одном элементе коллекции</returns>
        [DST]
        public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> selector)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        if (selector(item))
                            return true;
                    return false;

                case List<T> list:
                    foreach (var item in list)
                        if (selector(item))
                            return true;
                    return false;

                case IList<T> list:
                    foreach (var item in list)
                        if (selector(item))
                            return true;
                    return false;

                default: return collection.Any(selector);
            }
        }

        /// <summary>Найти элемент в перечислении, удовлетворяющий предикату</summary>
        /// <param name="collection">Перечисление элементов</param>
        /// <param name="selector">Предикат выбора</param>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <returns>Найденный элемент, либо пустая ссылка</returns>
        [DST]
        public static T? Find<T>(this IEnumerable<T> collection, Predicate<T> selector)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        if (selector(item))
                            return item;
                    return default;

                case List<T> list:
                    foreach (var item in list)
                        if (selector(item))
                            return item;
                    return default;

                case IList<T> list:
                    foreach (var item in list)
                        if (selector(item))
                            return item;
                    return default;

                default:
                    foreach (var item in collection)
                        if (selector(item))
                            return item;
                    return default;
            }
        }

        ///<summary>Выполнение действия для всех элементов коллекции</summary>
        ///<param name="collection">Коллекция элементов</param>
        ///<param name="Action">Выполняемое действие</param>
        ///<typeparam name="T">Тип элементов коллекции</typeparam>
        [DST]
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> Action)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        Action(item);
                    break;

                case List<T> list:
                    foreach (var item in list)
                        Action(item);
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        Action(item);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p">Параметр действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP">Тип параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP>(
            this IEnumerable<T> collection,
            TP? p,
            Action<T, TP?> Action)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        Action(item, p);
                    break;

                case List<T> list:
                    foreach (var item in list)
                        Action(item, p);
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        Action(item, p);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, p);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр 1 действия</param>
        /// <param name="p2">Параметр 2 действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 процесса перебора</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2>(
            this IEnumerable<T> collection,
            TP1? p1,
            TP2? p2,
            Action<T, TP1?, TP2?> Action)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        Action(item, p1, p2);
                    break;

                case List<T> list:
                    foreach (var item in list)
                        Action(item, p1, p2);
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        Action(item, p1, p2);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, p1, p2);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр 1 действия</param>
        /// <param name="p2">Параметр 2 действия</param>
        /// <param name="p3">Параметр 3 действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра процесса перебора</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра процесса перебора</typeparam>
        /// <typeparam name="TP3">Тип 3 параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2, TP3>(
            this IEnumerable<T> collection,
            TP1? p1,
            TP2? p2,
            TP3? p3,
            Action<T, TP1?, TP2?, TP3?> Action)
        {
            switch (collection)
            {
                case T[] list:
                    foreach (var item in list)
                        Action(item, p1, p2, p3);
                    break;

                case List<T> list:
                    foreach (var item in list)
                        Action(item, p1, p2, p3);
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        Action(item, p1, p2, p3);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, p1, p2, p3);
                    break;
            }
        }

        ///<summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        ///<param name="collection">Коллекция элементов</param>
        ///<param name="Action">Действие над элементом</param>
        ///<param name="index">Смещение индекса элемента коллекции</param>
        ///<typeparam name="T">Тип элемента коллекции</typeparam>
        [DST]
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T, int> Action, int index = 0)
        {
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index);
                    break;

                case List<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index);
                    break;

                case IList<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, index++);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p">Параметр действия</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP">Тип параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP>(
            this IEnumerable<T> collection,
            TP? p,
            Action<T, int, TP?> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p);
                    break;

                case List<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p);
                    break;

                case IList<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, index++, p);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр действия 1</param>
        /// <param name="p2">Параметр действия 2</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра процесса перебора 1</typeparam>
        /// <typeparam name="TP2">Тип параметра процесса перебора 2</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2>(
            this IEnumerable<T> collection,
            TP1? p1,
            TP2? p2,
            Action<T, int, TP1?, TP2?> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2);
                    break;

                case List<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2);
                    break;

                case IList<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, index++, p1, p2);
                    break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр действия 1</param>
        /// <param name="p2">Параметр действия 2</param>
        /// <param name="p3">Параметр действия 3</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра процесса перебора 1</typeparam>
        /// <typeparam name="TP2">Тип параметра процесса перебора 2</typeparam>
        /// <typeparam name="TP3">Тип параметра процесса перебора 3</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2, TP3>(
            this IEnumerable<T> collection,
            TP1? p1,
            TP2? p2,
            TP3? p3,
            Action<T, int, TP1?, TP2?, TP3?> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2, p3);
                    break;

                case List<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2, p3);
                    break;

                case IList<T> { Count: var count } list:
                    for (var i = 0; i < count; i++)
                        Action(list[i], i + index, p1, p2, p3);
                    break;

                default:
                    foreach (var item in collection)
                        Action(item, index++, p1, p2, p3);
                    break;
            }
        }

        /// <summary>Ленивое преобразование типов, пропускающее непреобразуемые объекты</summary>
        /// <param name="collection">Исходное перечисление объектов</param>
        /// <typeparam name="T">Тип объектов входного перечисления</typeparam>
        /// <returns>Коллекция объектов преобразованного типа</returns>
        [DST]
        public static IEnumerable<T> CastLazy<T>(IEnumerable collection)
        {
            var result = collection as IEnumerable<T>;
            return result ?? collection.Cast<object>().OfType<T>();
        }

        /// <summary>Ленивое преобразование типов элементов перечисления</summary>
        /// <typeparam name="TItem">Тип элементов входной перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов перечисления, в который требуется осуществить преобразование</typeparam>
        /// <param name="collection">Исходная перечисление элементов</param>
        /// <returns>Перечисление элементов преобразованного типа</returns>
        public static IEnumerable<TValue> CastLazy<TItem, TValue>(IEnumerable<TItem> collection) => typeof(TItem) == typeof(TValue) ? (IEnumerable<TValue>)collection : collection.OfType<TValue>();

        /// <summary>Действие, выполняемое в процессе перебора элементов для всех элементов перечисления при условии выполнения предиката</summary>
        /// <typeparam name="T">Ип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление элементов, для которых надо выполнить действие</param>
        /// <param name="Predicate">Условие выполнения действия</param>
        /// <param name="Action">Действие, выполняемое для всех элементов перечисления</param>
        /// <returns>Исходное перечисление</returns>
        public static IEnumerable<T> ForeachLazyIf<T>
        (
            this IEnumerable<T> collection,
            Func<T, bool> Predicate,
            Action<T>? Action
        ) =>
            Action is null ? collection : collection.Select(t =>
            {
                if (Predicate(t)) Action(t);
                return t;
            });

        /// <summary>Отложенное выполнение указанного действия для каждого элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <returns>Последовательность элементов, для элементов которой выполняется отложенное действие</returns>
        [DST]
        public static IEnumerable<T> ForeachLazy<T>(this IEnumerable<T> collection, Action<T>? Action) =>
            Action is null ? collection : collection.Select(t =>
            {
                Action(t);
                return t;
            });

        /// <summary>Отложенное выполнение указанного действия для каждого элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <returns>Последовательность элементов, для элементов которой выполняется отложенное действие</returns>
        [DST]
        public static IEnumerable<T> ForeachLazy<T, T1>(this IEnumerable<T> collection, Func<T, T1>? Action) =>
            Action is null ? collection : collection.Select(t =>
            {
                Action(t);
                return t;
            });

        /// <summary>Выполнение указанного действия на каждом шаге перебора последовательности после выдачи элемента</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, выполняемое после выдачи элемента последовательности</param>
        /// <returns>Исходная последовательность элементов</returns>
        public static IEnumerable<T> ForeachLazyLast<T>(this IEnumerable<T> collection, Action<T>? action)
        {
            if (action is null)
                foreach (var value in collection) yield return value;
            else
                foreach (var value in collection)
                {
                    yield return value;
                    action(value);
                }
        }

        /// <summary>Отложенное выполнение действия до перебора элементов последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <param name="index">Начальный индекс элемента последовательности</param>
        /// <returns>Последовательность элементов, для элементов которой которой выполняется действие</returns>
        [DST]
        public static IEnumerable<T> ForeachLazy<T>
        (
            this IEnumerable<T> collection,
            Action<T, int>? Action,
            int index = 0
        ) =>
            Action is null ? collection : collection.Select(t =>
            {
                Action(t, index++);
                return t;
            });

        /// <summary>Пересечение последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Source">Исходная последовательность элементов</param>
        /// <param name="Items">Последовательность элементов, пересечение с которой вычисляется</param>
        /// <returns>Последовательность элементов, входящих как в первую, так и во вторую последовательности</returns>
        public static IEnumerable<T> ExistingItems<T>(this IEnumerable<T> Source, IEnumerable<T> Items)
        {
            var list = Items.ToListFast();
            return Source.Where(t => list.Contains(i => Equals(i, t)));
        }

        /// <summary>Последовательность уникальных элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="values">Исходная последовательность элементов</param>
        /// <param name="Comparer">Метод сравнения элементов</param>
        /// <param name="Hasher">Функция вычисления хеш-кода элемента</param>
        /// <returns>Последовательность элементов, таких, что ранее они отсутствовали во входной последовательности</returns>
        public static IEnumerable<T> GetUnique<T>(this IEnumerable<T> values, Func<T, T, bool> Comparer, Func<T, int>? Hasher = null)
        {
            var hash = new HashSet<T>(new LambdaEqualityComparer<T>(Comparer, Hasher ?? (item => item.GetHashCode())));

            foreach (var value in values.Where(value => hash.Add(value)))
                yield return value;
        }

        /// <summary>Последовательность уникальных элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="values">Исходная последовательность элементов</param>
        /// <returns>Последовательность элементов, таких, что ранее они отсутствовали во входной последовательности</returns>
        public static IEnumerable<T> GetUnique<T>(this IEnumerable<T> values)
        {
            var set = new HashSet<T>();
            return values.Where(v => set.Add(v));
        }

        /// <summary>Найти элементы, которые не входят во вторую последовательность</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Source">Исходная последовательность</param>
        /// <param name="Items">Последовательность элементов, которых не должно быть в выходной последовательности</param>
        /// <returns>Последовательность элементов, которые отсутствуют во второй последовательности</returns>
        public static IEnumerable<T> MissingItems<T>(this IEnumerable<T> Source, IEnumerable<T> Items)
        {
            var set = Items.GetHashSet();
            return Source.Where(t => !set.Contains(t));
        }

        /// <summary>Пересечение последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Первая последовательность</param>
        /// <param name="B">Вторая последовательность</param>
        /// <returns>Массив элементов, входящих и в первую и во вторую последовательности</returns>
        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            var b = B.GetHashSet();
            return A.Where(a => b.Contains(a));
        }

        /// <summary>Последовательности элементов поэлементно равны</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Первая последовательность</param>
        /// <param name="B">Вторая последовательность</param>
        /// <returns>Истина, если последовательности равны с точностью до элементов</returns>
        public static bool ItemEquals<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            using (var a = A.GetEnumerator())
            using (var b = B.GetEnumerator())
            {
                var next_a = a.MoveNext();
                var next_b = b.MoveNext();
                while (next_a && next_b)
                {
                    if (a.Current is null && b.Current != null) return false;
                    if (a.Current is null || !a.Current.Equals(b.Current)) return false;
                    next_a = a.MoveNext();
                    next_b = b.MoveNext();
                }
                return next_a == next_b;
            }
        }

        /// <summary>Определение объектов, которые не входят в пересечение двух последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Исходная последовательность</param>
        /// <param name="B">Вторичная последовательность</param>
        /// <returns>Массив элементов, входящих либо в первую, либо во вторую последовательность</returns>
        public static IEnumerable<T> NotIntersection<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            var b_list = B.ToListFast();
            var b = b_list.GetHashSet();
            var a = new HashSet<T>();

            foreach (var a_item in A)
            {
                a.Add(a_item);
                if (!b.Contains(a_item))
                    yield return a_item;
            }

            foreach (var b_item in b_list)
                if (!a.Contains(b_item))
                    yield return b_item;
        }

        /// <summary>Нахождение пересечения элементов двух последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Исходная последовательность</param>
        /// <param name="B">Вторичная последовательность</param>
        /// <param name="MissingInAFromB">Массив элементов, отсутствующих в первой последовательности</param>
        /// <param name="MissingInBFromA">Массив элементов, отсутствующих во второй последовательности</param>
        /// <param name="ExistingInAFromB">Массив элементов, присутствующих в первой последовательности</param>
        /// <param name="ExistingInBFromA">Массив элементов, присутствующих во второй последовательности</param>
        /// <param name="Intersection">Пересечение элементов</param>
        /// <param name="NotIntersection">Остаток от пересечения элементов</param>
        public static void Xor<T>
        (
            this IEnumerable<T> A,
            IEnumerable<T> B,
            out T[] MissingInAFromB,
            out T[] MissingInBFromA,
            out T[] ExistingInAFromB,
            out T[] ExistingInBFromA,
            out T[] Intersection,
            out T[] NotIntersection
        )
        {
            var a = A.ToListFast();
            var b = B.ToListFast();

            var missing_in_a_from_b_list = new List<T>(a.Count + b.Count);
            var missing_in_b_from_a_list = new List<T>(a.Count + b.Count);
            var existing_in_a_from_b_list = new List<T>(a.Count + b.Count);
            var existing_in_b_from_a_list = new List<T>(a.Count + b.Count);
            var intersection_list = new List<T>(a.Count + b.Count);
            var not_intersection_list = new List<T>(a.Count + b.Count);

            var b_existing_in_a = new bool[b.Count];
            foreach (var a_item in a)
            {
                var a_existing_in_b = false;

                for (var j = 0; j < b.Count; j++)
                {
                    var b_item = b[j];
                    if (!Equals(a_item, b_item)) continue;

                    a_existing_in_b = b_existing_in_a[j] = true;
                    break;
                }

                if (a_existing_in_b)
                {
                    existing_in_a_from_b_list.Add(a_item);
                    not_intersection_list.Add(a_item);
                }
                else
                {
                    missing_in_a_from_b_list.Add(a_item);
                    intersection_list.Add(a_item);
                }
            }

            for (var i = 0; i < b.Count; i++)
            {
                var b_item = b[i];
                if (b_existing_in_a[i])
                {
                    existing_in_b_from_a_list.Add(b_item);
                    not_intersection_list.Add(b_item);
                }
                else
                {
                    missing_in_b_from_a_list.Add(b_item);
                    intersection_list.Add(b_item);
                }
            }

            ExistingInAFromB = existing_in_a_from_b_list.ToArray();
            ExistingInBFromA = existing_in_b_from_a_list.ToArray();
            MissingInAFromB = missing_in_a_from_b_list.ToArray();
            MissingInBFromA = missing_in_b_from_a_list.ToArray();
            Intersection = intersection_list.ToArray();
            NotIntersection = not_intersection_list.ToArray();
        }

        /// <summary>Преобразовать последовательность в строку с указанной строкой-разделителем</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность объектов, преобразуемая в строку с разделителями</param>
        /// <param name="Separator">Разделитель элементов в строке</param>
        /// <returns>Строка, составленная из строковых представлений объектов последовательности, разделённых указанной строкой-разделителем</returns>
        // ReSharper disable once EmptyString
        public static string ToSeparatedStr<T>(this IEnumerable<T> collection, string? Separator = "") =>
            string.Join(Separator, collection);

        /// <summary>Найти минимум и максимум последовательности вещественных чисел</summary>
        /// <param name="values">Последовательность вещественных чисел</param>
        /// <returns>Интервал, границы которого определяют минимум и максимум значений, которые принимала входная последовательность</returns>
        public static Interval GetMinMax(this IEnumerable<double> values) => new MinMaxValue(values).Interval;

        /// <summary>Добавить элемент в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="obj">Добавляемый объект</param>
        /// <returns>Последовательность, составленная из элементов исходной последовательности и добавленного элемента</returns>
        public static IEnumerable<T> AppendLast<T>(this IEnumerable<T>? collection, T? obj)
        {
            switch (collection)
            {
                case null: break;

                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in collection)
                        yield return value;
                    break;
            }

            if (obj != null)
                yield return obj;
        }

        /// <summary>Добавить последовательность элементов в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="first_collection">Начальная последовательность элементов</param>
        /// <param name="last_collection">Завершающая последовательность элементов</param>
        /// <returns>Последовательность элементов, составленная из элементов первичной и вторичной последовательностей</returns>
        public static IEnumerable<T> AppendLast<T>
        (
            this IEnumerable<T>? first_collection,
            IEnumerable<T>? last_collection
        )
        {
            switch (first_collection)
            {
                case null: break;

                case T[] { Length: 0 }: break;
                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in first_collection)
                        yield return value;
                    break;
            }

            switch (last_collection)
            {
                case null: break;

                case T[] { Length: 0 }: break;
                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in last_collection)
                        yield return value;
                    break;
            }
        }

        public static IEnumerable<T> AppendLast<T>(
            this IEnumerable<T>? first_collection,
            params T[] last) =>
            first_collection.AppendLast((IEnumerable<T>)last);

        /// <summary>Добавить объект в начало перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="collection">Основное перечисление</param>
        /// <param name="obj">Объект, добавляемый в начало перечисления</param>
        /// <returns>Перечисление объектов, составленное из первого объекта и остального перечисления</returns>
        public static IEnumerable<T> AppendFirst<T>(this IEnumerable<T>? collection, T? obj)
        {
            if (obj != null) yield return obj;
            switch (collection)
            {
                case null: break;

                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in collection)
                        yield return value;
                    break;
            }
        }

        /// <summary>Добавить перечисление объектов в начало основного перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="last_collection">Первая последовательность объектов</param>
        /// <param name="first_collection">Вторая последовательность объектов</param>
        /// <returns>Последовательность объектов, составленная из первой последовательности, за которой следует вторая последовательность</returns>
        public static IEnumerable<T> AppendFirst<T>
        (
            this IEnumerable<T>? last_collection,
            IEnumerable<T>? first_collection
        )
        {
            switch (first_collection)
            {
                case null: break;

                case T[] { Length: 0 }: break;
                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in first_collection)
                        yield return value;
                    break;
            }

            switch (last_collection)
            {
                case null: break;

                case T[] { Length: 0 }: break;
                case T[] list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case List<T> { Count: 0 }: break;
                case List<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                case IList<T> { Count: 0 }: break;
                case IList<T> list:
                    foreach (var value in list)
                        yield return value;
                    break;

                default:
                    foreach (var value in last_collection)
                        yield return value;
                    break;
            }
        }

        public static IEnumerable<T> AppendFirst<T>(
            this IEnumerable<T>? last_collection,
            params T[] first) =>
            last_collection.AppendFirst((IEnumerable<T>)first);

        /// <summary>Вставить элемент в указанное положение в последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов, в которую требуется вставить новый элемент</param>
        /// <param name="obj">Элемент, добавляемый в последовательность</param>
        /// <param name="pos">Положение в которое требуется вставить элемент</param>
        /// <returns>Последовательность элементов, в указанной позиции которой будет размещён указанный элемент</returns>
        public static IEnumerable<T> InsertAtPos<T>(this IEnumerable<T> collection, T obj, int pos)
        {
            var i = 0;
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        if (i == pos) yield return obj;
                        yield return list[i];
                    }
                    break;

                case List<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        if (i == pos) yield return obj;
                        yield return list[i];
                    }
                    break;

                case IList<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        if (i == pos) yield return obj;
                        yield return list[i];
                    }
                    break;

                default:
                    foreach (var value in collection)
                    {
                        if (i == pos) yield return obj;
                        yield return value;
                        i++;
                    }
                    break;
            }
        }

        /// <summary>Вставить элемент после указанной позиции в последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов, в которую требуется вставить новый элемент</param>
        /// <param name="obj">Элемент, добавляемый в последовательность</param>
        /// <param name="pos">Положение после которого требуется вставить элемент</param>
        /// <returns>Последовательность элементов, в указанной позиции которой будет размещён указанный элемент</returns>
        public static IEnumerable<T> InsertAfterPos<T>(this IEnumerable<T> collection, T obj, int pos)
        {
            var i = 0;
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (; i < count; i++)
                    {
                        yield return list[i];
                        if (i == pos) yield return obj;
                    }
                    break;

                case List<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        yield return list[i];
                        if (i == pos) yield return obj;
                    }
                    break;

                case IList<T> { Count: var count } list:
                    for (; i < count; i++)
                    {
                        yield return list[i];
                        if (i == pos) yield return obj;
                    }
                    break;

                default:
                    foreach (var value in collection)
                    {
                        yield return value;
                        if (i == pos) yield return obj;
                        i++;
                    }
                    break;
            }
        }

        /// <summary>Инверсная конкатенация перечислений</summary>
        /// <typeparam name="T">Тип элементов перечислений</typeparam>
        /// <param name="FirstCollection">Исходная последовательность, добавляемая в конец</param>
        /// <param name="SecondCollection">Вторичная последовательность, добавляемая в начало</param>
        /// <returns>Последовательность элементов, составленная из элементов вторичной последовательности и элементов первичной последовательности</returns>
        public static IEnumerable<T> ConcatInverted<T>(
            this IEnumerable<T> FirstCollection,
            IEnumerable<T> SecondCollection
        ) => FirstCollection.AppendFirst(SecondCollection);

        /// <summary>Сумма перечисления полиномов</summary>
        /// <param name="P">Перечисление полиномов, которые надо сложить</param>
        /// <returns>Полином, являющийся суммой полиномов</returns>
        public static Polynom Sum(this IEnumerable<Polynom> P)
        {
            Polynom? result;
            switch (P)
            {
                case Polynom[] { Length: 0 }: return new(0);
                case List<Polynom> { Count: 0 }: return new(0);
                case IList<Polynom> { Count: 0 }: return new(0);

                case Polynom[] { Length: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result += pp[i];
                    return result;

                case List<Polynom> { Count: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result += pp[i];
                    return result;

                case IList<Polynom> { Count: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result += pp[i];
                    return result;

                default:
                    result = null;
                    foreach (var p in P)
                        if (result is null) result = p;
                        else
                            result += p;
                    return result ?? new(0);
            }
        }

        /// <summary>Произведение перечисления полиномов</summary>
        /// <param name="P">Перечисление полиномов, которые надо перемножить</param>
        /// <returns>Полином, являющийся произведением полиномов</returns>
        public static Polynom Multiply(this IEnumerable<Polynom> P)
        {
            Polynom? result;
            switch (P)
            {
                case Polynom[] { Length: 0 }: return new(1);
                case List<Polynom> { Count: 0 }: return new(1);
                case IList<Polynom> { Count: 0 }: return new(1);

                case Polynom[] { Length: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result *= pp[i];
                    return result;

                case List<Polynom> { Count: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result *= pp[i];
                    return result;

                case IList<Polynom> { Count: var count } pp:
                    result = pp[0];
                    for (var i = 1; i < count; i++)
                        result *= pp[i];
                    return result;

                default:
                    result = null;
                    foreach (var p in P)
                        if (result is null) result = p;
                        else
                            result *= p;
                    return result ?? new(1);
            }
        }

        /// <summary>Проредить последовательность</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Прореживаемая последовательность</param>
        /// <param name="N">Размер выборки > 0</param>
        /// <param name="k">Положение в выборке (от 0 до N-1)</param>
        /// <returns>Последовательность из N-ых элементов выборки, стоящих на k-ом месте</returns>
        public static IEnumerable<T> Decimate<T>(this IEnumerable<T> collection, int N, int k = 0)
        {
            int i;
            switch (collection)
            {
                case T[] { Length: var count } list:
                    for (i = k; i < count; i += N)
                        yield return list[i];
                    break;

                case List<T> { Count: var count } list:
                    for (i = k; i < count; i += N)
                        yield return list[i];
                    break;

                case IList<T> { Count: var count } list:
                    for (i = k; i < count; i += N)
                        yield return list[i];
                    break;

                default:
                    i = 0;
                    foreach (var v in collection)
                        if (i++ % N == k)
                            yield return v;
                    break;
            }
        }

        /// <summary>Получить первый и последний элементы перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="enumerable">Перечисление</param>
        /// <returns>Перечисление, состоящее из первого и последнего элементов исходного перечисления</returns>
        public static IEnumerable<T> TakeFirstAndLast<T>(this IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case T[] { Length: 0 }: yield break;
                case T[] list:
                    yield return list[0];
                    yield return list[^1];
                    break;

                case List<T> { Count: 0 }: yield break;
                case List<T> list:
                    yield return list[0];
                    yield return list[^1];
                    break;

                case IList<T> { Count: 0 }: yield break;
                case IList<T> list:
                    yield return list[0];
                    yield return list[^1];
                    break;

                default:
                    var last = default(T);
                    var first_taken = false;
                    foreach (var item in enumerable)
                        if (!first_taken)
                        {
                            yield return item;
                            first_taken = true;
                        }
                        else
                            last = item;

                    if (first_taken)
                        yield return last;
                    break;
            }
        }

        /// <summary>Разбить перечисление на страницы</summary>
        /// <typeparam name="T">Тип элемента перечисления</typeparam>
        /// <param name="items">Исходное перечисление элементов</param>
        /// <param name="PageItemsCount">Количество элементов на одну страницу</param>
        /// <returns>Перечисление страниц</returns>
        public static IEnumerable<T?[]> WithPages<T>(this IEnumerable<T> items, int PageItemsCount) =>
            items.AsBlockEnumerable(PageItemsCount);

        /// <summary>Получить элементы перечисления для заданной страницы</summary>
        /// <typeparam name="T">Тип элемента перечисления</typeparam>
        /// <param name="items">Исходное перечисление элементов</param>
        /// <param name="PageNumber">Номер требуемой страницы</param>
        /// <param name="PageItemsCount">Количество элементов на одну страницу</param>
        /// <returns>Перечисление элементов из указанной страницы</returns>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int PageNumber, int PageItemsCount) =>
            items.Skip(PageItemsCount * PageNumber).Take(PageItemsCount);

        /// <summary>Заменить указанный элемент в перечислении</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="items">Исходное перечисление</param>
        /// <param name="ItemToReplace">Элемент, Который требуется заменить</param>
        /// <param name="NewItem">Новый элемент</param>
        /// <returns>Модифицированная последовательность</returns>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> items, T ItemToReplace, T NewItem)
        {
            switch (items)
            {
                case T[] list:
                    foreach (var item in list)
                        yield return Equals(ItemToReplace, item) ? NewItem : item;
                    break;

                case List<T> list:
                    foreach (var item in list)
                        yield return Equals(ItemToReplace, item) ? NewItem : item;
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        yield return Equals(ItemToReplace, item) ? NewItem : item;
                    break;

                default:
                    foreach (var item in items)
                        yield return Equals(ItemToReplace, item) ? NewItem : item;
                    break;
            }
        }

        /// <summary>Заменить указанный элемент в перечислении</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="items">Исходное перечисление</param>
        /// <param name="Selector">Метод оценки необходимости выполнить замену</param>
        /// <param name="NewItem">Новый элемент</param>
        /// <returns>Модифицированная последовательность</returns>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> items, Func<T, bool> Selector, T NewItem)
        {
            switch (items)
            {
                case T[] list:
                    foreach (var item in list)
                        yield return Selector(item) ? NewItem : item;
                    break;

                case List<T> list:
                    foreach (var item in list)
                        yield return Selector(item) ? NewItem : item;
                    break;

                case IList<T> list:
                    foreach (var item in list)
                        yield return Selector(item) ? NewItem : item;
                    break;

                default:
                    foreach (var item in items)
                        yield return Selector(item) ? NewItem : item;
                    break;
            }
        }

        /// <summary>Перебрать последовательно значения из нескольких перечислений</summary>
        /// <typeparam name="T">Тип элементов перечислений</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="Source">Источник данных</param>
        /// <param name="Selector">Метод определения источников данных</param>
        /// <returns>Последовательное перечисление всех элементов данных</returns>
        public static IEnumerable<TValue> SelectSequential<T, TValue>(this IEnumerable<T> Source, Func<T, IEnumerable<TValue>> Selector) =>
            new SequentialEnumerable<TValue>(Source.Select(Selector));

        /// <summary>Перебрать последовательно значения из нескольких перечислений</summary>
        /// <typeparam name="T">Тип элементов перечислений</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="Source">Источник данных</param>
        /// <param name="Selector">Метод определения источников данных</param>
        /// <param name="Comparer">Объект сравнения</param>
        /// <returns>Последовательное перечисление всех элементов данных</returns>
        public static IEnumerable<TValue> SelectSequential<T, TValue>(this IEnumerable<T> Source, Func<T, IEnumerable<TValue>> Selector, IComparer<TValue> Comparer) =>
            new SequentialEnumerable<TValue>(Comparer, Source.Select(Selector));
    }
}