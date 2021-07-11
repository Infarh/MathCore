using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class PatternStringTests
    {
        [TestMethod]
        public void ProcessTest()
        {
            const string file_name = "TestFile";
            const string ext = "log";
            var now = DateTime.Now;
            var processor = new PatternString("{FileName}[{date:yyy-MM-ddTHH-mm-ss}].{ext}")
            {
                //{ "FileName", () => "TestFile" },
                //["FileName"] = () => "TestFile",
                //["date"] = () => DateTime.Now,
                //["ext"] = () => ext,
                { "FileName", "TestFile" },
                { "date", () => now },
                { "ext", ext }
            };

            var result = processor.ToString();

            Assert.That.Value(result).IsEqual($"{file_name}[{now:yyy-MM-ddTHH-mm-ss}].{ext}");
        }
    }
}
