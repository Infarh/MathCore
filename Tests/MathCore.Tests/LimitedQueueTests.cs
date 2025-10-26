// file: Tests/MathCore.Tests/LimitedQueueTests.cs
namespace MathCore.Tests;

/// <summary>Тесты для ограниченной очереди</summary>
[TestClass]
public class LimitedQueueTests
{
    /// <summary>Очередь сохраняет порядок при добавлении без переполнения</summary>
    [TestMethod]
    public void Enqueue_Dequeue_Preserves_Order_Without_Overflow()
    {
        var q = new LimitedQueue<int>(5);

        for (var i = 1; i <= 5; i++) q.Enqueue(i);

        Assert.HasCount(5, q);
        Assert.IsTrue(q.IsFull);
        Assert.IsFalse(q.IsEmpty);

        for (var i = 1; i <= 5; i++)
            Assert.AreEqual(i, q.Dequeue());

        Assert.IsEmpty(q);
        Assert.IsTrue(q.IsEmpty);
        Assert.IsFalse(q.IsFull);
    }

    /// <summary>При переполнении переписывается самый старый элемент</summary>
    [TestMethod]
    public void Enqueue_Overflow_Rewrites_Oldest()
    {
        var q = new LimitedQueue<int>(3);

        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        Assert.IsTrue(q.IsFull);
        Assert.HasCount(3, q);

        q.Enqueue(4); // ожидаем, что 1 будет вытеснен, останутся 2,3,4

        Assert.HasCount(3, q);
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, q.ToArray());

        Assert.AreEqual(2, q.Dequeue());
        Assert.AreEqual(3, q.Dequeue());
        Assert.AreEqual(4, q.Dequeue());
        Assert.IsTrue(q.IsEmpty);
    }

    /// <summary>Peek возвращает первый элемент без удаления</summary>
    [TestMethod]
    public void Peek_Returns_Front_Without_Removing()
    {
        var q = new LimitedQueue<int>(3);
        q.Enqueue(10);
        q.Enqueue(20);

        Assert.AreEqual(10, q.Peek());
        Assert.HasCount(2, q);
        Assert.AreEqual(10, q.Dequeue());
        Assert.AreEqual(20, q.Dequeue());
    }

    /// <summary>ToArray возвращает снимок в корректном порядке</summary>
    [TestMethod]
    public void ToArray_Returns_Correct_Order_With_Wrap()
    {
        var q = new LimitedQueue<int>(3);

        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        _ = q.Dequeue();   // удаляем 1
        q.Enqueue(4);      // очередь: 2,3,4

        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, q.ToArray());
    }

    /// <summary>Очистка приводит очередь к пустому состоянию</summary>
    [TestMethod]
    public void Clear_Empties_Queue()
    {
        var q = new LimitedQueue<int>(2);
        q.Enqueue(7);
        q.Enqueue(8);

        q.Clear();

        Assert.IsEmpty(q);
        Assert.IsTrue(q.IsEmpty);
        Assert.IsFalse(q.IsFull);
        CollectionAssert.AreEqual(Array.Empty<int>(), q.ToArray());
    }

    /// <summary>Перечисление возвращает элементы в корректном порядке</summary>
    [TestMethod]
    public void Enumeration_Yields_In_Order()
    {
        var q = new LimitedQueue<int>(3);

        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        _ = q.Dequeue();   // удаляем 1
        q.Enqueue(4);      // очередь: 2,3,4

        var items = q.ToArray(); // снимок для сравнения
        var iterated = q.ToList();

        CollectionAssert.AreEqual(items, iterated);
    }

    /// <summary>Конструктор из коллекции заполняет буфер и отбрасывает самые старые при избытке</summary>
    [TestMethod]
    public void Construct_From_Enumerable_Trims_To_Capacity()
    {
        var source = Enumerable.Range(1, 5).ToArray();
        var q = new LimitedQueue<int>(source, 3);

        Assert.HasCount(3, q);
        CollectionAssert.AreEqual(new[] { 3, 4, 5 }, q.ToArray());
    }

    /// <summary>Dequeue и Peek на пустой очереди генерируют исключение</summary>
    [TestMethod]
    public void Dequeue_And_Peek_On_Empty_Throw()
    {
        var q = new LimitedQueue<int>(2);

        Assert.ThrowsExactly<InvalidOperationException>(() => q.Dequeue());
        Assert.ThrowsExactly<InvalidOperationException>(() => q.Peek());
    }

    /// <summary>Состояния IsFull и IsEmpty корректно меняются при операциях</summary>
    [TestMethod]
    public void State_Flags_Are_Correct()
    {
        var q = new LimitedQueue<int>(2);

        Assert.IsTrue(q.IsEmpty);
        Assert.IsFalse(q.IsFull);

        q.Enqueue(1);
        Assert.IsFalse(q.IsEmpty);
        Assert.IsFalse(q.IsFull);

        q.Enqueue(2);
        Assert.IsTrue(q.IsFull);

        _ = q.Dequeue();
        Assert.IsFalse(q.IsFull);

        q.Enqueue(3);
        Assert.IsTrue(q.IsFull);
    }

    /// <summary>Длительная работа с множественными обёртками индексов корректна</summary>
    [TestMethod]
    public void Works_Correctly_After_Many_Wraps()
    {
        var q = new LimitedQueue<int>(4);

        for (var i = 1; i <= 20; i++)
            q.Enqueue(i); // должны остаться последние 4: 17,18,19,20

        CollectionAssert.AreEqual(new[] { 17, 18, 19, 20 }, q.ToArray());

        for (var i = 17; i <= 20; i++)
            Assert.AreEqual(i, q.Dequeue());

        Assert.IsTrue(q.IsEmpty);
    }
}