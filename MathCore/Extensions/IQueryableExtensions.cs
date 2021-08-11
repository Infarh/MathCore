using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MathCore.Annotations;
using MathCore.Extensions.Expressions;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using InN = MathCore.Annotations.ItemNotNullAttribute;
using IcN = MathCore.Annotations.ItemCanBeNullAttribute;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    /// <summary>Класс методов-расширений для интерфейса <see cref="IQueryable{T}"/></summary>
    public static class IQueryableExtensions
    {
        ///// <summary>Разбить перечисление на страницы</summary>
        ///// <typeparam name="T">Тип элемента перечисления</typeparam>
        ///// <param name="items">Исходное перечисление эленметов</param>
        ///// <param name="PageItemsCount">Количество элементов на одну страницу</param>
        ///// <returns>Перечисление страниц</returns>
        //[NotNull]
        //public static IQueryable<IEnumerable<T>> WithPages<T>([NotNull] this IQueryable<T> items, int PageItemsCount)
        //{

        //}

        /// <summary>Получить элементы перечисления для заданной страницы</summary>
        /// <typeparam name="T">Тип элемента перечисления</typeparam>
        /// <param name="items">Исходное перечисление элементов</param>
        /// <param name="PageNumber">Номер требуемой страницы</param>
        /// <param name="PageItemsCount">Количество элементов на одну страницу</param>
        /// <returns>Перечисление элементов из указанной страницы</returns>
        [NotNull]
        public static IQueryable<T> Page<T>([NotNull] this IQueryable<T> items, int PageNumber, int PageItemsCount) =>
            items.Skip(PageItemsCount * PageNumber).Take(PageItemsCount);

        public static double Dispersion([NotNull] this IQueryable<double> query) => query.Average(x => x * x - query.Average() * query.Average());
        
        [NotNull]
        public static IQueryable<TResult> LeftOuterJoin<T1, T2, TKey, TResult>(
            [NotNull] this IQueryable<T1> OuterItems,
            [NotNull] IEnumerable<T2> InnerItems,
            [NotNull] Expression<Func<T1, TKey>> OuterKeySelector,
            [NotNull] Expression<Func<T2, TKey>> InnerKeySelector,
            [NotNull] Expression<Func<T1, T2, TResult>> ResultSelector
            )
        {
            var v = Expression.Parameter(typeof(Tuple<T1, IEnumerable<T2>>), "v");
            var t2 = ResultSelector.Parameters.Last();

            var t1 = Expression.Property(v, nameof(Tuple<T1, IEnumerable<T2>>.Item1));

            var body = ResultSelector.Body.Replace(ResultSelector.Parameters.First(), t1);
            var result_selector = body.CreateLambda<Func<Tuple<T1, IEnumerable<T2>>, T2, TResult>>(v, t2);

            return OuterItems
               .GroupJoin(
                    InnerItems,
                    OuterKeySelector,
                    InnerKeySelector,
                    (Primary, Items) => new Tuple<T1, IEnumerable<T2>>(Primary, Items))
               .SelectMany(
                    value => value.Item2.DefaultIfEmpty(),
                    result_selector
                );
        }
    }
}