using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal sealed class AssertDoubleEqualsChecker
    {
        private readonly double _Value;

        public AssertDoubleEqualsChecker(double Value) => _Value = Value;

        public void AreEqual(double value) => Assert.AreEqual(value, _Value, $"error:{Math.Abs(value - _Value):e2}");
        public void AreEqual(double value, double eps) => Assert.AreEqual(value, _Value, eps, $"error:{Math.Abs(value - _Value):e2}");
        public void AreNotEqual(double value) => Assert.AreNotEqual(value, _Value);
        public void AreNotEqual(double value, double eps) => Assert.AreNotEqual(value, _Value, eps);
        public void IsNull(double value) => Assert.IsNull(_Value);
        public void IsNotNull(double value) => Assert.IsNotNull(_Value);

        public void GreaterThen(double value) => Assert.IsTrue(_Value > value, $"Значение {_Value} должно быть больше {value}");
        public void GreaterOrEqualsThen(double value) => Assert.IsTrue(_Value >= value, $"Нарушено условие ({_Value} >= {value}). delta:{value - _Value:e2}");
        public void GreaterOrEqualsThen(double value, double eps) => Assert.IsTrue(_Value - value <= eps, $"Нарушено условие ({_Value} >= {value}) при точности {eps:e2} delta:{value - _Value:e2}");
        public void LessThen(double value) => Assert.IsTrue(_Value < value, $"Значение {_Value} должно быть меньше {value}");
        public void LessOrEqualsThen(double value) => Assert.IsTrue(_Value < value, $"Значение {_Value} должно быть меньше {value}");
    }
}