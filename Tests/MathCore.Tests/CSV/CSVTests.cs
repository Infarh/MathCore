using MathCore.Values;

namespace MathCore.Tests.CSV;

[TestClass]
public class CSVValueTests
{
    private static string TempFile { get; } = Path.Combine(Path.GetTempPath(), $"csv_test_{Guid.NewGuid()}.csv");

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(TempFile)) File.Delete(TempFile);
    }

    [TestMethod]
    public void Test_ReadSimpleCSV()
    {
        File.WriteAllLines(TempFile, ["Name,Age", "Alice,30", "Bob,25"]);
        var csv = new MathCore.Values.CSV(TempFile, ',');
        var items = csv.ToList();
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("Alice", items[0]["Name"]);
        Assert.AreEqual("30", items[0]["Age"]);
        Assert.AreEqual("Bob", items[1]["Name"]);
        Assert.AreEqual("25", items[1]["Age"]);
    }

    [TestMethod]
    public void Test_ReadSemicolonSeparator()
    {
        File.WriteAllLines(TempFile, ["X;Y;Z", "1;2;3"]);
        var csv = new MathCore.Values.CSV(TempFile, ';');
        var items = csv.ToList();
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("1", items[0]["X"]);
        Assert.AreEqual("2", items[0]["Y"]);
        Assert.AreEqual("3", items[0]["Z"]);
    }

    [TestMethod]
    public void Test_SkipEmptyLines()
    {
        File.WriteAllLines(TempFile, ["A,B", "1,2", "", "3,4"]);
        var csv = new MathCore.Values.CSV(TempFile, ',');
        var items = csv.ToList();
        Assert.AreEqual(2, items.Count);
    }

    [TestMethod]
    public void Test_IterateKeyValuePair()
    {
        File.WriteAllLines(TempFile, ["Key,Value", "k1,v1"]);
        var csv = new MathCore.Values.CSV(TempFile, ',');
        var item = csv.First();
        var kvp = item.ToList();
        Assert.AreEqual(2, kvp.Count);
    }

    [TestMethod]
    public void Test_ItemItemsCount()
    {
        File.WriteAllLines(TempFile, ["Col1,Col2,Col3", "a,b,c"]);
        var csv = new MathCore.Values.CSV(TempFile, ',');
        var item = csv.First();
        Assert.AreEqual(3, item.ItemsCount);
    }

    [TestMethod]
    public void Test_ItemToString()
    {
        File.WriteAllLines(TempFile, ["A,B", "x,y"]);
        var csv = new MathCore.Values.CSV(TempFile, ',');
        var item = csv.First();
        var str = item.ToString();
        Assert.IsTrue(str.Contains("x"));
        Assert.IsTrue(str.Contains("y"));
    }

    [TestMethod]
    public void Test_SkipFirstLines()
    {
        File.WriteAllLines(TempFile, ["# Comment", "A,B", "1,2"]);
        var csv = new MathCore.Values.CSV(TempFile, ',', SkipFirstLines: 1);
        var items = csv.ToList();
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("1", items[0]["A"]);
    }

    [TestMethod]
    public void Test_HeaderLineFalse()
    {
        File.WriteAllLines(TempFile, ["1,2", "3,4"]);
        var csv = new MathCore.Values.CSV(TempFile, ',', HeaderLine: false);
        var items = csv.ToList();
        Assert.AreEqual(2, items.Count);
    }
}
