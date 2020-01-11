using MathCore.Annotations;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор</summary>
    public abstract class OperatorNode : ComputedNode
    {
        /// <summary>Оператор является предвычислимым если предвычислимы его правое и левое поддерево</summary>
        public override bool IsPrecomputable
        {
            get
            {
                var is_left_null = Left is null;
                var is_right_null = Right is null;
                return !(is_left_null && is_right_null) && !is_left_null && Left.IsPrecomputable && !is_right_null && Right.IsPrecomputable;
            }
        }

        /// <summary>Приоритет оператора</summary>
        /// <remarks>
        /// Чем выше приоритет, тем глубже в дереве должен находиться оператор
        /// Шакала базовых приоритетов:
        ///  + - 0
        ///  - - 5
        ///  * - 10
        ///  / - 15
        ///  ^ - 20
        /// </remarks>
        public int Priority { get; protected set; }

        /// <summary>Ипя оператора</summary>
        public string Name { get; protected set; }

        /// <summary>Инициализация оператора</summary>
        protected OperatorNode() { }

        /// <summary>Инициализация оператора</summary>
        /// <param name="Name">Имя оператора</param>
        protected OperatorNode(string Name) : this() => this.Name = Name;

        /// <summary>Инициализация оператора</summary>
        /// <param name="Name">Имя оператора</param>
        /// <param name="Priority">Приоритет оператора</param>
        protected OperatorNode(string Name, int Priority) : this(Name) => this.Priority = Priority;

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => string.Format("{1}{0}{2}", Name, Left?.ToString() ?? string.Empty, Right?.ToString() ?? string.Empty);

        [NotNull]
        protected OperatorNode CloneOperatorNode<TOperatorNode>() where TOperatorNode : OperatorNode, new() => 
            new TOperatorNode { Left = Left?.Clone(), Right = Right?.Clone() };
    }
}