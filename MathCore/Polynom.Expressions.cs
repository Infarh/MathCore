#nullable enable
using System;
using System.Linq.Expressions;

using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class Polynom
{
    /// <summary>Класс операций над полиномом, связанных с деревьями выражений</summary>
    public static class Expression
    {
        /// <summary>Построение дерева выражения на основе коэффициентов полинома</summary>
        /// <param name="A">Коэффициенты полинома</param>
        /// <returns>Дерево выражения, представляющее собой метод вычисления значения полинома</returns>
        public static Expression<Func<double, double>> GetExpression(double[] A)
        {
            var length = A.Length;
            Ex  y;
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

        /// <summary>Построение дерева комплексного выражения на основе коэффициентов полинома</summary>
        /// <param name="A">Коэффициенты полинома</param>
        /// <returns>Дерево выражения, представляющее собой метод вычисления комплексного значения полинома</returns>
        public static Expression<Func<Complex, Complex>> GetExpressionComplex(double[] A)
        {
            var length = A.Length;
            Ex  y;
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

        /// <summary>Построение дерева комплексного выражения на основе комплексных коэффициентов полинома</summary>
        /// <param name="A">Комплексные коэффициенты полинома</param>
        /// <returns>Дерево выражения, представляющее собой метод вычисления комплексного значения полинома</returns>
        public static Expression<Func<Complex, Complex>> GetExpression(Complex[] A)
        {
            var length = A.Length;
            Ex  y;
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

    /// <summary>Получение дерева выражения полинома</summary>
    /// <returns>Дерево выражения полинома</returns>
    public Expression<Func<double, double>> GetExpression() => Expression.GetExpression(_a);

    /// <summary>Получение дерева выражения полинома</summary>
    /// <returns>Дерево выражения полинома</returns>
    public Expression<Func<Complex, Complex>> GetExpressionComplex() => Expression.GetExpressionComplex(_a);
}