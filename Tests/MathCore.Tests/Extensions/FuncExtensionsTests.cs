using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Extensions
{
    [TestClass]
    public class FuncExtensionsTests
    {
        [TestMethod]
        public async Task InvokeAsync_ExecutedSuccessfully()
        {
            const string expected_result = "Result";
            Func<string> func = () =>
            {
                Thread.Sleep(2);
                return expected_result;
            };

            var actual_result = await func.InvokeAsync();

            Assert.That.Value(actual_result).IsEqual(expected_result);
        }

        [TestMethod]
        public async Task InvokeAsync_WithParameter_ExecutedSuccessfully()
        {
            const string data_string = "Result";
            var expected_result = data_string.Length;
            Func<string, int> func = s =>
            {
                Thread.Sleep(2);
                return s.Length;
            };

            var actual_result = await func.InvokeAsync(data_string);

            Assert.That.Value(actual_result).IsEqual(expected_result);
        }
    }
}
