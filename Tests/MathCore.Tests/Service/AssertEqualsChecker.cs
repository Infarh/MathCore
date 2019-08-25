using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal sealed class AssertEqualsChecker<T>
    {
        private readonly T _Value;

        public AssertEqualsChecker(T Value) => _Value = Value;

        public void AreEqual(T value) => Assert.AreEqual(value, _Value);
        public void AreReferenceEquals(T value) => Assert.IsTrue(ReferenceEquals(_Value, value));
        public void AreNotEqual(T value) => Assert.AreNotEqual(value, _Value);
        public void AreNotReferenceEquals(T value) => Assert.IsFalse(ReferenceEquals(_Value, value));
        public void IsNull() => Assert.IsNull(_Value);
        public T IsNotNull()
        {
            Assert.IsNotNull(_Value);
            return _Value;
        }

        public void Is(Type type) => Assert.IsInstanceOfType(_Value, type);
        public TType Is<TType>() where TType : class, T => (TType) _Value;
    }
}