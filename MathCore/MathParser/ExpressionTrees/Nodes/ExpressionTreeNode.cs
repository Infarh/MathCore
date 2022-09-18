#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    using ChildNodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode, ExpressionTreeNode?>;
    using NodeSelector = Func<ExpressionTreeNode, ExpressionTreeNode?>;

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
            public ExpressionTreeNode? this[int i]
            {
                get
                {
                    var node_parent = Node.Parent;
                    for (var j = 0; j < i && node_parent is { }; j++)
                        node_parent = node_parent.Parent;
                    return node_parent;
                }
            }

            /// <summary>Новый итератор предков узла</summary>
            /// <param name="Node">Обрабатываемый узел</param>
            public ParentsIterator(ExpressionTreeNode Node) => this.Node = Node;

            /// <summary>Получить перечислитель предков узла</summary>
            /// <returns>Перечислитель предков узла</returns>
            IEnumerator<ExpressionTreeNode> IEnumerable<ExpressionTreeNode>.GetEnumerator() => GetParentsEnumerable().GetEnumerator();

            /// <summary>Получить перечислитель</summary>
            /// <returns>Перечислитель</returns>
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ExpressionTreeNode>)this).GetEnumerator();

            /// <summary>Метод получения перечисления предков узла</summary>
            /// <returns>Перечисление предков узла</returns>
            private IEnumerable<ExpressionTreeNode> GetParentsEnumerable()
            {
                for (var node = Node; node.Parent != null; node = node.Parent)
                    yield return node.Parent;
            }

            public override string ToString() => this.ToSeparatedStr(" | ");
        }

        /// <summary>Узел левого поддерева</summary>
        private ExpressionTreeNode? _Left;

        /// <summary> Узел правого поддерева</summary>
        private ExpressionTreeNode? _Right;

        /// <summary>Признак возможности получения тривиального значения</summary>
        public virtual bool IsPrecomputable { [DST] get => false; }

        /// <summary>Является ли узел дерева корнем?</summary>
        public bool IsRoot => Parent is null;

        /// <summary>Признак - является ли текущий узел левым поддеревом</summary>
        public bool IsLeftSubtree => Parent != null && Parent.Left == this;

        /// <summary>Признак - является ли текущий узел правым поддеревом</summary>
        public bool IsRightSubtree => Parent != null && Parent.Right == this;

        /// <summary>Ссылка на предка узла</summary>
        public ExpressionTreeNode? Parent { get; set; }

        /// <summary>Левое поддерево</summary>
        public ExpressionTreeNode? Left
        {
            [DST]
            get => _Left;
            set
            {
                if (_Left != null) _Left.Parent = null;
                _Left = value;
                if (value is null) return;
                if (value.IsLeftSubtree)
                    value.Parent!.Left = null;
                else if (value.IsRightSubtree)
                    value.Parent!.Right = null;
                value.Parent = this;
            }
        }

        /// <summary>Правое поддерево</summary>
        public ExpressionTreeNode? Right
        {
            [DST]
            get => _Right;
            set
            {
                if (_Right != null) _Right.Parent = null;
                _Right = value;
                if (value is null) return;
                if (value.IsLeftSubtree)
                    value.Parent!.Left = null;
                else if (value.IsRightSubtree)
                    value.Parent!.Right = null;
                value.Parent = this;
            }
        }

        /// <summary>Перечисление правых узлов правого поддерева включая корень</summary>
        public IEnumerable<ExpressionTreeNode> RightNodes => this[node => node.Right];

        /// <summary>Перечисление левых узлов левого поддерева включая корень</summary>
        public IEnumerable<ExpressionTreeNode> LeftNodes => this[node => node.Left];

        /// <summary>Перечислитель предков узла</summary>
        public ParentsIterator Parents => new(this);

        /// <summary>Ссылка на корень дерева</summary>
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
        public ExpressionTreeNode? this[string path]
        {
            get
            {
                var node = this;
                var elements = path.ToLower().Split('/', '\\');
                for (var i = 0; i < elements.Length; i++)
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
                    if (node is null) return null;
                }
                return node;
            }
        }

        /// <summary>Доступ к дочерним элементам узла с помощью метода выбора</summary>
        /// <param name="ChildSelector">Метод выбора элементов узла</param>
        /// <returns>Перечисление дочерних элементов по указанному методу</returns>
        public IEnumerable<ExpressionTreeNode> this[ChildNodeSelector ChildSelector]
        {
            get
            {
                yield return this;
                for (var node = ChildSelector(Left!, Right!); node != null; node = ChildSelector(node.Left!, node.Right!))
                    yield return node;
            }
        }

        /// <summary> Итератор элементов узла методом выборки</summary>
        /// <param name="Selector">Метод выборки узлов относительно текущего</param>
        /// <returns>Перечисление узлов по указанному методу</returns>
        public IEnumerable<ExpressionTreeNode> this[NodeSelector Selector]
        {
            get
            {
                yield return this;
                for (var node = Selector(this); node != null; node = Selector(node))
                    yield return node;
            }
        }

        /// <summary>Путь к узлу</summary>
        public string Path => this[node => node.Parent]
                    .Select(node => node.IsLeftSubtree ? "l" : (node.IsRightSubtree ? "r" : ".."))
                    .Reverse().ToSeparatedStr("/");

        /// <summary>Метод обхода дерева</summary>
        /// <param name="type">Тип обхода дерева</param>
        /// <returns>Перечисление элементов дерева</returns>
        public IEnumerable<ExpressionTreeNode> Bypassing(ExpressionTree.BypassingType type)
        {
            switch (type)
            {
                case ExpressionTree.BypassingType.RootRightLeft:
                    yield return this;
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.RightLeftRoot:
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    yield return this;
                    break;
                case ExpressionTree.BypassingType.RootLeftRight:
                    yield return this;
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.LeftRootRight:
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    yield return this;
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    break;
                case ExpressionTree.BypassingType.LeftRightRoot:
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    yield return this;
                    break;
                case ExpressionTree.BypassingType.RightRootLeft:
                    if (Right != null) foreach (var node in Right.Bypassing(type)) yield return node;
                    yield return this;
                    if (Left != null) foreach (var node in Left.Bypassing(type)) yield return node;
                    break;
                default:
                    throw new NotSupportedException($"Тип обхода дерева {type} не поддерживается");
            }
        }


        /// <summary>Перечисление дочерних элементов дерева</summary>
        /// <returns>Перечисление дочерних элементов дерева</returns>
        public IEnumerable<ExpressionTreeNode> GetChilds() => Bypassing(ExpressionTree.BypassingType.RootLeftRight).Skip(1);

        /// <summary>Поменять узел местами с дочерним</summary>
        /// <param name="Parent">Материнский узел</param>
        /// <param name="Child">Дочерний узел</param>
        private static void SwapToChild(ExpressionTreeNode Parent, ExpressionTreeNode Child)
        {
            var parent_parent = Parent.Parent;
            var is_parent_left = Parent.IsLeftSubtree;
            Parent.Parent = null;

            var child_left = Child.Left;
            var child_right = Child.Right;

            if (child_left != null)
                child_left.Parent = null;
            if (child_right != null)
                child_right.Parent = null;

            Child.Parent = null;

            if (Parent.Left == Child)
            {
                var parent_right = Parent.Right;
                Parent.Right = null;
                if (parent_right != null)
                    parent_right.Parent = null;

                Parent.Left = null;

                if (parent_parent != null)
                    if (is_parent_left)
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
                if (parent_left != null)
                    parent_left.Parent = null;

                Parent.Right = null;

                if (parent_parent != null)
                    if (is_parent_left)
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
        public static void Swap(ExpressionTreeNode A, ExpressionTreeNode B)
        {
            if (B.Left == A || B.Right == A) { SwapToChild(B, A); return; }
            if (A.Left == B || A.Right == B) { SwapToChild(A, B); return; }

            var a_parent = A.Parent;
            var b_parent = B.Parent;

            var a_left = A.Left;
            var b_left = B.Left;
            var a_right = A.Right;
            var b_right = B.Right;

            var a_is_left = A.IsLeftSubtree;
            var b_is_left = B.IsLeftSubtree;

            A.Parent = null;
            if (a_parent != null)
                if (a_is_left)
                    a_parent.Left = null;
                else
                    a_parent.Right = null;

            B.Parent = null;
            if (b_parent != null)
                if (b_is_left)
                    b_parent.Left = null;
                else
                    b_parent.Right = null;

            A.Left = null;
            A.Right = null;
            B.Left = null;
            B.Right = null;

            if (b_parent != null)
                if (b_is_left)
                    b_parent.Left = A;
                else
                    b_parent.Right = A;

            if (a_parent != null)
                if (a_is_left)
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
        public void SwapTo(ExpressionTreeNode Node) => Swap(this, Node);

        /// <summary>Удалить узел с перекоммутацией ссылок</summary>
        /// <returns>Если удаляется корень, то левое поддерево, иначе ссылка не предка узла</returns>
        public ExpressionTreeNode? Remove()
        {
            // Запоминаем что было в качестве
            var parent = Parent; // - родительского узла
            var left = Left;     // - узла левого поддерева
            var right = Right;   // - узла правого поддерева

            // Если родительский узел есть, то удаляя текущий узел надо заменить его узлом левого поддерева
            // При этом, всё левое поддерево левого поддерева станет левым поддеревом текущего узла,
            // А правое поддерево левого поддерева удаляемого узла должно быть перемещено в левое поддерево
            // самого правого листа правого поддерева текущего удаляемого узла
            //       root            root       root - родительский узел
            //        |               |         *    - текущий узел
            //      _-*-_     ->    _-L-_       L    - узел корня левого поддерева
            //     /     \         /     \      R    - узел корня правого поддерева
            //    L       R      Ll       R     Ll   - левый дочерний узел левого поддерева
            //   / \     / \             / \    Lr   - правый дочерний узел левого поддерева
            // Ll   Lr Rl   Rr         Rl   Rr  Rl   - левый дочерний узел правого поддерева
            //                        /         Rr   - правый дочерний узел правого поддерева
            //                      Lr
            if (parent is null) // Если родительского узла нет и текущий узел - это корень всего дерева...
            {
                // Отсоединяем пддеревья  - так как удаляемый узел корень и дерево разваливается
                Left = null;
                Right = null;

                // Проверяем случаи когда есть лишь одно из поддеревьев
                if (left is null) // Если левого поддерева не было...
                {
                    if (right != null)       // и при этом правое поддерево есть, то...
                        right.Parent = null; //    у правого поддерева убираем ссылку на корень (на текущий узел)
                    return right;            // В любом случае результатом будет правое поддерево что бы там не было.
                }
                if (right is null) // Если правого поддерева не было...
                {
                    if (left != null)       // и при этом левое поддерево есть, то...
                        left.Parent = null; //    у левого поддерева убираем ссылку на текущий узел (его корень)
                    return left;            // и всё что есть в левом поддереве возвращаем в качестве результата.
                }

                //        |             |          *    - текущий узел
                //      _-*-_     ->    L          L    - узел корня левого поддерева
                //     /     \         / \         R    - узел корня правого поддерева
                //    L       R      Ll   Lr       Ll   - левый дочерний узел левого поддерева
                //   / \     / \            \      Lr   - правый дочерний узел левого поддерева
                // Ll   Lr Rl   Rr           R     Rl   - левый дочерний узел правого поддерева
                //                          / \    Rr   - правый дочерний узел правого поддерева
                //                        Rl   Rr
                // Если есть оба поддерева, то берём левое поддерево и в самый правый его узел в его левое поддерево записываем текущее правое поддерево
                // Для всех узлов левого поддерева взять правое поддерево
                //left[node => node.Right]
                //    .TakeWhile(RightSubtreeNode => RightSubtreeNode != null) // до тех пор, пока есть узлы
                //    .Last() // взять последний из последовательности
                //    .Right = right; // записать в его правое поддерево правое поддерево корня
                left.LastRightChild.Right = right;
                return left;
            }

            //          root                root
            //         /    \              /    \
            //      _-*-_   ...   ->    _-L-_   ...
            //     /     \             /     \
            //    L       R          Ll       Lr
            //   / \     / \        /  \     /  \
            // Ll   Lr Rl   Rr    ...  ...  R   ...
            //                             / \
            //                           Rl   Rr
            if (IsLeftSubtree) // Если удаляемый узел является левым поддеревом у родительского узла, то
            {
                // на место удаляемого узла должен встать левый дочерний узел
                // Всё правое поддерево левого дочернего узла должно быть перемещено в левый дочерний элемент
                // самого правого листа правого поддерева текущего элемента
                // Правый дочерний элемент текущего узла должен стать правым элементом текущего
                // левого дочернего элемента

                //         root             root
                //        /    \           /    \
                //     _-*-_   ...   ->   R     ...
                //    /     \            / \
                //           R         Rl   Rr
                //          / \
                //        Rl   Rr
                if (left is null) // Если левого поддерева нет
                    parent.Left = right; // то левым поддеревом родительского узла будет правое поддерево удаляемого элемента
                else
                {   // иначе - если левое поддерево есть, то назначаем левым дочерним элементом
                    // текущего родительского узла то, что было в левом дочернем элементе
                    parent.Left = left;
                    // в самый правый дочерний узел левого поддерева записать правое
                    //          root                root
                    //         /    \              /    \
                    //      _-*-_   ...   ->    _-L-_   ...
                    //     /     \             /     \
                    //    L       R          Ll       Lr
                    //   / \     / \        /  \     /  \
                    // Ll   Lr Rl   Rr    ...  ...  R   ...
                    //                             / \
                    //                           Rl   Rr
                    left.LastRightChild.Right = right; //[node => node.Right].Last().Right = right;
                }
            }
            //     root                  root
            //    /    \                /    \
            // ...    _-*-_     ->   ...    _-R-_
            //       /     \               /     \
            //      L       R            Rl       Rr
            //     / \     / \          /  \     /  \
            //   Ll   Lr Rl   Rr       L   ... ...  ...
            //                        / \
            //                      Ll   Lr
            else // Узел является правым поддеревом
            {
                if (right is null) // Если правого поддерева нет
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
        public ExpressionTreeNode? GetNextLeft()
        {
            if (Left != null) return Left;
            if (Right != null) return Right;
            var is_left = IsLeftSubtree;
            var node = Parent;
            while (node != null)
            {
                if (is_left && node.Right != null)
                    return node.Right;
                is_left = node.IsLeftSubtree;
                node = node.Parent;
            }
            return null;
        }

        /// <summary>Получить следующий узел справа</summary>
        /// <returns>Узел справа от текущего</returns>
        public ExpressionTreeNode? GetNextRight()
        {
            if (Left != null) return Left;
            if (Right != null) return Right;
            var node = this;
            ExpressionTreeNode last;
            do
            {
                last = node;
                node = node.Parent;
            } while (!(node is null || node.Right == last));

            return node?.Left;
        }

        /// <summary>Перечисление переменных, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева переменных</returns>
        public virtual IEnumerable<ExpressionVariable> GetVariables()
        {
            IEnumerable<ExpressionVariable>? variables = null;
            if (_Left != null)
                variables = _Left.GetVariables();
            return _Right != null
                ? variables.AppendLast(_Right.GetVariables()).Distinct()
                : Enumerable.Empty<ExpressionVariable>();
        }

        /// <summary>Перечисление функций, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева функций</returns>
        public virtual IEnumerable<ExpressionFunction> GetFunctions()
        {
            IEnumerable<ExpressionFunction>? functions = null;
            if (_Left != null)
                functions = _Left.GetFunctions();
            return _Right != null
                ? functions.AppendLast(_Right.GetFunctions()).Distinct()
                : Enumerable.Empty<ExpressionFunction>();
        }

        /// <summary>Перечисление функционалов, известных данному узлу дерева</summary>
        /// <returns>Перечисление всех известных данному узлу дерева функционалов</returns>
        public virtual IEnumerable<Functional> GetFunctionals()
        {
            IEnumerable<Functional>? operators = null;
            if (_Left != null)
                operators = _Left.GetFunctionals();
            return _Right != null
                ? operators.AppendLast(_Right.GetFunctionals()).Distinct()
                : Enumerable.Empty<Functional>();
        }

        #region IDisposable

        /// <summary>Уничтожить узел рекуррентно с поддеревьями</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Объект считается уничтоженным</summary>
        private bool _Disposed;

        /// <summary>Уничтожить узел рекуррентно с поддеревьями</summary>
        /// <param name="disposing">Выполнить освобождение управляемых ресурсов</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed || !disposing) return;
            _Disposed = true;
            Left?.Dispose();
            Right?.Dispose();
        } 

        #endregion

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        public abstract ExpressionTreeNode Clone();

        /// <summary>Преобразование узла в строку</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => $"{Left?.ToString() ?? string.Empty}{Right?.ToString() ?? string.Empty}";

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        object ICloneable.Clone() => Clone();

        /// <summary>Оператор неявного преобразования строки в узел дерева</summary>
        /// <param name="value">Строковое значение</param>
        /// <returns>Строковый узел дерева</returns>
        public static implicit operator ExpressionTreeNode(string value) => new StringNode(value);
    }
}