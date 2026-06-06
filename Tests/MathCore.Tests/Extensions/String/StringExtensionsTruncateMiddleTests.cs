using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests;

[TestClass]
public class StringExtensionsTruncateMiddleTests
{
    [TestMethod]
    public void TruncateMiddle_BasicBehaviour()
    {
        var s = "HelloWorld";
        // default replacement "...", MaxLength = 5 -> "H...d"
        Assert.AreEqual("H...d", s.TruncateMiddle(5));

        // no truncation when MaxLength >= length
        var shortStr = "Short";
        Assert.AreEqual(shortStr, shortStr.TruncateMiddle(10));

        // odd remaining: prefer left side one char larger
        var abcd = "ABCDEFG";
        // MaxLength=4, replacement="..." -> remaining=1 -> left=1,right=0 -> "A..."
        Assert.AreEqual("A...", abcd.TruncateMiddle(4));
    }

    [TestMethod]
    public void TruncateMiddle_ReplacementLongerThanMax_ReturnsTruncatedReplacement()
    {
        var s = "HelloWorld";
        var replacement = "#####";
        // replacement length 5, MaxLength 3 -> return first 3 chars of replacement
        Assert.AreEqual("###", s.TruncateMiddle(3, replacement));
    }

    [TestMethod]
    public void TruncateMiddle_MaxLengthZero_ReturnsEmpty()
    {
        var s = "Hello";
        Assert.AreEqual(string.Empty, s.TruncateMiddle(0));
    }

    [TestMethod]
    public void TruncateMiddle_NegativeMax_Throws()
    {
        var s = "Hello";
        try
        {
            s.TruncateMiddle(-1);
            Assert.Fail("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException)
        {
            // expected
        }
    }

    [TestMethod]
    public void TruncateMiddle_NullSource_Throws()
    {
        string? s = null;
        try
        {
            s.TruncateMiddle(5);
            Assert.Fail("Expected ArgumentNullException was not thrown");
        }
        catch (ArgumentNullException)
        {
            // expected
        }
    }

    [TestMethod]
    public void TruncateMiddle_NullReplacement_Throws()
    {
        var s = "HelloWorld";
        string? replacement = null;
        try
        {
            s.TruncateMiddle(5, replacement!);
            Assert.Fail("Expected ArgumentNullException was not thrown");
        }
        catch (ArgumentNullException)
        {
            // expected
        }
    }

    [TestMethod]
    public void TruncateMiddle_MaxLengthEqualsLength_ReturnsSame()
    {
        var s = "Exact";
        Assert.AreEqual(s, s.TruncateMiddle(s.Length));
    }

    [TestMethod]
    public void TruncateMiddle_MaxLengthOneAndTwo_ReturnsTruncatedReplacement()
    {
        var s = "HelloWorld";
        // default replacement "..." -> for MaxLength 1 => "." ; MaxLength 2 => ".."
        Assert.AreEqual(".", s.TruncateMiddle(1));
        Assert.AreEqual("..", s.TruncateMiddle(2));
    }

    [TestMethod]
    public void TruncateMiddle_EmptyReplacement_WorksAsConcatenationOfEdges()
    {
        var s = "HelloWorld"; // length 10
        // MaxLength = 5, replacement empty -> left=3,right=2 => "Hel" + "" + "ld" = "Helld"
        Assert.AreEqual("Helld", s.TruncateMiddle(5, string.Empty));
    }

    [TestMethod]
    public void TruncateMiddle_ReplacementLengthEqualsMaxLength_ReturnsReplacement()
    {
        var s = "ABCDEFG";
        var replacement = "XY"; // length 2
        Assert.AreEqual("XY", s.TruncateMiddle(2, replacement));
    }

}
