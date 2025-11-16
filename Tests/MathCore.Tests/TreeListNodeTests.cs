using MathCore.Graphs;

// ReSharper disable UnusedMember.Global

namespace MathCore.Tests;

[TestClass]
public class TreeListNodeTests
{
    public TestContext TestContext { get; set; }

    #region Additional test attributes

    //[ClassInitialize]	public static void TreeListNodeInitialize(TestContext testContext)	{ }

    //[ClassCleanup] public static void TreeListNodeCleanup() {	}

    //[TestInitialize] public void Initialize() { }

    //[TestCleanup] public void Cleanup() { }

    #endregion

    [TestMethod, Priority(1), Description("")]
    public void CreateTest()
    {
        var value = 0;
        var node = new TreeListNode<int>(value);
        Assert.IsTrue(node.IsRoot);
        Assert.IsTrue(node.IsFirst);
        Assert.IsTrue(node.IsLast);
        Assert.IsNull(node.Prev);
        Assert.IsNull(node.Next);
        Assert.IsNull(node.Child);
        Assert.AreEqual(node.Value, value);
    }

    [TestMethod, Priority(1), Description("")]
    public void NextTest()
    {
        const int root_value = 0;
        var root = new TreeListNode<int>(root_value);
        Assert.AreEqual(root_value, root.Value);

        const int next_value = 1;
        var next = new TreeListNode<int>(next_value);
        Assert.AreEqual(next_value, next.Value);

        root.Next = next;

        Assert.IsNull(root.Prev);
        Assert.IsTrue(ReferenceEquals(root.Next, next));
        Assert.IsNull(root.Child);

        Assert.IsTrue(ReferenceEquals(next.Prev, root));
        Assert.IsNull(next.Next);
        Assert.IsNull(next.Child);

        const int new_next_value = 2;
        var new_next = new TreeListNode<int>(new_next_value);
        Assert.AreEqual(new_next_value, new_next.Value);
        root.Next = new_next;

        Assert.IsTrue(ReferenceEquals(root.Next, new_next));
        Assert.IsTrue(ReferenceEquals(new_next.Prev, root));
        Assert.IsNull(new_next.Next);
        Assert.IsNull(new_next.Child);

        Assert.IsNull(next.Prev);
        Assert.IsNull(next.Next);
        Assert.IsNull(next.Child);
    }

    [TestMethod, Priority(1), Description("")]
    public void ChildTest()
    {
        const int root_value = 0;
        var root = new TreeListNode<int>(root_value);
        Assert.AreEqual(root_value, root.Value);

        const int child_value = 1;
        var child = new TreeListNode<int>(child_value);
        Assert.AreEqual(child_value, child.Value);

        root.Child = child;

        Assert.IsNull(root.Prev);
        Assert.IsNull(root.Next);
        Assert.IsTrue(ReferenceEquals(root.Child, child));

        Assert.IsTrue(ReferenceEquals(child.Prev, root));
        Assert.IsNull(child.Next);
        Assert.IsNull(child.Child);

        const int new_child_value = 2;
        var new_child = new TreeListNode<int>(new_child_value);
        Assert.AreEqual(new_child_value, new_child.Value);
        root.Child = new_child;

        Assert.IsTrue(ReferenceEquals(root.Child, new_child));
        Assert.IsTrue(ReferenceEquals(new_child.Prev, root));
        Assert.IsNull(new_child.Next);
        Assert.IsNull(new_child.Child);

        Assert.IsNull(child.Prev);
        Assert.IsNull(child.Next);
        Assert.IsNull(child.Child);
    }
}