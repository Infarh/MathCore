using System.Text;
using MathCore.CSV;

namespace MathCore.Tests.CSV
{
    [TestClass]
    public class CsvWriterTests : CSVTestsBase
    {
        [TestMethod]
        public void WriteTest()
        {
            const string additional_header_name = "Name-Length";

            var result = new StringBuilder(365);
            using (var writer = result.CreateWriter())
                GetStudents().Take(5)
                   .AsCSV(';')
                   .AddDefaultHeaders()
                   .AddColumn(additional_header_name, s => s.Name.Length)
                   .WriteTo(writer);

            using var reader = result.CreateReader();
            var header_line = reader.ReadLine();
            var header_components = header_line?.Split(ValuesSeparator);
            Assert.That.Collection(header_components).IsEqualTo(Headers.Append(additional_header_name).ToArray());
        }
    }
}