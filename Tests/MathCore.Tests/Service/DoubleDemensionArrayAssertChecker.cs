using System;
using MathCore.Annotations;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    internal class DoubleDemensionArrayAssertChecker
    {
        private readonly double[,] _ActualArray;

        public DoubleDemensionArrayAssertChecker(double[,] ActualArray) => _ActualArray = ActualArray;

        public void AreEquals([NotNull] double[,] ExpectedArray)
        {
            Assert.AreEqual(ExpectedArray.GetLength(0), _ActualArray.GetLength(0), "Размеры массивов не совпадают");
            Assert.AreEqual(ExpectedArray.GetLength(1), _ActualArray.GetLength(1), "Размеры массивов не совпадают");

            for (var i = 0; i < _ActualArray.GetLength(0); i++)
                for (var j = 0; j < _ActualArray.GetLength(1); j++)
                {
                    var expected = ExpectedArray[i, j];
                    var actual = _ActualArray[i, j];
                    Assert.AreEqual(expected, actual, $"Несовпадение по индексу [{i},{j}], ожидалось:{expected}; получено:{actual}; error:{Math.Abs(expected - actual):e3}; rel_error:{Math.Abs(expected - actual) / expected}");
                }
        }

        public void AreEquals([NotNull] double[,] ExpectedArray, double delta)
        {
            Assert.AreEqual(ExpectedArray.GetLength(0), _ActualArray.GetLength(0), "Размеры массивов не совпадают");
            Assert.AreEqual(ExpectedArray.GetLength(1), _ActualArray.GetLength(1), "Размеры массивов не совпадают");

            for (var i = 0; i < _ActualArray.GetLength(0); i++)
                for (var j = 0; j < _ActualArray.GetLength(1); j++)
                {
                    var expected = ExpectedArray[i, j];
                    var actual = _ActualArray[i, j];
                    Assert.AreEqual(expected, actual, delta, $"Несовпадение по индексу [{i},{j}], ожидалось:{expected}; получено:{actual}; delta:{delta}; error:{Math.Abs(expected - actual):e2}; rel_error:{Math.Abs(expected - actual) / expected}");
                }
        }

        public void AllEquals(double Value)
        {
            var index = -1;
            foreach (var v in _ActualArray)
            {
                index++;
                Assert.AreEqual(Value, v, $"index:{index}; error:{Math.Abs(Value - v):e2}");
            }
        }

        public void AllEquals(double Value, double delta)
        {
            var index = -1;
            foreach (var v in _ActualArray)
            {
                index++;
                Assert.AreEqual(Value, v, delta, $"index:{index}; delta:{delta}; error:{Math.Abs(Value - v):e2}");
            }
        }
    }
}