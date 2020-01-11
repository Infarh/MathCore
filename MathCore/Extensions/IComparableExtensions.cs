// ReSharper disable once CheckNamespace
// ReSharper disable UnusedType.Global

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов-расширений для сравнимых объектов</summary>
    public static class IComparableExtensions
    {
        /// <summary>Метод поиска элемента в упорядоченной коллекции половинным делением</summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="Collection">Массив элементов, упорядоченный по возрастанию</param>
        /// <param name="From">Начальный индекс поиска</param>
        /// <param name="To">Конечный индекс поиска</param>
        /// <param name="Item">Искомый элемент</param>
        /// <returns>Индекс элемента в массиве</returns>
        public static int? Search<T>(this T[] Collection, int From, int To, T Item) where T : IComparable<T>
        {
            while(To >= From)
            {
                var middle = (From + To) / 2;

                var comp = Collection[middle].CompareTo(Item);
                if(comp > 0)
                    To = middle - 1;
                else if(comp < 0)
                    From = middle + 1;
                else
                    return middle;
            }
            return null;
        }

        public static T Max<T>([NotNull] this T x, T y) where T : IComparable<T> => x.CompareTo(y) >= 0 ? x : y;

        public static T Min<T>([NotNull] this T x, T y) where T : IComparable<T> => x.CompareTo(y) <= 0 ? x : y;

        public static bool IsGreater<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0;

        public static bool IsLess<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0;

        public static bool IsGreaterEqual<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0;

        public static bool IsLessEqual<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) <= 0;

        public static T Between<T>([NotNull] this T x, [NotNull] T a, [NotNull] T b) where T : IComparable<T>
        {
            var min = a.Min(b);
            var max = b.Max(a);
            return x.Max(min).Min(max);
        }
    }
}