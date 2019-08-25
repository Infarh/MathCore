using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal static class CollectionAssertExtensions
    {
        //public static CollectionAssertChecker Collection(this CollectionAssert assert, ICollection ActualCollection) => new CollectionAssertChecker(ActualCollection);
        public static DoubleCollectionAssertChecker Collection(this CollectionAssert assert, ICollection<double> ActualCollection) => new DoubleCollectionAssertChecker(ActualCollection);

        public static DoubleDemensionArrayAssertChecker Collection(this CollectionAssert assert, double[,] array) => new DoubleDemensionArrayAssertChecker(array);

        public static CollectionAssertChecker<T> Collection<T>(this CollectionAssert assert, ICollection<T> ActualCollection) => new CollectionAssertChecker<T>(ActualCollection);
    }

    public class CollectionAssertChecker<T>
    {
        private readonly ICollection<T> _ActualCollection;
        public CollectionAssertChecker(ICollection<T> ActualCollection) => _ActualCollection = ActualCollection;

        public void AreEquals(ICollection<T> ExpectedCollection) => CollectionAssert.AreEqual((ICollection)ExpectedCollection, (ICollection)_ActualCollection);
    }
}