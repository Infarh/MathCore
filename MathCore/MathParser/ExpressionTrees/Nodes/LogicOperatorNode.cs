using System;
using System.Linq.Expressions;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий логическую операцию</summary>
    public abstract class LogicOperatorNode : OperatorNode
    {
        protected LogicOperatorNode(string Name, int Priority) : base(Name, Priority) { }

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее логику оператора</returns>
        public abstract Expression LogicCompile();

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее логику оператора</returns>
        public abstract Expression LogicCompile([NotNull] params ParameterExpression[] Parameters);

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        [NotNull]
        public override Expression Compile() => LogicCompile().Condition(1d.ToExpression(), 0d.ToExpression());

        /// <summary>Компиляция узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        [NotNull]
        public override Expression Compile(ParameterExpression[] Parameters) => LogicCompile(Parameters).Condition(1d.ToExpression(), 0d.ToExpression());
    }
}