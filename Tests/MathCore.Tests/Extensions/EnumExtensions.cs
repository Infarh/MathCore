namespace MathCore.Tests.Extensions;

[TestClass]
public class EnumExtensions : UnitTest
{
    private enum TestEnum
    {
        [System.ComponentModel.Description("TestValueDescription")]
        TestValue
    }
        
    [TestMethod]
    public void GetDescription()
    {
        var value = TestEnum.TestValue;

        var description = value.GetDescription();

        Assert.That.Value(description).IsEqual("TestValueDescription");
    }
}