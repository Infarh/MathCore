namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal static class AssertExtensions
    {
        public static AssertEqualsChecker<T> Value<T>(this Assert that, T value) => new AssertEqualsChecker<T>(value);
        public static AssertDoubleEqualsChecker Value(this Assert that, double value) => new AssertDoubleEqualsChecker(value);
        public static AssertIntEqualsChecker Value(this Assert that, int value) => new AssertIntEqualsChecker(value);
    }
}
