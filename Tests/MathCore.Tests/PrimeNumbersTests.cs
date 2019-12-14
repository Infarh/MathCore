using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class PrimeNumbersTests : UnitTest
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


        /// <summary>Тестирование метода проверки числа на простоту</summary>
        [TestMethod, Priority(0), Description("Тестирование метода проверки числа на простоту")]
        public void IsPrimeTest()
        {
            Assert.IsTrue(2.IsPrime(), "2 не оказалось простым числом");
            Assert.IsTrue(3.IsPrime(), "3 не оказалось простым числом");
            Assert.IsFalse(4.IsPrime(), "4 не оказалось простым числом");
            Assert.IsFalse(100.IsPrime(), "100 оказалось(!) простым числом");
            Assert.IsTrue(101.IsPrime(), "101 не оказалось простым числом");
            Assert.IsTrue(113.IsPrime(), "113 не оказалось простым числом");

            Assert.IsTrue(42326593.IsPrime(), "42326593 не оказалось простым числом");
        }

        /// <summary>Тестирование метода получения простых чисел</summary>
        [TestMethod, Priority(1), Description("Тестирование метода получения простых чисел")]
        public void GetPrimeNumbersTest()
        {
            PrimeNumbers.GetNumbersTo(GetRNDInt(50, 1500))
                .Foreach(n => Assert.IsTrue(n.IsPrime(), "{0} не является простым", n));
        }
    }
}
