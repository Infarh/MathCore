﻿using System;
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
            Assert.AreEqual("", "".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("", " ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("", "\r ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("", "\r".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("", "\r\n".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe\r\n".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "qwe ".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "\r\nqwe".ClearSystemSymbolsAtBeginAndEnd());
            Assert.AreEqual("qwe", "\r\nqwe\r\n".ClearSystemSymbolsAtBeginAndEnd());
        }

        ///// <summary>A test for ClerSymbolsAtBegin</summary>
        //[TestMethod, Ignore]
        //public void ClerSymbolsAtBeginTest()
        //{
        //    var str = string.Empty; // TODO: Initialize to an appropriate value
        //    char[] symbols = null; // TODO: Initialize to an appropriate value
        //    var expected = string.Empty; // TODO: Initialize to an appropriate value
        //    var actual = str.ClerSymbolsAtBegin(symbols);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>A test for ClerSymbolsAtEnd</summary>
        [TestMethod]
        public void ClerSymbolsAtEndTest()
        {
            Assert.AreEqual("", "".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("", " ".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("", "\r ".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("", "\r".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("", "\r\n".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe\r\n".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("qwe", "qwe ".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("\r\nqwe", "\r\nqwe".ClerSymbolsAtEnd('\r', '\n', ' '));
            Assert.AreEqual("  qwe", "  qwe \n".ClerSymbolsAtEnd('\r', '\n', ' '));
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