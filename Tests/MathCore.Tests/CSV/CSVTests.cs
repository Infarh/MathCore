using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.CSV
{
    [TestClass]
    public class CSVTests : CSVTestsBase
    {
        [TestMethod]
        public void QuotesValuesTest()
        {
            var data_str = @"X,Y,Z
x1,y1,""z3,14""
x2,""y2"",""z3,14""";

        }
    }
}