using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using MathCore.Vectors;
// ReSharper disable UnusedMember.Global

namespace MathCore.Interpolation
{
    /// <summary><see url="http://ru.wikipedia.org/wiki/Кривая_Безье">Кривая Безье</see></summary>
    [Hyperlink("http://ru.wikipedia.org/wiki/Кривая_Безье")]
    public class BezierCurve : Interpolator
    {
        /* -------------------------------------------------------------------------------------------- */

        #region Статические члены

        /// <summary>
        /// Биномиальный коэффициент (1+x)^n из <paramref name="n"/> по <paramref name="k"/>
        /// <see url="http://ru.wikipedia.org/wiki/Биномиальный_коэффициент">Википедия:Биномиальный коэффициент</see>
        /// </summary>
        /// <param name="n">Степень <see url="http://ru.wikipedia.org/wiki/Бином_Ньютона">бинома Ньютона</see></param>
        /// <param name="k">Номер коэффициента</param>
        /// <returns>Коэффициент разложения Бинома Ньютона (1+x)^n</returns>
        [Hyperlink("http://ru.wikipedia.org/wiki/Биномиальный_коэффициент")]
        private static int BinomCoefficient(int n, int k)
        {
            if(k < 0 || (0 <= n && n < k)) return 0;
            var K = 1L;
            if (n >= 0 || 0 > K) return (int)(K * n.Factorial() / (k.Factorial() - (n - k).Factorial()));
            K = k % 2 == 0 ? 1 : -1;
            n = -n + k - 1;
            return (int)(K * n.Factorial() / (k.Factorial() - (n - k).Factorial()));
        }

        /// <summary>Получить <seealso url="http://ru.wikipedia.org/wiki/Многочлен_Бернштейна">Полином Бернштейна</seealso>></summary>
        /// <param name="k">Номер многочлена</param>
        /// <param name="n">Степень</param>
        /// <returns></returns>
        [Hyperlink("http://ru.wikipedia.org/wiki/Многочлен_Бернштейна")]
        [NotNull]
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
        //private Vector2D[] _SortedPoints;

        /// <summary><see url="http://ru.wikipedia.org/wiki/Многочлен_Бернштейна">Полином Бернштейна</see>></summary>        
        private Func<double, double>[] _BernshteynPolynoms;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Новая <see url="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="X">Список координат точек x</param>
        /// <param name="Y">Список координат точек y</param>
        public BezierCurve([NotNull] IEnumerable<double> X, [NotNull] IEnumerable<double> Y)
        {
            var x = X.Select((xx, i) => (X:xx, i));
            var y = Y.Select((yy, i) => (Y:yy, i));
            Initialize(x.Join(y, xx => xx.i, yy => yy.i, (xx, yy) => new Vector2D(xx.X, yy.Y)));
        }

        /// <summary>Новая <see url="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="Points">Набор точек в виде <see cref="MathCore.Complex">комплексных чисел</see></param>
        public BezierCurve([NotNull] IEnumerable<Complex> Points) => Initialize(Points.Select(c => (Vector2D)c));

        /// <summary>Новая <see url="http://ru.wikipedia.org/wiki/Кривая_Безье">кривая Безье</see></summary>
        /// <param name="Points">Набор точек</param>
        public BezierCurve([NotNull] IEnumerable<Vector2D> Points) => Initialize(Points);

        /// <summary>Инициализировать кривую Безье</summary>
        /// <param name="Points">Набор точек</param>
        private void Initialize([NotNull] IEnumerable<Vector2D> Points)
        {
            _Points = Points.ToArray();
            //{
            //    var points = _Points.ToList();
            //    points.Sort((v1, v2) => v1.X > v2.X ? 1 : v1.X.Equals(v2.X) ? 0 : -1);
            //    //_SortedPoints = points.ToArray();
            //}
            var count = _Points.Length;
            _BernshteynPolynoms = new Func<double, double>[count];
            for(var i = 0; i < count; i++)
                _BernshteynPolynoms[i] = GetBernshteynPolynom(i, count);
        }

        /* -------------------------------------------------------------------------------------------- */

        public Vector2D B(double t)
        {
            if(t is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(t), "t в не интервала [0;1]");

            var count = _Points.Length;
            var x = 0d;
            var y = 0d;
            for(var i = 0; i < count; i++)
            {
                var b = _BernshteynPolynoms[i](t);
                x += _Points[i].X * b;
                y += _Points[i].Y * b;
            }
            return new Vector2D(x, y);
        }

        /* -------------------------------------------------------------------------------------------- */
    }
}