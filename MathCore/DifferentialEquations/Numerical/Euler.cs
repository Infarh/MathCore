using System;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore.DifferentialEquations.Numerical
{
    // ReSharper disable CommentTypo
    /// <summary>Метод Рунге-Кутты</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE1006:Стили именования", Justification = "<Ожидание>")]
    public class Euler  //todo: Почему в комментарии указан метод Рунге-Кутты, а имя класса Эйлер?
    // ReSharper restore CommentTypo
    {
        private readonly DifferentialEquationsSystem _System;

        private readonly int _N;

        private double _t; // текущее время 

        private readonly double[] _X;

        /// <summary>Размерность системы</summary>
        public int N => _N;

        /// <summary>Искомые решения</summary>
        public double[] X => _X;


        /// <summary>Текущее время</summary>
        public double t => _t;

        // ReSharper disable CommentTypo
        /// <summary>Метод Рунге-Кутты</summary>
        /// <param name="N">Размерность</param>
        /// <param name="System">Решаемая система</param>
        // ReSharper restore CommentTypo
        protected Euler(int N, DifferentialEquationsSystem System)
        {
            if (N < 1) throw new ArgumentOutOfRangeException(nameof(N), "Размерность системы - величина положительная");

            _System = System;
            _N = N;
            _X = new double[N];
        }

        /// <summary>Начальные условия</summary>
        /// <param name="t0">Начальное время</param>
        /// <param name="Y0">Начальные условия</param>
        public void Initialize(double t0, [NotNull] params double[] Y0)
        {
            if (Y0.Length != _N)
                throw new ArgumentOutOfRangeException(nameof(Y0), "Размер вектора начальных значений не соответствует размерности решения метода");

            _t = t0;
            Array.Copy(Y0, _X, Y0.Length);
        }

        /// <summary>Расчёт решения</summary>
        /// <param name="dt">Шаг</param>
        [NotNull]
        public double[] Next(double dt)
        {
            if (dt <= 0)
                throw new ArgumentOutOfRangeException(nameof(dt), "Шаг должен быть больше нуля");

            var dx = _System(t, _X);
            for (var i = 0; i < _X.Length; i++)
                _X[i] += dt * dx[i];

            _t += dt;
            return _X;
        }
    }
}