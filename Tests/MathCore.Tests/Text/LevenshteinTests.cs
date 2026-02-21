using MathCore.Text;

namespace MathCore.Tests.Text;

[TestClass]
public class LevenshteinTests
{
    #region Distance Method Tests

    [TestMethod]
    public void Distance_BothEmptyStrings_ReturnsZero()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("", "");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Distance_FirstStringEmpty_ReturnsSecondStringLength()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("", "abc");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void Distance_SecondStringEmpty_ReturnsFirstStringLength()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("abc", "");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void Distance_IdenticalStrings_ReturnsZero()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("abc", "abc");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Distance_SingleCharacterDifference_ReturnsOne()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("abc", "adc");

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Distance_Kitten_Sitting_ReturnsThree()
    {
        // Arrange - k→s (1), e→i (2), +g (3)
        // Act
        var result = Levenshtein.Distance("kitten", "sitting");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void Distance_Saturday_Sunday_ReturnsThree()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("saturday", "sunday");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void Distance_Book_Back_ReturnsTwo()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("book", "back");

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void Distance_Intention_Execution_ReturnsFive()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("intention", "execution");

        // Assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void Distance_SingleCharacterStrings_ReturnOne()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("a", "b");

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Distance_SingleCharacterIdentical_ReturnZero()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("a", "a");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Distance_Algorithm_Altruistic_ReturnSix()
    {
        // Arrange & Act
        var result = Levenshtein.Distance("algorithm", "altruistic");

        // Assert
        Assert.AreEqual(6, result);
    }

    #endregion

    #region DistanceFast Method Tests

    [TestMethod]
    public void DistanceFast_BothEmptyStrings_ReturnsZero()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("", "");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void DistanceFast_FirstStringEmpty_ReturnsSecondStringLength()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("", "abc");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void DistanceFast_SecondStringEmpty_ReturnsFirstStringLength()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("abc", "");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void DistanceFast_IdenticalStrings_ReturnsZero()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("abc", "abc");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void DistanceFast_SingleCharacterDifference_ReturnsOne()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("abc", "adc");

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void DistanceFast_Kitten_Sitting_ReturnsThree()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("kitten", "sitting");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void DistanceFast_Saturday_Sunday_ReturnsThree()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("saturday", "sunday");

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void DistanceFast_Book_Back_ReturnsTwo()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("book", "back");

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void DistanceFast_Intention_Execution_ReturnsFive()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("intention", "execution");

        // Assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void DistanceFast_SingleCharacterStrings_ReturnOne()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("a", "b");

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void DistanceFast_SingleCharacterIdentical_ReturnZero()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("a", "a");

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void DistanceFast_Algorithm_Altruistic_ReturnSix()
    {
        // Arrange & Act
        var result = Levenshtein.DistanceFast("algorithm", "altruistic");

        // Assert
        Assert.AreEqual(6, result);
    }

    [TestMethod]
    public void DistanceFast_LargeStrings_PerformsCorrectly()
    {
        // Arrange - create large strings
        var long_str1 = new string('a', 1000) + "xyz";
        var long_str2 = new string('a', 1000) + "abc";

        // Act
        var result = Levenshtein.DistanceFast(long_str1, long_str2);

        // Assert
        Assert.AreEqual(3, result);
    }

    #endregion

    #region Consistency Tests

    [TestMethod]
    [DataRow("", "")]
    [DataRow("a", "b")]
    [DataRow("abc", "def")]
    [DataRow("kitten", "sitting")]
    [DataRow("saturday", "sunday")]
    [DataRow("book", "back")]
    [DataRow("intention", "execution")]
    public void Distance_and_DistanceFast_ProduceConsistentResults(string s1, string s2)
    {
        // Arrange & Act
        var distance1 = Levenshtein.Distance(s1, s2);
        var distance2 = Levenshtein.DistanceFast(s1, s2);

        // Assert
        Assert.AreEqual(distance1, distance2);
    }

    [TestMethod]
    public void Distance_IsSymmetric()
    {
        // Arrange & Act
        var distance1 = Levenshtein.Distance("abc", "def");
        var distance2 = Levenshtein.Distance("def", "abc");

        // Assert
        Assert.AreEqual(distance1, distance2);
    }

    [TestMethod]
    public void DistanceFast_IsSymmetric()
    {
        // Arrange & Act
        var distance1 = Levenshtein.DistanceFast("abc", "def");
        var distance2 = Levenshtein.DistanceFast("def", "abc");

        // Assert
        Assert.AreEqual(distance1, distance2);
    }

    [TestMethod]
    public void Distance_SatisfiesTriangleInequality()
    {
        // Arrange
        var str1 = "kitten";
        var str2 = "sitting";
        var str3 = "mitten";

        // Act
        var d12 = Levenshtein.Distance(str1, str2);
        var d23 = Levenshtein.Distance(str2, str3);
        var d13 = Levenshtein.Distance(str1, str3);

        // Assert
        Assert.IsTrue(d13 <= d12 + d23, $"Triangle inequality violated: {d13} > {d12} + {d23}");
    }

    #endregion
}
