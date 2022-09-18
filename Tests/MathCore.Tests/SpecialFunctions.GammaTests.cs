namespace MathCore.Tests
{
    [TestClass]
    public class SpecialFunctionsGammaTests
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

        [TestMethod]
        public void GammaTest()
        {
            Func<double, double> Г = MathCore.SpecialFunctions.Gamma.G;

            Assert.AreEqual(1, Г(1), "Г(1) не равно 0!=1");
            Assert.AreEqual(1.Factorial(), Г(2), "Г(2) не равно 1!=1");
            Assert.AreEqual(2.Factorial(), Г(3), "Г(3) не равно 2!=2");
            Assert.AreEqual(3.Factorial(), Г(4), "Г(4) не равно 3!=6");

            Assert.AreEqual(Math.Sqrt(Math.PI), Г(0.5), "Г(0.5) не равно Sqrt(pi)");
            Assert.AreEqual(Math.Sqrt(Math.PI) / 2, Г(1.5), "Г(1.5) не равно Sqrt(pi)/2");
            Assert.AreEqual(-2 * Math.Sqrt(Math.PI), Г(-0.5), "Г(-0.5) не равно -2*Sqrt(pi)");
            Assert.AreEqual(Г(5 + 1) / 5, Г(5), "Г(5) не равно Г(5+1)/5");
            Assert.AreEqual(Г(5 + 1) / 5, Г(5), "Г(5) не равно Г(5+1)/5");
        }
    }
}
