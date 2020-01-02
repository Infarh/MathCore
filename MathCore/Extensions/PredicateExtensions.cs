using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class PredicateExtensions
    {
        [DST] [NotNull] public static Predicate<T> Invert<T>(this Predicate<T> p) => t => !p(t);

        [DST] [NotNull] public static Predicate<T> And<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) & q(t);

        [DST] [NotNull] public static Predicate<T> AndLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) && q(t);

        [DST] [NotNull] public static Predicate<T> Or<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) | q(t);

        [DST] [NotNull] public static Predicate<T> OrLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) || q(t);

        [DST]
        [NotNull]
        public static Predicate<T> XOr<T>(this Predicate<T> p, Predicate<T> q)
        {
            return t =>
                       {
                           var pp = p(t);
                           var qq = q(t);
                           return (pp && !qq) || (!pp && qq);
                       };
        }
    }
}