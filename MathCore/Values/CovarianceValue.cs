using System;
using System.Collections.Generic;
using System.Text;
using MathCore.Annotations;

namespace MathCore.Values
{
    public class CovarianceValue : IValue<double>, IResettable
    {
        private double _Covariance;
        private double _AverageX;
        private double _AverageY;

        private int _Length = 1;

        public double Value
        {
            get => _Covariance;
            set => _Covariance = value;
        }

        public double StdDev => Math.Sqrt(_Covariance);

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
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Размер окна должен быть больше 0");
                _Length = value;
            }
        }

        public CovarianceValue(int Length) => this.Length = Length;

        public double AddValue(double x, double y)
        {
            var m2 = _AverageX * _AverageY;
            var dv = (x * y - _Covariance - m2) / _Length + m2;

            _AverageX += (x - _AverageX) / _Length;
            _AverageY += (y - _AverageY) / _Length;

            return _Covariance += dv - _AverageX * _AverageY;
        }

        public void Reset()
        {
            _Covariance = 0;
            _AverageX = 0;
            _AverageY = 0;
        }

        [NotNull] public override string ToString() => $"cov:{_Covariance}(sgm:{Math.Sqrt(_Covariance)}) avgX:{_AverageX} avgY:{_AverageY}";
    }
}
