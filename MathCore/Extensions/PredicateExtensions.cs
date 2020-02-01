using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Методы-расширения для функций-предикатов</summary>
    public static class PredicateExtensions
    {
        /// <summary>Создать предикат, значение которого отрицает значение исходного предиката</summary>
        /// <param name="p">Предикат, значение которого необходимо инвертировать</param>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого обратно значению исходного предиката</returns>
        [DST, NotNull] public static Predicate<T> Invert<T>(this Predicate<T> p) => t => !p(t);

        /// <summary>Вычисление логического "И" для двух предикатов</summary>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого является результатом вычисления логического "И" для двух исходных предикатов</returns>
        [DST, NotNull] public static Predicate<T> And<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) & q(t);

        /// <summary>Вычисление логического "ленивого И" для двух предикатов</summary>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого является результатом вычисления логического "ленивого И" для двух исходных предикатов</returns>
        [DST, NotNull] public static Predicate<T> AndLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) && q(t);

        /// <summary>Вычисление логического "ИЛИ" для двух предикатов</summary>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого является результатом вычисления логического "ИЛИ" для двух исходных предикатов</returns>
        [DST, NotNull] public static Predicate<T> Or<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) | q(t);

        /// <summary>Вычисление логического "ленивого ИЛИ" для двух предикатов</summary>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого является результатом вычисления логического "ленивого ИЛИ" для двух исходных предикатов</returns>
        [DST, NotNull] public static Predicate<T> OrLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) || q(t);


        /// <summary>Вычисление логического "ленивого исключающего ИЛИ" для двух предикатов</summary>
        /// <typeparam name="T">Тип параметра предиката</typeparam>
        /// <returns>Предикат, значение которого является результатом вычисления логического "ленивого исключающего ИЛИ" для двух исходных предикатов</returns>
        [DST, NotNull]
        public static Predicate<T> XOr<T>(this Predicate<T> p, Predicate<T> q) =>
           t =>
           {
               var pp = p(t);
               var qq = q(t);
               return (pp && !qq) || (!pp && qq);
           };
    }
}