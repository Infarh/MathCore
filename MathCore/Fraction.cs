using System;

namespace MathCore
{
    /// <summary>Дробь</summary>
    public readonly struct Fraction
    {
        /// <summary>Наибольшее общее кратное двух чисел</summary>
        private static int GCD(int a, int b)
        {
            while (a != b)
                if (a > b)
                    a -= b;
                else
                    b -= a;
            return a;
        }

        /// <summary>Упрощение дроби</summary>
        /// <param name="Numerator">Числитель</param>
        /// <param name="Denominator">Знаменатель</param>
        /// <returns>Истина, если дробь можно упростить</returns>
        private static bool Simplify(ref int Numerator, ref int Denominator)
        {
            var gcd = GCD(Numerator, Denominator);
            if (gcd == 1) return false;
            Numerator /= gcd;
            Denominator /= gcd;
            return true;
        }

        /// <summary>Числитель</summary>
        private readonly int _Numerator;

        /// <summary>Знаменатель</summary>
        private readonly int _Denomirator;

        /// <summary>Числитель</summary>
        public int Numerator => _Numerator;

        /// <summary>Знаменатель</summary>
        public int Deniminator => _Denomirator;

        /// <summary>Десятичное значение дроби</summary>
        public double DecimalValue => (double)_Numerator / _Denomirator;

        /// <summary>Новая дробь</summary>
        /// <param name="Nominator">Числитель</param>
        /// <param name="Deniminator">Знаменатель</param>
        public Fraction(int Nominator, int Deniminator)
        {
            if (Deniminator == 0) throw new ArgumentException("Знаменатель не может быть равен 0", nameof(Deniminator));

            _Numerator = Deniminator < 0 ? -Nominator : Nominator;
            _Denomirator = Math.Abs(Deniminator);
        }

        /// <summary>Получить упрощённую дробь</summary>
        /// <returns>Упрощённая дробь</returns>
        public Fraction GetSimplified()
        {
            var gcd = GCD(_Numerator, _Denomirator);
            return gcd == 1 ? this : new Fraction(_Numerator / gcd, _Denomirator / gcd);
        }

        /// <inheritdoc />
        public override string ToString() => $"{_Numerator}/{_Denomirator}";

        /// <inheritdoc />
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>Проверка на эквивалентность дроби</summary>
        /// <param name="other">Проверяемая дробь</param>
        /// <returns>Истина, если дроби идентичные</returns>
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

        /// <inheritdoc />
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