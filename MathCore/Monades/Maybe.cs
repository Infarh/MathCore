using System;

namespace MathCore.Monades
{
    public delegate T IMaybe<out T>();

    public static class Maybe
    {
        public static IMaybe<T> Return<T>(T x) => () => x;

        public static IMaybe<T> Nothing<T>() => ()
            =>
        { throw new InvalidOperationException("Нельзя получить значение"); };

        public static IMaybe<TResult> Bind<TArg, TResult>(this IMaybe<TArg> maybe, Func<TArg, IMaybe<TResult>> func)
            => () =>
            {
                var value = maybe();
                var newMaybe = func(value);
                return newMaybe();
            };

        public static IMaybe<TResult> Select<TArg, TResult>(this IMaybe<TArg> maybe, Func<TArg, TResult> func)
            => maybe.Bind(value => Return(func(value)));

        public static IMaybe<T> Where<T>(this IMaybe<T> maybe, Func<T, bool> predicate)
            => maybe.Bind(x => predicate(x) ? Return(x) : Nothing<T>());

        public static IMaybe<TC> SelectMany<TA, TB, TC>(this IMaybe<TA> ma, Func<TA, IMaybe<TB>> maybeSelector, Func<TA, TB, TC> resultSelector)
            => ma.Bind(a => maybeSelector(a).Select(b => resultSelector(a, b)));

        //public static IEnumerable<T> Return<T>(T x) => new[] { x };

        //public static IEnumerable<T> Nothing<T>()
        //{
        //    yield break;
        //}

        //public static IEnumerable<TResult> Bind<TArg, TResult>(this IEnumerable<TArg> m, Func<TArg, IEnumerable<TResult>> func) 
        //    => m.SelectMany(func);

        public static void Test()
        {
            var one = Maybe.Return(1);
            var nothing = Maybe.Nothing<int>();
            var nothing2 =
                from ax in one
                from ay in nothing
                select ax + ay;

            var two = one.Where(z => z > 0).Select(z => z + 1);

            try
            {
                Console.WriteLine(one());
                Console.WriteLine(two());
                Console.WriteLine(nothing2());
            } catch(InvalidOperationException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
