using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    public class NumberedValues<TValue> : IEnumerable<KeyValuePair<double, TValue>>
    {
        private readonly double _t0;
        private readonly double _dt;
        private readonly TValue[] _Values;
        public double t0 => _t0;
        public double dt => _dt;
        public int Count => _Values.Length;
        public double T => Count * dt;

        public TValue this[int i] { get => _Values[i]; set => _Values[i] = value; }

        public TValue this[double t] { get => this[IndexOf(t)]; set => this[IndexOf(t)] = value; }

        public NumberedValues(double dt, int N, double t0 = 0) : this(dt, new TValue[N], t0) { }
        public NumberedValues(double dt, TValue[] Values, double t0 = 0)
        {
            _t0 = t0;
            _dt = dt;
            _Values = Values;
        }

        public NumberedValues(double dt, IEnumerable<TValue> Values, double t0 = 0)
        {
            _t0 = t0;
            _dt = dt;
            _Values = Values.ToArray();
        }

        public int IndexOf(double t)
        {
            var i = (int)Math.Round((t - _t0) / dt);
            return i < 0 ? 0 : (i >= Count ? Count - 1 : i);
        }

        public IEnumerator<KeyValuePair<double, TValue>> GetEnumerator() => _Values.Cast<TValue>()
                    .Select((v, i) => new KeyValuePair<double, TValue>(i * _dt + _t0, v))
                    .GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<TValue> GetValues() => _Values;

        public IEnumerable<double> GetTimes() => Enumerable.Range(0, Count).Select(i => i * _dt + _t0);
    }
}
