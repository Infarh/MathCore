using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        ///// <summary>A test for ClearSymbolsAtBeginAndEnd</summary>
        //[TestMethod, Ignore]
        //public void ClearSymbolsAtBeginAndEndTest()
        //{
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>A test for ClearSystemSymbolsAtBeginAndEnd</summary>
        [TestMethod]
        public void ClearSystemSymbolsAtBeginAndEndTest()
        {
            Assert.AreEqual(string.Empty, string.Empty.ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual(string.Empty, " ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual(string.Empty, "\r ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual(string.Empty, "\r".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual(string.Empty, "\r\n".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe\r\n".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "\r\nqwe".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "\r\nqwe\r\n".ClearSystemSymbolsAtBeginAndEnd());
        }

        ///// <summary>A test for ClearSymbolsAtBegin</summary>
        //[TestMethod, Ignore]
        //public void ClearSymbolsAtBeginTest()
        //{
        //    var str = string.Empty; // TODO: Initialize to an appropriate value
        //    char[] symbols = null; // TODO: Initialize to an appropriate value
        //    var expected = string.Empty; // TODO: Initialize to an appropriate value
        //    var actual = str.ClearSymbolsAtBegin(symbols);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>A test for ClearSymbolsAtEnd</summary>
        [TestMethod]
        public void ClearSymbolsAtEndTest()
        {
            Assert.AreEqual(string.Empty, string.Empty.ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual(string.Empty, " ".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual(string.Empty, "\r ".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual(string.Empty, "\r".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual(string.Empty, "\r\n".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe\r\n".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe ".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("\r\nqwe", "\r\nqwe".ClearSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("  qwe", "  qwe \n".ClearSymbolsAtEnd('\r', '\n', ' '));
        }

        ///// <summary>A test for IsNullOrEmpty</summary>
        //[TestMethod, Ignore]
        //public void IsNullOrEmptyTest()
        //{
        //    var Str = string.Empty; // TODO: Initialize to an appropriate value
        //    const bool expected = false; // TODO: Initialize to an appropriate value
        //    var actual = Str.IsNullOrEmpty();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
