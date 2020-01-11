using MathCore.Annotations;
using ExF = System.Linq.Expressions.Expression<System.Func<double, double>>;
using ExAF = System.Linq.Expressions.Expression<System.Func<double[], double>>;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    public static class ExpressionEx
    {
        [NotNull]
        public static ExAF ArrayMaximum
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

        [NotNull]
        public static ExAF ArrayMinimum
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

        public static ExF GetDifferential(this ExF f, [CanBeNull] Func<MethodCallExpression, Expression> FunctionDifferentiator = null)
        {
            var visitor = new DifferentialVisitor();
            if(FunctionDifferentiator != null)
                visitor.MethodDifferential += (s, e) => e.DifferentialExpression = FunctionDifferentiator(e.Method);
            var d = visitor.Visit(f);
            return (ExF)d;
        }

        [NotNull]
        public static Expression<Func<TFirstParam, TResult>>
            Compose<TFirstParam, TIntermediate, TResult>(
            [NotNull] this Expression<Func<TFirstParam, TIntermediate>> first,
            [NotNull] Expression<Func<TIntermediate, TResult>> second)
        {
            var param = Expression.Parameter(typeof(TFirstParam), "param");

            var new_first = first.Body.Replace(first.Parameters[0], param);
            var new_second = second.Body.Replace(second.Parameters[0], new_first);

            return Expression.Lambda<Func<TFirstParam, TResult>>(new_second, param);
        }

        public static Expression Replace(this Expression expression,
            Expression SearchEx, Expression ReplaceEx) => new ReplaceVisitor(SearchEx, ReplaceEx).Visit(expression);

        [NotNull]
        public static Expression<Func<TSource, bool>> IsNotNull<TSource, TKey>(
            [NotNull] this Expression<Func<TSource, TKey>> expression) => expression.Compose(key => key != null);
    }
}