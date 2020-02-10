using System.Collections.Generic;

using MathCore.Annotations;
// ReSharper disable once CheckNamespace
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов-расширений для объектов, поддерживающий интерфейс <see cref="IComparable{T}"/></summary>
    public static class IComparableExtensions
    {
        /// <summary>Метод поиска элемента в списке половинным делением</summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="list">Список, упорядоченный по возрастанию</param>
        /// <param name="From">Начальный индекс поиска</param>
        /// <param name="To">Конечный индекс поиска</param>
        /// <param name="Item">Искомый элемент</param>
        /// <returns>Индекс элемента в списке, либо <see langword="null"/>, если элемент не был найден</returns>
        public static int? SearchBinary<T>(this IReadOnlyList<T> list, int From, int To, T Item) where T : IComparable<T>
        {
            while (To >= From)
            {
                var middle = (From + To) / 2;

                var compare_result = list[middle].CompareTo(Item);
                if (compare_result > 0)
                    To = middle - 1;
                else if (compare_result < 0)
                    From = middle + 1;
                else
                    return middle;
            }
            return null;
        }

        /// <summary>Возвращает максимальный элемент из текущего и, элемента, переданного в качестве параметра</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="x">Текущий элемент в процессе сравнения</param>
        /// <param name="y">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Максимальный из элементов</returns>
        public static T Max<T>([NotNull] this T x, T y) where T : IComparable<T> => x.CompareTo(y) >= 0 ? x : y;

        /// <summary>Возвращает минимальный элемент из текущего и, элемента, переданного в качестве параметра</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="x">Текущий элемент в процессе сравнения</param>
        /// <param name="y">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Минимальный из элементов</returns>
        public static T Min<T>([NotNull] this T x, T y) where T : IComparable<T> => x.CompareTo(y) <= 0 ? x : y;

        /// <summary>Является ли текущий элемент элементом больше, чем элемент, переданный в параметре</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="a">Текущий элемент в процессе сравнения</param>
        /// <param name="b">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Истина, если текущий элемент больше, чем элемент, с которым производится сравнение</returns>
        public static bool IsGreater<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0;

        /// <summary>Является ли текущий элемент элементом меньше, чем элемент, переданный в параметре</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="a">Текущий элемент в процессе сравнения</param>
        /// <param name="b">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Истина, если текущий элемент меньше, чем элемент, с которым производится сравнение</returns>
        public static bool IsLess<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0;

        /// <summary>Является ли текущий элемент элементом больше, либо равен элементу, переданному в параметре</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="a">Текущий элемент в процессе сравнения</param>
        /// <param name="b">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Истина, если текущий элемент больше, либо равен элементу, переданному в параметре</returns>
        public static bool IsGreaterEqual<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0;

        /// <summary>Является ли текущий элемент элементом меньше, либо равен элементу, переданному в параметре</summary>
        /// <typeparam name="T">Тип сравниваемых элементов</typeparam>
        /// <param name="a">Текущий элемент в процессе сравнения</param>
        /// <param name="b">Сравниваемый элемент в процессе сравнения</param>
        /// <returns>Истина, если текущий элемент меньше, либо равен элементу, переданному в параметре</returns>
        public static bool IsLessEqual<T>([NotNull] this T a, T b) where T : IComparable<T> => a.CompareTo(b) <= 0;

        /// <summary>
        /// Возвращает значение <paramref name="x"/>, если оно находится в интервале между <paramref name="min"/> и
        /// <paramref name="max"/>, либо соответствующую границу интервала
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="x">Проверяемое значение</param>
        /// <param name="min">Нижняя граница интервала</param>
        /// <param name="max">Верхняя граница интервала</param>
        /// <returns>Значение из интервала</returns>
        public static T Between<T>([NotNull] this T x, [NotNull] T min, [NotNull] T max) where T : IComparable<T> =>
            x.Max(min.Min(max)).Min(max.Max(min));
    }
}