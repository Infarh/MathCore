using System;

namespace MathCore.DifferencialEquations.Numerical
{
    /// <summary>
    /// Метод Рунге-Кутты
    /// </summary>
    public class RungeKutta
    {

        private readonly DifferencialEquationsSystem _System;
        private readonly ArgumentFunction _X;

        private readonly int _N;

        private double _t; // текущее время 

        private double[] _Y;

        private readonly double[] _Yy; // внутренние переменные 
        //private double[] Y1; // внутренние переменные 
        //private double[] Y2; // внутренние переменные 
        //private double[] Y3; // внутренние переменные 
        //private double[] Y4; // внутренние переменные

        /// <summary>
        /// Размерность системы
        /// </summary>
        public int N => _N;

        /// <summary>
        /// Искомые решения
        /// </summary>
        public double[] Y => _Y;

        /// <summary>
        /// Текущее время
        /// </summary>
        public double t => _t;

        /// <summary>
        /// Метод Рунге-Кутты
        /// </summary>
        /// <param name="N">Размерность</param>
        /// <param name="System">Решаемая система</param>
        /// <param name="X">Производящая функция аргумента</param>
        protected RungeKutta(int N, DifferencialEquationsSystem System, ArgumentFunction X)
        {
            _X = X;
            _System = System;

            _N = N; // сохранить размерность системы

            if(N < 1)
                throw new ArgumentOutOfRangeException(nameof(N), "Размерность системы - величина положительная");


            _Y = new double[N]; // создать вектор решения
            _Yy = new double[N]; // и внутренних решений
            //Y1 = new double[N];
            //Y2 = new double[N];
            //Y3 = new double[N];
            //Y4 = new double[N];
        }

        /// <summary>
        /// Начальные условия
        /// </summary>
        /// <param name="t0">Начальное время</param>
        /// <param name="Y0">Начальные условия</param>
        public void Initialize(double t0, params double[] Y0)
        {
            if(Y0.Length != _N)
                throw new ArgumentOutOfRangeException(nameof(Y0), "Размер вектора начальных значений не соответствует размерности решения метода");

            _t = t0;
            Array.Copy(Y0, _Y, Y0.Length);
        }

        //public abstract void F(double t, double[] Y, ref double[] dY); // правые части системы.

        /// <summary>
        /// Рассчёт решения
        /// </summary>
        /// <param name="dt">Шаг</param>
        public double[] NextStep(double dt)
        {
            if(dt <= 0)
                throw new ArgumentOutOfRangeException(nameof(dt), "Шаг должен быть больше нуля");

            var dt2 = dt / 2;

            int i;

            var X = _X(t);
            var X2 = _X(t + dt / 2);
            var X3 = _X(t + dt);

            if(X.Length != _N || X2.Length != _N || X3.Length != _N)
                throw new ArgumentException("Функция определения аргумента вернула вектор аргумента размера, не соответствующего размерности системы");

            var lv_Y = (double[])_Y.Clone();

            // рассчитать Y1
            //F(_t, Y, ref _Y1);
            var Y1 = _System(X, lv_Y);

            for(i = 0; i < N; i++)
                _Yy[i] = lv_Y[i] + Y1[i] * dt2;

            // рассчитать Y2
            //F(_t + dt / 2, _Yy, ref _Y2);
            var Y2 = _System(X2, _Yy);

            for(i = 0; i < N; i++)
                _Yy[i] = lv_Y[i] + Y2[i] * dt2;

            // рассчитать Y3
            //F(_t + dt / 2, _Yy, ref _Y3);
            var Y3 = _System(X2, _Yy);


            for(i = 0; i < N; i++)
                _Yy[i] = lv_Y[i] + Y3[i] * dt;

            // рассчитать Y4
            //F(_t + dt, _Yy, ref _Y4);
            var Y4 = _System(X3, _Yy);

            // рассчитать решение на новом шаге
            for(i = 0; i < N; i++)
                lv_Y[i] += dt / 6 * (Y1[i] + 2 * Y2[i] + 2 * Y3[i] + Y4[i]);

            // увеличить шаг
            _t += dt;

            return _Y = lv_Y;
        }
    }
}