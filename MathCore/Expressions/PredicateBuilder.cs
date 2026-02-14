// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() => f => true;

    public static Expression<Func<T, bool>> False<T>() => f => false;

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var second_body = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]).NotNull();
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, second_body), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var second_body = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]).NotNull();
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, second_body), expr1.Parameters);
    }
}