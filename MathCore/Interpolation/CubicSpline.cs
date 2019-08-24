using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathCore.Annotations;
using MathCore.Vectors;

namespace MathCore.Interpolation
{
    /// <summary>Интерполирование функций естественными кубическими сплайнами</summary>
    /// <remarks>Разработчик: Назар Андриенко Email: nuzikprogrammer@gmail.com</remarks>
    public class CubicSpline : IInterpolator
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Структура, описывающая сплайн на каждом сегменте сетки</summary>
        private struct SplineState
        {
            public readonly double a;
            public double b;
            public double c;
            public double d;
            public double x;

            public SplineState(double a, double b, double c, double d, double x)
            { this.a = a; this.b = b; this.c = c; this.d = d; this.x = x; }

            public SplineState(double a, double x) { this.a = a; this.x = x; b = 0; c = 0; d = 0; }
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Сплайн</summary>
        private SplineState[] _SplinStates;

        /* -------------------------------------------------------------------------------------------- */

        public double this[double x] => Value(x);


        /* -------------------------------------------------------------------------------------------- */

        public CubicSpline(double[] X, double[] Y)
        {
            Contract.Requires(X != null);
            Contract.Requires(Y != null);
            Initialize(X, Y);
        }

        public CubicSpline(IList<Complex> Points)
        {
            Contract.Requires(Points != null);
            var lv_X = new double[Points.Count];
            var lv_Y = new double[Points.Count];
            for(var i = 0; i < Points.Count; i++)
            {
                lv_X[i] = Points[i].Re;
                lv_Y[i] = Points[i].Im;
            }
            Initialize(lv_X, lv_Y);
        }

        public CubicSpline(IList<Vector2D> Points)
        {
            Contract.Requires(Points != null);
            var lv_X = new double[Points.Count];
            var lv_Y = new double[Points.Count];
            for(var i = 0; i < Points.Count; i++)
            {
                lv_X[i] = Points[i].X;
                lv_Y[i] = Points[i].Y;
            }
            Initialize(lv_X, lv_Y);
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Инициализация сплайна</summary>
        /// <param name="X">Массив аргументов</param><param name="Y">Массив значений</param>
        /// <exception cref="ArgumentException">Возникает в случае, если размерности массивов не равны</exception>
        public void Initialize(double[] X, double[] Y)
        {
            Contract.Requires(X != null);
            Contract.Requires(Y != null);
            Contract.Requires(X.Length == Y.Length, "Размеры массивов должны совпадать");
            if(X.Length != Y.Length) throw new ArgumentException("Размеры массивов должны совпадать");

            var lv_N = X.Length;
            _SplinStates = new SplineState[lv_N];

            for(var i = 0; i < lv_N; i++)
                _SplinStates[i] = new SplineState(Y[i], X[i]);

            _SplinStates[0].c = _SplinStates[lv_N - 1].c = 0;

            // Решение СЛАУ относительно коэфициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
            // Вычисление прогоночных коэфициентов - прямой ход метода прогонки
            var alpha = new double[lv_N - 1];
            var beta = new double[lv_N - 1];
            alpha[0] = beta[0] = 0;
            for(var i = 1; i < lv_N - 1; i++)
            {
                var h_i = X[i] - X[i - 1];
                var h_i1 = X[i + 1] - X[i];
                var A = h_i;
                var C = 2 * (h_i + h_i1);
                var B = h_i1;
                var F = 6 * ((Y[i + 1] - Y[i]) / h_i1 - (Y[i] - Y[i - 1]) / h_i);
                var z = (A * alpha[i - 1] + C);
                alpha[i] = -B / z;
                beta[i] = (F - A * beta[i - 1]) / z;
            }

            // Нахождение решения - обратный ход метода прогонки
            for(var i = lv_N - 2; i > 0; i--)
                _SplinStates[i].c = alpha[i] * _SplinStates[i + 1].c + beta[i];

            // По известным коэфициентам c[i] находим значения b[i] и d[i]
            for(var i = lv_N - 1; i > 0; i--)
            {
                var h_i = X[i] - X[i - 1];
                _SplinStates[i].d = (_SplinStates[i].c - _SplinStates[i - 1].c) / h_i;
                _SplinStates[i].b = h_i * (2 * _SplinStates[i].c + _SplinStates[i - 1].c) / 6 + (Y[i] - Y[i - 1]) / h_i;
            }
        }

        /* -------------------------------------------------------------------------------------------- */

        public double Value(double x)
        {
            var lv_N = _SplinStates.Length;
            SplineState lv_State;
            if(x <= _SplinStates[0].x) // Если x меньше точки сетки x[0] - пользуемся первым эл-тов массива
                lv_State = _SplinStates[0];
            else if(x >= _SplinStates[lv_N - 1].x) // Если x больше точки сетки x[n - 1] - пользуемся последним эл-том массива
                lv_State = _SplinStates[lv_N - 1];
            else // Иначе x лежит между граничными точками сетки - производим бинарный поиск нужного эл-та массива
            {
                int i = 0, j = lv_N - 1;
                while(i + 1 < j)
                {
                    var k = i + (j - i) / 2;
                    if(x <= _SplinStates[k].x) j = k; else i = k;
                }
                lv_State = _SplinStates[j];
            }

            var dx = (x - lv_State.x);
            // Вычисляем значение сплайна в заданной точке по схеме Горнера 
            return lv_State.a + (lv_State.b + (lv_State.c / 2 + lv_State.d * dx / 6) * dx) * dx;
        }

        [NotNull]
        public Func<double, double> GetFunction()
        {
            Contract.Ensures(Contract.Result<Func<double, double>>() != null);
            return Value;
        }
    }
}
