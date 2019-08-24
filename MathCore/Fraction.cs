using System;

namespace MathCore
{
    public struct Fraction
    {
        private static int GCD(int a, int b)
        {
            while (a != b)
                if (a > b)
                    a -= b;
                else
                    b -= a;
            return a;
        }

        private static bool Simplify(ref int Numerator, ref int Denominator)
        {
            var gcd = GCD(Numerator, Denominator);
            if (gcd == 1) return false;
            Numerator /= gcd;
            Denominator /= gcd;
            return true;
        }

        private readonly int _Numerator;
        private readonly int _Denomirator;

        public int Numerator => _Numerator;

        public int Deniminator => _Denomirator;

        public double DecimalValue => (double)_Numerator / _Denomirator;

        public Fraction(int Nominator, int Deniminator)
        {
            if (Deniminator == 0) throw new ArgumentException("Знаменатель не может быть равен 0", nameof(Deniminator));

            _Numerator = Deniminator < 0 ? -Nominator : Nominator;
            _Denomirator = Math.Abs(Deniminator);
        }

        public Fraction GetSimplified()
        {
            var gcd = GCD(_Numerator, _Denomirator);
            if (gcd == 1) return this;
            return new Fraction(_Numerator / gcd, _Denomirator / gcd);
        }

        public override string ToString() => $"{_Numerator}/{_Denomirator}";

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Fraction other)
        {
            var numerator = _Numerator;
            var denomirator = _Denomirator;
            var other_numerator = other._Numerator;
            var other_denomirator = other._Denomirator;
            Simplify(ref numerator, ref denomirator);
            Simplify(ref other_numerator, ref other_denomirator);
            return numerator == other_numerator && denomirator == other_denomirator;
        }

        public override int GetHashCode()
        {
            var numerator = _Numerator;
            var denomirator = _Denomirator;
            Simplify(ref numerator, ref denomirator);
            unchecked
            {
                return (numerator * 397) ^ denomirator;
            }
        }
    }
}
