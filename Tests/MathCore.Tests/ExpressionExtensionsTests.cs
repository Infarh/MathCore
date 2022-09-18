using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.Tests
{
    [TestClass]
    public class ExpressionExtensionsTests
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
        public void TestSimpleFunction()
        {
            Expression<Func<double, double>> original_expression = t => t * t * t + 20;

            Expression<Func<double, double>> substitute_expression = t => t * t;

            var modified_expression = original_expression.Substitute("t", substitute_expression);

            var modified_function = (Func<double, double>)modified_expression.Compile();

            Assert.AreEqual((int)modified_function(1), 21);
            Assert.AreEqual((int)modified_function(2), 2 * 2 * 2 * 2 * 2 * 2 + 20);
        }

        [TestMethod]
        public void TestComplexPolynomial()
        {
            Expression<Func<double, double>> original_expression =
                t => (1 - t) * (1 - t) * (1 - t) * -50 +
                        3 * (1 - t) * (1 - t) * t * -25 +
                        3 * (1 - t) * t * t * 25 +
                        t * t * t * 50;

            Expression<Func<double, double, double, double>> substitute_expression = (t, factor, shift) => t * factor + shift;

            var modified_expression = original_expression.Substitute("t", substitute_expression);

            var modified_function = (Func<double, double, double, double>)modified_expression.Compile();

            var result = modified_function(1, 10, -9);
            Assert.AreEqual(result, 50);

            Console.WriteLine(original_expression);
            Console.WriteLine();
            Console.WriteLine(modified_expression);
        }
    }
}
