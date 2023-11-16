#nullable enable
using System.Collections;

using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees;

/// <summary>Дерево выражения</summary>
public sealed class ExpressionTree : IDisposable, ICloneable<ExpressionTree>, IEnumerable<ExpressionTreeNode>
{
    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Метод обхода дерева</summary>
    public enum BypassingType
    {
        /// <summary>Левое поддерево, правое поддерево, корень</summary>
        LeftRightRoot,
        /// <summary>Левое поддерево, корень, правое поддерево</summary>
        LeftRootRight,
        /// <summary>Корень, левое поддерево, правое поддерево</summary>
        RootLeftRight,
        /// <summary>Правое поддерево, левое поддерево, корень</summary>
        RightLeftRoot,
        /// <summary>Правое поддерево, корень, левое поддерево</summary>
        RightRootLeft,
        /// <summary>Корень, правое поддерево, левое поддерево</summary>
        RootRightLeft
    }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Корень</summary>
    public ExpressionTreeNode Root { get; set; }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового дерева математического выражения</summary>
    public ExpressionTree() { }

    /// <summary>Инициализация нового дерева математического выражения</summary><param name="Root">Узел - корень дерева</param>
    public ExpressionTree(ExpressionTreeNode Root) => this.Root = Root;

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Очистить дерево</summary>
    public void Clear()
    {
        var root = Root;
        Root = null;
        root?.Dispose();
    }

    /// <summary>Удалить узел</summary><param name="Node">Удаляемый узел</param>
    public void Remove(ExpressionTreeNode Node)
    {
        // сохраняем ссылку на предка узла
        var parent = Node.Parent;
        // Сохраняем ссылки на поддеревья
        var right = Node.Right;
        var left  = Node.Left;

        Node.Parent = null;
        Node.Left   = null;
        Node.Right  = null;

        if(parent is null)   // Если у узла нет родительского узла
            if(Node == Root) // и при этом он является корнем
            {
                if(left is null)
                {
                    if(right is null) return;
                    right.Parent = null; // обнулить ссылку на корень
                    Root         = right;
                    return;
                }
                if(right is null) // Если нет правого поддерева
                {
                    left.Parent = null; // Обнулить ссылку у левого поддерева на корень
                    Root        = left;
                    return;
                }

                Root = left; // корнем дерева назначается левое поддерево удаляемого узла
                //Выбираем в левом поддереве самый правый узел
                left.LastRightChild
                   .Right = right; // и в его правое поддерево записываем правое поддерево удаляемого узла
                return;
            }
            else //если узел не является корнем и не имеет предка, то это ошибка - узел не принадлежит дереву
                throw new ArgumentException("Удаляемый узел не принадлежит дереву");

        // узел не является корневым.
        if(Node.IsLeftSubtree) // Если узел является левым поддеревом
        {
            if(left is null)         // Если левого поддерева нет
                parent.Left = right; // то левым поддеревом родительского узла будет правое поддерево
            else
            { //иначе - левое поддерево
                parent.Left = left;
                // в самый правый дочерний узел левого поддерева записать правое
                left.LastRightChild.Right = right;
            }
        }
        else // иначе узел является правым поддеревом
        {
            if(right is null)        // Если правого поддерева нет
                parent.Right = left; // то правым поддеревом родительского узла будет левое поддерево
            else
            { //иначе - правое поддерево
                parent.Right = right;
                // в самый левый дочерний узел правого поддерева записать левое
                right.LastLeftChild.Left = left;
            }
        }
    }

    /// <summary>Заменить узел</summary><param name="OldNode">Исходный узел</param><param name="NewNode">Новый узел</param>
    public void Swap(ExpressionTreeNode OldNode, ExpressionTreeNode NewNode)
    {
        OldNode.SwapTo(NewNode);
        if(Root == OldNode) Root = NewNode;
    }

    /// <summary>Переместить узел вниз</summary><param name="Node">Перемещаемый узел</param>
    public void MoveParentDown(ExpressionTreeNode Node)
    {
        var parent          = Node.Parent;
        var is_left_subtree = Node.IsLeftSubtree;

        if(is_left_subtree)
        {
            parent.Left = null;

            if(parent.IsLeftSubtree)
                parent.Parent.Left = Node;
            else if(parent.IsRightSubtree)
                parent.Parent.Right = Node;
            else
                Node.Parent = null;

            var right = Node.Right;
            Node.Right = null;
            if(right != null)
                right.Parent = null;

            Node.Right  = parent;
            parent.Left = right;
        }
        else
        {
            parent.Right = null;

            if(parent.IsLeftSubtree)
                parent.Parent.Left = Node;
            else if(parent.IsRightSubtree)
                parent.Parent.Right = Node;
            else
                Node.Parent = null;

            var left = Node.Left;
            Node.Left = null;
            if(left != null)
                left.Parent = null;

            Node.Left    = parent;
            parent.Right = left;
        }

        if(Root == parent) Root = Node;
    }

    /// <summary>Обойти дерево</summary><param name="type">Способ обхода</param><returns>Перечисление узлов дерева по указанному способу обхода</returns>
    public IEnumerable<ExpressionTreeNode> Bypass(BypassingType type = BypassingType.LeftRootRight) => Root.Bypassing(type);

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Уничтожить дерево</summary>
    public void Dispose() => Clear();

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Возвращает объект <see cref="T:System.String"/>, который представляет текущий объект <see cref="T:System.Object"/>.</summary>
    /// <returns>Объект <see cref="T:System.String"/>, представляющий текущий объект <see cref="T:System.Object"/>.</returns>
    public override string ToString() => Root?.ToString() ?? "ExpressionTree";

    /// <summary>Клонировать дерево</summary><returns>Клон дерева</returns>
    public ExpressionTree Clone() => new(Root.Clone());

    /// <summary>Клонировать дерево</summary><returns>Клон дерева</returns>
    object ICloneable.Clone() => Clone();

    /// <summary>Получить перечислитель узлов дерева по методу ЛКП</summary><returns>Перечислитель узлов дерева по методу ЛКП</returns>
    public IEnumerator<ExpressionTreeNode> GetEnumerator() => Bypass().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}