#nullable enable
namespace MathCore.Collections;

/// <summary>Функциональное дерево</summary>
/// <typeparam name="T"></typeparam>
public class FTree<T>
{
    /// <summary>Пустое дерево</summary>
    public static FTree<T> Empty { get; } = new();

    /// <summary>Дерево пусто</summary>
    public bool IsEmpty => ReferenceEquals(this, Empty);

    /// <summary>Поддерево является листом (отсутствуют дочерние элементы)</summary>
    public bool IsLeaf => IsEmpty || Left.IsEmpty && Right.IsEmpty;

    /// <summary>Корень дерева</summary>
    public T Root { get; }

    /// <summary>Левое поддерево</summary>
    public FTree<T> Left { get; }

    /// <summary>Правое поддерево</summary>
    public FTree<T> Right { get; }

    /// <summary>Инициализация нового пустого дерева</summary>
    /// <exception cref="InvalidOperationException">При попытке создать ещё одно новое пустое поддерево - вызвать данный конструктор повторно</exception>
    private FTree()
    {
        if (Empty is not null)
            throw new InvalidOperationException("Нельзя создать ещё один пустой узел дерева. Используйте статическое свойство Empty");
        Root  = default!;
        Left  = null!;
        Right = null!;
    }

    /// <summary>Инициализация нового поддерева</summary>
    /// <param name="Root">Корень поддерева</param>
    /// <param name="Left">Левое поддерево</param>
    /// <param name="Right">Правое поддерево</param>
    private FTree(T Root, FTree<T>? Left = null, FTree<T>? Right = null)
    {
        this.Root  = Root;
        this.Left  = Left ?? Empty;
        this.Right = Right ?? Empty;
    }

    /// <summary>Создание нового узла дерева</summary>
    /// <param name="root">Корень</param>
    /// <param name="left">Левое поддерево</param>
    /// <param name="right">Правое поддерево</param>
    /// <returns>Созданный узел поддерева</returns>
    public static FTree<T> New(T root, FTree<T>? left = null, FTree<T>? right = null) => new(root, left, right);

    public FTree<T> AddLeft(T root, FTree<T>? right = null) => new(root, this, right);

    public FTree<T> AddRight(T root, FTree<T>? left = null) => new(root, left, this);

    public IEnumerable<T> EnumLeftRootRight()
    {
        if(IsEmpty) yield break;
        if(IsLeaf) yield return Root;

        var nodes = new Stack<FTree<T>> { this };
        while (nodes.Count > 0)
        {
            var left = nodes.Peek().Left;
            if (!left.IsEmpty) 
                nodes.Push(left);
            else
            {
                var (_, root, right) = nodes.Pop();
                yield return root;
                if(!right.IsEmpty)
                    nodes.Push(right);
            }
        }
    }

    public IEnumerable<T> EnumRightRootLeft()
    {
        if (IsEmpty) yield break;
        if (IsLeaf) yield return Root;

        var nodes = new Stack<FTree<T>> { this };
        while (nodes.Count > 0)
        {
            var right = nodes.Peek().Right;
            if (!right.IsEmpty)
                nodes.Push(right);
            else
            {
                var (left, root, _) = nodes.Pop();
                yield return root;
                if (!left.IsEmpty)
                    nodes.Push(left);
            }
        }
    }

    public IEnumerable<T> EnumRootLeftRight()
    {
        if (IsEmpty) yield break;

        var nodes = new Stack<FTree<T>> { this };

        while (nodes.Count > 0)
        {
            var (left, value, right) = nodes.Pop();
            yield return value;
            if(!left.IsEmpty) nodes.Add(left);
            if(!right.IsEmpty) nodes.Add(right);
        }
    }

    public IEnumerable<T> EnumRootRightLeft()
    {
        if (IsEmpty) yield break;

        var nodes = new Stack<FTree<T>> { this };

        while (nodes.Count > 0)
        {
            var (left, value, right) = nodes.Pop();
            yield return value;
            if (!right.IsEmpty) nodes.Add(right);
            if (!left.IsEmpty) nodes.Add(left);
        }
    }

    public IEnumerable<T> EnumLeftRightRoot()
    {
        if (IsEmpty) yield break;
        if (IsLeaf) yield return Root;

        var nodes = new Stack<FTree<T>> { this };
        if(!Right.IsEmpty) nodes.Push(Right);
        if(!Left.IsEmpty) nodes.Push(Left);

        if (!Left.IsEmpty)
            foreach (var item in Left.EnumLeftRootRight())
                yield return item;

        if (!Right.IsEmpty)
            foreach (var item in Right.EnumLeftRootRight())
                yield return item;

        yield return Root;
    }

    public IEnumerable<T> EnumRightLeftRoot()
    {
        if (IsEmpty) yield break;
        if (IsLeaf) yield return Root;

        if (!Right.IsEmpty)
            foreach (var item in Right.EnumLeftRootRight())
                yield return item;

        if (!Left.IsEmpty)
            foreach (var item in Left.EnumLeftRootRight())
                yield return item;

        yield return Root;
    }

    public void Deconstruct(out FTree<T> left, out T root, out FTree<T> right)
    {
        left  = Left;
        root  = Root;
        right = Right;
    }
}