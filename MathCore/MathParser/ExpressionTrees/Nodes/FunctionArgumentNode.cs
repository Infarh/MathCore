using System.Collections.Generic;
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, хранящий данные об аргументе функции</summary>
    public class FunctionArgumentNode : OperatorNode
    {
        /// <summary>Перечисление аргументов начиная с указанного</summary>
        /// <param name="Node">Первый узел аргумента</param>
        /// <returns>Перечисление пар имени-корня дерева аргумента</returns>
        public static IEnumerable<KeyValuePair<string, ExpressionTreeNode>> EnumArguments(FunctionArgumentNode Node)
        {
            while(Node != null)
            {
                yield return new KeyValuePair<string, ExpressionTreeNode>(Node.ArgumentName, Node.ArgumentSubtree);
                Node = Node.Right as FunctionArgumentNode;
            }
        }

        /// <summary>Значение аргумента - правое поддерево</summary>
        public ExpressionTreeNode ArgumentSubtree => Left is FunctionArgumentNameNode ? Left.Right : Left;

        /// <summary>Имя аргумента - левое поддерево</summary>
        public string ArgumentName => Left is FunctionArgumentNameNode node ? node.ArgumentName : "";

        /// <summary>Инициализация узла-аргумента</summary>
        public FunctionArgumentNode() : base(",", -20) { }

        /// <summary>Инициализация узла-аргумента</summary>
        /// <param name="Name">Имя аргумента</param>
        /// <param name="Node">Узел поддерева аргумента</param>
        public FunctionArgumentNode(string Name, ExpressionTreeNode Node) : this(new FunctionArgumentNameNode(Name, Node)) { }

        /// <summary>Инициализация узла-аргумента</summary>
        /// <param name="Node">Узел поддерева аргумента</param>
        public FunctionArgumentNode(FunctionArgumentNameNode Node) : this() => Left = Node;

        /// <summary>Вычисление значения узла</summary>
        /// <returns>Значение узла</returns>
        public override double Compute() => ((ComputedNode)ArgumentSubtree).Compute();

        /// <summary>Компиляция узла аргумента</summary>
        /// <returns>Скомпилированное выражение корня поддерева аргумента</returns>
        public override Expression Compile() => ((ComputedNode)ArgumentSubtree).Compile();

        /// <summary>Компиляция узла аргумента</summary>
        /// <param name="Parameters">Список параметров выражения</param>
        /// <returns>Скомпилированное выражение корня поддерева аргумента</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => ((ComputedNode)ArgumentSubtree).Compile(Parameters);

        /// <summary>Клонирование узла</summary>
        /// <returns>Клонирование узла</returns>
        public override ExpressionTreeNode Clone() => new FunctionArgumentNode { Left = Left.Clone(), Right = Right?.Clone() };
    }
}