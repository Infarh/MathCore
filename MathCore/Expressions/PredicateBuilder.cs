using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    public static class PredicateBuilder
    {
        [NotNull] public static Expression<Func<T, bool>> True<T>() => f => true;
        [NotNull] public static Expression<Func<T, bool>> False<T>() => f => false;

        [NotNull]
        public static Expression<Func<T, bool>> Or<T>(
            [NotNull] this Expression<Func<T, bool>> expr1,
            [NotNull] Expression<Func<T, bool>> expr2)
        {
            var second_body = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, second_body), expr1.Parameters);
        }

        [NotNull]
        public static Expression<Func<T, bool>> And<T>(
            [NotNull] this Expression<Func<T, bool>> expr1,
            [NotNull] Expression<Func<T, bool>> expr2)
        {
            var second_body = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(expr1.Body, second_body), expr1.Parameters);
        }
    }
}