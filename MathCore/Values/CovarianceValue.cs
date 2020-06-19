using System;
using MathCore.Annotations;
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Values
{
    /// <summary>
    /// Скользащая ковариация
    /// </summary>
    public class CovarianceValue : IValue<double>, IResettable
    {
        private long _Count = 1;
        private double _AverageX;
        private double _AverageY;
        private double _VarianceX;
        private double _VarianceY;
        private double _Covariance;

        private int _Length = 1;

        public double Value
        {
            get => _Covariance;
            set => _Covariance = value;
        }

        public double VarianceX => _VarianceX;
        public double VarianceY => _VarianceY;

        public double StdDevX => Math.Sqrt(_VarianceX);
        public double StdDevY => Math.Sqrt(_VarianceY);

        public double Correlation => _Covariance / (StdDevX * StdDevY);

        public double AverageX
        {
            get => _AverageX;
            set => _AverageX = value;
        }

        public double AverageY
        {
            get => _AverageY;
            set => _AverageY = value;
        }

        public int Length
        {
            get => _Length;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Размер окна должен быть неотрицательным");
                _Length = value;
                if (_Count > value) _Count = value;
            }
        }

        public long Count => _Count;

        public bool Completed => _Count >= _Length;

        public CovarianceValue() { }

        public CovarianceValue(int Length) => this.Length = Length;

        public double AddValue(double x, double y)
        {
            if (_Count < _Length || _Length == 0) _Count++;

            var m_xy = _AverageX * _AverageY;
            var m_x2 = _AverageX * _AverageX;
            var m_y2 = _AverageY * _AverageY;

            var d_x = (x * x - _VarianceX - m_x2) / _Count + m_x2;
            var d_y = (y * y - _VarianceY - m_y2) / _Count + m_y2;
            var d_c = (x * y - _Covariance - m_xy) / _Count + m_xy;

            _AverageX += (x - _AverageX) / _Count;
            _AverageY += (y - _AverageY) / _Count;

            _VarianceX += d_x - _AverageX * _AverageX;
            _VarianceY += d_y - _AverageY * _AverageY;

            return _Covariance += d_c - _AverageX * _AverageY;
        }

        public void Reset()
        {
            _AverageX = 0;
            _AverageY = 0;
            _VarianceX = 0;
            _VarianceY = 0;
            _Covariance = 0;

            _Count = 1;
        }

        [NotNull] public override string ToString() => $"cov:{_Covariance}(var:{Math.Sqrt(_Covariance)}) varX:{_VarianceX}(avgX:{_AverageX}) varY:{_VarianceY}(avgY:{_AverageY})";
    }
}
