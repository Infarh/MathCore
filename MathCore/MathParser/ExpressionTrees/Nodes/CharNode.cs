// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using MathCore.Annotations;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Символьный узел дерева математического выражения</summary>
    public class CharNode : ParsedNode
    {
        /// <summary>Значение символа узла</summary>
        public char Value { get; set; }

        /// <summary>Инициализация нового строкового узла</summary>
        public CharNode() { }

        /// <summary>Инициализация нового строкового узла</summary>
        /// <param name="value">Значение узла</param>
        public CharNode(char value) => Value = value;

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => new CharNode(Value)
        {
            Left = Left?.Clone(),
            Right = Right?.Clone()
        };

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => $"{(Left is null ? string.Empty : $"{Left}")}{Value}{(Right is null ? string.Empty : $"{Right}")}";

        /// <summary>Оператор неявного преобразования строки к типу строкового узла</summary>
        /// <param name="value">Строковое значение</param>
        /// <returns>Символьный узел</returns>
        [NotNull]
        public static implicit operator CharNode(char value) => new CharNode(value);

        /// <summary>Оператор неявного преобразования строкового узла к символьному типу</summary>
        /// <param name="node">Символьный узел</param>
        /// <returns>Значение Символьного узла</returns>
        public static implicit operator char([NotNull] CharNode node) => node.Value;
    }
}