#nullable enable
using SArray = System.Array;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore;

public partial class Polynom
{
    private static readonly Lazy<Random> __PolynomRandom = new(LazyThreadSafetyMode.PublicationOnly);

    /// <summary>Операции над коэффициентами полинома, в представлении массива значений</summary>
    public static class Array
    {
        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static double GetValue(double x, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Length)
            {
                case 0: return double.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * x;
                case 3: return A[0] + (A[1] + A[2] * x) * x;
                default:
                    var y = A[^1];
                    for (var n = A.Length - 2; n >= 0; n--)
                        y = y * x + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Length)
            {
                case 0: return Complex.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    Complex y = A[^1];
                    for (var n = A.Length - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(double z, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Length)
            {
                case 0: return Complex.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    var y = A[^1];
                    for (var n = A.Length - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома с комплексными коэффициентами</summary>
        /// <param name="z">Комплексный аргумент полинома</param>
        /// <param name="A">Массив комплексных коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Length)
            {
                case 0: return double.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    var y = A[^1];
                    for (var n = A.Length - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static double GetValue(double x, IList<double> A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Count)
            {
                case 0: return double.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * x;
                case 3: return A[0] + (A[1] + A[2] * x) * x;
                default:
                    var y = A[^1];
                    for (var n = A.Count - 2; n >= 0; n--)
                        y = y * x + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, IList<double> A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Count)
            {
                case 0: return Complex.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    Complex y = A[^1];
                    for (var n = A.Count - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(double z, IList<Complex> A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Count)
            {
                case 0: return Complex.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    var y = A[^1];
                    for (var n = A.Count - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /// <summary>Рассчитать комплексное значение полинома с комплексными коэффициентами</summary>
        /// <param name="z">Комплексный аргумент полинома</param>
        /// <param name="A">Массив комплексных коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, IList<Complex> A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            switch (A.Count)
            {
                case 0: return Complex.NaN;
                case 1: return A[0];
                case 2: return A[0] + A[1] * z;
                case 3: return A[0] + (A[1] + A[2] * z) * z;
                default:
                    var y = A[^1];
                    for (var n = A.Count - 2; n >= 0; n--)
                        y = y * z + A[n];
                    return y;
            }
        }

        /* ------------------------------------------------------------------------ */

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static double GetValue(double x, IEnumerable<double> A)
        {
            using var a = A.NotNull().GetEnumerator();

            var xx = 1d;

            var y = 0d;
            var any = false;
            while (a.MoveNext())
            {
                y += a.Current * xx;
                xx *= x;
                any = true;
            }

            return any ? y : double.NaN;
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, IEnumerable<double> A)
        {
            using var a = A.NotNull().GetEnumerator();

            var zz = Complex.Real;

            var y = Complex.Zero;
            var any = false;
            while (a.MoveNext())
            {
                y += a.Current * zz;
                zz *= z;
                any = true;
            }

            return any ? y : Complex.NaN;
        }

        /// <summary>Рассчитать комплексное значение полинома</summary>
        /// <param name="z">Аргумент полинома</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(double z, IEnumerable<Complex> A)
        {
            using var a = A.NotNull().GetEnumerator();

            var zz = Complex.Real;

            var y = Complex.Zero;
            var any = false;
            while (a.MoveNext())
            {
                y += a.Current * zz;
                zz *= z;
                any = true;
            }

            return any ? y : Complex.NaN;
        }

        /// <summary>Рассчитать комплексное значение полинома с комплексными коэффициентами</summary>
        /// <param name="z">Комплексный аргумент полинома</param>
        /// <param name="A">Массив комплексных коэффициентов полинома</param>
        /// <returns>Комплексное значение полинома</returns>
        public static Complex GetValue(Complex z, IEnumerable<Complex> A)
        {
            using var a = A.NotNull().GetEnumerator();

            var zz = Complex.Real;

            var y = Complex.Zero;
            var any = false;
            while (a.MoveNext())
            {
                y += a.Current * zz;
                zz *= z;
                any = true;
            }

            return any ? y : Complex.NaN;
        }

        /* ------------------------------------------------------------------------ */

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static double GetValue(double x, out double dy, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (double.IsNaN(x))
            {
                dy = double.NaN;
                return double.NaN;
            }

            double y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    dy = double.NaN;
                    y = double.NaN;
                    break;

                case 1:
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    dy = A[1];
                    y = A[0] + A[1] * x;
                    break;

                case 3:
                    dy = A[1] + 2 * A[2] * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);

                    for (var n = N - 2; n >= 1; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                    }

                    y = y * x + A[0];
                    break;
            }

            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(Complex x, out Complex dy, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (x.IsNaN)
            {
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    dy = A[1];
                    y = A[0] + A[1] * x;
                    break;

                case 3:
                    dy = A[1] + 2 * A[2] * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);

                    for (var n = N - 2; n >= 1; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                    }

                    y = y * x + A[0];
                    break;
            }

            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(double x, out Complex dy, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (double.IsNaN(x))
            {
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    dy = A[1];
                    y = A[0] + A[1] * x;
                    break;

                case 3:
                    dy = A[1] + 2 * A[2] * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);

                    for (var n = N - 2; n >= 1; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                    }

                    y = y * x + A[0];
                    break;
            }

            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(Complex x, out Complex dy, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (x.IsNaN)
            {
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    dy = A[1];
                    y = A[0] + A[1] * x;
                    break;

                case 3:
                    dy = A[1] + 2 * A[2] * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);

                    for (var n = N - 2; n >= 1; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                    }

                    y = y * x + A[0];
                    break;
            }

            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="d2y">Значение второй производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static double GetValue(double x, out double dy, out double d2y, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (double.IsNaN(x))
            {
                d2y = double.NaN;
                dy = double.NaN;
                return double.NaN;
            }

            double y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    d2y = double.NaN;
                    dy = double.NaN;
                    y = double.NaN;
                    break;

                case 1:
                    d2y = 0;
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    d2y = 0;
                    dy = A[1];
                    y = A[0] + dy * x;
                    break;

                case 3:
                    d2y = 2 * A[2];
                    dy = A[1] + d2y * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);
                    d2y = A[^1] * (N - 1) * (N - 2);

                    for (var n = N - 2; n >= 2; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                        d2y = d2y * x + a * n * (n - 1);
                    }

                    y = (y * x + A[1]) * x + A[0];
                    dy = dy * x + A[1];
                    break;
            }
            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="d2y">Значение второй производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(double x, out Complex dy, out Complex d2y, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (double.IsNaN(x))
            {
                d2y = Complex.NaN;
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    d2y = Complex.NaN;
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    d2y = 0;
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    d2y = 0;
                    dy = A[1];
                    y = A[0] + dy * x;
                    break;

                case 3:
                    d2y = 2 * A[2];
                    dy = A[1] + d2y * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);
                    d2y = A[^1] * (N - 1) * (N - 2);

                    for (var n = N - 2; n >= 2; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                        d2y = d2y * x + a * n * (n - 1);
                    }

                    y = (y * x + A[1]) * x + A[0];
                    dy = dy * x + A[1];
                    break;
            }
            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="d2y">Значение второй производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(Complex x, out Complex dy, out Complex d2y, params double[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (x.IsNaN)
            {
                d2y = Complex.NaN;
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    d2y = Complex.NaN;
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    d2y = 0;
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    d2y = 0;
                    dy = A[1];
                    y = A[0] + dy * x;
                    break;

                case 3:
                    d2y = 2 * A[2];
                    dy = A[1] + d2y * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);
                    d2y = A[^1] * (N - 1) * (N - 2);

                    for (var n = N - 2; n >= 2; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                        d2y = d2y * x + a * n * (n - 1);
                    }

                    y = (y * x + A[1]) * x + A[0];
                    dy = dy * x + A[1];
                    break;
            }
            return y;
        }

        /// <summary>Рассчитать значение полинома</summary>
        /// <param name="x">Аргумент полинома</param>
        /// <param name="dy">Значение первой производной в точке</param>
        /// <param name="d2y">Значение второй производной в точке</param>
        /// <param name="A">Массив коэффициентов полинома</param>
        /// <returns>Значение полинома</returns>
        public static Complex GetValue(Complex x, out Complex dy, out Complex d2y, params Complex[] A)
        {
            if (A is null) throw new ArgumentNullException(nameof(A));
            if (x.IsNaN)
            {
                d2y = Complex.NaN;
                dy = Complex.NaN;
                return Complex.NaN;
            }

            Complex y;
            var N = A.Length;
            switch (N)
            {
                case 0:
                    d2y = Complex.NaN;
                    dy = Complex.NaN;
                    y = Complex.NaN;
                    break;

                case 1:
                    d2y = 0;
                    dy = 0;
                    y = A[0];
                    break;

                case 2:
                    d2y = 0;
                    dy = A[1];
                    y = A[0] + dy * x;
                    break;

                case 3:
                    d2y = 2 * A[2];
                    dy = A[1] + d2y * x;
                    y = A[0] + (A[1] + A[2] * x) * x;
                    break;

                default:
                    y = A[^1];
                    dy = A[^1] * (N - 1);
                    d2y = A[^1] * (N - 1) * (N - 2);

                    for (var n = N - 2; n >= 2; n--)
                    {
                        var a = A[n];

                        y = y * x + a;
                        dy = dy * x + a * n;
                        d2y = d2y * x + a * n * (n - 1);
                    }

                    y = (y * x + A[1]) * x + A[0];
                    dy = dy * x + A[1];
                    break;
            }
            return y;
        }

        public static (double y, double dy) GetValueWithDiff(double x, params double[] A) => (GetValue(x, out var dy, A), dy);

        public static (double y, double dy, double d2y) GetValueWithDiff2(double x, params double[] A) => (GetValue(x, out var dy, out var d2y, A), dy, d2y);

        /* ------------------------------------------------------------------------ */

        public static double GetRootsValue(double x, params double[] X0)
        {
            if (X0 is null) throw new ArgumentNullException(nameof(X0));
            if (X0.Length == 0) return double.NaN;

            var y = x - X0[0];
            for (var i = 1; i < X0.Length; i++)
                y *= x - X0[i];

            return y;
        }

        public static Complex GetRootsValue(double x, params Complex[] X0)
        {
            if (X0.NotNull().Length == 0) return Complex.NaN;

            var y = x - X0[0];
            for (var i = 1; i < X0.Length; i++)
                y *= x - X0[i];

            return y;
        }

        public static Complex GetRootsValue(Complex x, params double[] X0)
        {
            if (X0.NotNull().Length == 0) return Complex.NaN;

            var y = x - X0[0];
            for (var i = 1; i < X0.Length; i++)
                y *= x - X0[i];

            return y;
        }

        public static Complex GetRootsValue(Complex x, params Complex[] X0)
        {
            if (X0.NotNull().Length == 0) return Complex.NaN;

            var y = x - X0[0];
            for (var i = 1; i < X0.Length; i++)
                y *= x - X0[i];

            return y;
        }

        public static double GetRootsValue(double x, IEnumerable<double> X0)
        {
            using var xx = X0.NotNull().GetEnumerator();
            if (!xx.MoveNext())
                return double.NaN;

            var y = x - xx.Current;
            while (xx.MoveNext())
                y *= x - xx.Current;

            return y;
        }

        public static Complex GetRootsValue(double x, IEnumerable<Complex> X0)
        {
            using var xx = X0.NotNull().GetEnumerator();
            if (!xx.MoveNext())
                return Complex.NaN;

            var y = x - xx.Current;
            while (xx.MoveNext())
                y *= x - xx.Current;

            return y;
        }

        public static Complex GetRootsValue(Complex x, IEnumerable<double> X0)
        {
            using var xx = X0.NotNull().GetEnumerator();
            if (!xx.MoveNext())
                return Complex.NaN;

            var y = x - xx.Current;
            while (xx.MoveNext())
                y *= x - xx.Current;

            return y;
        }

        public static Complex GetRootsValue(Complex x, IEnumerable<Complex> X0)
        {
            using var xx = X0.NotNull().GetEnumerator();
            if (!xx.MoveNext())
                return Complex.NaN;

            var y = x - xx.Current;
            while (xx.MoveNext())
                y *= x - xx.Current;

            return y;
        }

        /* ------------------------------------------------------------------------ */

        /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при степенях</returns>
        public static double[] GetCoefficients(params double[] Root) => Root switch
        {
            null => throw new ArgumentNullException(nameof(Root)),
            { Length: 0 } => throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root)),
            { Length: 1 } => [-Root[0], 1],
            _ => GetCoefficients(Root, new double[Root.Length + 1])
        };

        /// <summary>Преобразовать последовательность корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при степенях</returns>
        public static double[] GetCoefficients(IEnumerable<double> Root)
        {
            using var root = Root.NotNull().GetEnumerator();
            if (!root.MoveNext())
                throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

            var a = new List<double> { -root.Current, 1 };

            while (root.MoveNext())
            {
                var r = root.Current;
                for (var i = a.Count - 1; i > 0; i--)
                    a[i] = a[i - 1] - a[i] * r;
                a[0] *= -r;
                a.Add(1);
            }

            return [.. a];
        }

        public static IList<double> GetCoefficients(IEnumerable<double> Root, IList<double> Coefficients)
        {
            if (Root is null)
                throw new ArgumentNullException(nameof(Root));

            if (Coefficients.IsReadOnly)
                throw new ArgumentException("Список коэффициентов невозможно изменить", nameof(CommandLineArgs))
                {
                    Data =
                    {
                        { "CoefficientsType", Coefficients.GetType() }
                    }
                };

            using var root = Root.GetEnumerator();
            if (!root.MoveNext())
                throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

            switch (Coefficients.Count)
            {
                default:
                    Coefficients.Clear();
                    goto case 0;
                case 0:
                    Coefficients.Add(-root.Current);
                    Coefficients.Add(1);
                    break;
                case 2:
                    Coefficients[0] = -root.Current;
                    Coefficients[1] = 1;
                    break;
            }

            if (Coefficients.Count > 0)
                Coefficients.Clear();

            Coefficients.Add(-root.Current);
            Coefficients.Add(1);

            while (root.MoveNext())
            {
                var r = root.Current;
                for (var i = Coefficients.Count - 1; i > 0; i--)
                    Coefficients[i] = Coefficients[i - 1] - Coefficients[i] * r;
                Coefficients[0] *= -r;

                Coefficients.Add(1);
            }

            return Coefficients;
        }

        /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Массив корней полинома</param>
        /// <param name="a">Массив, который требуется рассчитать коэффициенты полинома (длина должна быть на 1 больше массива <paramref name="Root"/>)</param>
        /// <exception cref="InvalidOperationException">При <paramref name="a"/><c>.Length</c> != <paramref name="Root"/><c>.Length + 1</c></exception>
        /// <exception cref="ArgumentException">При <paramref name="Root"/><c>.Length</c> == 0</exception>
        public static double[] GetCoefficients(double[] Root, double[] a)
        {
            if (Root.NotNull().Length == 0)
                throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));

            const string root_length = $"{nameof(Root)}.Length";
            const string a_length = $"{nameof(a)}.Length";
            if (Root.Length != a.Length - 1)
                throw new InvalidOperationException("Размер массива коэффициентов полинома должен быть равен длине массива корней полинома + 1")
                {
                    Data =
                    {
                        { root_length, Root.Length },
                        { a_length, a.Length },
                    }
                };

            a[0] = -Root[0];
            a[1] = 1;

            if (Root.Length == 1)
                return a;

            for (var (n, N) = (2, Root.Length + 1); n < N; n++)
            {
                a[n] = 1;
                var root = Root[n - 1];

                for (var i = n - 1; i > 0; i--)
                    a[i] = a[i - 1] - a[i] * root;

                a[0] *= -root;
            }

            return a;
        }

        //public static double GetCoefficient(double[] Root, int Power)
        //{
        //    if (Root is null) throw new ArgumentNullException(nameof(Root));
        //    var root_length = Root.Length;
        //    if (root_length == 0) throw new InvalidOperationException("Нет корней");

        //    if (Power == root_length) return 1;
        //    if (Power > root_length) return 0;

        //    switch (root_length)
        //    {
        //        case 1: return -Root[0];
        //        case 2: return Power == 0 ? Root[0] * Root[1] : -Root[0] + -Root[1];
        //        case 3:
        //            return Power switch
        //            {
        //                0 => -Root[0] * -Root[1] * -Root[2],
        //                1 => Root[0] * Root[1]
        //                    + Root[0] * Root[2]
        //                    + Root[1] * Root[2],
        //                2 => -Root[0] + -Root[1] + -Root[2],
        //                _ => throw new ArgumentOutOfRangeException(nameof(Power), Power, null)
        //            };
        //        case 4:
        //            return Power switch
        //            {
        //                0 => -Root[0] * -Root[1] * -Root[2] * -Root[3],
        //                1 => -Root[1] * -Root[2] * -Root[3]
        //                    - Root[0] * Root[2] * Root[3]
        //                    - Root[0] * Root[1] * Root[3]
        //                    - Root[0] * Root[1] * Root[2],
        //                2 => -Root[2] * -Root[3]
        //                    + -Root[1] * -Root[3]
        //                    + -Root[0] * -Root[3]
        //                    + -Root[0] * -Root[2]
        //                    + -Root[0] * -Root[1],
        //                3 => -Root[0] + -Root[1] + -Root[2] + -Root[3],
        //                _ => throw new ArgumentOutOfRangeException(nameof(Power), Power, null)
        //            };
        //    }

        //    return ComputeCoefficient(new double[Power + 1], Root, Root.Length);
        //    static double ComputeCoefficient(double[] A, double[] X, int RootLength)
        //    {
        //        var x0 = -X[RootLength - 1];
        //        if (RootLength == 1)
        //        {
        //            A[0] = x0;
        //            A[1] = 1;
        //            return 0;
        //        }

        //        ComputeCoefficient(A, X, RootLength - 1);
        //        for (var i = A.Length - 1; i > 0; i--) 
        //            A[i] = A[i - 1] + A[i] * x0;

        //        A[0] = x0;

        //        return A[^2];
        //    }

        //}

        /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при обратных степенях</returns>
        public static double[] GetCoefficientsInverted(params double[] Root)
        {
            switch (Root)
            {
                case null: throw new ArgumentNullException(nameof(Root));
                case { Length: 0 }: throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                case { Length: 1 }: return [1, -Root[0]];
            }

            var N = Root.Length + 1;
            var a = new double[N];

            a[^1] = -Root[0];
            a[^2] = 1;

            for (var k = 2; k < N; k++)
            {
                a[N - 1 - k] = a[N - 1 - k + 1];
                var root = Root[k - 1];

                for (var i = k - 1; i > 0; i--)
                    a[N - 1 - i] = a[N - 1 - (i - 1)] - a[N - 1 - i] * root;

                a[^1] *= -root;
            }

            return a;
        }


        /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при степенях</returns>
        public static Complex[] GetCoefficients(params Complex[] Root) =>
            Root switch
            {
                null => throw new ArgumentNullException(nameof(Root)),
                { Length: 0 } => throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root)),
                { Length: 1 } => [-Root[0], 1],
                _ => GetCoefficients(Root, new Complex[Root.Length + 1])
            };

        /// <summary>Преобразовать последовательность корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при степенях</returns>
        public static Complex[] GetCoefficients(IEnumerable<Complex> Root)
        {
            using var root = Root.NotNull().GetEnumerator();
            if (!root.MoveNext())
                throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

            var a = new List<Complex> { -root.Current, 1 };

            while (root.MoveNext())
            {
                var r = root.Current;
                for (var i = a.Count - 1; i > 0; i--)
                    a[i] = a[i - 1] - a[i] * r;
                a[0] *= -r;
                a.Add(1);
            }

            return [.. a];
        }

        public static IList<Complex> GetCoefficients(IEnumerable<Complex> Root, IList<Complex> Coefficients)
        {
            if (Root is null)
                throw new ArgumentNullException(nameof(Root));

            if (Coefficients.IsReadOnly)
                throw new ArgumentException("Список коэффициентов невозможно изменить", nameof(CommandLineArgs))
                {
                    Data =
                    {
                        { "CoefficientsType", Coefficients.GetType() }
                    }
                };

            using var root = Root.GetEnumerator();
            if (!root.MoveNext())
                throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

            switch (Coefficients.Count)
            {
                default:
                    Coefficients.Clear();
                    goto case 0;
                case 0:
                    Coefficients.Add(-root.Current);
                    Coefficients.Add(1);
                    break;
                case 2:
                    Coefficients[0] = -root.Current;
                    Coefficients[1] = 1;
                    break;
            }

            if (Coefficients.Count > 0)
                Coefficients.Clear();

            Coefficients.Add(-root.Current);
            Coefficients.Add(1);

            while (root.MoveNext())
            {
                var r = root.Current;
                for (var i = Coefficients.Count - 1; i > 0; i--)
                    Coefficients[i] = Coefficients[i - 1] - Coefficients[i] * r;
                Coefficients[0] *= -r;

                Coefficients.Add(1);
            }

            return Coefficients;
        }

        /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
        /// <param name="Root">Массив корней полинома</param>
        /// <param name="a">Массив, который требуется рассчитать коэффициенты полинома (длина должна быть на 1 больше массива <paramref name="Root"/>)</param>
        /// <exception cref="InvalidOperationException">При <paramref name="a"/><c>.Length</c> != <paramref name="Root"/><c>.Length + 1</c></exception>
        /// <exception cref="ArgumentException">При <paramref name="Root"/><c>.Length</c> == 0</exception>
        public static Complex[] GetCoefficients(Complex[] Root, Complex[] a)
        {
            if (Root.NotNull().Length == 0)
                throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));

            const string root_length = $"{nameof(Root)}.Length";
            const string a_length = $"{nameof(a)}.Length";
            if (Root.Length != a.Length - 1)
                throw new InvalidOperationException("Размер массива коэффициентов полинома должен быть равен длине массива корней полинома + 1")
                {
                    Data =
                    {
                        { root_length, Root.Length },
                        { a_length, a.Length },
                    }
                };

            a[0] = -Root[0];
            a[1] = 1;

            if (Root.Length == 1)
                return a;

            for (var (n, N) = (2, Root.Length + 1); n < N; n++)
            {
                a[n] = 1;
                var root = Root[n - 1];

                for (var i = n - 1; i > 0; i--)
                    a[i] = a[i - 1] - a[i] * root;

                a[0] *= -root;
            }

            return a;
        }

        /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при обратных степенях</returns>
        public static Complex[] GetCoefficientsInverted(params Complex[] Root)
        {
            switch (Root)
            {
                case null: throw new ArgumentNullException(nameof(Root));
                case { Length: 0 }: throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                case { Length: 1 }: return [1, -Root[0]];
            }

            var N = Root.Length + 1;
            var a = new Complex[N];

            a[N - 1] = -Root[0];
            a[N - 1 - 1] = 1;

            for (var k = 2; k < N; k++)
            {
                a[N - 1 - k] = a[N - 1 - k + 1];

                for (var i = k - 1; i > 0; i--)
                    a[N - 1 - i] = a[N - 1 - (i - 1)] - a[N - 1 - i] * Root[k - 1];

                a[N - 1] = -a[N - 1] * Root[k - 1];
            }

            return a;
        }

        /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях, вещественная часть</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Коэффициенты при обратных степенях, вещественная часть</returns>
        public static double[] GetCoefficientsInvertedRe(params Complex[] Root)
        {
            switch (Root)
            {
                case null: throw new ArgumentNullException(nameof(Root));
                case { Length: 0 }: throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                case { Length: 1 }: return [1, -Root[0].Re];
            }

            var N = Root.Length + 1;
            var re = new double[N];
            var im = new double[N];

            (re[N - 1], im[N - 1]) = -Root[0];
            re[N - 1 - 1] = 1;

            for (var k = 2; k < N; k++)
            {
                re[N - 1 - k] = re[N - 1 - k + 1];
                im[N - 1 - k] = im[N - 1 - k + 1];

                for (var i = k - 1; i > 0; i--)
                {
                    var (t_re, t_im) = (re[N - 1 - i], im[N - 1 - i]) * Root[k - 1];

                    re[N - 1 - i] = re[N - 1 - (i - 1)] - t_re;
                    im[N - 1 - i] = im[N - 1 - (i - 1)] - t_im;
                }

                (re[N - 1], im[N - 1]) = (-re[N - 1], -im[N - 1]) * Root[k - 1];
            }

            return re;
        }

        /// <summary>
        /// Преобразовать массив корней полинома в коэффициенты при обратных степенях,
        /// возвращая отдельно массивы действительных и мнимых частей.
        /// </summary>
        /// <param name="Root">Массив комплексных корней полинома.</param>
        /// <returns>Кортеж, содержащий массивы коэффициентов при обратных степенях:
        /// действительная часть и мнимая часть.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="Root"/> равен null.</exception>
        /// <exception cref="ArgumentException">Выбрасывается, если длина <paramref name="Root"/> равна 0.</exception>
        public static (double[] Re, double[] Im) GetCoefficientsInvertedReIm(params Complex[] Root)
        {
            switch (Root)
            {
                case null: throw new ArgumentNullException(nameof(Root));
                case { Length: 0 }: throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                case { Length: 1 }: return (new[] { 1, -Root[0].Re }, new[] { 1, -Root[0].Im });
            }

            var N = Root.Length + 1;
            var re = new double[N];
            var im = new double[N];

            (re[N - 1], im[N - 1]) = -Root[0];
            re[N - 1 - 1] = 1;

            for (var k = 2; k < N; k++)
            {
                re[N - 1 - k] = re[N - 1 - k + 1];
                im[N - 1 - k] = im[N - 1 - k + 1];

                for (var i = k - 1; i > 0; i--)
                {
                    var (t_re, t_im) = (re[N - 1 - i], im[N - 1 - i]) * Root[k - 1];

                    re[N - 1 - i] = re[N - 1 - (i - 1)] - t_re;
                    im[N - 1 - i] = im[N - 1 - (i - 1)] - t_im;
                }

                (re[N - 1], im[N - 1]) = (-re[N - 1], -im[N - 1]) * Root[k - 1];
            }

            return (re, im);
        }

        #region Интегрирование/дифференцирование

        /// <summary>Вычислить производную от полинома, заданного своими коэффициентами</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="Result">Массив для хранения результата</param>
        /// <param name="Order">Порядок производной (по умолчанию 1)</param>
        /// <returns>Массив, содержащий коэффициенты производной от полинома</returns>
        /// <exception cref="ArgumentNullException"><paramref name="p"/> or <paramref name="Result"/> is null</exception>
        /// <exception cref="ArgumentException">Длина <paramref name="Result"/> меньше чем длина <paramref name="p"/> минус <paramref name="Order"/></exception>
        public static double[] GetDifferential(double[] p, double[] Result, int Order = 1)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));
            if (Result is null) throw new ArgumentNullException(nameof(Result));

            if (Order == 0)
            {
                p.CopyTo(Result, 0);
                return Result;
            }

            var result_length = p.Length - Order;
            if (Result.Length - result_length < 0)
                throw new ArgumentException("Длина массива результата меньше чем длина исходного массива минус порядок интегрирования - недостаточна для хранения результатов интегрирования")
                {
                    Data =
                    {
                        { nameof(p) + ".Length", p.Length },
                        { nameof(Result) + ".Length", Result.Length },
                        { nameof(Order), Order },
                    }
                };

            if (Order >= p.Length)
            {
                SArray.Clear(Result, 0, Result.Length);
                return Result;
            }

            if (Result.Length - result_length is > 0 and var delta_length)
                SArray.Clear(Result, result_length, delta_length);

            switch (Order)
            {
                case 1:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 1] * (i + 1);
                    break;
                case 2:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 2] * ((i + 3) * i + 2); // * (i + 1) * (i + 2) // * (i*i + 3 * i + 2)
                    break;
                case 3:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 3] * (((i + 6) * i + 11) * i + 6); // * (i + 1) * (i + 2) * (i + 3) // * (i * i * i + 6 * i * i + 11 * i + 6)
                    break;
                case 4:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 4] * ((((i + 10) * i + 35) * i + 50) * i + 24); // * (i + 1) * (i + 2) * (i + 3) * (i + 4) // * (i * i * i * i + 10 * i * i * i + 35 * i * i + 50 * i + 24)
                    break;
                default:
                    var k = Order.Factorial();
                    Result[0] = p[Order] * k;
                    for (var i = 1; i < result_length; i++)
                    {
                        var i0 = i + Order;

                        k = k / i * i0;
                        //k *= 1 + Order / i;

                        Result[i] = p[i0] * k;
                    }

                    break;
            }
            return Result;
        }

        /// <summary>Получить производную полинома, заданного комплексными коэффициентами</summary>
        /// <param name="p">Массив комплексных коэффициентов полинома</param>
        /// <param name="Result">Массив для хранения результата</param>
        /// <param name="Order">Порядок производной</param>
        /// <returns>Массив <paramref name="Result"/>, содержащий производную полинома</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="p"/> или <paramref name="Result"/> равен null</exception>
        /// <exception cref="ArgumentException">Выбрасывается, если длина <paramref name="p"/> слишком мала, или если длина <paramref name="Result"/> недостаточна для хранения результата</exception>
        public static Complex[] GetDifferential(Complex[] p, Complex[] Result, int Order = 1)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));
            if (Result is null) throw new ArgumentNullException(nameof(Result));

            if (Order == 0)
            {
                p.CopyTo(Result, 0);
                return Result;
            }

            var result_length = p.Length - Order;
            if (Result.Length - result_length < 0)
                throw new ArgumentException("Длина массива результата меньше чем длина исходного массива минус порядок интегрирования - недостаточна для хранения результатов интегрирования")
                {
                    Data =
                    {
                        { nameof(p) + ".Length", p.Length },
                        { nameof(Result) + ".Length", Result.Length },
                        { nameof(Order), Order },
                    }
                };

            if (Order >= p.Length)
            {
                SArray.Clear(Result, 0, Result.Length);
                return Result;
            }

            if (Result.Length - result_length is > 0 and var delta_length)
                SArray.Clear(Result, result_length, delta_length);

            switch (Order)
            {
                case 1:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 1] * (i + 1);
                    break;
                case 2:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 2] * ((i + 3) * i + 2); // * (i + 1) * (i + 2) // * (i*i + 3 * i + 2)
                    break;
                case 3:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 3] * (((i + 6) * i + 11) * i + 6); // * (i + 1) * (i + 2) * (i + 3) // * (i * i * i + 6 * i * i + 11 * i + 6)
                    break;
                case 4:
                    for (var i = 0; i < result_length; i++)
                        Result[i] = p[i + 4] * ((((i + 10) * i + 35) * i + 50) * i + 24); // * (i + 1) * (i + 2) * (i + 3) * (i + 4) // * (i * i * i * i + 10 * i * i * i + 35 * i * i + 50 * i + 24)
                    break;
                default:
                    var k = Order.Factorial();
                    Result[0] = p[Order] * k;
                    for (var i = 1; i < result_length; i++)
                    {
                        var i0 = i + Order;

                        k = k / i * i0;
                        //k *= 1 + Order / i;

                        Result[i] = p[i0] * k;
                    }

                    break;
            }
            return Result;
        }

        /// <summary>Дифференциал полинома</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="Order">Порядок дифференцирования</param>
        /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="Order"/> &gt; 20</exception>
        /// <returns>Массив коэффициентов полинома - дифференциала</returns>
        public static double[] GetDifferential(double[] p, int Order = 1) => p switch
        {
            null => throw new ArgumentNullException(nameof(p)),
            _ => Order switch
            {
                > 20 => throw new ArgumentOutOfRangeException(nameof(Order), Order, "Значение не должно быть больше 20"),
                < 0 => GetIntegral(p, Order: -Order),
                0 => p,
                var o when o == p.Length - 1 => [p[^1] * Order.Factorial()],
                var o when o >= p.Length => [0d],
                _ => GetDifferential(p, new double[p.Length - Order], Order)
            }
        };

        /// <summary>Дифференциал полинома</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="Order">Порядок дифференцирования</param>
        /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="Order"/> &gt; 20</exception>
        /// <returns>Массив коэффициентов полинома - дифференциала</returns>
        public static Complex[] GetDifferential(Complex[] p, int Order = 1) => p switch
        {
            null => throw new ArgumentNullException(nameof(p)),
            _ => Order switch
            {
                > 20 => throw new ArgumentOutOfRangeException(nameof(Order), Order, "Значение не должно быть больше 20"),
                < 0 => GetIntegral(p, Order: -Order),
                0 => p,
                var o when o == p.Length - 1 => [p[^1] * Order.Factorial()],
                var o when o >= p.Length => [Complex.Zero],
                _ => GetDifferential(p, new Complex[p.Length - Order], Order)
            }
        };

        /// <summary>Вычислить значение производной полинома</summary>
        /// <param name="x">Аргумент</param>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="Order">Порядок производной. 0 - значение полинома, 1 - производная 1-го порядка, 2 - производная 2-го порядка и т.д.</param>
        /// <returns>Значение производной полинома в точке <paramref name="x"/></returns>
        public static double GetDifferentialValue(double x, double[] p, int Order = 1)
        {
            var result = 0d;

            switch (Order)
            {
                case 0:
                    for (var i = p.Length - 1; i >= 0; i--)
                        result = result * x + p[i];
                    break;

                case 1:
                    for (var i = p.Length - 1; i >= 1; i--)
                        result = result * x + p[i] * i;
                    break;

                case 2:
                    for (var i = p.Length - 1; i >= 2; i--)
                        result = result * x + p[i] * i * (i - 1);
                    break;

                case 3:
                    for (var i = p.Length - 1; i >= 3; i--)
                        result = result * x + p[i] * i * (i - 1) * (i - 2);
                    break;

                default:
                    var k = p.Length - 1;
                    for (var i = 2; i <= Order; i++)
                        k *= p.Length - i;

                    for (var i = p.Length - 1; i >= Order; i--)
                    {
                        result = result * x + p[i] * k;
                        k = k / i * (i - Order);
                    }
                    break;
            }

            return result;
        }

        /// <summary>Вычислить производную от полинома</summary>
        /// <param name="p">Полином</param>
        /// <param name="Order">Порядок дифференциала. Default = 1</param>
        /// <exception cref="ArgumentNullException"><paramref name="p"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="Order"/> is less than 0</exception>
        public static void Differential(double[] p, int Order = 1)
        {
            if (p is null)
                throw new ArgumentNullException(nameof(p));
            if (Order < 0)
                throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок дифференциала не может быть меньше 0");
            if (Order == 0)
                return;

            for (var n = p.Length - 1; n > 0 && Order > 0; n--, Order--)
            {
                (var last, p[n]) = (p[n], 0);
                for (var i = n - 1; i >= 0; i--)
                    (p[i], last) = (last * (i + 1), p[i]);
            }
        }

        /// <summary>Интеграл полинома</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        /// <param name="Order">Кратность интегрирования</param>
        /// <returns>Массив коэффициентов полинома - интеграла</returns>
        public static double[] GetIntegral(double[] p, double C = 0, int Order = 1) => p switch
        {
            null => throw new ArgumentNullException(nameof(p)),
            _ => Order switch
            {
                0 => p,
                < 0 => GetDifferential(p, -Order),
                _ => Integral(p, C, new double[p.Length + Order], Order)
            }
        };

        /// <summary>Интеграл полинома</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        /// <param name="Order">Кратность интегрирования</param>
        /// <returns>Массив коэффициентов полинома - интеграла</returns>
        public static Complex[] GetIntegral(Complex[] p, Complex C = default, int Order = 1) => p switch
        {
            null => throw new ArgumentNullException(nameof(p)),
            _ => Order switch
            {
                0 => p,
                < 0 => GetDifferential(p, -Order),
                _ => Integral(p, C, new Complex[p.Length + Order])
            }
        };

        /// <summary>Вычисляет интеграл полинома заданного порядка</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        /// <param name="result">Массив для хранения коэффициентов интеграла</param>
        /// <param name="Order">Кратность интегрирования</param>
        /// <returns>Массив коэффициентов полинома - интеграла</returns>
        /// <exception cref="ArgumentException">Если размер массива результата не соответствует ожидаемому</exception>
        public static double[] Integral(double[] p, double C, double[] result, int Order = 1)
        {
            var length = p.Length;
            if (result.Length != length + Order)
                throw new ArgumentException($"Размер массива результата должен быть на {Order} больше массива коэффициентов полинома")
                {
                    Data =
                    {
                        { "p.Length", p.Length },
                        { "result.Length", p.Length },
                        { "Order", Order },
                    }
                };

            result[0] = C;

            for (var n = 0; n < Order; n++)
            {
                for (var i = length; i > 0; i--)
                    result[i] = p[i - 1] / i;
                p = result;
                length++;
            }

            return result;
        }

        /// <summary>Вычисляет интеграл полинома заданного порядка</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        /// <param name="result">Массив для хранения коэффициентов интеграла</param>
        /// <param name="Order">Кратность интегрирования</param>
        /// <returns>Массив коэффициентов полинома - интеграла</returns>
        /// <exception cref="ArgumentException">Если размер массива результата не соответствует ожидаемому</exception>
        public static Complex[] Integral(Complex[] p, Complex C, Complex[] result, int Order = 1)
        {
            var length = p.Length;
            if (result.Length != length + Order)
                throw new ArgumentException($"Размер массива результата должен быть на {Order} больше массива коэффициентов полинома")
                {
                    Data =
                    {
                        { "p.Length", p.Length },
                        { "result.Length", p.Length },
                        { "Order", Order },
                    }
                };

            result[0] = C;

            for (var n = 0; n < Order; n++)
            {
                for (var i = length; i > 0; i--)
                    result[i] = p[i - 1] / i;
                p = result;
                length++;
            }

            return result;
        }

        /// <summary>Вычисляет интеграл полинома заданного порядка</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        public static void Integral(double[] p, double C = 0)
        {
            for (var i = p.Length - 1; i >= 1; i--)
                p[i] = p[i - 1] / i;
            p[0] = C;
        }

        /// <summary>Вычисляет интеграл полинома заданного порядка</summary>
        /// <param name="p">Массив коэффициентов полинома</param>
        /// <param name="C">Константа интегрирования</param>
        /// <remarks>Массив <paramref name="p"/> будет изменен</remarks>
        public static void Integral(Complex[] p, Complex C = default)
        {
            for (var i = p.Length - 1; i >= 1; i--)
                p[i] = p[i - 1] / i;
            p[0] = C;
        }

        #endregion

        #region Операторы

        /// <summary>Суммирование полиномов</summary>
        /// <param name="p">Коэффициенты полинома - первого слагаемого</param>
        /// <param name="q">Коэффициенты полинома - первого слагаемого</param>
        /// <returns>Коэффициенты полинома - суммы</returns>
        public static double[] Sum(double[] p, double[] q)
        {
            if (p is null)
                throw new ArgumentNullException(nameof(p));
            if (q is null)
                throw new ArgumentNullException(nameof(q));

            var length_p = p.Length;
            var length_q = q.Length;
            if (length_p > length_q)
            {
                var result = (double[])p.Clone();
                for (var i = 0; i < length_q; i++)
                    result[i] = p[i] + q[i];
                return result;
            }
            else
            {
                var result = (double[])q.Clone();
                for (var i = 0; i < length_p; i++)
                    result[i] = p[i] + q[i];
                return result;
            }
        }

        /// <summary>Разность полиномов</summary>
        /// <param name="p">Коэффициенты полинома - первого уменьшаемого</param>
        /// <param name="q">Коэффициенты полинома - первого вычитаемого</param>
        /// <returns>Коэффициенты полинома - разности</returns>

        public static double[] Subtract(double[] p, double[] q)
        {
            if (p is null)
                throw new ArgumentNullException(nameof(p));
            if (q is null)
                throw new ArgumentNullException(nameof(q));

            var length_p = p.Length;
            var length_q = q.Length;
            if (length_p > length_q)
            {
                var result = (double[])p.Clone();
                for (var i = 0; i < length_q; i++)
                    result[i] -= q[i];
                return result;
            }
            else
            {
                var result = (double[])q.Clone();
                for (var i = 0; i < length_p; i++)
                    result[i] = p[i] - q[i];
                for (var i = length_p; i < length_q; i++)
                    result[i] = -q[i];

                return result;
            }
        }

        /// <summary>Деление полиномов</summary>
        /// <param name="dividend">Коэффициенты полинома - делимое</param>
        /// <param name="divisor">Коэффициенты полинома - делитель</param>
        /// <param name="quotient">Коэффициенты полинома - частного</param>
        /// <param name="remainder">Коэффициенты полинома - остаток от деления</param>
        /// <returns>Коэффициенты полинома - частное</returns>
        // ReSharper disable once EmptyString
        [Copyright("", url = "http://losev-al.blogspot.ru/2012/09/blog-post_14.htm")]
        public static void Divide(double[] dividend, double[] divisor, out double[] quotient, out double[] remainder)
        {
            if (dividend[^1] == 0)
                throw new ArithmeticException("Старший член многочлена делимого не может быть 0");
            if (divisor[^1] == 0)
                throw new ArithmeticException("Старший член многочлена делителя не может быть 0");

            remainder = (double[])dividend.Clone();
            quotient = new double[remainder.Length - divisor.Length + 1];
            for (var i = 0; i < quotient.Length; i++)
            {
                var k = remainder[^(i + 1)] / divisor[^1];
                quotient[quotient.Length - i - 1] = k;
                for (var j = 0; j < divisor.Length; j++)
                    remainder[^(i + j + 1)] -= k * divisor[^(j + 1)];
            }
        }

        /// <summary>Умножение полиномов</summary>
        /// <param name="p">Коэффициенты полинома - первый сомножитель</param>
        /// <param name="q">Коэффициенты полинома - второй сомножитель</param>
        /// <returns>Коэффициенты полинома - произведение</returns>
        public static double[] Multiply(double[] p, double[] q)
        {
            var length = p.Length + q.Length;
            var a = new double[length - 1];
            for (var i = 0; i < p.Length; i++)
                for (var j = 0; j < q.Length; j++)
                    a[i + j] += p[i] * q[j];

            var zeros_count = 0;
            for (var i = a.Length - 1; i >= 0 && a[i] == 0; i--)
                zeros_count++;

            if (zeros_count == 0) return a;
            if (zeros_count == a.Length) return [];
            SArray.Resize(ref a, a.Length - zeros_count);
            return a;
        }

        /// <summary>Возведение полинома в целую степень</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="Power">Степень</param>
        /// <returns>Коэффициенты полинома, возведенного в указанную степень</returns>
        /// <exception cref="ArgumentOutOfRangeException">Степень должна быть больше, либо равна 0</exception>
        public static double[] Power(double[] p, int Power)
        {
            switch (Power)
            {
                case 0: return [1d];
                case 1: return p;
                case < 0: throw new ArgumentOutOfRangeException(nameof(Power), Power, "Степень должна быть больше, либо равна 0");
            }

            var length = p.Length * Power;
            var result = new double[length];
            var q = new double[length];
            p.CopyTo(q, 0);

            var a_length = p.Length;
            var q_length = a_length;

            for (var n = 0; n < Power; n++)
            {
                for (var i = 0; i < p.Length; i++)
                    for (var j = 0; j < q_length; j++)
                        result[i + j] += p[i] * q[j];

                q_length += a_length;
                (result, q) = (q, result);
            }

            return result;
        }

        /// <summary>Сложение полинома с вещественным числом</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - суммы</returns>

        public static double[] Add(double[] p, double x)
        {
            var result = new double[p.NotNull().Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = p[i] + x;
            return result;
        }

        /// <summary>Вычитание вещественного числа из полинома</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - разности</returns>

        public static double[] Subtract(double[] p, double x)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = p[i] - x;
            return result;
        }

        /// <summary>Вычитание полинома из вещественного числа</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - разности</returns>

        public static double[] Subtract(double x, double[] p)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = x - p[i];
            return result;
        }

        /// <summary>Отрицание полинома</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <returns>Коэффициенты полинома Q(x) = 0 - P(x)</returns>

        public static double[] Negate(double[] p)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = -p[i];
            return result;
        }

        /// <summary>Умножение полинома на вещественное число</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - произведения</returns>

        public static double[] Multiply(double[] p, double x)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = p[i] * x;
            return result;
        }

        /// <summary>Деление полинома на вещественное число</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - частного</returns>

        public static double[] Divide(double[] p, double x)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = p[i] / x;
            return result;
        }

        /// <summary>Скалярное деление полинома на вещественное число</summary>
        /// <param name="p">Коэффициенты полинома</param>
        /// <param name="x">Вещественное число</param>
        /// <returns>Коэффициенты полинома - частного</returns>

        public static double[] DivideScalar(double x, double[] p)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            var result = new double[p.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = x / p[i];
            return result;
        }

        #endregion
    }

    /// <summary>Результат деления полиномов</summary>
    /// <remarks>Инициализация результата деления полиномов</remarks>
    /// <param name="Divisor"></param>
    /// <param name="Result">Частное</param>
    /// <param name="Remainder">Остаток от деления</param>
    public readonly struct PolynomDivisionResult(Polynom Result, Polynom Remainder, Polynom Divisor)
    {
        /// <summary>Частное полиномов</summary>
        public Polynom Result { get; } = Result;

        /// <summary>Остаток деления полиномов</summary>
        public Polynom Remainder { get; } = Remainder;

        /// <summary>Полином - делитель</summary>
        public Polynom Divisor { get; } = Divisor;

        /// <summary>Значение результата деления полиномов</summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Результат вычисления значения результата деления полиномов</returns>
        public double Value(double x) => Result.Value(x) + Remainder.Value(x) / Divisor.Value(x);

        /// <summary>Получить функцию</summary>
        /// <returns>Функция вычисления значения результата деления полиномов</returns>

        public Func<double, double> GetFunction() => Value;

        /// <inheritdoc />
        public override string ToString() => $"({Result.ToMathString()}) + ({Remainder.ToMathString()}) / ({Divisor.ToMathString()})";

        /// <summary>Оператор неявного преобразования результата деления полиномов в полином результата</summary>
        /// <param name="Result">Результат деления полиномов</param>
        /// <returns>Частное</returns>
        public static implicit operator Polynom(PolynomDivisionResult Result) => Result.Result;
    }

    /// <summary>Случайный полином</summary>
    /// <param name="Power">Степень полинома</param>
    /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
    /// <param name="Da">Дисперсия коэффициентов полинома</param>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <returns>Случайный полином</returns>
    public static Polynom Random(int Power = 3, double Ma = 0, double Da = 1, Random? rnd = null)
    {
        rnd ??= __PolynomRandom.Value;
        var a = new double[Power];
        for (var i = 0; i < Power; i++)
            a[i] = (rnd.NextDouble() - .5) * Da + Ma;
        return new(a);
    }

    /// <summary>Случайный полином случайной степени</summary>
    /// <param name="MaxPower">Максимальная степень полинома</param>
    /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
    /// <param name="Da">Дисперсия коэффициентов полинома</param>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <returns>Случайный полином</returns>
    public static Polynom RandomPower(int MaxPower = 5, double Ma = 0, double Da = 1, Random? rnd = null) =>
        Random((rnd ?? __PolynomRandom.Value).Next(MaxPower), Ma, Da);

    /// <summary>Случайный полином с целыми коэффициентами</summary>
    /// <param name="Power">Степень полинома</param>
    /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
    /// <param name="Da">Дисперсия коэффициентов полинома</param>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <returns>Случайный полином</returns>
    public static Polynom RandomWithIntCoefficients(int Power, int Ma = 0, int Da = 10, Random? rnd = null)
    {
        rnd ??= __PolynomRandom.Value;
        var a = new double[Power];
        for (var i = 0; i < Power; i++)
            a[i] = Math.Round(rnd.Next(Da) - .5 * Da + Ma);
        return new(a);
    }

    /// <summary>Случайный полином с целыми коэффициентами случайной степени</summary>
    /// <param name="MaxPower">Максимальная степень полинома</param>
    /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
    /// <param name="Da">Дисперсия коэффициентов полинома</param>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <returns>Случайный полином</returns>
    public static Polynom RandomWithIntCoefficients_P(int MaxPower = 5, int Ma = 0, int Da = 10, Random? rnd = null) =>
        RandomWithIntCoefficients((rnd ?? __PolynomRandom.Value).Next(MaxPower), Ma, Da);

    /// <summary>Создать полином по его коэффициентам</summary>
    /// <param name="Coefficients">Массив коэффициентов полинома, начиная с свободного члена</param>
    /// <returns>Полином</returns>
    public static Polynom PolynomCoefficients(params double[] Coefficients) =>
        new(Coefficients.FilterNullValuesFromEnd());
}