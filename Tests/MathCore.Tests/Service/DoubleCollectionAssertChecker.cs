using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal class DoubleCollectionAssertChecker
    {
        [NotNull] private readonly ICollection<double> _ActualCollection;

        public DoubleCollectionAssertChecker([NotNull] ICollection<double> ActualCollection) => _ActualCollection = ActualCollection;

        public void AreEqualValues([NotNull] params double[] ExpectedValues) => AreEquals(ExpectedValues);

        public void AreEquals([NotNull] ICollection<double> ExpectedCollection)
        {
            Assert.That.Value(_ActualCollection.Count).AreEqual(ExpectedCollection.Count);
            IEnumerator<double> expected_collection_enumerator = null;
            IEnumerator<double> actual_collection_enumerator = null;
            try
            {
                expected_collection_enumerator = ExpectedCollection.GetEnumerator();
                actual_collection_enumerator = _ActualCollection.GetEnumerator();

                var index = -1;
                while (actual_collection_enumerator.MoveNext() && expected_collection_enumerator.MoveNext())
                {
                    index++;
                    var expected = expected_collection_enumerator.Current;
                    var actual = actual_collection_enumerator.Current;
                    Assert.AreEqual(expected, actual, $"Несовпадение по индексу {index}, ожидалось:{expected}; получено:{actual}; error:{Math.Abs(expected - actual):e3}; rel_error:{Math.Abs(expected - actual)/expected}");
                }
            }
            finally
            {
                expected_collection_enumerator?.Dispose();
                actual_collection_enumerator?.Dispose();
            }
        }

        public void AreEquals([NotNull] ICollection<double> ExpectedCollection, double Delta)
        {
            Assert.That.Value(_ActualCollection.Count).AreEqual(ExpectedCollection.Count);
            IEnumerator<double> expected_collection_enumerator = null;
            IEnumerator<double> actual_collection_enumerator = null;
            try
            {
                expected_collection_enumerator = ExpectedCollection.GetEnumerator();
                actual_collection_enumerator = _ActualCollection.GetEnumerator();

                var index = -1;
                while (actual_collection_enumerator.MoveNext() && expected_collection_enumerator.MoveNext())
                {
                    index++;
                    var expected = expected_collection_enumerator.Current;
                    var actual = actual_collection_enumerator.Current;
                    Assert.AreEqual(expected, actual, Delta, $"Несовпадение по индексу {index}, ожидалось:{expected}; получено:{actual}; delta:{Delta}; error:{Math.Abs(expected - actual):e2}; rel_error:{Math.Abs(expected - actual) / expected}");
                }
            }
            finally
            {
                expected_collection_enumerator?.Dispose();
                actual_collection_enumerator?.Dispose();
            }
        }

        public void AllEquals(double Value)
        {
            var index = -1;
            foreach (var v in _ActualCollection)
            {
                index++;
                Assert.AreEqual(Value, v, $"index:{index}; error:{Math.Abs(Value - v):e2}");
            }
        }

        public void All([NotNull] Func<double, bool> Check)
        {
            var index = -1;
            foreach (var v in _ActualCollection)
            {
                index++;
                if(!Check(v)) throw new AssertFailedException($"index:{index}; value:{v}");
            }
        }

        public void All([NotNull] Func<double, int, bool> Check)
        {
            var index = -1;
            foreach (var v in _ActualCollection)
            {
                index++;
                if (!Check(v, index)) throw new AssertFailedException($"index:{index}; value:{v}");
            }
        }

        public void AllEquals(double Value, double Delta)
        {
            var index = -1;
            foreach (var v in _ActualCollection)
            {
                index++;
                Assert.AreEqual(Value, v, Delta, $"index:{index}; delta:{Delta}; error:{Math.Abs(Value - v):e2}");
            }
        }
    }
}