using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    using ChildNodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode, ExpressionTreeNode>;
    using NodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode>;

    /// <summary>Узел дерева вычислений</summary>
    [ContractClass(typeof(ExpressionTreeNodeContract))]
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
                    Contract.Requires(i >= 0);
                    var Parent = Node.Parent;
                    for(var j = 0; j < i && Parent != null; j++)
                        Parent = Parent.Parent;
                    return Parent;
                }
            }

            /// <summary>Новый итератор предков узла</summary>
            /// <param name="Node">Обрабатываемый узел</param>
            public ParentsIterator([NotNull] ExpressionTreeNode Node)
            {
                Contract.Requires(Node != null);
                this.Node = Node;
            }

            /// <summary>Получить перечислитель предков узла</summary>
            /// <returns>Перечислитель предков узла</returns>
            IEnumerator<ExpressionTreeNode> IEnumerable<ExpressionTreeNode>.GetEnumerator()
            {
                Contract.Ensures(Contract.Result<IEnumerator<ExpressionTreeNode>>() != null);
                return GetParrentsEnumerable().GetEnumerator();
            }

            /// <summary>Получить перечислитель</summary>
            /// <returns>Перечислитель</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                Contract.Ensures(Contract.Result<IEnumerator>() != null);
                return ((IEnumerable<ExpressionTreeNode>)this).GetEnumerator();
            }

            /// <summary>Метод получения перечисления предков узла</summary>
            /// <returns>Перечисление предков узла</returns>
            [NotNull]
            private IEnumerable<ExpressionTreeNode> GetParrentsEnumerable()
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
                for(var node = Node; node.Parent != null; node = node.Parent)
                    yield return node.Parent;
            }

            public override string ToString() => this.ToSeparatedStr(" | ");
        }

        /// <summary>Узел левого поддерева</summary>
        [CanBeNull]
        private ExpressionTreeNode _Left;

        /// <summary> Узел правого поддерева</summary>
        [CanBeNull]
        private ExpressionTreeNode _Right;

        /// <summary>Признак возможности получения тревиального значения</summary>
        public virtual bool IsPrecomputable { [DebuggerStepThrough] get; } = false;

        /// <summary>Является ли узел дерева корнем?</summary>
        public bool IsRoot => Parent == null;

        /// <summary>Признак - является ли текущий узел левым поддеревом</summary>
        public bool IsLeftSubtree
        {
            [DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (Parent != null && Parent.Left == this));
                return Parent != null && Parent.Left == this;
            }
        }

        /// <summary>Признак - является ли текущий узел правым поддеревом</summary>
        public bool IsRightSubtree
        {
            [DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (Parent != null && Parent.Right == this));
                return Parent != null && Parent.Right == this;
            }
        }

        /// <summary>Ссылка на предка узла</summary>
        public ExpressionTreeNode Parent { get; set; }

        /// <summary>Левое поддерево</summary>
        [CanBeNull]
        public ExpressionTreeNode Left
        {
            [DebuggerStepThrough]
            get { return _Left; }
            set
            {
                Contract.Ensures(_Left == value);
                Contract.Ensures(value == null || value.Parent == this);
                if(_Left != null) _Left.Parent = null;
                _Left = value;
                if(value == null) return;
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
            [DebuggerStepThrough]
            get { return _Right; }
            set
            {
                Contract.Ensures(_Right == value);
                Contract.Ensures(value == null || value.Parent == this);
                if(_Right != null) _Right.Parent = null;
                _Right = value;
                if(value == null) return;
                if(value.IsLeftSubtree)
                    value.Parent.Left = null;
                else if(value.IsRightSubtree)
                    value.Parent.Right = null;
                value.Parent = this;
            }
        }

        /// <summary>Перечисление правых узлов правого поддерева включая корень</summary>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> RightNodes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
                return this[node => node.Right];
            }
        }

        /// <summary>Перечисление левых узлов левого поддерева включая корень</summary>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> LeftNodes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
                return this[node => node.Left];
            }
        }

        /// <summary>Перечислитель предков узла</summary>
        [NotNull]
        public ParentsIterator Parents
        {
            get
            {
                Contract.Ensures(Contract.Result<ParentsIterator>() != null);
                return new ParentsIterator(this);
            }
        }

        /// <summary>Ссылка на корень дерева</summary>
        [NotNull]
        public ExpressionTreeNode Root
        {
            get
            {
                Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
                return this[node => node.Parent].Last();
            }
        }

        /// <summary>Глубина положения узла в дереве</summary>
        public int Depth
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return this[node => node.Parent].Count() - 1;
            }
        }

        /// <summary>Самый левый дочерний узел, либо текущий</summary>
        public ExpressionTreeNode LastLeftChild
        {
            get
            {
                Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
                return this[node => node.Left].Last();
            }
            set => LastLeftChild.Left = value;
        }

        /// <summary>Самый правый дочерний узел, либо текущий</summary>
        public ExpressionTreeNode LastRightChild
        {
            get
            {
                Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
                return this[node => node.Right].Last();
            }
            set => LastRightChild.Right = value;
        }

        /// <summary>Доступ к элементам узла по указанному пути</summary>
        /// <param name="Path">Путь к элементам узла Пример:.\.\l\r\r\l  ..\l\r\r</param>
        /// <returns>Элемент узла, выбранный по указанному пути</returns>
        public ExpressionTreeNode this[[NotNull] string Path]
        {
            get
            {
                Contract.Requires(!string.IsNullOrEmpty(Path));
                var node = this;
                var elements = Path.ToLower().Split('/', '\\');
                for(var i = 0; i < elements.Length; i++)
                {
                    var element = elements[i];
                    switch(element)
                    {
                        case ".":
                            node = node.Parent;
                            break;
                        case "..":
                            node = node.Root;
                            break;
                        case "l":
                            node = node.Left;
                            break;
                        case "r":
                            node = node.Right;
                            break;
                        default:
                            throw new FormatException($"Неверный параметр в пути узла {Path} -> {element}",
                                new ArgumentException(nameof(Path)));
                    }
                    if(node == null) return null;
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
                Contract.Requires(ChildSelector != null);
                Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
                yield return this;
                for(var node = ChildSelector(Left, Right); node != null; node = ChildSelector(node.Left, node.Right))
                    yield return node;
            }
        }

        /// <summary> Итератор элементов узла методом выборки</summary>
        /// <param name="Selector">Метод выбборки узлов относительно текущего</param>
        /// <returns>Перечисление узлов по указанному методу</returns>
        [NotNull]
        public IEnumerable<ExpressionTreeNode> this[[NotNull] NodeSelector Selector]
        {
            get
            {
                Contract.Requires(Selector != null);
                Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
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
            Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
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
        public IEnumerable<ExpressionTreeNode> GetChilds()
        {
            Contract.Ensures(Contract.Result<IEnumerable<ExpressionTreeNode>>() != null);
            return Bypassing(ExpressionTree.BypassingType.RootLeftRight).Skip(1);
        }

        /// <summary>Поменять узел местами с дочерним</summary>
        /// <param name="Parent">Материнский узел</param>
        /// <param name="Child">Дочерний узел</param>
        private static void SwapToChild([NotNull] ExpressionTreeNode Parent, [NotNull] ExpressionTreeNode Child)
        {
            Contract.Requires(Parent != null);
            Contract.Requires(Child != null);
            Contract.Requires(Parent.Left == Child || Parent.Right == Child);

            var Parent_Parent = Parent.Parent;
            var IsParentLeft = Parent.IsLeftSubtree;
            Parent.Parent = null;

            var Child_Left = Child.Left;
            var Child_Right = Child.Right;

            if(Child_Left != null)
                Child_Left.Parent = null;
            if(Child_Right != null)
                Child_Right.Parent = null;

            Child.Parent = null;

            if(Parent.Left == Child)
            {
                var Parent_Right = Parent.Right;
                Parent.Right = null;
                if(Parent_Right != null)
                    Parent_Right.Parent = null;

                Parent.Left = null;

                if(Parent_Parent != null)
                    if(IsParentLeft)
                        Parent_Parent.Left = Child;
                    else
                        Parent_Parent.Right = Child;

                Child.Right = Parent_Right;
                Child.Left = Parent;
                Parent.Left = Child_Left;
                Parent.Right = Child_Right;
            }
            else
            {
                var Parent_Left = Parent.Left;
                Parent.Left = null;
                if(Parent_Left != null)
                    Parent_Left.Parent = null;

                Parent.Right = null;

                if(Parent_Parent != null)
                    if(IsParentLeft)
                        Parent_Parent.Left = Child;
                    else
                        Parent_Parent.Right = Child;

                Child.Left = Parent_Left;
                Child.Right = Parent;
                Parent.Left = Child_Left;
                Parent.Right = Child_Right;
            }

        }

        /// <summary>Подменить узел А узлом В</summary>
        /// <param name="A">Замещаемый узел</param>
        /// <param name="B">Подменяемый узел</param>
        public static void Swap([NotNull] ExpressionTreeNode A, [NotNull] ExpressionTreeNode B)
        {
            Contract.Requires(A != null);
            Contract.Requires(B != null);

            if(B.Left == A || B.Right == A) { SwapToChild(B, A); return; }
            if(A.Left == B || A.Right == B) { SwapToChild(A, B); return; }

            var A_Parent = A.Parent;
            var B_Parent = B.Parent;

            var A_Left = A.Left;
            var B_Left = B.Left;
            var A_Right = A.Right;
            var B_Right = B.Right;

            var A_IsLeft = A.IsLeftSubtree;
            var B_IsLeft = B.IsLeftSubtree;

            A.Parent = null;
            if(A_Parent != null)
                if(A_IsLeft)
                    A_Parent.Left = null;
                else
                    A_Parent.Right = null;

            B.Parent = null;
            if(B_Parent != null)
                if(B_IsLeft)
                    B_Parent.Left = null;
                else
                    B_Parent.Right = null;

            A.Left = null;
            A.Right = null;
            B.Left = null;
            B.Right = null;

            if(B_Parent != null)
                if(B_IsLeft)
                    B_Parent.Left = A;
                else
                    B_Parent.Right = A;

            if(A_Parent != null)
                if(A_IsLeft)
                    A_Parent.Left = B;
                else
                    A_Parent.Right = B;

            A.Left = B_Left;
            A.Right = B_Right;
            B.Left = A_Left;
            B.Right = A_Right;
        }

        /// <summary>Заменить узел на указанный</summary>
        /// <param name="Node">Ухел, на который производится замена</param>
        public void SwapTo([NotNull] ExpressionTreeNode Node)
        {
            Contract.Requires(Node != null);
            Swap(this, Node);
        }

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
            if(parent == null) // Если узел является корнем 
            {
                Left = null;
                Right = null;
                if(left == null) // Если нет левого поддерева
                {
                    if(right != null) // Если есть правое поддерево
                        right.Parent = null; // обнулить ссылку на корень
                    return right;
                }
                if(right == null) // Если нет правого поддерева
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
                if(left == null) // Если левого поддерева нет
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
                if(right == null) // Если правого поддерева нет
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
            } while(!(node == null || node.Right == last));

            return node?.Left;
        }

        /// <summary>Перечисление переменных, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева переменных</returns>
        [NotNull]
        public virtual IEnumerable<ExpressionVariabel> GetVariables()
        {
            IEnumerable<ExpressionVariabel> variables = null;
            if(_Left != null)
                variables = _Left.GetVariables();
            return _Right != null
                ? variables.AppendLast(_Right.GetVariables()).Distinct()
                : Enumerable.Empty<ExpressionVariabel>();
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
        public virtual IEnumerable<Functional> GetFunctionals()
        {
            IEnumerable<Functional> operators = null;
            if(_Left != null)
                operators = _Left.GetFunctionals();
            return _Right != null
                ? operators.AppendLast(_Right.GetFunctionals()).Distinct()
                : Enumerable.Empty<Functional>();
        }

        /// <summary>Уничтожить узел рекурентно с поддеревьями</summary>
        public void Dispose() { Left?.Dispose(); Right?.Dispose(); }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        [NotNull]
        public abstract ExpressionTreeNode Clone();

        /// <summary>Преобразование узла в строку</summary>
        /// <returns>Строковое представление узла</returns>
        [NotNull]
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return $"{Left?.ToString() ?? ""}{Right?.ToString() ?? ""}";
        }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        [NotNull]
        object ICloneable.Clone()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return Clone();
        }

        /// <summary>Оператор неявного преобразования строки в узел дерева</summary>
        /// <param name="value">Строковое значение</param>
        /// <returns>Строковый узел дерева</returns>
        [NotNull]
        public static implicit operator ExpressionTreeNode([NotNull] string value)
        {
            Contract.Requires(!string.IsNullOrEmpty(value));
            Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
            return new StringNode(value);
        }
    }

    [ContractClassFor(typeof(ExpressionTreeNode))]
    [ExcludeFromCodeCoverage]
    abstract class ExpressionTreeNodeContract : ExpressionTreeNode
    {
        private ExpressionTreeNodeContract() { }

        public override ExpressionTreeNode Clone()
        {
            Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
            throw new NotImplementedException();
        }
    }
}
