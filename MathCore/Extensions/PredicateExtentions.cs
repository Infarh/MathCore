using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace System
{
    public static class PredicateExtentions
    {
        [DST]
        public static Predicate<T> Invert<T>(this Predicate<T> p) => t => !p(t);

        [DST]
        public static Predicate<T> And<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) & q(t);

        [DST]
        public static Predicate<T> AndLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) && q(t);

        [DST]
        public static Predicate<T> Or<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) | q(t);

        [DST]
        public static Predicate<T> OrLazy<T>(this Predicate<T> p, Predicate<T> q) => t => p(t) || q(t);

        [DST]
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