using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using MathCore.Vectors;

namespace MathCore.Interpolation
{
    /// <summary><see cref="http://ru.wikipedia.org/wiki/Кривая_Безье">Кривая Безье</see></summary>
    [Hyperlink("http://ru.wikipedia.org/wiki/Кривая_Безье")]
    public class BezierCurve
    {
        /* -------------------------------------------------------------------------------------------- */

        #region Статические члены

        /// <summary>
        /// Биномиальный коэффициент (1+x)^n из <paramref name="n"/> по <paramref name="k"/>
        /// <see cref="http://ru.wikipedia.org/wiki/Биномиальный_коэффициент">Википедия:Биномиальный коэффициент</see>>
        /// </summary>
        /// <param name="n">Степень <see cref="http://ru.wikipedia.org/wiki/Бином_Ньютона">бинома Ньютона</see>></param>
        /// <param name="k">Номер коэффициента</param>
        /// <returns>Коэффициент разложения Бинома Ньютона (1+x)^n</returns>
        [Hyperlink("http://ru.wikipedia.org/wiki/Биномиальный_коэффициент")]
        private static int BinomCoefficient(int n, int k)
        {
            if(k < 0 || (0 <= n && n < k)) return 0;
            var K = 1L;
            if(n < 0 && 0 <= K)
            {
                K = k % 2 == 0 ? 1 : -1;
                n = -n + k - 1;
            }
            return (int)(K * n.Factorial() / (k.Factorial() - (n - k).Factorial()));
        }

        /// <summary>Получить <see cref="http://ru.wikipedia.org/wiki/Многочлен_Бернштейна">Полином Бернштейна</see>></summary>
        /// <param name="k">Номер многочлена</param>
        /// <param name="n">Степень</param>
        /// <returns></returns>
        [Hyperlink("http://ru.wikipedia.org/wiki/Многочлен_Бернштейна")]
        private static Func<double, double> GetBernshteynPolynom(int k, int n)
        {
            var B = BinomCoefficient(n, k);
            var I = (double)k;
            var K = (double)(n - k);
            return t => B * Math.Pow(t, I) * Math.Pow(1 - t, K);
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Набор точек аппроксимации</summary>
        private Vector2D[] _Points;
        private Vector2D[] _SortedPoints;

        /// <summary><see cref="http://ru.wikipedia.org/wiki/Многочлен_Бернштейна">Полином Бернштейна</see>></summary>        
        private Func<double, double>[] _BernshteynPolynoms;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Новая <see cref="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="X">Список координато точек x</param>
        /// <param name="Y">Список кординат точек y</param>
        public BezierCurve(IEnumerable<double> X, IEnumerable<double> Y)
        {
            Contract.Requires(X.Count() == Y.Count());
            var x = X.Select((xx, i) => new { X = xx, i });
            var y = Y.Select((yy, i) => new { Y = yy, i });
            Initialize(x.Join(y, xx => xx.i, yy => yy.i, (xx, yy) => new Vector2D(xx.X, yy.Y)));
        }

        /// <summary>Новая <see cref="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="Points">Набор точек в виде <see cref="MathCore.Complex">комплексных чисел</see></param>
        public BezierCurve(IEnumerable<Complex> Points)
        {
            Contract.Requires(Points != null);
            Initialize(Points.Select(c => (Vector2D)c));
        }

        /// <summary>Новая <see cref="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="Points">Набор точек</param>
        public BezierCurve(IEnumerable<Vector2D> Points)
        {
            Contract.Requires(Points != null);
            Initialize(Points);
        }

        /// <summary>Инициализировать кривую Безье</summary>
        /// <param name="Points">Набор точек</param>
        private void Initialize(IEnumerable<Vector2D> Points)
        {
            _Points = Points.ToArray();
            {
                var points = _Points.ToList();
                points.Sort((v1, v2) => v1.X > v2.X ? 1 : (v1.X.Equals(v2.X) ? 0 : -1));
                _SortedPoints = points.ToArray();
            }
            var lv_N = _Points.Length;
            _BernshteynPolynoms = new Func<double, double>[lv_N];
            for(var i = 0; i < lv_N; i++)
                _BernshteynPolynoms[i] = GetBernshteynPolynom(i, lv_N);
        }

        /* -------------------------------------------------------------------------------------------- */

        public Vector2D B(double t)
        {
            Contract.Requires(t >= 0);
            Contract.Requires(t <= 1);

            if(t < 0 || t > 1)
                throw new ArgumentOutOfRangeException(nameof(t), "t в не интервала [0;1]");

            var lv_N = _Points.Length;
            var x = 0d;
            var y = 0d;
            for(var i = 0; i < lv_N; i++)
            {
                var b = _BernshteynPolynoms[i](t);
                x += _Points[i].X * b;
                y += _Points[i].Y * b;
            }
            return new Vector2D(x, y);
        }



        /* -------------------------------------------------------------------------------------------- */
    }

    internal class BezierCurve2
    {
        private double[] FactorialLookup;

        public BezierCurve2() => CreateFactorialTable();

        // just check if n is appropriate, then return the result
        private double FastFactorial(int n)
        {
            if(n < 0) throw new Exception("n is less than 0");
            if(n > 32) return n.Factorial();

            return FactorialLookup[n]; /* returns the value n! as a SUMORealing point number */
        }

        // create lookup table for fast Factorial calculation
        private void CreateFactorialTable() => FactorialLookup = new[]
        {
            1d,
            1d,
            2d,
            6d,
            24d,
            120d,
            720d,
            5040d,
            40320d,
            362880d,
            3628800d,
            39916800d,
            479001600d,
            6227020800d,
            87178291200d,
            1307674368000d,
            20922789888000d,
            355687428096000d,
            6402373705728000d,
            121645100408832000d,
            2432902008176640000d,
            51090942171709440000d,
            1124000727777607680000d,
            25852016738884976640000d,
            620448401733239439360000d,
            15511210043330985984000000d,
            403291461126605635584000000d,
            10888869450418352160768000000d,
            304888344611713860501504000000d,
            8841761993739701954543616000000d,
            265252859812191058636308480000000d,
            8222838654177922817725562880000000d,
            263130836933693530167218012160000000d
        };

        private double Ni(int n, int i) => FastFactorial(n) / (FastFactorial(i) * FastFactorial(n - i));

        // Calculate Bernstein basis
        private double Bernstein(int n, int i, double t)
        {
            double ti; /* t^i */
            double tni; /* (1 - t)^i */

            /* Prevent problems with pow */

            if(t.Equals(0d) && i == 0)
                ti = 1.0;
            else
                ti = Math.Pow(t, i);

            if(n == i && t.Equals(1d))
                tni = 1.0;
            else
                tni = Math.Pow(1 - t, n - i);

            //Bernstein basis
            var basis = Ni(n, i) * ti * tni;
            return basis;
        }

        public void Bezier2D([NotNull]double[] b, int cpts, [NotNull]double[] p)
        {
            var npts = b.Length / 2;

            // Calculate points on curve

            var icount = 0;
            var t = 0d;
            var step = 1d / (cpts - 1);

            for(var i1 = 0; i1 != cpts; i1++)
            {
                if(1.0 - t < 5e-6)
                    t = 1.0;

                var jcount = 0;
                p[icount] = 0.0;
                p[icount + 1] = 0.0;
                for(var i = 0; i != npts; i++)
                {
                    var basis = Bernstein(npts - 1, i, t);
                    p[icount] += basis * b[jcount];
                    p[icount + 1] += basis * b[jcount + 1];
                    jcount = jcount + 2;
                }

                icount += 2;
                t += step;
            }
        }
    }
}