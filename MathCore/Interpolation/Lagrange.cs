#nullable enable
using System.Linq.Expressions;

using MathCore.Vectors;

namespace MathCore.Interpolation;

/// <summary>Полином Лагранжа</summary>
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

    public static double Interpolate(double x, double x0, double dx, double[] Y)
    {
        var result = 0d;
        for (int i = 0, j = 0, count = Y.Length; i < count; i++, j = 0)
        {
            var xi = x0 + i * dx;
            var p = 1d;

            for (; j < i; j++)
            {
                var xj = x0 + j * dx;
                p *= (x - xj) / (xi - xj);
            }

            for (j++; j < count; j++)
            {
                var xj = x0 + j * dx;
                p *= (x - xj) / (xi - xj);
            }

            result += Y[i] * p;
        }

        return result;
    }

    public static Polynom GetPolynom(double[] X, double[] Y) => new(GetPolynomCoefficients(X, Y));

    /// <summary>Рассчитать коэффициенты полинома Лагранжа для заданного набора точек</summary>
    /// <param name="X">Массив значений на оси X</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <returns>Массив коэффициентов полинома Лагранжа</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="X"/>, или <paramref name="Y"/></exception>
    /// <exception cref="InvalidOperationException">Если длина <paramref name="X"/> не равна <paramref name="Y"/></exception>
    public static double[] GetPolynomCoefficients(double[] X, double[] Y) => GetPolynomCoefficients(X, Y, P: new double[Y.Length]);

    /// <summary>Рассчитать коэффициенты полинома Лагранжа для заданного набора точек</summary>
    /// <param name="x0">Начальное смещение аргумента</param>
    /// <param name="dx">Шаг сетки аргумента</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="Y"/></exception>
    public static double[] GetPolynomCoefficients(double x0, double dx, double[] Y) => GetPolynomCoefficients(x0, dx, Y, P: new double[Y.Length]);

    /// <summary>Рассчитать коэффициенты полинома Лагранжа для заданного набора точек</summary>
    /// <param name="X">Массив значений на оси X</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <param name="P">Массив, в который будут вычислены коэффициенты полинома</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="X"/>, или <paramref name="Y"/></exception>
    /// <exception cref="InvalidOperationException">Если длина <paramref name="X"/> не равна <paramref name="Y"/></exception>
    /// <exception cref="ArgumentException">Если размер массива <paramref name="P"/> меньше размера массива <paramref name="X"/></exception>
    public static double[] GetPolynomCoefficients(double[] X, double[] Y, double[] P)
    {
        if (X is null) throw new ArgumentNullException(nameof(X));
        if (Y is null) throw new ArgumentNullException(nameof(Y));
        if (P is null) throw new ArgumentNullException(nameof(P));

        const string x_length = nameof(X) + "." + nameof(Array.Length);
        const string y_length = nameof(X) + "." + nameof(Array.Length);
        if (X.Length != Y.Length)
            throw new InvalidOperationException("Размеры массивов не совпадают")
            {
                Data =
                {
                    { x_length, X.Length },
                    { y_length, Y.Length },
                }
            };

        var length = X.Length;

        const string p_length = nameof(P) + "." + nameof(Array.Length);
        if (P.Length < length)
            throw new ArgumentException("Размер массива результата недостаточен для записи в него коэффициентов полинома", nameof(P))
            {
                Data =
                {
                    { x_length, X.Length },
                    { y_length, Y.Length },
                    { p_length , P.Length },
                }
            };

        if (P.Length > length)
            Array.Clear(P, length, P.Length - length);

        var a = new double[length];

        // O(N)
        for (var i = 0; i < length; i++) // По всем y из Y
        {
            var j = 0;
            var k = 1;

            a[0] = 1;

            var b = 1d;
            var xi = X[i];
            double x;
            int n;
            for (; j < i; j++, k++)
            {
                x = X[j];
                for (n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }

            for (j++; j < length; j++, k++)
            {
                x = X[j];
                for (n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }
            // O(N)

            x = Y[i] / b;
            for (j = 0; j < length; j++)
                P[j] += a[j] * x;
        }

        // O(N*(2N)) = O(2N^2)

        return P;
    }

    /// <summary>Рассчитать коэффициенты полинома Лагранжа для заданного набора точек</summary>
    /// <param name="x0">Начальное смещение аргумента</param>
    /// <param name="dx">Шаг сетки аргумента</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <param name="P">Массив, в который будут вычислены коэффициенты полинома</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="Y"/></exception>
    public static double[] GetPolynomCoefficients(double x0, double dx, double[] Y, double[] P)
    {
        if (Y is null) throw new ArgumentNullException(nameof(Y));
        if (P is null) throw new ArgumentNullException(nameof(P));

        var length = Y.Length;

        const string p_length = nameof(P) + "." + nameof(Array.Length);
        if (P.Length < length)
            throw new ArgumentException("Размер массива результата недостаточен для записи в него коэффициентов полинома", nameof(P))
            {
                Data =
                {
                    { nameof(x0), x0 },
                    { nameof(dx), dx },
                    { p_length , P.Length },
                }
            };

        if (P.Length > length)
            Array.Clear(P, length, P.Length - length);

        var a = new double[length];

        // O(N)
        for (var i = 0; i < length; i++) // По всем y из Y
        {
            var j = 0;
            var k = 1;

            a[0] = 1;

            var b = 1d;
            var xi = x0 + i * dx;
            double x;
            int n;
            for (; j < i; j++, k++)
            {
                x = x0 + j * dx;
                for (n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }

            for (j++; j < length; j++, k++)
            {
                x = x0 + j * dx;
                for (n = k - 1; n > 0; n--)
                    a[n] = a[n - 1] - a[n] * x;
                a[0] *= -x;
                a[k] = 1;
                b *= xi - x;
            }
            // O(N)

            x = Y[i] / b;
            for (j = 0; j < length; j++)
                P[j] += a[j] * x;
        }

        // O(N*(2N)) = O(2N^2)

        return P;
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

    private readonly Polynom _Polynom;

    /* ------------------------------------------------------------------------------------------ */

    // ReSharper disable once AsyncApostle.AsyncWait
    public Polynom Polynom => _Polynom;

    public double this[double x] => Value(x);

    /* ------------------------------------------------------------------------------------------ */

    public Lagrange(double x0, double dx, double[] Y) : this(GetPolynomCoefficients(x0, dx, Y)) { }

    public Lagrange(double[] X, double[] Y) : this(GetPolynomCoefficients(X, Y)) { }

    private Lagrange(double[] P) => _Polynom = new(P);

    public Lagrange(params IReadOnlyList<Vector2D> P)
    {
        var length = P.Count;
        var x = new double[length];
        var y = new double[length];

        for (var i = 0; i < length; i++)
            P[i].Deconstruct(out x[i], out y[i]);

        _Polynom = GetPolynom(x, y);
    }

    /* ------------------------------------------------------------------------------------------ */

    public double Value(double x) => Polynom.Value(x);

    public Func<double, double> GetFunction() => Polynom.Value;

    public Expression<Func<double, double>> GetExpression() => Polynom.GetExpression();

    /* ------------------------------------------------------------------------------------------ */
}