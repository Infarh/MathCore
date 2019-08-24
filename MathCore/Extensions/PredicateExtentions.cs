using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace System
{
    public static class PredicateExtentions
    {
        [DST, Pure]
        public static Predicate<T> Invert<T>(this Predicate<T> p) { return t => !p(t); }

        [DST, Pure]
        public static Predicate<T> And<T>(this Predicate<T> p, Predicate<T> q) { return t => p(t) & q(t); }

        [DST, Pure]
        public static Predicate<T> AndLazy<T>(this Predicate<T> p, Predicate<T> q) { return t => p(t) && q(t); }

        [DST, Pure]
        public static Predicate<T> Or<T>(this Predicate<T> p, Predicate<T> q) { return t => p(t) | q(t); }

        [DST, Pure]
        public static Predicate<T> OrLazy<T>(this Predicate<T> p, Predicate<T> q) { return t => p(t) || q(t); }

        [DST, Pure]
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