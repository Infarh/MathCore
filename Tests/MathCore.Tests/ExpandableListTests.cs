namespace MathCore.Tests;

[TestClass]
public class ExpandableListTests
{
    [TestMethod]
    public void AddItemOverMaxCount_thorough_indexer()
    {
        var list1 = new ExpandableList<int>(3);
        list1[10] = 10;
        list1[15] = 15;

        list1.BaseList.Count.AssertEquals(16);
        list1.BaseList.Capacity.AssertEquals(16);

        var list2 = new ExpandableList<int>(20);
        list2[10] = 10;
        list2[15] = 15;

        list2.BaseList.Count.AssertEquals(16);
        list2.BaseList.Capacity.AssertEquals(20);
    }

    [TestMethod]
    public void TrimExcess()
    {
        var list = new ExpandableList<int> { [10] = 10, [15] = 15 };

        list.BaseList.Count.AssertEquals(16);
        list.BaseList.Capacity.AssertEquals(16);

        list[15] = default;

        list.TrimExcess();

        list.BaseList.Count.AssertEquals(11);
        list.BaseList.Capacity.AssertEquals(11);

        list[10] = default;

        list.TrimExcess();

        list.BaseList.Count.AssertEquals(0);
        list.BaseList.Capacity.AssertEquals(0);
    }

    [TestMethod]
    public void IncreaseCount()
    {
        var list = new ExpandableList<int> { [5] = 5 };

        list.BaseList.Count.AssertEquals(6);
        list.BaseList.Capacity.AssertEquals(6);

        list.Count = 10;

        list.BaseList.Count.AssertEquals(10);
        list.BaseList.Capacity.AssertEquals(10);
    }

    [TestMethod]
    public void DecreaseCount()
    {
        var list = new ExpandableList<int> { [5] = 5 };

        list.BaseList.Count.AssertEquals(6);
        list.BaseList.Capacity.AssertEquals(6);

        list.Count = 3;

        list.BaseList.Count.AssertEquals(3);
        list.BaseList.Capacity.AssertEquals(6);
    }

    [TestMethod]
    public void IncreaseCapacity()
    {
        var list = new ExpandableList<int> { [5] = 5 };

        list.BaseList.Count.AssertEquals(6);
        list.BaseList.Capacity.AssertEquals(6);

        list.Capacity = 10;

        list.BaseList.Count.AssertEquals(6);
        list.BaseList.Capacity.AssertEquals(10);
    }

    [TestMethod]
    public void DecreaseCapacity()
    {
        var list = new ExpandableList<int> { [5] = 5 };

        list.BaseList.Count.AssertEquals(6);
        list.BaseList.Capacity.AssertEquals(6);

        list.Capacity = 3;

        list.BaseList.Count.AssertEquals(3);
        list.BaseList.Capacity.AssertEquals(3);
    }
}
