using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal sealed class AssertIntEqualsChecker
    {
        private readonly int _Value;

        public AssertIntEqualsChecker(int Value) => _Value = Value;

        public void AreEqual(int value) => Assert.AreEqual(value, _Value, $"error:{Math.Abs(value - _Value)}");
        public void AreEqual(int value, double eps) => Assert.AreEqual(value, _Value, eps, $"error:{Math.Abs(value - _Value)}");
        public void AreNotEqual(int value) => Assert.AreNotEqual(value, _Value);
        public void AreNotEqual(int value, double eps) => Assert.AreNotEqual(value, _Value, eps);
        public void IsNull(int value) => Assert.IsNull(_Value);
        public void IsNotNull(int value) => Assert.IsNotNull(_Value);

        public void GreaterThen(int value) => Assert.IsTrue(_Value > value);
        public void GreaterOrEqualsThen(int value) => Assert.IsTrue(_Value >= value);
        public void LessThen(int value) => Assert.IsTrue(_Value < value);
        public void LessOrEqualsThen(int value) => Assert.IsTrue(_Value < value);
    }
}