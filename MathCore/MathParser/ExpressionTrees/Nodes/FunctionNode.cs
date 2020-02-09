using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева выражения, содержащий функцию</summary>
    public class FunctionNode : ComputedNode
    {
        /// <summary>Имя функции</summary>
        public string Name { get; }

        /// <summary>Массив имён аргументов функции</summary>
        [NotNull]
        public string[] ArgumentsNames => Arguments.Select(a => a.Key).ToArray();

        /// <summary>Перечисление аргументов функции</summary>
        public IEnumerable<KeyValuePair<string, ExpressionTreeNode>> Arguments => GetFunctionArgumentNodes(this);

        /// <summary>Функция узла</summary>
        public ExpressionFunction Function { get; set; }

        /// <summary>Инициализация нового функционального узла</summary>
        internal FunctionNode() { }

        /// <summary>Инициализация нового функционального узла</summary>
        /// <param name="Name">Имя функции</param>
        internal FunctionNode(string Name) => this.Name = Name;

        /// <summary>Инициализация нового функционального узла</summary>
        /// <param name="Name">Имя функции</param>
        internal FunctionNode([NotNull] StringNode Name) : this(Name.Value) { }

        /// <summary>Инициализация нового функционального узла</summary>
        /// <param name="Term">Выражение функции</param>
        /// <param name="Parser">Парсер выражения</param>
        /// <param name="Expression">Математическое выражение</param>
        internal FunctionNode([NotNull] FunctionTerm Term, [NotNull] ExpressionParser Parser, [NotNull] MathExpression Expression)
            : this(Term.Name)
        {

            var arg = Term.Block.GetSubTree(Parser, Expression);
            if (!(arg is FunctionArgumentNode))
                arg = arg switch
                {
                    FunctionArgumentNameNode name => new FunctionArgumentNode(name),
                    VariableValueNode _ => new FunctionArgumentNode(null, arg),
                    VariantOperatorNode _ when arg.Left is VariableValueNode => new FunctionArgumentNode(
                        ((VariableValueNode) arg.Left).Name, arg.Right),
                    _ => new FunctionArgumentNode(null, arg)
                };
            Right = arg;
            Function = Expression.Functions[Name, ArgumentsNames];
        }

        /// <summary>Вычисление значения узла</summary>
        /// <returns>Значение функции</returns>
        public override double Compute() => Function.GetValue(Arguments.Select(k => ((ComputedNode)k.Value).Compute()).ToArray());

        /// <summary>Получить перечисление аргументов функции</summary>
        /// <param name="FunctionNode">Узел функции</param>
        /// <returns>Перечисление аргументов функции</returns>
        private static IEnumerable<KeyValuePair<string, ExpressionTreeNode>> GetFunctionArgumentNodes([NotNull] ExpressionTreeNode FunctionNode) => 
            FunctionNode.Right is FunctionArgumentNode node
                ? FunctionArgumentNode.EnumArguments(node)
                : throw new FormatException();

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile() =>
            Expression.Call(Function.Delegate.Target != null ? Expression.Constant(Function.Delegate.Target) : null,
                    Function.Delegate.Method,
                    Arguments.Select(a => ((ComputedNode)a.Value).Compile()));

        /// <summary>Компиляция узла</summary>
        /// <param name="Args">Список параметров выражения</param>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile(ParameterExpression[] Args) =>
            Expression.Call(Function.Delegate.Target != null ? Expression.Constant(Function.Delegate.Target) : null,
                    Function.Delegate.Method,
                    Arguments.Select(a => ((ComputedNode)a.Value).Compile(Args)));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => new FunctionNode(Name)
        {
            Left = Left?.Clone(),
            Right = Right?.Clone(),
            Function = Function.Clone()
        };

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => $"{Name}({Arguments.Select(v => string.IsNullOrEmpty(v.Key) ? v.Value.ToString() : $"{v.Key}:{v.Value.ToString()}").ToSeparatedStr(", ")})";
    }
}