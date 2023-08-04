#nullable enable
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева выражения, содержащий сведения об аргументе функции</summary>
public class FunctionArgumentNameNode : OperatorNode
{
    /// <summary>Узел аргумента</summary>
    public ExpressionTreeNode? ArgumentNode => Right;

    /// <summary>Узел имени аргумента</summary>
    public string? ArgumentName => ((StringNode?)Left)?.Value;

    /// <summary>Инициализация узла дерева информации об аргументе функции</summary>
    public FunctionArgumentNameNode() : base(":", -100) { }

    /// <summary>Инициализация узла дерева информации об аргументе функции</summary>
    /// <param name="Name">Имя</param>
    /// <param name="Expression">Узел выражения аргумента</param>
    public FunctionArgumentNameNode(string Name, ExpressionTreeNode? Expression = null) : this(new StringNode(Name), Expression) { }

    /// <summary>Инициализация узла дерева информации об аргументе функции</summary>
    /// <param name="Name">Имя</param>
    /// <param name="Expression">Выражение узла</param>
    public FunctionArgumentNameNode(StringNode Name, ExpressionTreeNode Expression)
        : this()
    {
        if(!Name.Value.IsNullOrEmpty())
            Left = Name;
        Right = Expression;
    }

    /// <summary>Метод вычисления значения узла</summary>
    /// <returns>Значение аргумента</returns>
    public override double Compute() => ((ComputedNode?)ArgumentNode).Compute();

    /// <summary>Компиляция узла аргумента</summary>
    /// <returns>Скомпилированное выражение</returns>
    public override Expression Compile() => ((ComputedNode?)ArgumentNode).Compile();

    /// <summary>Компиляция узла аргумента с учётом списка параметров</summary>
    /// <param name="Args">Массив параметров процесса компиляции</param>
    /// <returns>Скомпилированное значение узла аргумента дерева выражения</returns>
    public override Expression Compile(ParameterExpression[] Args) => ((ComputedNode?)ArgumentNode).Compile(Args);

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => new FunctionArgumentNameNode { Right = Right.Clone(), Left = Left.Clone() };
}