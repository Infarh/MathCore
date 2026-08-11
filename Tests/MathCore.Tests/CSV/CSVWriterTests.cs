using System.Text;

using MathCore.CSV;

namespace MathCore.Tests.CSV;

[TestClass]
public class CSVWriterTests : CSVTestsBase
{
    [TestMethod]
    public void Test_WriteTo_StringBuilder()
    {
        var result = new StringBuilder();
        using var sw = new StringWriter(result);
        GetStudents().Take(3)
           .AsCSV(',')
           .AddDefaultHeaders()
           .WriteTo(sw);
        var csv = result.ToString();
        Assert.IsTrue(csv.Contains("Id"));
        Assert.IsTrue(csv.Contains("Name"));
        Assert.IsTrue(csv.Contains("Name-1"));
        Assert.IsTrue(csv.Contains("Name-2"));
        Assert.IsTrue(csv.Contains("Name-3"));
    }

    [TestMethod]
    public void Test_WriteTo_StringWriter()
    {
        using var sw = new StringWriter();
        GetStudents().Take(2).AsCSV(';').AddDefaultHeaders().WriteTo(sw);
        var csv = sw.ToString();
        var lines = csv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.IsTrue(lines.Length >= 3); // заголовок + 2 строки
    }

    [TestMethod]
    public void Test_WriteTo_TextWriter()
    {
        using var sw = new StringWriter();
        GetStudents().Take(5).AsCSV(',').AddDefaultHeaders().WriteTo(sw);
        var csv = sw.ToString();
        Assert.IsTrue(csv.Contains("Id"));
        Assert.IsTrue(csv.Contains("Name"));
    }

    [TestMethod]
    public void Test_AddColumn()
    {
        var result = new StringBuilder();
        using var sw = new StringWriter(result);
        GetStudents().Take(3)
           .AsCSV(',')
           .AddDefaultHeaders()
           .AddColumn("NameLen", s => s.Name.Length)
           .WriteTo(sw);
        var csv = result.ToString();
        Assert.IsTrue(csv.Contains("NameLen"));
        Assert.IsTrue(csv.Contains("8")); // Name-1.Length = 8
    }

    [TestMethod]
    public void Test_WithoutHeaders()
    {
        var result = new StringBuilder();
        using var sw = new StringWriter(result);
        GetStudents().Take(2)
           .AsCSV(',')
           .WriteHeader(false)
           .WriteTo(sw);
        var csv = result.ToString();
        Assert.IsTrue(csv.Contains("1"));
    }

    [TestMethod]
    public void Test_CustomSeparator()
    {
        var result = new StringBuilder();
        using var sw = new StringWriter(result);
        GetStudents().Take(1)
           .AsCSV(',')
           .Separator('|')
           .WriteTo(sw);
        var csv = result.ToString();
        Assert.IsTrue(csv.Contains('|'));
    }

    [TestMethod]
    public void Test_HeadersProperty()
    {
        var writer = GetStudents().Take(1).AsCSV(',').AddDefaultHeaders();
        var headers = writer.Headers.ToList();
        Assert.AreEqual(6, headers.Count);
        Assert.AreEqual("Id", headers[0]);
        Assert.AreEqual("Name", headers[1]);
    }

    [TestMethod]
    public void Test_RemoveColumn()
    {
        var result = new StringBuilder();
        using var sw = new StringWriter(result);
        GetStudents().Take(1)
           .AsCSV(',')
           .AddDefaultHeaders()
           .RemoveColumn("GroupId")
           .WriteTo(sw);
        var csv = result.ToString();
        Assert.IsFalse(csv.Contains("GroupId"));
        Assert.IsTrue(csv.Contains("Id"));
    }
}

