using System;
using System.Linq.Expressions;
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий скобки с возможностью вычисления</summary>
    public class ComputedBracketNode : ComputedNode
    {
        /// <summary>Скобки</summary>
        private readonly Bracket _Bracket;

        /// <summary>Скобки</summary>
        public Bracket Bracket => _Bracket;

        /// <summary>Вычислимый блочный узел дерева</summary>
        /// <param name="bracket">Скобки</param>
        /// <param name="Node">Узел-содержимое</param>
        public ComputedBracketNode(Bracket bracket, ExpressionTreeNode Node = null)
        {
            _Bracket = bracket;
            Left = Node;
        }

        /// <summary>Вычислить значение узла</summary>
        /// <returns>Значение вложенного узла</returns>
        public override double Compute() => ((ComputedNode)Left).Compute();

        /// <summary>Компиляция узла</summary>
        /// <returns>Компиляция содержимого узла</returns>
        public override Expression Compile() => ((ComputedNode)Left).Compile();

        /// <summary>Компиляция узла с параметрами</summary>
        /// <param name="Parameters">Список параметров выражения</param>
        /// <returns>Компиляция вложенного узла</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => ((ComputedNode)Left).Compile(Parameters);

        /// <summary>Клон узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => new ComputedBracketNode(_Bracket)
        {
            Left = Left?.Clone(),
            Right = Right?.Clone()
        };

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => 
            $"{_Bracket.Suround((Left ?? throw new InvalidOperationException()).ToString())}{Right?.ToString() ?? string.Empty}";
    }
}