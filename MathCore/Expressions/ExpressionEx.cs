using ExF = System.Linq.Expressions.Expression<System.Func<double, double>>;
using ExAF = System.Linq.Expressions.Expression<System.Func<double[], double>>;
// ReSharper disable UnusedMember.Global

namespace System.Linq.Expressions
{
    public static class ExpressionEx
    {
        public static ExAF ArrrayMaximum
        {
            get
            {
                Func<double[], double> GetMaximum = X =>
                {
                    var max = double.NegativeInfinity;
                    var L = X.Length;
                    for(var i = 0; i < L; i++)
                        if(X[i] > max) max = X[i];
                    return max;
                };

                ExAF result = X => GetMaximum(X);
                return result;
            }
        }

        public static ExAF ArrrayMinimum
        {
            get
            {
                Func<double[], double> GetMaximum = X =>
                {
                    var min = double.PositiveInfinity;
                    var L = X.Length;
                    for(var i = 0; i < L; i++)
                        if(X[i] < min) min = X[i];
                    return min;
                };

                ExAF result = X => GetMaximum(X);
                return result;
            }
        }

        public static ExF GetDifferential(this ExF f, Func<MethodCallExpression, Expression> FunctionDifferentiator = null)
        {
            var visitor = new DifferentialVisitor();
            if(FunctionDifferentiator != null)
                visitor.MethodDifferential += (s, e) => e.DifferentialExpression = FunctionDifferentiator(e.Method);
            var d = visitor.Visit(f);
            return (ExF)d;
        }

        public static Expression<Func<TFirstParam, TResult>>
            Compose<TFirstParam, TIntermediate, TResult>(
            this Expression<Func<TFirstParam, TIntermediate>> first,
            Expression<Func<TIntermediate, TResult>> second)
        {
            var param = Expression.Parameter(typeof(TFirstParam), "param");

            var newFirst = first.Body.Replace(first.Parameters[0], param);
            var newSecond = second.Body.Replace(second.Parameters[0], newFirst);

            return Expression.Lambda<Func<TFirstParam, TResult>>(newSecond, param);
        }

        public static Expression Replace(this Expression expression,
            Expression searchEx, Expression replaceEx) => new ReplaceVisitor(searchEx, replaceEx).Visit(expression);

        public static Expression<Func<TSource, bool>> IsNotNull<TSource, TKey>(
            this Expression<Func<TSource, TKey>> expression) => expression.Compose(key => key != null);
    }

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() => f => true;
        public static Expression<Func<T, bool>> False<T>() => f => false;

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }
    }
}