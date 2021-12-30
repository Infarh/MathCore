#nullable enable
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MathCore.Vectors;

namespace MathCore.Interpolation;

public class Lagrange : Interpolator, IInterpolator
{
    /* ------------------------------------------------------------------------------------------ */

    public static double Interpolate(double x, double[] X, double[] Y)
    {
        var result = 0d;
        for (int i = 0, j = 0, count = X.Length; i < count; i++, j = 0)
        {
            var p = 1d;

            for (; j < i; j++)
                p *= (x - X[j]) / (X[i] - X[j]);

            for (j++; j < count; j++)
                p *= (x - X[j]) / (X[i] - X[j]);

            result += Y[i] * p;
        }

        return result;
    }

    private static Task<Polynom> GetPolynomAsync(double[] X, double[] Y) => (X, Y).Async(p => GetPolynom(p.X, p.Y));

    public static Polynom GetPolynom(double[] X, double[] Y) => new(GetPolynomCoefficients(X, Y));

    public static double[] GetPolynomCoefficients(double[] X, double[] Y)
    {
        if (X.Length != Y.Length)
            throw new InvalidOperationException("Размеры массивов не равны")
            {
                Data =
                {
                    { "X.Length", X.Length },
                    { "Y.Length", Y.Length }

                }
            };

        var result = new double[X.Length];

        GetPolynomCoefficients(X, Y, result);

        return result;
    }

    public static void GetPolynomCoefficients(double[] X, double[] Y, double[] P)
    {
        if (X.Length != Y.Length)
            throw new InvalidOperationException("Размеры массивов не равны")
            {
                Data =
                {
                    { "X.Length", X.Length },
                    { "Y.Length", Y.Length },
                }
            };
        if (P.Length < X.Length)
            throw new ArgumentException("Размер массива результата недостаточен для записи в него коэффициентов полинома", nameof(P))
            {
                Data =
                {
                    { "X.Length", X.Length },
                    { "Y.Length", Y.Length },
                    { "P.Length", P.Length },
                }
            };

        var count = X.Length;
        var a = new double[count];

        for (int i = 0, j = 0, k = 1; i < count; i++, j = 0, k = 1) // По всем y из Y
        {
            a[0] = 1;

            var b = 1d;
            var xi = X[i];
            for (; j < i; j++, k++)
            {
                var x = X[j];
                for (var n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }

            for (j++; j < count; j++, k++)
            {
                var x = X[j];
                for (var n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }

            P.AddMultiply(a, Y[i] / b);
        }
    }

    /* ------------------------------------------------------------------------------------------ */

    public event EventHandler OnInitialized;

    private void Invoke_OnInitialized(EventArgs Args) => OnInitialized?.Invoke(this, Args);
    private void Invoke_OnInitialized() => Invoke_OnInitialized(EventArgs.Empty);

    /* ------------------------------------------------------------------------------------------ */

    private readonly Task<Polynom> _PolynomTask;

    /* ------------------------------------------------------------------------------------------ */

    public Polynom Polynom => _PolynomTask.Result;

    public bool Initialized => _PolynomTask.Status == TaskStatus.RanToCompletion;

    public double this[double x] => Value(x);

    /* ------------------------------------------------------------------------------------------ */

    public Lagrange(double[] X, double[] Y) => _PolynomTask = GetPolynomAsync(X, Y);

    public Lagrange(params Vector2D[] P)
    {
        var x = new double[P.Length];
        var y = new double[P.Length];
        for (var i = 0; i < P.Length; i++) { x[i] = P[i].X; y[i] = P[i].Y; }

        _PolynomTask = GetPolynomAsync(x, y);
    }

    /* ------------------------------------------------------------------------------------------ */

    public double Value(double x) => Polynom.Value(x);

    public Func<double, double> GetFunction() => Polynom.Value;

    public Expression<Func<double, double>> GetExpression() => Polynom.GetExpression();

    /* ------------------------------------------------------------------------------------------ */
}