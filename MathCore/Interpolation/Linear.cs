using System;

namespace MathCore.Interpolation
{
    public class Linear : Interpolator, IInterpolator
    {
        private readonly double[] _X;
        private readonly double[] _Y;

        public double this[double x] => Value(x);

        public Linear(double[] X, double[] Y)
        {
            _X = X;
            _Y = Y;
        }

        public double Value(double x)
        {
            var i1 = 0;
            var i2 = _X.Length - 1;
            if (x < _X[i1]) i2 = i1 + 1;
            else if(x > _X[i2]) i1 = i2 - 1;
            else
                do
                {
                    if(_X[i1].Equals(x)) return _Y[i1];
                    if(_X[i2].Equals(x)) return _Y[i2];

                    var i = (i1 + i2) / 2;
                    var xi = _X[i];
                    if(xi == x) return _Y[i];

                    if(x > xi)
                        i1 = i;
                    else
                        i2 = i;
                } while(i2 - i1 > 1);

            return Mapping.GetValue(x, _X[i1], _X[i2], _Y[i1], _Y[i2]);
        }

        public Func<double, double> GetFunction() => Value;
    }
}