using System;

namespace MathCore.Monads;

public delegate T DMaybe<out T>();

public static class Maybe
{
    public static DMaybe<T> Return<T>(T x) => () => x;

    public static DMaybe<T> Nothing<T>() => () => throw new InvalidOperationException("Нельзя получить значение");

    public static DMaybe<TResult> Bind<TArg, TResult>(this DMaybe<TArg> maybe, Func<TArg, DMaybe<TResult>> func) => () => func(maybe())();

    public static DMaybe<TResult> Select<TArg, TResult>(this DMaybe<TArg> maybe, Func<TArg, TResult> func) => maybe.Bind(value => Return(func(value)));

    public static DMaybe<T> Where<T>(this DMaybe<T> maybe, Func<T, bool> predicate) => maybe.Bind(x => predicate(x) ? Return(x) : Nothing<T>());

    public static DMaybe<TC> SelectMany<TA, TB, TC>(this DMaybe<TA> ma, Func<TA, DMaybe<TB>> Selector, Func<TA, TB, TC> resultSelector) => ma.Bind(a => Selector(a).Select(b => resultSelector(a, b)));

    //public static IEnumerable<T> Return<T>(T x) => new[] { x };

    //public static IEnumerable<T> Nothing<T>()
    //{
    //    yield break;
    //}

    //public static IEnumerable<TResult> Bind<TArg, TResult>(this IEnumerable<TArg> m, Func<TArg, IEnumerable<TResult>> func) 
    //    => m.SelectMany(func);
}