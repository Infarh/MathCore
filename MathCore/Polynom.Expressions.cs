using System;
using System.Linq.Expressions;
using MathCore.Annotations;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore
{
    public partial class Polynom
    {
        public static class Expression
        {
            [NotNull]
            public static Expression<Func<double, double>> GetExpression([NotNull] double[] A)
            {
                var length = A.Length;
                Ex y;
                var px = Ex.Parameter(typeof(double), "x");
                if (length == 0) y = Ex.Constant(0d);
                else
                {
                    y = Ex.Constant(A[length - 1]);
                    for (var i = 1; i < length; i++)
                        y = Ex.Add(Ex.Multiply(y, px), Ex.Constant(A[length - 1 - i]));
                }

                return Ex.Lambda<Func<double, double>>(y, px);
            }

            [NotNull]
            public static Expression<Func<Complex, Complex>> GetExpressionComplex([NotNull] double[] A)
            {
                var length = A.Length;
                Ex y;
                var px = Ex.Parameter(typeof(Complex), "z");
                if (length == 0) y = Ex.Constant(0d);
                else
                {
                    y = Ex.Constant(A[length - 1]);
                    for (var i = 1; i < length; i++)
                        y = Ex.Add(Ex.Multiply(y, px), Ex.Constant(A[length - 1 - i]));
                }

                return Ex.Lambda<Func<Complex, Complex>>(y, px);
            }

            [NotNull]
            public static Expression<Func<Complex, Complex>> GetExpression([NotNull] Complex[] A)
            {
                var length = A.Length;
                Ex y;
                var px = Ex.Parameter(typeof(Complex), "z");
                if (length == 0) y = Ex.Constant(0d);
                else
                {
                    y = Ex.Constant(A[length - 1]);
                    for (var i = 1; i < length; i++)
                        y = Ex.Add(Ex.Multiply(y, px), Ex.Constant(A[length - 1 - i]));
                }

                return Ex.Lambda<Func<Complex, Complex>>(y, px);
            }
        }

        [NotNull] public Expression<Func<double, double>> GetExpression() => Expression.GetExpression(_a);

        [NotNull] public Expression<Func<Complex, Complex>> GetExpressionComplex() => Expression.GetExpressionComplex(_a);
    }
}