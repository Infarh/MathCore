using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using MathCore.Collections;
using MathCore.Extensions.Expressions;

using BindingFlags = System.Reflection.BindingFlags;

namespace MathCore.Tests.Collections;

[TestClass]
public class FListTests
{
    [TestMethod]
    public void EmptyListIsSingleton()
    {
        var empty1 = FList<int>.Empty;
        var empty2 = FList<int>.Empty;
        Assert.Instance.Value(empty1).IsReferenceEquals(empty2);
    }

    [TestMethod]
    public void FListEmptyEqualsFListTypedEmpty()
    {
        var empty1 = FList.Empty<int>();
        var empty2 = FList<int>.Empty;
        Assert.Instance.Value(empty1).IsReferenceEquals(empty2);
    }

    [TestMethod]
    public void FList_New_CreateNewFListWithSingleElement()
    {
        const int head = 42;
        var       list = FList.New(head);
        Assert.Instance.Value(list)
           .Where(l => l.IsEmpty).CheckEquals(false)
           .Where(l => l.Head).CheckEquals(head)
           .Where(l => l.Tail).Check(tail => tail.IsReferenceEquals(FList<int>.Empty));
    }

    [TestMethod]
    public void FList_NewParams_CreateNewFListWithItems()
    {
        int[] items = [1, 2, 3];
        var   list  = FList.New(items);
        Assert.Instance.Value(list)
           .Where(l => l.IsEmpty).CheckEquals(false)
           .Where(l => l.Head).CheckEquals(items[0])
           .Where(l => l.Tail).Check(tail1 => tail1
               .Where(l => l.IsEmpty).CheckEquals(false)
               .Where(l => l.Head).CheckEquals(items[1])
               .Where(l => l.Tail).Check(tail2 => tail2
                   .Where(l => l.IsEmpty).CheckEquals(false)
                   .Where(l => l.Head).CheckEquals(items[2])
                   .Where(l => l.Tail).Check(tail3 => tail3
                       .Where(l => l.IsEmpty).CheckEquals(true))))
            ;
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public void FList_NewEnum_CreateNewFListWithItems()
    {
        var items       = Enumerable.Range(1, 3);
        var items_array = items.ToArray();
        var list        = FList.New(items);
        Assert.Instance.Value(list)
           .Where(l => l.IsEmpty).CheckEquals(false)
           .Where(l => l.Head).CheckEquals(items_array[0])
           .Where(l => l.Tail).Check(tail1 => tail1
               .Where(l => l.IsEmpty).CheckEquals(false)
               .Where(l => l.Head).CheckEquals(items_array[1])
               .Where(l => l.Tail).Check(tail2 => tail2
                   .Where(l => l.IsEmpty).CheckEquals(false)
                   .Where(l => l.Head).CheckEquals(items_array[2])
                   .Where(l => l.Tail).Check(tail3 => tail3
                       .Where(l => l.IsEmpty).CheckEquals(true))))
            ;
    }

    [TestMethod]
    public void PrivateParameterlessConstructorThrowInvalidOperationException()
    {
        var ctor    = typeof(FList<int>).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [], null);
        var creator = ctor.NewExpression().CreateLambda<Func<FList<int>>>().Compile();
        Assert.ThrowsExactly<InvalidOperationException>(() => creator());
    }

    [TestMethod]
    public void EnumerationListNodes()
    {
        var items      = Enumerable.Range(1, 100).ToArray();
        var list       = FList.New(items);
        var item_index = 0;
        foreach (var item in (IEnumerable)list) 
            Assert.Instance.Value(item).IsEqual(items[item_index++]);
    }

    [TestMethod]
    public void ToStringContainsAllItems()
    {
        var items              = Enumerable.Range(1, 10).ToArray();
        var list               = FList.New(items);
        var list_string        = list.ToString();
        var string_list_values = Regex.Matches(list_string, @"\d+").Select(m => int.Parse(m.Value)).ToArray();
        Assert.Instance.Collection(string_list_values).IsEqualTo(items);
    }
}