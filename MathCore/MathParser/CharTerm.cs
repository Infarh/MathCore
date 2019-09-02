using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser
{
    /// <summary>Символьный элемент математического выражения</summary>
    internal sealed class CharTerm : Term
    {
        /// <summary>Символьное значение элемента</summary>
        public char Value => _Value[0];

        /// <summary>Новый символьный элемент</summary>
        /// <param name="c">Символьное значение элемента</param>
        public CharTerm(char c) : base(new string(c, 1)) { }

        /// <summary>Получить поддерево</summary>
        /// <param name="Parser">Парсер мат.выражения</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Результат вызова метода Parser.GetOperatorNode(Value)</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression) => Parser.GetOperatorNode(Value);
    }
}