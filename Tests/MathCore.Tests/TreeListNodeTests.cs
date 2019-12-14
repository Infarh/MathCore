using MathCore.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            const int RootValue = 0;
            var root = new TreeListNode<int>(RootValue);
            Assert.AreEqual(root.Value, RootValue);

            const int NextValue = 1;
            var next = new TreeListNode<int>(NextValue);
            Assert.AreEqual(next.Value, NextValue);

            root.Next = next;

            Assert.IsTrue(root.Prev is null);
            Assert.IsTrue(ReferenceEquals(root.Next, next));
            Assert.IsTrue(root.Child is null);

            Assert.IsTrue(ReferenceEquals(next.Prev, root));
            Assert.IsTrue(next.Next is null);
            Assert.IsTrue(next.Child is null);

            const int NewNextValue = 2;
            var newnext = new TreeListNode<int>(NewNextValue);
            Assert.AreEqual(newnext.Value, NewNextValue);
            root.Next = newnext;

            Assert.IsTrue(ReferenceEquals(root.Next, newnext));
            Assert.IsTrue(ReferenceEquals(newnext.Prev, root));
            Assert.IsTrue(newnext.Next is null);
            Assert.IsTrue(newnext.Child is null);

            Assert.IsTrue(next.Prev is null);
            Assert.IsTrue(next.Next is null);
            Assert.IsTrue(next.Child is null);
        }

        [TestMethod, Priority(1), Description("")]
        public void ChildTest()
        {
            const int RootValue = 0;
            var root = new TreeListNode<int>(RootValue);
            Assert.AreEqual(root.Value, RootValue);

            const int ChildValue = 1;
            var child = new TreeListNode<int>(ChildValue);
            Assert.AreEqual(child.Value, ChildValue);

            root.Child = child;

            Assert.IsTrue(root.Prev is null);
            Assert.IsTrue(root.Next is null);
            Assert.IsTrue(ReferenceEquals(root.Child, child));

            Assert.IsTrue(ReferenceEquals(child.Prev, root));
            Assert.IsTrue(child.Next is null);
            Assert.IsTrue(child.Child is null);

            const int NewChildValue = 2;
            var newchild = new TreeListNode<int>(NewChildValue);
            Assert.AreEqual(newchild.Value, NewChildValue);
            root.Child = newchild;

            Assert.IsTrue(ReferenceEquals(root.Child, newchild));
            Assert.IsTrue(ReferenceEquals(newchild.Prev, root));
            Assert.IsTrue(newchild.Next is null);
            Assert.IsTrue(newchild.Child is null);

            Assert.IsTrue(child.Prev is null);
            Assert.IsTrue(child.Next is null);
            Assert.IsTrue(child.Child is null);
        }
    }
}
