using System;

using MathCore.Annotations;

namespace MathCore.Values
{
    public class VarianceValue : IAddValue<double>, IResettable
    {
        private long _Count = 1;
        private double _Variance;
        private double _Average;

        private int _Length = 1;

        public double Value
        {
            get => _Variance;
            set => _Variance = value;
        }

        public double StdDev => Math.Sqrt(_Variance);

        public double Average
        {
            get => _Average;
            set => _Average = value;
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

        public VarianceValue() { }

        public VarianceValue(int Length) => this.Length = Length;

        public double AddValue(double value)
        {
            if (_Count < _Length || _Length == 0) _Count++;

            var m2 = _Average * _Average;
            var dv = (value * value - _Variance - m2) / _Count + m2;

            _Average += (value - _Average) / _Count;

            return _Variance += dv - _Average * _Average;
        }

        public void Reset()
        {
            _Variance = 0;
            _Average = 0;
            _Count = 1;
        }

        [NotNull] public override string ToString() => $"var:{_Variance}(sgm:{Math.Sqrt(_Variance)}) avg:{_Average}";
    }
}
