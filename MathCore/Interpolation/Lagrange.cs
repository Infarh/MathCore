#nullable enable
using System;
using System.Linq;
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

    public static double Integral(double[] X, double[] Y, double a, double b)
    {
        var p = GetPolynom(X, Y);
        var I = p.GetIntegral();
        
        var count = X.Length;
        var c = new double[count];
        var C = new double[count];

        var ia = 0d;
        var ib = 0d;

        for (int i = 0, j = 0, k = 1; i < count; i++, j = 0, k = 1) // По всем y из Y
        {
            c[0] = 1;

            var d = 1d;
            var xi = X[i];
            for (; j < i; j++, k++)
            {
                var x = X[j];
                for (var n = k - 1; n > 0; n--)
                    c[n] = c[n - 1] - c[n] * x;
                c[0] *= -x;
                c[k] = 1;
                d *= xi - x;
            }

            for (j++; j < count; j++, k++)
            {
                var x = X[j];
                for (var n = k - 1; n > 0; n--)
                    c[n] = c[n - 1] - c[n] * x;
                c[0] *= -x;
                c[k] = 1;
                d *= xi - x;
            }

            var q = Y[i] / d;
            var c_c = new Polynom(c.Select(t => t * q));
            var c_i = c_c.GetIntegral();

            for (var n = 0; n < count; n++) 
                C[n] += c[n] * q / (n + 1);

            var ya = c[^1] * q;
            var yb = ya;
            for (var n = count - 2; n >= 0; n--)
            {
                var cnd = c[n] * q / (n + 1);
                ya = ya * a + cnd;
                yb = yb * b + cnd;
            }

            var c_v_a = c_i.Value(a);
            var c_v_b = c_i.Value(b);

            ia += ya * a;
            ib += yb * b;
        }

        var IB = I.Value(b);
        var IA = I.Value(a);

        var r = ib - ia;
        var result = IB - IA;

        return result;
    }

    /* ------------------------------------------------------------------------------------------ */

    public event EventHandler OnInitialized;

    private void Invoke_OnInitialized(EventArgs Args) => OnInitialized?.Invoke(this, Args);
    private void Invoke_OnInitialized() => Invoke_OnInitialized(EventArgs.Empty);

    /* ------------------------------------------------------------------------------------------ */

    private readonly Task<Polynom> _PolynomTask;

    /* ------------------------------------------------------------------------------------------ */

    // ReSharper disable once AsyncApostle.AsyncWait
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