using System.Collections;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal class CollectionAssertChecker
    {
        private readonly ICollection _ActualCollection;

        public CollectionAssertChecker(ICollection ActualCollection) => _ActualCollection = ActualCollection;

        public void AreEquals(ICollection ExpectedCollection)
        {
            CollectionAssert.AreEqual(ExpectedCollection, _ActualCollection);
        }
    }
}