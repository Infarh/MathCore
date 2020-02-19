using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class RefArrayViewTests
    {
        [TestMethod]
        public void WriteMixArrayItems()
        {
            var array = new int[5];
            var ref_view = new RefArrayView<int>(array);
            ref_view.Index[0] = 4;
            ref_view.Index[1] = 2;
            ref_view.Index[2] = 0;
            ref_view.Index[3] = 1;
            ref_view.Index[4] = 3;
            for (var i = 0; i < array.Length; i++)
                ref_view[i] = i + 1;
            
            CollectionAssert.That.Collection(array).IsEqualTo(new [] { 3, 4, 2, 5, 1 });
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