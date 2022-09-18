namespace MathCore.Tests
{
    [TestClass]
    public class RefArrayViewTests
    {
        [TestMethod]
        public void SourceTest()
        {
            var array = new int[10];
            var ref_view = new RefArrayView<int>(array);

            Assert.That.Value(ref_view.Source)
               .IsReferenceEquals(array);
        }

        [TestMethod]
        public void IndexTest()
        {
            var array = new int[10];
            var ref_view = new RefArrayView<int>(array);

            for (var i = 0; i < array.Length; i++)
                Assert.That.Value(ref_view.Index[i]).IsEqual(i);
        }

        [TestMethod]
        public void SetIndexesTest()
        {
            var array = new int[10];
            var ref_view = new RefArrayView<int>(array);

            for (var i = 0; i < array.Length; i++)
                ref_view.Index[i] = array.Length - i - 1;

            for (var i = 0; i < array.Length; i++)
                Assert.That.Value(ref_view.Index[i]).IsEqual(array.Length - i - 1);
        }

        [TestMethod]
        public void SetIndexTest()
        {
            var array = new int[10];
            var ref_view = new RefArrayView<int>(array);

            ref_view.Index[3] = 5;
            Assert.That.Value(ref_view.Index[3]).IsEqual(5);
        }

        [TestMethod]
        public void InverseCreatedIndexTest()
        {
            var array = new int[10];
            var ref_view = new RefArrayView<int>(array, true);

            for (var i = 0; i < array.Length; i++)
                Assert.That.Value(ref_view.Index[i]).IsEqual(array.Length - i - 1);
        }

        [TestMethod]
        public void IndexRestrictionsTest()
        {
            const int count = 10;
            var array = new int[count];
            var ref_view = new RefArrayView<int>(array, true);

            Assert.That.Method(ref_view, view => view.Index[-1] = 0)
               .Throw<ArgumentOutOfRangeException>()
               .Where(ex => ex.ParamName).Check(p => p.IsEqual("index"))
               .Where(ex => ex.ActualValue).IsEqual(-1);

            Assert.That.Method(ref_view, view => view.Index[count] = 0)
               .Throw<ArgumentOutOfRangeException>()
               .Where(ex => ex.ParamName).Check(p => p.IsEqual("index"))
               .Where(ex => ex.ActualValue).IsEqual(count);

            Assert.That.Method(ref_view, view => view.Index[0] = -1)
               .Throw<ArgumentOutOfRangeException>()
               .Where(ex => ex.ParamName).Check(p => p.IsEqual("value"))
               .Where(ex => ex.ActualValue).IsEqual(-1);

            Assert.That.Method(ref_view, view => view.Index[0] = count)
               .Throw<ArgumentOutOfRangeException>()
               .Where(ex => ex.ParamName).Check(p => p.IsEqual("value"))
               .Where(ex => ex.ActualValue).IsEqual(count);
        }

        [TestMethod]
        public void WriteMixArrayItems()
        {
            var array = new int[5];
            // ReSharper disable once CollectionNeverQueried.Local
            var ref_view = new RefArrayView<int>(array)
            {
                Index =
                {
                    [0] = 4,
                    [1] = 2,
                    [2] = 0,
                    [3] = 1,
                    [4] = 3
                }
            };
            for (var i = 0; i < array.Length; i++)
                ref_view[i] = i + 1;

            CollectionAssert.That.Collection(array).IsEqualTo(new[] { 3, 4, 2, 5, 1 });
        }

        [TestMethod]
        public void InvertedCreation()
        {
            var array = Enumerable.Range(1, 5).ToArray();
            // ReSharper disable once CollectionNeverQueried.Local
            var ref_view = new RefArrayView<int>(array, true);

            var reversed = new int[array.Length];
            for (var i = 0; i < array.Length; i++)
                reversed[i] = ref_view[i];

            CollectionAssert.That.Collection(reversed).IsEqualTo(array.GetReversed());
        }
    }
}