using MathCore.Graphs;

// ReSharper disable UnusedMember.Global

namespace MathCore.Tests
{
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
            Assert.IsTrue(node.Prev is null);
            Assert.IsTrue(node.Next is null);
            Assert.IsTrue(node.Child is null);
            Assert.AreEqual(node.Value, value);
        }

        [TestMethod, Priority(1), Description("")]
        public void NextTest()
        {
            const int root_value = 0;
            var root = new TreeListNode<int>(root_value);
            Assert.AreEqual(root.Value, root_value);

            const int next_value = 1;
            var next = new TreeListNode<int>(next_value);
            Assert.AreEqual(next.Value, next_value);

            root.Next = next;

            Assert.IsTrue(root.Prev is null);
            Assert.IsTrue(ReferenceEquals(root.Next, next));
            Assert.IsTrue(root.Child is null);

            Assert.IsTrue(ReferenceEquals(next.Prev, root));
            Assert.IsTrue(next.Next is null);
            Assert.IsTrue(next.Child is null);

            const int new_next_value = 2;
            var new_next = new TreeListNode<int>(new_next_value);
            Assert.AreEqual(new_next.Value, new_next_value);
            root.Next = new_next;

            Assert.IsTrue(ReferenceEquals(root.Next, new_next));
            Assert.IsTrue(ReferenceEquals(new_next.Prev, root));
            Assert.IsTrue(new_next.Next is null);
            Assert.IsTrue(new_next.Child is null);

            Assert.IsTrue(next.Prev is null);
            Assert.IsTrue(next.Next is null);
            Assert.IsTrue(next.Child is null);
        }

        [TestMethod, Priority(1), Description("")]
        public void ChildTest()
        {
            const int root_value = 0;
            var root = new TreeListNode<int>(root_value);
            Assert.AreEqual(root.Value, root_value);

            const int child_value = 1;
            var child = new TreeListNode<int>(child_value);
            Assert.AreEqual(child.Value, child_value);

            root.Child = child;

            Assert.IsTrue(root.Prev is null);
            Assert.IsTrue(root.Next is null);
            Assert.IsTrue(ReferenceEquals(root.Child, child));

            Assert.IsTrue(ReferenceEquals(child.Prev, root));
            Assert.IsTrue(child.Next is null);
            Assert.IsTrue(child.Child is null);

            const int new_child_value = 2;
            var new_child = new TreeListNode<int>(new_child_value);
            Assert.AreEqual(new_child.Value, new_child_value);
            root.Child = new_child;

            Assert.IsTrue(ReferenceEquals(root.Child, new_child));
            Assert.IsTrue(ReferenceEquals(new_child.Prev, root));
            Assert.IsTrue(new_child.Next is null);
            Assert.IsTrue(new_child.Child is null);

            Assert.IsTrue(child.Prev is null);
            Assert.IsTrue(child.Next is null);
            Assert.IsTrue(child.Child is null);
        }
    }
}