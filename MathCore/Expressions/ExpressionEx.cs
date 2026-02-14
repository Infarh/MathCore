using ExAF = System.Linq.Expressions.Expression<System.Func<double[], double>>;
using ExF = System.Linq.Expressions.Expression<System.Func<double, double>>;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public static class ExpressionEx
{
    public static ExAF ArrayMaximum
    {
        get
        {
            Func<double[], double> GetMaximum = X =>
            {
                var max = double.NegativeInfinity;
                var L = X.Length;
                for (var i = 0; i < L; i++)
                    if (X[i] > max) max = X[i];
                return max;
            };

            ExAF result = X => GetMaximum(X);
            return result;
        }
    }

    public static ExAF ArrayMinimum
    {
        get
        {
            Func<double[], double> GetMaximum = X =>
            {
                var min = double.PositiveInfinity;
                var L = X.Length;
                for (var i = 0; i < L; i++)
                    if (X[i] < min) min = X[i];
                return min;
            };

            ExAF result = X => GetMaximum(X);
            return result;
        }
    }

    public static ExF? GetDifferential(this ExF f, Func<MethodCallExpression, Expression>? FunctionDifferentiator = null)
    {
        var visitor = new DifferentialVisitor();
        if (FunctionDifferentiator != null)
            visitor.MethodDifferential += (_, e) => e.DifferentialExpression = FunctionDifferentiator(e.Method);
        var d = visitor.Visit(f);
        return (ExF?)d;
    }

    public static Expression<Func<TFirstParam, TResult>>
        Compose<TFirstParam, TIntermediate, TResult>(
            this Expression<Func<TFirstParam, TIntermediate>> first,
            Expression<Func<TIntermediate, TResult>> second)
    {
        var param = Expression.Parameter(typeof(TFirstParam), "param");

        var new_first = first.Body.Replace(first.Parameters[0], param);
        var new_second = second.Body.Replace(second.Parameters[0], new_first.NotNull());

        return Expression.Lambda<Func<TFirstParam, TResult>>(new_second.NotNull(), param);
    }

    public static Expression? Replace(this Expression expression,
        Expression SearchEx, Expression ReplaceEx) => new ReplaceVisitor(SearchEx, ReplaceEx).Visit(expression);

    public static Expression<Func<TSource, bool>> IsNotNull<TSource, TKey>(
        this Expression<Func<TSource, TKey>> expression) => expression.Compose(key => key != null);

    public static Expression<TResultDelegate> Substitute<TDelegate, TResultDelegate, TParameter, TParameterResult>(
        this Expression<TDelegate> Source,
        string Parameter,
        Expression<Func<TParameter, TParameterResult>> Converter)
    {
        var inpine_parameter = Source.Parameters.First(p => p.Name == Parameter);
        return Expression.Lambda<TResultDelegate>(
            Source.Body.Replace(inpine_parameter, Converter.Body).NotNull(),
            Source.Parameters.Replace(inpine_parameter, Converter.Parameters.Single()));
    }
}