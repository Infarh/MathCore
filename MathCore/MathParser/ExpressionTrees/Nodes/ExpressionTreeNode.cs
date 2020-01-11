using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    using ChildNodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode, ExpressionTreeNode>;
    using NodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode>;

    /// <summary>Узел дерева вычислений</summary>
    public abstract class ExpressionTreeNode : IDisposable, ICloneable<ExpressionTreeNode>
    {
        /// <summary>Перечислитель предков узла</summary>
        public sealed class ParentsIterator : IEnumerable<ExpressionTreeNode>
        {
            /// <summary>Исходный узел</summary>
            private ExpressionTreeNode Node { get; }

            /// <summary>Итератор предков узла, где узел с индексом 0 - первый предок узла</summary>
            /// <param name="i">Номер предка</param>
            /// <returns>Предок указанного поколения</returns>
            [CanBeNull]
            public ExpressionTreeNode this[int i]
            {
                get
                {
                    var Parent = Node.Parent;
                    for(var j = 0; j < i && Parent != null; j++)
                        Parent = Parent.Parent;
                    return Parent;
                }
            }

            /// <summary>Новый итератор предков узла</summary>
            /// <param name="Node">Обрабатываемый узел</param>
            public ParentsIterator([NotNull] ExpressionTreeNode Node) => this.Node = Node;

            /// <summary>Получить перечислитель предков узла</summary>
            /// <returns>Перечислитель предков узла</returns>
            IEnumerator<ExpressionTreeNode> IEnumerable<ExpressionTreeNode>.GetEnumerator() => GetParentsEnumerable().GetEnumerator();

            /// <summary>Получить перечислитель</summary>
            /// <returns>Перечислитель</returns>
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ExpressionTreeNode>)this).GetEnumerator();

            /// <summary>Метод получения перечисления предков узла</summary>
            /// <returns>Перечисление предков узла</returns>
            [NotNull]
            [ItemNotNull]
            private IEnumerable<ExpressionTreeNode> GetParentsEnumerable()
            {
                for(var node = Node; node.Parent != null; node = node.Parent)
                    yield return node.Parent;
            }

            [NotNull] public override string ToString() => this.ToSeparatedStr(" | ");
        }

        /// <summary>Узел левого поддерева</summary>
        [CanBeNull]
        private ExpressionTreeNode _Left;

        /// <summary> Узел правого поддерева</summary>
        [CanBeNull]
        private ExpressionTreeNode _Right;

        /// <summary>Признак возможности получения тривиального значения</summary>
        public virtual bool IsPrecomputable { [DST] get; } = false;

        /// <summary>Является ли узел дерева корнем?</summary>
        public bool IsRoot => Parent is null;

        /// <summary>Признак - является ли текущий узел левым поддеревом</summary>
        public bool IsLeftSubtree => Parent != null && Parent.Left == this;

        /// <summary>Признак - является ли текущий узел правым поддеревом</summary>
        public bool IsRightSubtree => Parent != null && Parent.Right == this;

        /// <summary>Ссылка на предка узла</summary>
        public ExpressionTreeNode Parent { get; set; }

        /// <summary>Левое поддерево</summary>
        [CanBeNull]
        public ExpressionTreeNode Left
        {
            [DST]
            get => _Left;
            set
            {
                if(_Left != null) _Left.Parent = null;
                _Left = value;
                if(value is null) return;
                if(value.IsLeftSubtree)
                    value.Parent.Left = null;
                else if(value.IsRightSubtree)
                    value.Parent.Right = null;
                value.Parent = this;
            }
        }

        /// <summary>Правое поддерево</summary>
        [CanBeNull]
        public ExpressionTreeNode Right
        {
            [DST]
            get => _Right;
            set
            {
                if(_Right != null) _Right.Parent = null;
                _Right = value;
                if(value is null) return;
                if(value.IsLeftSubtree)
                    value.Parent.Left = null;
                else if(value.IsRightSubtree)
                    value.Parent.Right = null;
                value.Parent = this;
            }
        }

        /// <summary>Перечисление правых узлов правого поддерева включая корень</summary>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> RightNodes => this[node => node.Right];

        /// <summary>Перечисление левых узлов левого поддерева включая корень</summary>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> LeftNodes => this[node => node.Left];

        /// <summary>Перечислитель предков узла</summary>
        [NotNull]
        public ParentsIterator Parents => new ParentsIterator(this);

        /// <summary>Ссылка на корень дерева</summary>
        [NotNull]
        public ExpressionTreeNode Root => this[node => node.Parent].Last();

        /// <summary>Глубина положения узла в дереве</summary>
        public int Depth => this[node => node.Parent].Count() - 1;

        /// <summary>Самый левый дочерний узел, либо текущий</summary>
        public ExpressionTreeNode LastLeftChild
        {
            get => this[node => node.Left].Last();
            set => LastLeftChild.Left = value;
        }

        /// <summary>Самый правый дочерний узел, либо текущий</summary>
        public ExpressionTreeNode LastRightChild
        {
            get => this[node => node.Right].Last();
            set => LastRightChild.Right = value;
        }

        /// <summary>Доступ к элементам узла по указанному пути</summary>
        /// <param name="path">Путь к элементам узла Пример:.\.\l\r\r\l  ..\l\r\r</param>
        /// <returns>Элемент узла, выбранный по указанному пути</returns>
        [CanBeNull]
        public ExpressionTreeNode this[[NotNull] string path]
        {
            get
            {
                var node = this;
                var elements = path.ToLower().Split('/', '\\');
                for(var i = 0; i < elements.Length; i++)
                {
                    node = elements[i] switch
                    {
                        "." => node.Parent,
                        ".." => node.Root,
                        "l" => node.Left,
                        "r" => node.Right,
                        _ => throw new FormatException($"Неверный параметр в пути узла {path} -> {elements[i]}",
                            new ArgumentException(nameof(path)))
                    };
                    if(node is null) return null;
                }
                return node;
            }
        }

        /// <summary>Доступ к дочерним элементам узла с помощью метода выбора</summary>
        /// <param name="ChildSelector">Метод выбора элементов узла</param>
        /// <returns>Перечисление дочерних элементов по указанному методу</returns>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> this[[NotNull] ChildNodeSelector ChildSelector]
        {
            get
            {
                yield return this;
                for(var node = ChildSelector(Left, Right); node != null; node = ChildSelector(node.Left, node.Right))
                    yield return node;
            }
        }

        /// <summary> Итератор элементов узла методом выборки</summary>
        /// <param name="Selector">Метод выборки узлов относительно текущего</param>
        /// <returns>Перечисление узлов по указанному методу</returns>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> this[[NotNull] NodeSelector Selector]
        {
            get
            {
                yield return this;
                for(var node = Selector(this); node != null; node = Selector(node))
                    yield return node;
            }
        }

        /// <summary>Путь к узлу</summary>
        [NotNull]
        public string Path => this[node => node.Parent]
                    .Select(node => node.IsLeftSubtree ? "l" : (node.IsRightSubtree ? "r" : ".."))
                    .Reverse().ToSeparatedStr("/");

        /// <summary>Метод обхода дерева</summary>
        /// <param name="type">Тип обхода дерева</param>
        /// <returns>Перечисление элементов дерева</returns>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> Bypassing(ExpressionTree.BypassingType type)
        {
            switch(type)
            {
                case ExpressionTree.BypassingType.RootRightLeft:
                    yield return this;
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.RightLeftRoot:
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    yield return this;
                    break;
                case ExpressionTree.BypassingType.RootLeftRight:
                    yield return this;
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.LeftRootRight:
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    yield return this;
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.LeftRightRoot:
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    yield return this;
                    break;
                case ExpressionTree.BypassingType.RightRootLeft:
                    if(Right != null) foreach(var node in Right.Bypassing(type)) yield return node;
                    yield return this;
                    if(Left != null) foreach(var node in Left.Bypassing(type)) yield return node;
                    break;
                default:
                    throw new NotSupportedException($"Тип обхода дерева {type} не поддерживается");
            }
        }


        /// <summary>Перечисление дочерних элементов дерева</summary>
        /// <returns>Перечисление дочерних элементов дерева</returns>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> GetChilds() => Bypassing(ExpressionTree.BypassingType.RootLeftRight).Skip(1);

        /// <summary>Поменять узел местами с дочерним</summary>
        /// <param name="Parent">Материнский узел</param>
        /// <param name="Child">Дочерний узел</param>
        private static void SwapToChild([NotNull] ExpressionTreeNode Parent, [NotNull] ExpressionTreeNode Child)
        {
            var parent_parent = Parent.Parent;
            var is_parent_left = Parent.IsLeftSubtree;
            Parent.Parent = null;

            var child_left = Child.Left;
            var child_right = Child.Right;

            if(child_left != null)
                child_left.Parent = null;
            if(child_right != null)
                child_right.Parent = null;

            Child.Parent = null;

            if(Parent.Left == Child)
            {
                var parent_right = Parent.Right;
                Parent.Right = null;
                if(parent_right != null)
                    parent_right.Parent = null;

                Parent.Left = null;

                if(parent_parent != null)
                    if(is_parent_left)
                        parent_parent.Left = Child;
                    else
                        parent_parent.Right = Child;

                Child.Right = parent_right;
                Child.Left = Parent;
                Parent.Left = child_left;
                Parent.Right = child_right;
            }
            else
            {
                var parent_left = Parent.Left;
                Parent.Left = null;
                if(parent_left != null)
                    parent_left.Parent = null;

                Parent.Right = null;

                if(parent_parent != null)
                    if(is_parent_left)
                        parent_parent.Left = Child;
                    else
                        parent_parent.Right = Child;

                Child.Left = parent_left;
                Child.Right = Parent;
                Parent.Left = child_left;
                Parent.Right = child_right;
            }

        }

        /// <summary>Подменить узел А узлом В</summary>
        /// <param name="A">Замещаемый узел</param>
        /// <param name="B">Подменяемый узел</param>
        public static void Swap([NotNull] ExpressionTreeNode A, [NotNull] ExpressionTreeNode B)
        {
            if(B.Left == A || B.Right == A) { SwapToChild(B, A); return; }
            if(A.Left == B || A.Right == B) { SwapToChild(A, B); return; }

            var a_parent = A.Parent;
            var b_parent = B.Parent;

            var a_left = A.Left;
            var b_left = B.Left;
            var a_right = A.Right;
            var b_right = B.Right;

            var a_is_left = A.IsLeftSubtree;
            var b_is_left = B.IsLeftSubtree;

            A.Parent = null;
            if(a_parent != null)
                if(a_is_left)
                    a_parent.Left = null;
                else
                    a_parent.Right = null;

            B.Parent = null;
            if(b_parent != null)
                if(b_is_left)
                    b_parent.Left = null;
                else
                    b_parent.Right = null;

            A.Left = null;
            A.Right = null;
            B.Left = null;
            B.Right = null;

            if(b_parent != null)
                if(b_is_left)
                    b_parent.Left = A;
                else
                    b_parent.Right = A;

            if(a_parent != null)
                if(a_is_left)
                    a_parent.Left = B;
                else
                    a_parent.Right = B;

            A.Left = b_left;
            A.Right = b_right;
            B.Left = a_left;
            B.Right = a_right;
        }

        /// <summary>Заменить узел на указанный</summary>
        /// <param name="Node">Узел, на который производится замена</param>
        public void SwapTo([NotNull] ExpressionTreeNode Node) => Swap(this, Node);

        /// <summary>Удалить узел с перекоммутацией ссылок</summary>
        /// <returns>Если удаляется корень, то левое поддерево, иначе ссылка не предка узла</returns>
        [NotNull]
        public ExpressionTreeNode Remove()
        {
            var parent = Parent;
            var left = Left;
            var right = Right;

            //   |        |
            //   *    ->  L
            //  / \        \
            // L   R        R
            if(parent is null) // Если узел является корнем 
            {
                Left = null;
                Right = null;
                if(left is null) // Если нет левого поддерева
                {
                    if(right != null) // Если есть правое поддерево
                        right.Parent = null; // обнулить ссылку на корень
                    return right;
                }
                if(right is null) // Если нет правого поддерева
                {
                    left.Parent = null; // Обнулить ссылку у левого поддерева на корень
                    return left;
                }

                // Для всех узлов левого поддерева взять правое поддерево
                //left[node => node.Right]
                //    .TakeWhile(RightSubtreeNode => RightSubtreeNode != null) // до тех пор, пока есть узлы
                //    .Last() // взять последний из последовательности
                //    .Right = right; // записать в его правое поддерево правое поддерево корня
                left.LastRightChild.Right = right;
                return left;
            }

            //     P             P
            //    / \           / \
            //   *   ...  ->   L   ...
            //  / \           / \  
            // L    R      ...   R    
            if(IsLeftSubtree) // Узел является левым поддеревом
            {
                if(left is null) // Если левого поддерева нет
                    parent.Left = right; // то левым поддеревом родительского узла будет правое поддерево
                else
                {   //иначе - левое поддерево
                    parent.Left = left;
                    // в самый правый дочерний узел левого поддерева записать правое
                    left.LastRightChild.Right = right; //[node => node.Right].Last().Right = right;
                }
            }
            //     P                P
            //    / \              / \
            // ...   *     ->   ...   R
            //      / \              / \
            //     L   R            L   ...
            else // Узел является правым поддеревом
            {
                if(right is null) // Если правого поддерева нет
                    parent.Right = left; // то правым поддеревом родительского узла будет левое поддерево
                else
                {   //иначе - правое поддерево
                    parent.Right = right;
                    // в самый левый дочерний узел правого поддерева записать левое
                    right.LastLeftChild.Left = left; //[node => node.Left].Last().Left = left;
                }
            }
            Parent = null;
            Left = null;
            Right = null;
            return parent;
        }

        /// <summary>Получить следующий узел слева</summary>
        /// <returns>Узел слева от текущего</returns>
        [CanBeNull]
        public ExpressionTreeNode GetNextLeft()
        {
            if(Left != null) return Left;
            if(Right != null) return Right;
            var is_left = IsLeftSubtree;
            var node = Parent;
            while(node != null)
            {
                if(is_left && node.Right != null)
                    return node.Right;
                is_left = node.IsLeftSubtree;
                node = node.Parent;
            }
            return null;
        }

        /// <summary>Получить следующий узел справа</summary>
        /// <returns>Узел справа от текущего</returns>
        [CanBeNull]
        public ExpressionTreeNode GetNextRight()
        {
            if(Left != null) return Left;
            if(Right != null) return Right;
            var node = this;
            ExpressionTreeNode last;
            do
            {
                last = node;
                node = node.Parent;
            } while(!(node is null || node.Right == last));

            return node?.Left;
        }

        /// <summary>Перечисление переменных, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева переменных</returns>
        [NotNull]
        public virtual IEnumerable<ExpressionVariable> GetVariables()
        {
            IEnumerable<ExpressionVariable> variables = null;
            if(_Left != null)
                variables = _Left.GetVariables();
            return _Right != null
                ? variables.AppendLast(_Right.GetVariables()).Distinct()
                : Enumerable.Empty<ExpressionVariable>();
        }

        /// <summary>Перечисление функций, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева функций</returns>
        [NotNull]
        public virtual IEnumerable<ExpressionFunction> GetFunctions()
        {
            IEnumerable<ExpressionFunction> functions = null;
            if(_Left != null)
                functions = _Left.GetFunctions();
            return _Right != null
                ? functions.AppendLast(_Right.GetFunctions()).Distinct()
                : Enumerable.Empty<ExpressionFunction>();
        }

        /// <summary>Перечисление функционалов, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева функционалов</returns>
        [NotNull]
        public virtual IEnumerable<Functional> GetFunctionals()
        {
            IEnumerable<Functional> operators = null;
            if(_Left != null)
                operators = _Left.GetFunctionals();
            return _Right != null
                ? operators.AppendLast(_Right.GetFunctionals()).Distinct()
                : Enumerable.Empty<Functional>();
        }

        /// <summary>Уничтожить узел рекуррентно с поддеревьями</summary>
        public void Dispose() { Left?.Dispose(); Right?.Dispose(); }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        [NotNull]
        public abstract ExpressionTreeNode Clone();

        /// <summary>Преобразование узла в строку</summary>
        /// <returns>Строковое представление узла</returns>
        [NotNull]
        public override string ToString() => $"{Left?.ToString() ?? string.Empty}{Right?.ToString() ?? string.Empty}";

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        [NotNull]
        object ICloneable.Clone() => Clone();

        /// <summary>Оператор неявного преобразования строки в узел дерева</summary>
        /// <param name="value">Строковое значение</param>
        /// <returns>Строковый узел дерева</returns>
        [NotNull]
        public static implicit operator ExpressionTreeNode([NotNull] string value) => new StringNode(value);
    }
}